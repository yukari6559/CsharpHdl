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
- [ ] T2: RV64 向け原語（tickets 参照）
