# CsharpHdl — C# で書くハードウェア記述（→ Verilog）

C# の DSL で同期回路を記述し、人が読める Verilog を生成するライブラリです。  
特定の OS・CPU・ISA には依存しません。

| できること | できないこと |
|------------|--------------|
| Reg / Wire / Comb / Seq、Mem、階層、スライス・拡張など | 任意の C# を HLS する |
| nuget.org から参照して消費者側で Module を書く | GC / CLR を回路化する |

**現状:** RV64 向けの基本原語（64bit 幅、Slice/Concat、2R1W Mem、バイト書き、Sign/Zero 拡張）まで。  
**配布:** [SharpHdl.Core](https://www.nuget.org/packages/SharpHdl.Core) / [SharpHdl.Emit](https://www.nuget.org/packages/SharpHdl.Emit)（例: `0.1.0`）

```
C# Module（消費者プロジェクト）
        ↓ emit
   Verilog
        ↓
  シミュレーション / FPGA / その他
```

## Quick Start

1. PackageReference（詳細: [docs/consumers.md](docs/consumers.md)）

```xml
<PackageReference Include="SharpHdl.Core" Version="0.1.0" />
<PackageReference Include="SharpHdl.Emit" Version="0.1.0" />
```

2. 自分の `Module` を書く（参考: [`examples/`](examples/)）  
3. `VerilogEmitter` で `.v` を出力する  

## ドキュメント

- [docs/INDEX.md](docs/INDEX.md)
- [docs/getting-started.md](docs/getting-started.md)
- [docs/dsl-spec.md](docs/dsl-spec.md)

## このリポジトリをビルドする

```bash
dotnet build CsharpHdl.slnx
dotnet test CsharpHdl.slnx
```

`SharpHdl.Cli` は任意のスタブです。本線はライブラリ参照です。

## 方針

- 同期設計・明示的なクロック／リセット
- 生成 Verilog は人が読めること
- 消費者は nuget.org の PackageReference を推奨

## License

[MIT](LICENSE)
