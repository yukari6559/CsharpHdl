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
- [ ] CLI emit
- [ ] プロジェクト参照または pack
- [ ] consumers.md の手順で MyOs から接続できる
