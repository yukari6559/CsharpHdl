# リファクタ計画（設計負債・後回し整理）

**目的:** 機能チケット（M2–M5）とは別に、いまは動くが後で直した方がよい設計を一覧化し、着手順を固定する。  
**原則:** 意味論を勝手に変えない。変える場合は仕様＋テスト更新を明示する。  
**ゲート:** **M3（定数 Expr）のテスト＋版上げが終わるまで Emitter 大規模リファクタ（R1 以降）に入らない。**

**プロセス:** M3 完了後のリファクタ／機能は [dev-workflow.md](dev-workflow.md) に従い **Issue + ブランチ + PR** で進める（R-E1 ≈ 1 Issue）。

関連: [ROADMAP.md](ROADMAP.md) / [dsl-spec.md](dsl-spec.md) / [verilog-emit-spec.md](verilog-emit-spec.md) / [sim-impl.md](sim-impl.md)

---

## いまの位置

| 段階 | 内容 | 状態 |
|------|------|------|
| R0 | M3 Core/Sim/Emit + テスト + 版上げ（`0.3.8`） | **完了** |
| R1〜 | 本ドキュメントのリファクタ本線 | R0 のあと |

機能本線（R0 後）: M3 完了 →（任意でリファクタ塊）→ M4 / M2 / M5。  
リファクタは **M4 の前に R-E1 を挟む**のが安全（stmt 追加の二重修正を減らす）。

---

## 推奨着手順（要約）

```
R0  M3 完了（Emit/テスト/版上げ）
  → R-E1  Emitter 二重実装の統合（挙動維持）
  → R-E2 + R-D3  If の wire/reg・Comb 生成方針の固定
  → R-C1 + R-C2  CurrentWrite / Switch 親リストの統一
      → ネスト If・Seq 内 If を解禁する土台
  → R-E5  Mem 配列名の一意化（M5 より前が望ましい）
  → R-C5 / R-S3  幅・visitor 共有（M2 前が安全）
    → R-V1 → R-V2 → R-V3 → R-V4  Stmt／Expr Visitor（Accept）化（M4／M2 前）
      → R-S4  ModuleFlat 再帰（R-V2 の Visitor 上で）
並行可: R-T1 / R-T3 / R-D1 / R-C6 / R-E9
```

**挙動維持を最優先する最初の塊:** R-E1 → R-T1/T2 → R-C6 → R-E9  
**合成・正しさ:** R-E2、R-E5  
**次機能の土台:** R-C1/C2、R-S1/S2

---

## Emit

| ID | 重要度 | 内容 | なぜ痛いか | 方針 | 依存 | 挙動維持 |
|----|--------|------|------------|------|------|----------|
| **R-E1** | 高 | `Emitter` と `SingleModuleEmitter` の二重実装 | stmt 追加のたびに二重修正漏れ（If で実際に起きた） | 1 つのモジュール emit に統合。階層は薄いラッパ | R0 後 | はい（スナップショット一致） |
| **R-E2** | 高 | If の `always @(*)` 手続代入なのに `output wire` | 合成／Verilator で壊れやすい | If 左辺を `regOuts` に含めるか、Comb 方針を三項/`assign` に統一。仕様に固定 | R-E1 後可 | 生成は変わる可能性 |
| **R-E3** | 高 | Switch emit が `Cases[i].Stmts[0]` を Assign 前提・裸 Exception | 複数代入・空枝で落ちる／意味が不明 | 検証付き walk。default 方針を仕様化 | ネスト制御前 | 現行 ALU は維持 |
| **R-E4** | 中 | If 枝が Assign のみ（ネスト未対応） | 仕様どおりだが拡張しづらい | visitor。本格対応は R-C1 後 | R-C1 | 厳密化は維持可 |
| **R-E5** | 高 | Mem 配列名が常に `mem` | 1 モジュール複数 Mem で識別子衝突 | 一意名（`mem_{rdata}` 等）または名前引数 | **M5 より前** | 名前変更あり |
| **R-E6** | 中 | 子モジュール名 = `GetType().Name` | 衝突・リネームに弱い | emit 名 API / 属性（dsl-spec §7） | なし | 名前変更あり得 |
| **R-E7** | 中 | ConstExpr Emit 未配線 | — | **M3 で解消済み（`0.3.8`）** | — | — |
| **R-E8** | 中 | Emitter が `Describe()` を呼ばない | 呼び出し忘れで空 module が「成功」に見える | 入口で Describe、または契約を明記し二重呼び出し防止 | なし | 契約次第 |
| **R-E9** | 低 | `EmitPlaceholder` + Smoke が stub | 本番経路を守っていない | 削除または実 Emitter スモーク | なし | テスト更新 |
| **R-E10** | 低 | Seq を信号ごとに if/else reset | 読み味・冗長 | 同一 clk/reset を 1 always にまとめる（emit-spec 推奨） | なし | 差分出る |
| **R-E11** | 低 | width=1 の `output reg [0:0]` と wire の表記ゆれ | 見た目・ツール差 | 1bit 表記を統一 | なし | 差分出る |

