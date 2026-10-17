# チェックリスト

## Phase 0
- [x] 独立リポジトリ作成
- [x] docs 骨格
- [x] .NET ソリューション / プロジェクト作成（Core / Emit / Cli / Tests）
- [ ] DSL 範囲を自分の言葉で説明できる
- [ ] Phase 1: ALU を生成（実装）

## Phase 1
- [ ] ALU を C# から Verilog 生成
- [ ] Verilator または同等で確認

## Phase 2
- [ ] AST + DSL API
- [ ] Comb / Seq
- [ ] スナップショットテスト

## Phase 3
- [x] Mem 1R1W（emit・幅チェック・テスト。Verilator は Phase 4）
- [x] 階層モジュール

## Phase 4
- [x] T1: nuget.org に Core / Emit を公開し PackageReference でビルド
- [x] README Quick Start が本線と一致（参照手段は nuget.org）
- [ ] CLI emit（examples 補助・最小で可）
- [ ] 消費者で自前 Module を emit（任意・接続確認の延長）
- [x] T2: RV64 向け原語（tickets 参照）

## Sim（S1 → S2）— ゲート完了

正本: [sim-spec.md](sim-spec.md) / [sim-impl.md](sim-impl.md)

- [x] S1: Comb + Seq、`Run`/`Settle`/`Advance`、Counter/Alu
- [x] S2: Instance エイリアス、Mem 1R1W / wstrb / 2R1W
- [x] S2: LoadMem / PeekMem / PokeMem（rdata キー・階層・LE）
- [x] S2: AdvanceWhile（maxCycles 超過は例外）
- [x] **GS2:** S2 完了 + nuget 版上げ（`0.3.5`）

## MidiSynth 宿題（L1）— いまの本線

正本: [tickets/midisynth-tang-primer25k-requests.md](tickets/midisynth-tang-primer25k-requests.md)  
位置づけ: [ROADMAP.md](ROADMAP.md)

- [x] M1a 比較（`Eq`/`Neq` + `< <= > >=`、結果 1 bit）— `0.3.6`
- [x] M1b `If` / `Else`（名前付き `then:` / `@else:`）— `0.3.7`
- [x] M1c Switch と条件分岐の役割分担を dsl-spec に明記
- [x] M3 定数 Expr — `0.3.8`
- [ ] [refactor-plan.md](refactor-plan.md) R-E1 Emitter 統合など — **次候補**（Issue #4）
- [x] GitHub Milestone / Issue 先行作成（v0.3.8 / refactor-emitter / m4 / m2 / m5）— 以降はブランチ+PR（[dev-workflow.md](dev-workflow.md)）
- [x] M4 ローカル Wire/Reg — PR #85 / #100
- [ ] M2 乗算・シフト・^/~ — 進行中（`^` / `~^` は PR #103。#102 済み。先に #105 / #104、単項は #101）
- [ ] M5 波形 ROM 初期化
- [ ] **GM:** M1–M5 完了（単音シンセがフォークなしで書ける）
- [ ] （任意）M6 / M7
