# SharpHDL Sim 実装方針（S1 → S2）

仕様の正本: [sim-spec.md](sim-spec.md)。ここは **どう切って実装するか**。

原則: Core / Emit は触らない（バグ修正以外）。新規 `SharpHdl.Sim` のみ。版は Core/Emit と揃えて後で pack。

---

## 1. リポジトリ作業順

```
1. src/SharpHdl.Sim/ 追加（net10.0、ProjectReference → Core のみ）
2. ソリューション / CI にプロジェクト追加
3. SharpHdl.Tests から ProjectReference → Sim（回路テスト用）
4. S1 実装 → 緑
5. S2 実装 → 緑 → Version bump（例 0.3.0）で三パッケージ同時
```

Emit への参照は S1 不要（解釈は AST のみ）。

---

## 2. 型・ファイル分割（推奨）

```
SharpHdl.Sim/
  SimSession.cs          # Run 結果。Set/Get/Settle/Advance/AdvanceWhile
  ModuleSimExtensions.cs # module.Run()
  SimUnsupportedException.cs
  BitUtil.cs             # Mask(width), 幅チェック、LE パック（S2）
  Runtime/
    SimWorld.cs          # Signal→値、Mem 表、索引
    MemState.cs          # 配列 + kind + rdataKey（S2）
  Interp/
    EvalExpr.cs          # Expr → ulong の入口（EvalExprVisitor に Accept）
    EvalExprVisitor.cs   # IExprVisitor<ulong>。OpExpr は Op の switch 式
    CombSettle.cs        # CombStmtVisitor 1 個で不動点ループ（IsUpdate / Reset）
    CombStmtVisitor.cs   # Assign / Mem2R1W read を評価。If / Switch / Instance は同じ Visitor で入れ子を辿る
    SeqAdvance.cs        # SeqStmtVisitor で全 stmt を Accept → Commit()（NBA）
    SeqStmtVisitor.cs    # clk 一致の SeqBlock + 同期 Mem（1R1W / 2R1W / wstrb）。Instance は同じ Visitor で子へ
    SeqBlockVisitor.cs   # SeqBlock Body の検証と SeqAssign 収集（SeqAssign 以外は SimUnsupportedException）
    ModuleFlat.cs        # ポート登録 → SimInitVisitor でルート stmt を走査
    SimInitVisitor.cs    # Instance エイリアス登録・Mem 配列確保（直下のみ。再帰は R-S4 / #29）。トップレベル SeqAssign は SimUnsupportedException
```

> **Visitor 化済み（R-V2 / #76）:** Sim の stmt 解釈はすべて Core の `IStmtVisitor` 実装（`Stmt.Accept`）で、`is XxxStmt` 分岐は無い。旧 `SimCombStmtHandler` と Core `Walk`（`ICombStmtHandler`／`CombStmtDispatch`）は削除済み。Seq の reset 判定は `SeqStmtVisitor.VisitSeqBlock` で 1 回だけ評価し、0 / 1 以外なら何もしない。`StmtSupport` 許可リストは R-V3（#77）で廃止。トップレベルに置けない stmt は `SimInitVisitor` が弾く（現状 SeqAssign のみ。`Stmt` の独自派生は `Accept` を自前実装するしかなく Visitor に到達しない）。多重駆動は Core `CheckMultiDriveVisitor`（下記 4.2）。Expr も R-V4（#78）で Core の `IExprVisitor<T>`（`Expr.Accept<T>`）に移行し、`is XxxExpr` 分岐は無い。詳細は [refactor-plan.md](refactor-plan.md)「Visitor」節。

公開 API は `SimSession` と `Run()` に薄く寄せ、解釈器は internal。

---

## 3. `Run()` でやること

```
new T() 済み（または Run 内で Activator — 推奨は呼び出し側 new）
  → module.Describe()
  → ModuleFlat.Build(module):
       全 Signal を values[signal] = 0
       ルート stmts を走査し
         SeqBlock → clk キーのリストへ
         Assign/Switch → comb リストへ
         Instance/Mem → S1 では SimUnsupportedException
  → Settle() 一回（初期 Comb）
  → SimSession を返す
```

**Signal の同一性:** `Dictionary` のキーは **オブジェクト参照**（エイリアスは S2 で同一エントリを共有）。

---