### Emitter 作業の切り方（旧 R1–R3）

| 段 | 内容 | 完了条件 |
|----|------|----------|
| 旧 R1 | `emitExpr` / ポート枠 / `regOuts` 収集の共通化 | 既存 emit テスト緑 |
| 旧 R2 | Stmt 本体を1パスに（**= R-E1 本体**） | 同上 |
| 旧 R3 | （任意）`ExprEmit` / `StmtEmit` ファイル分割 | 同上 |

---

## Core

| ID | 重要度 | 内容 | なぜ痛いか | 方針 | 依存 | 挙動維持 |
|----|--------|------|------------|------|------|----------|
| **R-C1** | 高 | `CurrentWrite` ThreadStatic グローバル | ネスト／例外／並列 Describe で脆い | スタック push/pop または `DescribeContext` | ネスト解禁の前提 | API 維持なら可 |
| **R-C2** | 高 | Switch が常に `CurrentStmts = Stmts`、If だけ親復元 | Switch を If/Seq 内にネストできない | If と同じ親ポインタ＋ finally | R-C1 と同時がよい | ルート Comb 内は維持 |
| **R-C3** | 中 | `Switch(In op, …)` のみ | セレクタを一般 Expr にできない | `Signal`/`Expr` へ拡張（定数 case は維持） | なし | オーバーロードで可 |
| **R-C4** | 中 | Seq `ResetValue` が `uint` | 64bit・Const と非対称 | `ulong` または Const 系に揃える | M3 後が自然 | 広い値は breaking 可 |
| **R-C5** | 中 | `GetWidth(this object)` | 型安全が弱い。Op 幅は左辺のみ | Expr 上の幅 API、構築時チェック強化（演算の左右の幅を揃える検査は #105 で済み） | M2 前が望ましい | 厳格化は一部 breaking |
| **R-C6** | 中 | 裸の `throw new Exception()` | 原因が分からない | 専用例外 | なし | はい |
| **R-C7** | 中 | 同一信号への二重駆動未検出（dsl-spec はエラー） | 黙って多重駆動 | Describe 後または emit 時の静的検査 | L4 と近い | 新規失敗増 |
| **R-C8** | 低 | fluent `.Else` / `When` 三項（チケットで後回し） | DX | 任意 API 整理 | なし | 新 API |

---

## Sim

| ID | 重要度 | 内容 | なぜ痛いか | 方針 | 依存 | 挙動維持 |
|----|--------|------|------------|------|------|----------|
| **R-S1** | 高 | `SeqAdvance` が Body を `SeqAssignStmt` 前提 | Seq 内 If 等でキャスト失敗 | 型分岐＋未対応は明示例外。将来 visitor | R-C1 後に解禁 | 現行 Seq のみは維持 |
| **R-S2** | 中 | `Run()` の空 allowlist | 新 stmt 忘れ／空洞 if | 共有レジストリまたは Flat 時検証 | R-E1 と相性良 | はい |
| **R-S3** | 中 | Emit/Sim の stmt 解釈がそれぞれ複製 | 意味論ズレ（sim-spec: 二重仕様にしない） | 共有 visitor / 薄い IR。**部分完了（#28 / PR #74）:** Core `Walk`（`ICombStmtHandler` + `WalkComb`）で Comb 5 型のみ共有。全型・入れ子・Seq／Flat／Validate は **R-V 系で完遂** | R-E1 後 | 目標は維持 |
| **R-S4** | 中 | `ModuleFlat` が浅い（直下のみ） | 深い階層・孫 Mem が脆い | 再帰 Build（#29）。**R-V2 で Visitor 化した後に着手** | R-V2 | 拡張 |
| **R-S5** | 低 | `Port().Value` 糖衣（仕様で後回し） | DX | 任意 S1.1 | なし | 新 API |

---

## Visitor（R-S3 の完遂）

Milestone: **refactor-visitor**。1 Issue ≈ 1 PR、上から順に。

