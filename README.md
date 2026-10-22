# CsharpHdl — C# で書くハードウェア記述（→ Verilog）

C# の DSL で同期回路を記述し、人が読める Verilog を生成するライブラリです。  
特定の OS・CPU・ISA には依存しません。

| できること | できないこと |
|------------|--------------|
| Reg / Wire / Comb / Seq、Mem、階層、スライス・拡張など | 任意の C# を HLS する |
| nuget.org から参照して消費者側で Module を書く | GC / CLR を回路化する |

**現状:** RV64 向け原語＋ Sim S2。比較・`If`/`Else`・幅付き定数 `Lit.Bits`（M1–M3）、ローカル `Wire` / `Reg`（M4）、XOR `^` / XNOR `Xnor` あり。`0.5.0` で二項演算・比較の左右は同じ幅が必須になり、`Slice` / `SignExtend` は信号（または信号のスライス）にだけ付けられるようになった（Sim と Verilog の食い違いを防ぐため）。次は演算子の穴埋め（シフト・乗算など）。  
**開発:** [GitLab sharphdl/SharpHDL](https://gitlab.com/sharphdl/SharpHDL)（Issue・マージリクエスト・CI）。GitHub の [yukari6559/CsharpHdl](https://github.com/yukari6559/CsharpHdl) は `main` とタグだけを同期するコピーです。  
**配布:** [SharpHdl.Core](https://www.nuget.org/packages/SharpHdl.Core) / [SharpHdl.Emit](https://www.nuget.org/packages/SharpHdl.Emit) / [SharpHdl.Sim](https://www.nuget.org/packages/SharpHdl.Sim)（例: `0.5.0`）。メンテナーの公開手順は [docs/consumers.md](docs/consumers.md)（タグ `v*.*.*` → CI）。

```
C# Module（消費者プロジェクト）
        ↓ emit / または SharpHdl.Sim
   Verilog または C# テスト
```

## Quick Start

1. PackageReference（詳細: [docs/consumers.md](docs/consumers.md)）

```xml
<PackageReference Include="SharpHdl.Core" Version="0.5.0" />
<PackageReference Include="SharpHdl.Emit" Version="0.5.0" />
<!-- 回路を xUnit 等で回すとき -->
<PackageReference Include="SharpHdl.Sim" Version="0.5.0" />
```

2. 自分の `Module` を書く（参考: [`examples/`](examples/)）  
3. `VerilogEmitter` で `.v` を出す、または `module.Run()` でシミュする  

## ドキュメント

- [docs/INDEX.md](docs/INDEX.md)
- [docs/getting-started.md](docs/getting-started.md)
- [docs/dsl-spec.md](docs/dsl-spec.md)

## このリポジトリをビルドする

```bash
dotnet build CsharpHdl.slnx
dotnet test CsharpHdl.slnx
```

## 方針

- 同期設計・明示的なクロック／リセット
- 生成 Verilog は人が読めること
- 消費者は nuget.org の PackageReference を推奨

## License

[MIT](LICENSE)
