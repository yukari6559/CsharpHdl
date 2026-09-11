# はじめに

**CsharpHdl** は、C# で同期回路を記述し Verilog を生成するライブラリです。  
特定の OS や CPU プロジェクトには依存しません。

## 要件

- .NET 10 SDK

## このリポジトリをビルドする

```bash
dotnet build CsharpHdl.slnx
dotnet test CsharpHdl.slnx
```

## 回路を書く（本線）

1. [SharpHdl.Core](https://www.nuget.org/packages/SharpHdl.Core) / [SharpHdl.Emit](https://www.nuget.org/packages/SharpHdl.Emit) を PackageReference する（手順: [consumers.md](consumers.md)）
2. 自分のプロジェクトに `Module` を書く（参考: [`examples/`](../examples/)）
3. `VerilogEmitter.Emitter` などで `.v` を出力する
4. 生成 Verilog をシミュレーション / FPGA フローへ渡す

DSL の意味論: [dsl-spec.md](dsl-spec.md)