**背景:** #28 の `ICombStmtHandler` は Comb 5 型（Assign／Switch／If／Instance／Mem2R1W）だけで、Mem／MemWstrb／SeqBlock／SeqAssign は各所の `is` 分岐のまま。未登録型は `WalkComb` が黙ってスキップし、入れ子（If／Switch／SeqBlock の子）は各 handler が自前で辿る。新 Stmt（M4／M5）追加時の漏れを検出できない。

**方式の決定:** 古典的 Visitor（`Stmt.Accept(visitor)`）。中央ディスパッチャ（全型 switch + default 例外）は採らない。

| 観点 | 理由 |
|------|------|
| 漏れ検出 | 型追加でコンパイルエラー → 全 Emit／Sim／Validate 経路を強制。R-S3（二重仕様にしない）の目的そのもの |
| 増え方 | Stmt 型（M4／M5／M6-7）も走査側（Emit・Sim×3・Validate・将来のバックエンド／制約生成）も増える |
| 破壊的変更 | `Stmt` 抽象化は公開 API 変更だが 0.x・消費者は自リポのみで今が最安 |
| 閉じた型 | Stmt のサブクラスは Core だけが定義（利用者は DSL のみ） |

**Visitor の規則:**

- Visitor インターフェースは **全 Stmt 型**を持つ。既定 no-op の基底クラスは作らない（「何もしない」も明示）
- 未知型スキップをしない。入れ子（If Then/Else、Switch Cases、SeqBlock Body）も同じ Visitor で辿る
- 戻り値はまず `void`。値を返す走査が必要になったらジェネリック版を追加
- Expr にも同方式を適用（R-V4、M2 前）

| ID | Issue | 内容 | 完了条件 | 依存 |
|----|-------|------|----------|------|
| **R-V1** | #75 | Core に Stmt Visitor（`Stmt` を abstract 化・9 型に `Accept`）＋ Emit 移行（`EmitModuleBody`／`CollectRegOuts`／`EmitOneIf`／`CollectIfRegOuts`／`CollectInstances`） | Emit に `is XxxStmt` 分岐なし／出力順を元の stmt 順に復帰（スナップショット不変）／`EmitCombStmtHandler` 削除 | なし |
| **R-V2** | #76 | Sim 移行（`CombSettle`／`SeqAdvance`／`ModuleFlat`）。旧 `ICombStmtHandler`・`CombStmtDispatch`・`SimCombStmtHandler` 削除 | Sim に `is XxxStmt` 分岐なし／`CombSettle` ↔ handler 循環参照の解消／handler 状態をコンストラクタ引数で露出しない | R-V1 |
| **R-V3** | #77 | `CheckMultiDrive` を Visitor 化。`StmtSupport` 許可リストを廃止（または存続理由を明記） | Core に `Accept` 以外の `is XxxStmt` 分岐なし／sim-impl 等の内部ドキュメント更新 | R-V1, R-V2 |
| **R-V4** | #78 | Expr Visitor（`EmitExpr`／`EvalExpr`、`GetWidth` の置き場所検討） | Expr 追加でコンパイルエラー／Emit・Sim に `is XxxExpr` 分岐なし | R-V3。**#9（M2）より前** |

後続: **R-S4（#29）** は R-V2 の Visitor 上で再帰化する。

---

## 将来検討（未 Issue）

GitHub Issue はまだ切らない。着手するときに Milestone／Issue 化する。

| 由来 | 内容 | メモ |
|------|------|------|
| ~~R-S3 フォロー~~ | `CombSettle` ↔ `SimCombStmtHandler` の循環参照 | **R-V2（#76）へ移管** |
| ~~R-S3 フォロー~~ | Emit 側の `ICombStmtHandler` 寄せ | Comb 分は #74 で実施。全型は **R-V1（#75）へ移管** |
| ~~R-S3 フォロー~~ | `WalkComb` を Seq／全 stmt へ拡張するか | **全 Stmt の Visitor（R-V 系）で決着** |

---

## Tests

| ID | 重要度 | 内容 | なぜ痛いか | 方針 | 依存 | 挙動維持 |
|----|--------|------|------------|------|------|------|
| **R-T1** | 中 | 巨大 `ModuleTest.cs` | AST／幅／Emit が混在 | `EmitSnapshot` / `Width` / `Ast` に分割 | R-E1 と並行可 | はい |
| **R-T2** | 中 | 二重 emit 経路のカバレッジ不足 | 統合前は片側漏れ | R-E1 後は 1 入口＋階層ゴールデン | R-E1 | はい |
| **R-T3** | 中 | `examples/` と `TestModule/` の複製 | 乖離 | 共有または examples 参照 | なし | はい |
| **R-T4** | 中 | If の Verilog 合法性を見ていない | wire+always を見逃す | 完全スナップショット＋任意 Verilator | R-E2 | 強化 |
| **R-T5** | 低 | Smoke が Placeholder | 本番未守護 | 実 Emitter へ | R-E9 | はい |

