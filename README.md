# CsharpHdl — C# で書くハードウェア記述（→ Verilog）

**C# で回路を記述し、Verilog / SystemVerilog を生成する**ための独立ツールキットです。  
特定の OS や CPU プロジェクトに依存しません。FPGA・TinyTapeOut・MPW・学習用 SoC など、別件でも使えます。

実装は人間が行います。このリポジトリには仕様とガイドを置きます。

## できること / できないこと

| できる | できない |
|--------|----------|
| C# DSL で Reg / Wire / Comb / Seq を書く | 普通の C# アプリをそのまま FPGA に載せる |
| Verilog を生成して yosys / Vivado / TT に渡す | GC・CLR を回路化する |
| 複数プロジェクトからライブラリ参照 | 「魔法の HLS」で任意 C# を高速化 |

```
C# HDL 記述（このプロジェクト）
        ↓ emit
   Verilog
        ↓
  シミュレーション / FPGA / TinyTapeOut / MPW
```

## まず読む

1. [docs/INDEX.md](docs/INDEX.md)
2. [docs/getting-started.md](docs/getting-started.md)
3. [docs/guides/phase-0-foundation.md](docs/guides/phase-0-foundation.md)

## 利用者の例

| 利用者 | 使い方 |
|--------|--------|
| **MyOsProject** | SimpleRISC CPU を C# HDL で書き、生成 Verilog を `rtl/generated/` へ |
| 別の学習 CPU | 同じ DSL で別 ISA のコアを書く |
| TinyTapeOut 提出 | 縮小コアだけを記述して公式ラッパーに接続 |
| 周辺 IP | UART / タイマー等をモジュールとして再利用 |

## リポジトリ構成

```
CsharpHdl/
├── CsharpHdl.slnx        # ソリューション（.NET 10）
├── docs/                 # 仕様・ガイド（日本語）
├── src/
│   ├── SharpHdl.Core/    # AST・DSL API（あなたが実装）
│   ├── SharpHdl.Emit/    # Verilog emitter（あなたが実装）
│   ├── SharpHdl.Cli/     # 生成 CLI
│   └── SharpHdl.Tests/
├── examples/             # ALU・blink 等の記述例
└── out/generated/        # 生成物の出力先（gitignore）
```

## ビルド

```bash
cd CsharpHdl
dotnet build CsharpHdl.slnx
dotnet test CsharpHdl.slnx
dotnet run --project src/SharpHdl.Cli -- --version
```

## Quick Start（本線）

回路は **消費者側の C#** に書く。CLI だけで `.v` を吐くのはデモ用。

1. 隣のプロジェクトから [SharpHdl.Core](src/SharpHdl.Core) / [SharpHdl.Emit](src/SharpHdl.Emit) を ProjectReference（手順: [docs/consumers.md](docs/consumers.md)）  
2. 自分の `Module` を記述する（書き方の参考: `examples/`）  
3. Program またはテストから Emitter を呼び、`.v` を出力する  
4. 生成 Verilog をシミュレーション / FPGA / TinyTapeOut へ  

補助 CLI（examples 再生成など）: [docs/cli-spec.md](docs/cli-spec.md)

## 方針

- **同期設計・明示的なクロック／リセット**を前提にする
- 最初は機能を極小に（ALU → レジスタ → ステートマシン）
- 生成 Verilog は **人が読める**こと
- 消費者は **プロジェクト参照を推奨**（submodule / 必要なら NuGet）。C# コンパイルの恩恵は参照経路が本線
