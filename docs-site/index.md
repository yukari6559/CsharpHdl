# SharpHdl

C# で同期回路を記述し、人が読める Verilog を生成するライブラリです。C# のサイクルシミュレータ（xUnit からそのまま回せる）も含みます。

| パッケージ | 内容 |
|------------|------|
| [SharpHdl.Core](https://www.nuget.org/packages/SharpHdl.Core) | DSL（`Module`、`In` / `Out` / `Wire` / `Reg`、Comb / Seq、メモリ、階層） |
| [SharpHdl.Emit](https://www.nuget.org/packages/SharpHdl.Emit) | Verilog-2001 の出力 |
| [SharpHdl.Sim](https://www.nuget.org/packages/SharpHdl.Sim) | `Run` / `Set` / `Get` / `Settle` / `Advance` によるシミュレーション |

- はじめに: [Getting started](../docs/getting-started.md)
- DSL の仕様（概要）: [DSL](../docs/dsl-spec.md)
- 利用者向けの手順（パッケージの参照・開発版・公開手順）: [Consumers](../docs/consumers.md)
- API リファレンス: [API](api/index.md)

ソースコード・Issue・マージリクエストは [GitLab sharphdl/SharpHDL](https://gitlab.com/sharphdl/SharpHDL) にあります。
