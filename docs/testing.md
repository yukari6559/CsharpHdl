# テスト戦略

| 層 | 内容 |
|----|------|
| 単体 | AST 構築、幅チェック、Emitter 文字列スナップショット |
| ゴールデン | 手書き Verilog と生成結果の（整形後）一致、または双方シミュレーション一致 |
| ツール | Verilator で lint / 実行 |
| 回帰 | `examples/alu` `examples/blink` が常に通る |

## 必須ケース（Phase 1〜2）

- [x] ALU add/sub/and/or  
- [x] 同期カウンタ（rst で 0）  
- [x] 幅不一致で Assign が失敗する（Comb / Seq）  
- [x] 生成ファイルが空でない  

## Integration

結果は [integration-log.md](integration-log.md) に残す。
