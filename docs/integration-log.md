# 統合ログ

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
