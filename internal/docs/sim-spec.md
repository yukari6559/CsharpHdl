# SharpHDL シミュレータ仕様（実装案）

使いやすさ最優先。ChiselTest の語彙・囲い込み DSL は採用しない。  
記述は現行 `Module` / Comb・Seq のまま、実行だけを `SharpHdl.Sim` が担う。

関連: [dsl-spec.md](dsl-spec.md)、[verilog-emit-spec.md](verilog-emit-spec.md)、[testing.md](testing.md)、[tickets/riscv-sharp-requests.md](tickets/riscv-sharp-requests.md)、**実装方針: [sim-impl.md](sim-impl.md)**

**消費者合意（RISC-Sharp）:** Module を変えず `SharpHdl.Sim` で Settle/Advance する方向に合意。  
**CPU/SoC の利用開始条件は S2 完了**（S1 は道具の健全性確認）。

---

## 1. 目標

- **xUnit のまま**回路の入出力と時間を扱える
- ポートは既存 Module の信号をキーにする（語彙を増やしすぎない）
- 時間は DSL と同じく明示（`Settle` = Comb、`Advance(clk)` = そのクロックの同期更新）
- **意味論の正 = DSL + Emit**。ズレたら Sim のバグ（二重仕様にしない）
- 未対応機能は黙って動かず、**明確に例外**

### 1.1 非目標（当面・消費者も不要と確認）

- Chisel 風 `poke` / `expect` / `step`
- 波形ビューア必須化
- 幅 > 64 の早期対応
- 暗黙クロック
- ISA / `rv64ui` を SharpHDL に入れること（消費者側）

---

## 2. パッケージと配布

```
src/SharpHdl.Sim/          # 新規
  SimExtensions.cs         # Module.Run()
  SimSession.cs
  Interpreter/
  …
```

- `SharpHdl.Core` にシミュ状態を持たない
- **nuget.org に `SharpHdl.Sim` を載せる**（消費者は `Core` + `Emit` + `Sim` の三参照）
- **版番号は Core / Emit / Sim で揃える**

---

## 3. ユーザー API（正）

### 3.1 書き味（最終 DX イメージ）

```csharp
using SharpHdl.Sim;
using Xunit;

var c = new Counter().Run();
c.Set(c.Module.Rst, 1);
c.Advance(c.Module.Clk);
Assert.Equal(0u, c.Get(c.Module.Count));
```

`.Value` 糖衣は急がない（RISC-Sharp: S1 は `Set`/`Get` で十分・歓迎）。

### 3.2 表面 API

| メンバー | 意味 |
|----------|------|
| `TModule.Run()` | `Describe()` → 多重駆動検査 → 未登録ローカル検査 → セッション開始。ポート・ローカル・Mem は 0 初期化 |
| `Set(Signal, ulong)` / `Get(Signal)` | ポートの読み書き（幅超過は例外）。**ローカル（`Internal`）への `Set` は `SimException`**。ローカルの `Get` は可（観測用） |
| `Settle()` | Comb / 2R1W の組み合わせ読みを再評価 |
| `Advance(In clk)` | そのクロックの **同期更新（posedge 相当）** → `Settle()`。**`clk` を 0/1 でトグルする必要はない**（レベルをいじる抽象ではない） |
| `Advance(In clk, int n)` | `n` 回 |
| `AdvanceWhile(In clk, int maxCycles, Func<SimSession, bool> cont)` | `cont` が true の間 Advance。**`maxCycles` 超過は例外**（黙って `false` にしない。テストで失敗を見逃しにくい）。長時間 `rv64ui` 向け |
| Mem 初期化 API | §4.3（**S2 必須**。階層下も可） |

**採用しない語彙:** Chisel の `poke` / `expect` / `step`。断言は xUnit。

**S1 必須:** `Run` + `Set`/`Get` + `Settle`/`Advance`。  
**S1.1 任意:** `Port(signal).Value`。型付き `c.Rst.Value` は後回し可。

### 3.3 ローカル信号（M4b / HC0）

- `ModuleFlat.Build` はトップの `GetLocals()` を値 0 で `Values` に登録する。子インスタンスのローカルは `SimInitVisitor` が 1 階層分登録する（深い階層は #29）
- キーは子の `Signal` オブジェクトそのもの（名前連結はしない）
- Comb / Seq の評価はこれまでどおり `SimWorld.Set` 経由でローカルへ書き込む（テストベンチの `SimSession.Set` とは別経路）
- 子モジュールの Seq / Mem のクロック比較は `ResolveAlias` 後に行う（子の `clk` ポートが親へエイリアスされているとき、親のクロックで発火する）
- 既知の制限: 子の中の Mem 初期化は `VisitInstance` が子の文を巡回しないため未対応（#29）

---

## 4. 時間・意味論

### 4.1 全体フロー

