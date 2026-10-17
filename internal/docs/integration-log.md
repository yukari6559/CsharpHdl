# 統合ログ

## 2026-09-18 — エージェントが GitHub 作業フローを担当すると明記

### 実施
- [x] [dev-workflow.md](dev-workflow.md) v1.2 / `.cursor/rules/dev-workflow.mdc`
- [x] 作業開始時の checkout・Issue、完了時の PR・コメント・委任時マージをエージェント担当に

## 2026-09-18 — GitHub Milestone / Issue を先行作成

### 実施
- [x] Labels: feat / refactor / docs / test / chore / core / emit / sim
- [x] Milestones: `v0.3.8`, `refactor-emitter`, `m4-local-signals`, `m2-ops`, `m5-rom-init`
- [x] Issues #2–#10（M3・版上げ・R-E1/E2/E5・M4a/b・M2・M5）
- URL: https://github.com/yukari6559/CsharpHdl/milestones

## 2026-09-18 — ワークフロー粒度を王道に合わせて更新

### 実施
- [x] [dev-workflow.md](dev-workflow.md) v1.1: Milestone→Issue→PR、細ステップはチェックリスト
- [x] 元提案の「サブIssue＝1コミット」は非採用と明記。テーマ束ねの Milestone は採用
- [x] `.cursor/rules/dev-workflow.mdc` を同期。タスク生成フローを必須化

## 2026-09-18 — M3 後の開発プロセス（Issue / PR）を文書化

### 実施
- [x] [dev-workflow.md](dev-workflow.md) 新設
- [x] Cursor ルール `.cursor/rules/dev-workflow.mdc`（alwaysApply）
- [x] INDEX / ROADMAP / refactor-plan / checklist / midisynth 受け取り方からリンク

### 仕様
- M3 版上げ完了を切替点。以降は Issue + ブランチ + PR。内部 tickets は仕様正本のまま

## 2026-09-18 — リファクタ計画を文書化

### 実施
- [x] [refactor-plan.md](refactor-plan.md) 新設（Emit/Core/Sim/Tests/Docs の負債 ID）
- [x] INDEX / ROADMAP / phase-checklist からリンク
- [x] M3 完了まで大規模 Emitter リファクタは後続と明記

### 仕様
- 最初の塊: R0(M3) → R-E1 → R-E2 → R-C1/C2。M5 前に R-E5

## 2026-09-17 — M1c If/Switch 使い分けを dsl-spec に固定

### 実施
- [x] [dsl-spec.md](dsl-spec.md) v0.3: If vs Switch 表、現状制限、Switch 凡例
- [x] 公開 [docs/dsl-spec.md](../../docs/dsl-spec.md) に要約を追記
- [x] midisynth チケット M1c / checklist / ROADMAP 更新

### 仕様
- いまの本線: **M3（定数）/ M4（ローカル線）**

## 2026-09-17 — M1b If/Else 完了・`0.3.7`

### 実施
- [x] Core `IfStmt` / `Module.If(then:/@else:)`、Sim CombSettle、Emit `always @(*)`、テスト
- [x] nuget 版上げ Core/Emit/Sim **`0.3.7`**

### 仕様
- いまの本線: **M1c（仕様）→ M3/M4 → M2 → M5**

## 2026-09-17 — S2 / M1a 完了を文書横断で反映

### 実施
- [x] GS2（Sim S2）完了を ROADMAP / phase-checklist / sim-impl / sim-spec / long-term-plan に反映（版 `0.3.5`）
- [x] M1a 比較完了（`Eq`/`Neq` + 関係演算、版 `0.3.6`）を midisynth チケット・チェックリストに反映
- [x] いまの本線を **M1b If/Else（名前付き then / @else）** に更新
- [x] dsl-spec に比較 API と If 方針を追記

### 仕様
- いまの本線: **M1b → M1c → (M3/M4) → M2 → M5**
- S2 ゲートは閉じた。RISC-Sharp / MidiSynth とも Sim 利用可

## 2026-09-16 — MidiSynth 宿題をロードマップ本線に組み込み

### 実施
- [x] [tickets/midisynth-tang-primer25k-requests.md](tickets/midisynth-tang-primer25k-requests.md) を正とし、S2 完了後の本線として文書横断で固定
- [x] ROADMAP / sim-spec / sim-impl / phase-checklist / long-term-plan / INDEX / consumers を更新

### 仕様
- 当時の本線: **S2 残りを閉じる → M1 → (M3/M4) → M2 → M5**
- （その後 09-17: S2・M1a 完了。本線は M1b へ）

## 2026-09-07 — T1 クローズ（nuget.org `0.1.0`）

### 実施
- [x] csproj パッケージメタ + LICENSE + `dotnet pack`
- [x] nuget.org へ push（Core / Emit `0.1.0`）
- [x] consumers.md / README に nuget.org 手順
- [x] 消費者 PackageReference のみでビルド確認 → **T1 クローズ**

### 次
- [ ] T2 RV64 向け原語（T2a 幅64 から）
- [ ] CLI emit（補助）

パッケージ:
- https://www.nuget.org/packages/SharpHdl.Core/0.1.0
- https://www.nuget.org/packages/SharpHdl.Emit/0.1.0

## 2026-09-07 — T1 方針: 配布先は nuget.org

### 実施
- [x] docs: 推奨取り込みを nuget.org（PackageReference）に変更
- [x] 私有フィードは使わない旨をチケット / consumers / phase-4 に反映

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
