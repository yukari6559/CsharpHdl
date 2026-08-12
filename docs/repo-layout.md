# リポジトリ構成

```
CsharpHdl/
├── README.md
├── docs/                    # 仕様・ガイド
├── src/
│   ├── SharpHdl.Core/       # DSL・AST（実装する）
│   ├── SharpHdl.Emit/       # Verilog 出力（実装する）
│   ├── SharpHdl.Cli/        # CLI（実装する）
│   └── SharpHdl.Tests/      # テスト（実装する）
├── examples/
│   ├── alu/                 # 記述例（実装する）
│   └── blink/
└── out/generated/           # emit 出力（gitignore 推奨）
```

| 置き場 | 内容 |
|--------|------|
| Core | Module, Signal, Comb, Seq |
| Emit | `EmitVerilog(Module) → string` |
| Cli | emit コマンド |
| examples | 消費者向けサンプル。製品 ISA は置かない |
| out/ | 生成物。コミットしない |