```
入力を Set
  → Settle():
       Comb の Assign / Switch
       階層の Comb
       Mem2R1W の組み合わせ読み（その時点の配列内容）
  → Advance(clk):
       ① 同期読みの右辺・Seq の next / ResetValue を「更新前状態」で評価
       ② 同期書き・Seq 左辺を一括反映（NBA 相当）
       ③ Settle()
```

同一 `Advance` 内での Mem 同一アドレス R/W は、DSL・Emit どおり **旧値読み（バイパスなし）**。命令フェッチ用途でも同じ（別規則にしない）。

### 4.2 メモリ読みレイテンシ（Emit と文字で固定）

Emit 形（1R1W / wstrb）は概ね:

```verilog
always @(posedge clk) begin
  if (we) mem[addr] <= wdata;   // wstrb 時はレーンごと
  rdata <= mem[addr];
end
```

| 種類 | 読みの性質 | 同一 `Advance` 後に読める値 | 新値が読みポートに出るタイミング |
|------|------------|------------------------------|----------------------------------|
| **1R1W** | 同期読み（レイテンシ 1 拍） | `rdata` は **更新前**の `mem[addr]`（同一アドレス書き込みがあっても旧値） | **次の** `Advance` 以降（そのとき `addr` が指す内容） |
| **1R1W+wstrb** | 同上＋バイト更新 | 同上（読んだワードは更新前。書かれたレーンもその拍の `rdata` には反映されない） | 同上 |
| **2R1W** | 組み合わせ読み | `Advance` 内は **書き込みコミット前**に読みを評価 → 同一アドレスは旧値。`Advance` 完了後の `Settle`／継続シミュレーションでは配列は更新済みなので、**以降の読みは新値** | 書き込みした `Advance` の完了後（追加の Advance は不要）。同一拍の「書きながら読む」だけ旧値 |

要約（フェッチ／RF 向け）:

- **命令メモリを 1R1W にする** → addr 提示の **次拍**で `rdata` が有効
- **レジスタファイル 2R1W** → 同一拍の RAW は旧値。次の組み合わせ評価から新値（x0 ゼロ化は消費者側）

**消費者向け注記（シングルサイクル × 命令メモリ）:**  
同一拍でフェッチしたいシングルサイクル芯なら、命令側は **1R1W ではなく組み合わせ読み**（例: 2R1W で `we=0` 固定 + `LoadMem`）を使う。1R1W は次拍有効。これは SharpHDL の欠陥ではなく、**Emit どおりの同期読みの帰結**である。

Sim が §4.2 と違う動きをしたら **Sim のバグ**。

### 4.3 Mem のホスト側初期化・点検（S2 必須）

全ビット 0 初期化に加え、テストハーネスがイメージを載せるため次を **S2 完了条件に含める**。

#### 指し方（S2 で固定）

- **キーは、その Mem の代表読み出しポート `Signal`（`rdata` / `rdata0` 等）**
- 2R1W は `rdata0` を代表キーとする（`rdata1` でも同一 Mem に解決してよいが、文書では `rdata0` を正とする）
- Describe 順インデックスは使わない（複数 RAM で脆い）

#### 階層（S2 必須）

- `LoadMem` / `PeekMem` / `PokeMem` は **Instance 配下の Mem も**、子の代表 `rdata`（エイリアス接続後の `Signal`）をキーに指せる
- 親モジュール直下のポートだけ、では **S2 未完了**

#### API

| API（名称は実装で調整可） | 意味 |
|---------------------------|------|
| `LoadMem(Signal rdataKey, ReadOnlySpan<byte> image)` | バイト列プレロード |
| `LoadMem(Signal rdataKey, ReadOnlySpan<ulong> words)` | ワード列プレロード（各要素は Mem 幅にマスク） |
| `PeekMem` / `PokeMem` | **`Advance` 外**で配列を直接読む／書く（ワード単位を第一。バイト API は幅が 8 の倍数なら推奨） |

#### `LoadMem(byte[])` のエンディアン（固定）

- バイト列を幅 32/64 等のワードへ詰めるときは **little-endian 固定**（RV イメージ・`rv64ui` と一致）
- 例: width=32 なら `image[i*4 + 0]` が LSB … `image[i*4 + 3]` が MSB
- 未定義にしない（ハーネスと Sim の食い違い防止）
- 長さ不足は 0 埋め、超過は例外

`PokeMem` 後、1R1W の `rdata` レジスタは自動では変わらない（同期読みの意味論を壊さない）。必要ならその後 `Advance` する。2R1W は `Settle()` で読みポートに反映。

### 4.4 Instance ポート接続（S2 で固定）

**エイリアス（同一信号）** とする（RISC-Sharp 希望・採用）。

- 親の `Signal` と子ポートは Sim 上 **同じ値スロット**を共有する
- コピー／遅延付き接続は当面やらない（ズレの温床）

### 4.5 Stmt 対応表

