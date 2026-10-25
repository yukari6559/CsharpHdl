# CsharpHdl — C# で書くハードウェア記述（→ Verilog）

C# の DSL で同期回路を記述し、人が読める Verilog を生成するライブラリです。  
特定の OS・CPU・ISA には依存しません。

| できること | できないこと |
|------------|--------------|
| Reg / Wire / Comb / Seq、Mem、階層、スライス・拡張など | 任意の C# を HLS する |
| nuget.org から参照して消費者側で Module を書く | GC / CLR を回路化する |

**現状:** RV64 向け原語＋ Sim S2。比較・`If`/`Else`・幅付き定数 `Lit.Bits`（M1–M3）、ローカル `Wire` / `Reg`（M4）、XOR `^` / XNOR `Xnor` あり。`0.8.0` で符号なしの除算 `/` と剰余 `%` が入った。`0.7.0` で全幅の積 `MulFull` と論理積・論理和 `LogicAnd` / `LogicOr` が入り、インスタンスのポート接続は 1 行に 1 つになった。`0.6.0` で論理シフト `<<` `>>`・乗算 `*`・単項演算（`~` `-` `!`、リダクション `ReduceAnd()` など）が入り、階層の Verilog 出力（モジュールの区切り、3 段以上）が直った。二項演算・比較の左右は同じ幅が必須（`0.5.0` から。論理積・論理和とシフトの量は除く）。  
**ドキュメント:** [sharphdl.gitlab.io/SharpHDL](https://sharphdl.gitlab.io/SharpHDL)（ガイド・DSL・API リファレンス）  
**開発:** [GitLab sharphdl/SharpHDL](https://gitlab.com/sharphdl/SharpHDL)（Issue・マージリクエスト・CI）。GitHub の [yukari6559/CsharpHdl](https://github.com/yukari6559/CsharpHdl) は `main` とタグだけを同期するコピーです。  
**配布:** [SharpHdl.Core](https://www.nuget.org/packages/SharpHdl.Core) / [SharpHdl.Emit](https://www.nuget.org/packages/SharpHdl.Emit) / [SharpHdl.Sim](https://www.nuget.org/packages/SharpHdl.Sim)（例: `0.8.0`）。メンテナーの公開手順は [docs/consumers.md](docs/consumers.md)（タグ `v*.*.*` → CI）。

```
C# Module（消費者プロジェクト）
        ↓ emit / または SharpHdl.Sim
   Verilog または C# テスト
```

## Quick Start

1. PackageReference（詳細: [docs/consumers.md](docs/consumers.md)）

```xml
<PackageReference Include="SharpHdl.Core" Version="0.8.0" />
<PackageReference Include="SharpHdl.Emit" Version="0.8.0" />
<!-- 回路を xUnit 等で回すとき -->
<PackageReference Include="SharpHdl.Sim" Version="0.8.0" />
```

2. 自分の `Module` を書く（参考: [`examples/`](examples/)）  
3. `VerilogEmitter` で `.v` を出す、または `module.Run()` でシミュする  

## ドキュメント

- [docs/INDEX.md](docs/INDEX.md)
- [docs/getting-started.md](docs/getting-started.md)
- [docs/dsl-spec.md](docs/dsl-spec.md)

## CI/CD コンポーネント（GitLab）

自分のプロジェクトで出力した Verilog を、GitLab CI で Verilator と Icarus Verilog にかけるコンポーネントを公開しています（[CI/CD Catalog](https://gitlab.com/explore/catalog)）。エラーがあるとジョブが失敗し、ファイルごとの結果が MR のテスト結果に出ます。

```yaml
include:
  - component: gitlab.com/sharphdl/SharpHDL/verilog-lint@v0.8.0
    inputs:
      emit_command: dotnet run --project tools/EmitVerilog -- artifacts/verilog
```

| 入力 | 既定値 | 内容 |
|------|--------|------|
| `emit_command` | （必須） | `verilog_dir` に `.v` を書き出すコマンド |
| `verilog_dir` | `artifacts/verilog` | 出力先。JUnit の結果もここに書かれる |
| `image` | `mcr.microsoft.com/dotnet/sdk:10.0` | `emit_command` を実行できる Debian / Ubuntu 系のイメージ |
| `allow_failure` | `false` | 既知のエラーを直す間だけパイプラインを止めないとき `true` |
| `verilator_flags` | `-Wall` | `verilator --lint-only` に渡す警告のフラグ（例: ファイル名とモジュール名が違うとき `-Wall -Wno-DECLFILENAME`） |
| `job_name` / `stage` | `verilog-lint` / `test` | ジョブ名とステージ |

版はリリースのタグ（`v0.8.0` 以降）を指定します。

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
