# 統合ログ

## 2026-09-07 — Phase 4 方針: 消費者参照が本線

### 実施
- [x] docs / README: 本線＝ProjectReference＋自前 Module、CLI は補助
- [x] `consumers.md` / `cli-spec.md` / `phase-4-tooling.md` を同方針に更新
- [x] README Quick Start を本線に合わせる

### 残り
- [ ] MyOs（等）から参照して自前 Module を emit
- [ ] CLI emit（examples・最小）
- [ ] pack は必要になったら

## 2026-09-07 — Phase 3 Mem（1R1W）emit 完了

### 実施
- [x] `MemStmt` / `Module.Mem(clk, depth, width, we, addr, wdata, rdata)`
- [x] Emitter: `reg [...] mem [...]` + 同期 read/write
- [x] seq/Mem 駆動ポートを `output reg` に分類（Counter スナップショット更新）
- [x] `SimpleRam` テスト 2 件、`examples/mem`
- [x] `Module.Mem` 幅・depth チェック（`WidthMismatchException`）

### 仕様
- 読みレイテンシ 1 サイクル（`rdata <= mem[addr]`）
- `Module.Mem` 入口で幅・depth チェック（`WidthMismatchException`）
- Verilator は未（Phase 4）

## 2026-09-04 — Phase 3 階層モジュール完了（docs 完了印）

### 実施
- [x] `PortConnection` / `InstanceStmt` / `Module.Instance`
- [x] Emitter: 子→親連結 + インスタンス行、二重 `module` 防止
- [x] `AluTop` / `DualAluTop` テスト、`examples/alu-top`
- [x] [phase-3-cpu-ready.md](guides/phase-3-cpu-ready.md) に階層の完了印

### 仕様
- Phase 3 全体は未完了（Mem 残り）。階層部分のみフェーズ内で区切り

## 2026-09-03 — Phase 2 完了（幅不一致）

### 実施
- [x] `Expr.GetWidth()`（Signal = Width、OpExpr = Left）
- [x] `WidthMismatchException` と Assign 時チェック（Comb / Seq）
- [x] BadWidth テスト（8←32）で Comb / Seq の失敗を固定

### 仕様
- 検出タイミングは代入時。演算結果幅は左オペランド幅

## 2026-09-02 — 長期開発計画ドキュメント追加

### 実施
- [x] `docs/long-term-plan.md` を追加（VSCode 統合・ピン Attribute・マルチバックエンド・合成探究）
- [x] `docs/INDEX.md` / `docs/ROADMAP.md` にリンクと Phase 5+ 概要を追記

### 仕様
- Phase 0–4 は既存 ROADMAP のまま。Phase A–H は長期計画として分離
- 実装は人手主体。エージェントは明示指示時のみ

## 2026-08-12 — リポジトリ分離

### 実施
- [x] `/home/araiguma/workspace/CsharpHdl` を MyOsProject から独立して作成
- [x] ドキュメント初版

### 仕様
- C# HDL は OS プロジェクトに埋め込まず、消費者が参照する