## 4. S1 意味論の実装ポイント

### 4.1 `EvalExpr`（`EvalExprVisitor`）

| ノード | 動作 |
|--------|------|
| `Signal` | `values[s]` |
| `OpExpr` | +/-/&/\|（幅は左辺。結果は `Mask(leftWidth)`。減算は幅内ラップ） |
| `SliceExpr` | シフト＆マスク |
| `ConcatExpr` | 左が MSB で連結 |
| `SignExtend` / `ZeroExtend` | 仕様どおり |
| `ConstExpr` | `Value & Mask(Width)` |
| 未知の `Op` | `SimUnsupportedException`（Expr 型の取りこぼしは `IExprVisitor<T>` でコンパイルエラー） |

定数リテラル用 `Expr` が Core に無いので、S1 は **Signal と上記ノードだけ**でよい（現行 Counter/Alu で足りる）。

### 4.2 `Settle`（Comb）

1. `changed = true` ループ（上限例: 64、超過で例外）
2. 各 `AssignStmt`: `rhs = Eval` → 左辺が変われば更新
3. 各 `SwitchStmt`: `Get(sel)` に一致する `Case` の Assign だけ実行。不一致ならその Switch 由来の更新なし
4. 変化がなくなったら終了

多重駆動（同一 Settle 内で同一 Signal へ複数回 Assign）: **検出して例外**（最後勝ちにしない）。`Run()` 冒頭で Core `CheckMultiDrive`（`CheckMultiDriveVisitor`）が判定する。If／Switch は**枝ごと**に数え、枝の和集合を外側へ 1 回だけ足す（別枝の同一 Signal は可、同一枝内や外側との重複は `MultiDriveException`）。SeqBlock 内の SeqAssign も同じ集合で数える。

### 4.3 `Advance(clk)`（posedge 抽象）

1. `clk` に紐づく `SeqBlockStmt` を列挙（無ければ何もしないか、将来 Mem のみ）
2. **評価フェーズ（更新前状態）:** 各 `SeqAssignStmt` について  
   `next = (Get(reset) == 1) ? ResetValue : Eval(Expr)`  
   をバッファへ（まだ `values` に書かない）
3. **コミットフェーズ:** バッファを一括で `values` へ
4. `Settle()`

`clk` / `reset` のトグルは不要。`Set(rst,1)` のあと `Advance(clk)` でリセット値が載る。

### 4.4 `AdvanceWhile`

```
for (i = 0; i < maxCycles; i++) {
  if (!cont(session)) return;
  Advance(clk);
}
throw … // maxCycles 超過
```

S1 でも小さなヘルパとして入れてよい（Mem 無しでもテストに便利）。S2 完了条件には含める。

---

## 5. S2 で足すもの（S1 の上に載せる）

### 5.1 Instance（エイリアス）

`ModuleFlat` で子を再帰:

- 子の `Describe()` が親から既に呼ばれている前提か、Flat 時に呼ぶかを **一方に固定**（推奨: 親 `Describe` 内で子を構築済み＝現行どおり。Flat は stmts の `InstanceStmt.ChildModule` を辿る）
- 各 `PortConnection`: 子ポートと親 Signal を **同一スロット**にマップ（`values` は親 Signal を正とし、子ポート参照もそこに indirection、または登録時に両方同じキーへリダイレクトする辞書）

実装案: `Signal canonical = aliasMap.GetOrAdd(childPort, parentSignal)` のあと、Eval 時は常に `canonical` で読む。

### 5.2 Mem

`MemState`: `ulong[]` または `byte[]`（幅≤64 なら `ulong[depth]` で足りる）、`rdataKey`、kind。

| kind | Advance | Settle |
|------|---------|--------|
| 1R1W / wstrb | 旧配列で `rdata` 次値を評価 → we で配列更新 → `rdata` コミット | （rdata は同期なので Comb では触らない） |
| 2R1W | we で配列更新のみ（読みはしない） | `rdata0/1 = mem[raddr]` |

同一 Advance 内の旧値読みは「読み評価 → 書きコミット」順で自然に満たす。

### 5.3 Load / Peek / Poke

- キー: 代表 `rdata` / `rdata0`（`Signal`）
- 階層: Flat 時に **全 Mem を rdataKey で登録**（子も含む）
- `LoadMem(key, ReadOnlySpan<byte>)`: **little-endian**、ワード幅バイト数で詰める
- Peek/Poke: ワード index。`Advance` 外。1R1W の `rdata` レジスタは Poke では更新しない