---

## Docs / 運用

| ID | 重要度 | 内容 | 方針 |
|----|--------|------|------|
| **R-D1** | 中 | 公開 `docs/dsl-spec.md` と内部 dsl-spec の二重管理。Emit コメントの死んだパス | 正本を1つに、または同期ルール |
| **R-D2** | 低 | guides / repo-layout の陳腐化（Sim 欠落、スライス「後回し」のまま等） | 現状に合わせて更新 |
| **R-D3** | 中 | Comb を `assign` 三項にするか `always @(*)` か未固定（Switch と If で混在） | **R-E2 と一体**で固定済み（Switch=三項+wire / If=always@(*)+reg）。実装は CollectRegOuts） |

---

## 意図的にリファクタ対象外（境界）

| 項目 | 理由 |
|------|------|
| M2–M5・M6/M7 の機能追加そのもの | 別チケット（本線） |
| 幅 > 64 / BitVector | sim-spec 非目標。急がない |
| 左辺部分代入 | 痛みが出てから（L1） |
| HLS・暗黙クロック・自動パイプライン・SV 全対応 | やらない |
| CLI の任意 DLL 読み込み等 | ツール後回し。Emit コアと分離 |
| VCD / Verilator バックエンド | Sim S3/S4 |

---

## チェックリスト（着手用）

### R0（先に閉じる）
- [ ] M3a ConstExpr API + GetWidth + 幅検証
- [ ] M3b Eval + Emit + テスト
- [ ] 版上げ（Core/Emit/Sim 同一）

### 次のリファクタ塊（推奨）
- [ ] R-E1 Emitter 統合
- [ ] R-E2 / R-D3 If・Comb 方針（仕様 0.2 済み・Emit の regOuts 収集が残）
- [ ] R-C1 / R-C2 CurrentWrite・Switch 親
- [x] R-E5 Mem 一意名（M5 前）— optional `name` + 省略時 `mem_{rdata}`

### Visitor（refactor-visitor）
- [x] R-V1 Stmt Visitor + Emit（#75 / PR #79）
- [x] R-V2 Sim（#76 / PR #80）
- [x] R-V3 Validate / StmtSupport 廃止（#77 / PR #81）
- [x] R-V4 Expr Visitor（#78 / PR #82）
- [ ] R-S4 ModuleFlat 再帰（#29、R-V2 後）

### 並行・低優先
- [ ] R-T1 / R-T3 / R-D1 / R-C6 / R-E9 / R-E8

---

## 改版履歴

| 版 | 日付 | 変更 |
|----|------|------|
| 1.5 | 2026-10-13 | R-V4（#78 / PR #82）完了。`IExprVisitor<T>` と `Expr.Accept<T>`、`Signal` 以外の Expr を sealed。Emit は `EmitExprVisitor`、Sim は `EvalExprVisitor`。`GetWidth` は Visitor にせず `Expr` の抽象メンバのまま（Core の構築時幅チェックで使うため。新 Expr 型は abstract なので実装漏れはコンパイルエラー）。Milestone refactor-visitor 完了 |
| 1.4 | 2026-10-10 | R-V3（#77 / PR #81）完了。`CheckMultiDriveVisitor`（If／Switch は枝ごとに数えて和集合を外側へ）、`StmtSupport` 廃止（トップレベル SeqAssign は `SimInitVisitor` で拒否） |
| 1.3 | 2026-10-09 | R-V2（#76 / PR #80）完了。Sim を `CombStmtVisitor`／`SeqStmtVisitor`＋`SeqBlockVisitor`／`SimInitVisitor` に移行し、Core `Walk` と `SimCombStmtHandler` を削除 |
| 1.2 | 2026-09-30 | R-S3 を部分完了（#74）に更新。「Visitor（R-S3 の完遂）」節と R-V1〜V4（#75〜#78、Milestone refactor-visitor）を追加。方式は古典的 Visitor に決定。R-S4 を R-V2 依存に。将来検討の R-S3 フォロー 3 件を R-V へ移管 |
| 1.1 | 2026-09-25 | 「将来検討（未 Issue）」追加。R-S3 循環参照・Emit 寄せ・Walk 拡張を記載（Issue 未作成） |
| 1.0 | 2026-09-18 | 初版。Emitter 計画＋Core/Sim/Tests/Docs の後回し設計を統合 |