| 要素 | シミュでの扱い |
|------|----------------|
| `AssignStmt` | Settle 時 |
| `SwitchStmt` | セレクタ一致枝のみ |
| `SeqBlockStmt` | `Advance(その Clk)` 時 |
| `SeqAssignStmt` | reset==1 なら ResetValue、否则 next |
| `InstanceStmt` | 子を同一ランタイム。ポートはエイリアス |
| `MemStmt` / `MemWstrbStmt` | §4.2 同期 |
| `Mem2R1WStmt` | 書き込み Advance、読み Settle。§4.2 |

**未対応:** `SimUnsupportedException`（機能名付き）。

---

## 5. 値表現

| 段階 | 型 |
|------|-----|
| S1–S2 | `ulong`（`Width <= 64`。RV64 本線と一致） |
| 将来 | `BitVector`（急がない） |

---

## 6. 実装構成

```
SimSession
  ├─ Module root
  ├─ Signal → ulong（エイリアスは同一エントリ）
  ├─ clk → SeqBlock 索引
  ├─ Mem 配列 + Load/Peek/Poke
  └─ Instance 展開

Interpreter: EvalExpr / SettleComb / AdvanceClock
```

- 組み合わせループ: Settle 上限超過でエラー
- 多重駆動: 検出したらエラー（最後勝ちにしない）

---

## 7. テスト層

| 層 | 内容 |
|----|------|
| AST / emit スナップショット | 既存 `ModuleTests` |
| 回路（C#） | `Run` + xUnit |
| Verilator | S4・任意。同一シナリオ突き合わせ用の余白 |

---

## 8. 段階（実装チケット）

| ID | 内容 | 完了条件 |
|----|------|----------|
| **S0** | 本仕様の合意 | この文書を正とする（消費者フィードバック取り込み済み） |
| **S1** | Comb + Seq、単一モジュール、`Run`/`Set`/`Get`/`Settle`/`Advance` | Counter・Alu が xUnit で緑。**CPU/SoC 利用開始には使わない** |
| **S1.1** | `Port().Value` 等 | 任意・急がない |
| **S2** | Mem 3種 + Instance（エイリアス）+ **Load/Peek/Poke（`rdata` キー・階層下・LE）** + `AdvanceWhile`（超過は例外） | RegFile / 子 RAM への Load / 階層が回る。**ここから CPU 消費者が利用開始** |
| **S3** | 失敗時信号ダンプ、任意 VCD | デバッグ用（ビューア必須ではない） |
| **S4** | Verilator 同一シナリオ | 任意補強 |

### 8.1 利用者向けゲート（明示）

| 用途 | 必要マイルストーン |
|------|-------------------|
| 道具の健全性・小さな Comb/Seq | S1 |
| **CPU / SoC / `rv64ui` ハーネス** | **S2 完了済み（利用可）** |
| **MidiSynth（制御・乗算・ROM 等の L1 原語）** | **S2 完了済み。M1–M5 が本線** → [tickets/midisynth-tang-primer25k-requests.md](tickets/midisynth-tang-primer25k-requests.md) |

ドキュメント・README / consumers 追記時も「CPU/SoC は S2 待ち」「MidiSynth 宿題 M1–M5 は S2 後」と書く。

### 8.2 S2 の次（ロードマップ本線）

```
S2 完了 → M1 → (M3/M4 並行可) → M2 → M5 →（任意）M6/M7
```

全体の位置づけ: [ROADMAP.md](ROADMAP.md)「いまの本線（Sim → MidiSynth）」。

---

## 9. 設計判断の要約

| 判断 | 選択 |
|------|------|
| Chisel 語彙 | 捨てる |
| 断言 | xUnit |
| 時間 | `Settle` / `Advance(clk)`（posedge 抽象・トグル不要） |
| Instance | **エイリアス** |
| Mem キー | 代表 `rdata`（`Signal`）。階層下も可 |
| Mem 初期化 | S2 必須。`byte[]` は **little-endian** |
| `AdvanceWhile` 超過 | **例外** |
| 読みレイテンシ | §4.2＝Emit と同じ。フェッチも例外なし |
| パッケージ | Sim を NuGet。版は Core/Emit と揃える |
| CPU ゲート | **S2** |

---

## 10. 次のアクション

1. ~~S2 残りを閉じる~~ → **完了**（`0.3.5`）  
2. 消費者が `rv64ui` / 階層 Mem を Sim 本線に寄せる（RISC-Sharp 側）  
3. **いまの本線:** MidiSynth 宿題 — M1・M3・M4 済み → **M2 進行中**（XOR / XNOR は PR #103。演算の左右は同じ幅が必須になり（#105）、Eval の左幅マスクと Verilog の幅が一致）→ M5（[tickets/midisynth-tang-primer25k-requests.md](tickets/midisynth-tang-primer25k-requests.md)）