### 5.4 `AdvanceWhile`

§4 のとおり超過は例外。S2 完了条件に含める。

---

## 6. テスト方針

| 段階 | テスト | 置き場 |
|------|--------|--------|
| S1 | Counter: rst→0、step 加算 | `SharpHdl.Tests`（既存 Counter モジュール流用） |
| S1 | Alu: Theory で +/-/&/\| | 同上 |
| S1 | 未対応 Mem を含む Module は `Run` または初回 Advance で例外 | 同上 |
| S2 | SimpleRam / RegFile2R1W / ByteWrite の読み書きレイテンシ | 同上 |
| S2 | AluTop 階層 | 同上 |
| S2 | LoadMem LE + 子モジュール RAM | 新規ミニ SoC 風テスト Module でも可 |

既存の emit スナップショットテストは触らない。

---

## 7. 実装チケット（作業チェックリスト）

### S1（道具の健全性）

- [x] `SharpHdl.Sim` csproj + ソリューション登録
- [x] `Run` / `SimSession.Set|Get|Settle|Advance`
- [x] `ExprEval` + Comb Settle + Seq NBA
- [x] Counter / Alu の xUnit
- [x]（任意→S2 必須）`AdvanceWhile`（超過例外）

### S2（CPU / MidiSynth 利用開始ゲート）— **完了（`0.3.5`）**

- [x] Instance エイリアス（階層 Comb）
- [x] Mem 1R1W + 同期読みレイテンシ
- [x] Mem wstrb / 2R1W + §4.2 レイテンシ
- [x] `LoadMem`/`Peek`/`Poke`（rdata キー・階層・LE）
- [x] `AdvanceWhile` 超過例外
- [x] 回帰テスト一式
- [x] パッケージメタ
- [x] S2 完了時の版上げ + タグ（Core/Emit/Sim 同一 Version・`0.3.5`）

### MidiSynth 宿題（L1）— **いまの本線**

正本: [tickets/midisynth-tang-primer25k-requests.md](tickets/midisynth-tang-primer25k-requests.md)  
ロードマップ: [ROADMAP.md](ROADMAP.md)

- [x] **M1a** 比較（`Eq`/`Neq` + 関係演算）— `0.3.6`
- [x] **M1b** `If`/`Else`（名前付き `then:` / `@else:`）— `0.3.7`
- [x] **M1c** Switch との役割分担（dsl-spec）
- [x] **M3** 定数リテラル Expr（旧「後回し ConstExpr」）— `0.3.8`
- [x] **M4** ローカル Wire/Reg — PR #85 / #100（Sim: ローカル登録、外からの `Set` 禁止、子 Seq のクロック別名解決）
- [ ] **M2** 乗算・シフト・`^`/`~` — **進行中**（`^` / `~^` は PR #103。左右の幅は #105 で同じ幅が必須に）
- [ ] **M5** 波形 ROM 初期化（Emit + Sim/`LoadMem` 整合）
- [ ] （任意）**M6** ポリ / **M7** 複数 CLK — 単音 MVP 後

---

## 8. 意図的に後回し

- `Port().Value` / ソースジェネレータ（S1.1）
- VCD（S3）
- Verilator バックエンド（S4）
- ConstExpr → **M3**（S2 後・上表）

---

## 9. 役割分担（このリポジトリの慣例）

| 担当 | 内容 |
|------|------|
| 実装者（人手） | `SharpHdl.Sim` 本体・解釈器 |
| エージェント（依頼時） | テスト追加、csproj/CI、仕様追記、レビュー |

実装開始の合図は「S1 から書いて」等の明示。それまでは本方針＋ sim-spec が正。

---

## 10. 最初のコミット粒度（案）

1. `chore: add SharpHdl.Sim project skeleton`
2. `feat(sim): S1 Run/Settle/Advance for Comb+Seq`
3. `test(sim): Counter and Alu circuit tests`
4. （後）`feat(sim): S2 mem, hierarchy, LoadMem`
5. `chore: bump`（S2 完了時）and pack Sim
6. M1a〜M4 は済み。（進行中）M2 → M5（MidiSynth チケット順）
