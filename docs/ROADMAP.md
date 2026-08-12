# CsharpHdl ロードマップ

## ゴール

1. C# DSL で同期回路を記述できる  
2. 人が読める Verilog を生成できる  
3. NuGet またはプロジェクト参照で **他リポジトリから使える**  
4. 例: ALU、点滅、（消費者側で）CPU コア  

## フェーズ

| Phase | 内容 | ガイド | 目安 |
|-------|------|--------|------|
| 0 | 方針・用語・DSL 範囲の確定 | [phase-0](guides/phase-0-foundation.md) | 数日〜1 週 |
| 1 | ALU を生成（StringBuilder でも可） | [phase-1](guides/phase-1-alu.md) | 1〜2 週 |
| 2 | AST + DSL + Emitter | [phase-2](guides/phase-2-dsl.md) | 2〜4 週 |
| 3 | Mem・階層・CPU 向け部品 | [phase-3](guides/phase-3-cpu-ready.md) | 3〜6 週 |
| 4 | CLI・パッケージ・消費者連携 | [phase-4](guides/phase-4-tooling.md) | 1〜2 週 |

## マイルストーン

| ID | 到達点 |
|----|--------|
| G0 | 手で書いた Verilog と同等の ALU を C# から出力 |
| G1 | `Module` DSL で ALU / カウンタ |
| G2 | 1R1W メモリ + 階層モジュール |
| G3 | `dotnet pack` またはプロジェクト参照で MyOsProject から利用 |
| G4 | examples が Verilator で通る |

## やらないこと（初期）

- 任意 C# の HLS 変換
- パイプライン自動挿入
- SystemVerilog の高度な機能全対応
- GUI

## 消費者との関係

```
CsharpHdl（本リポジトリ）
    ↑ 参照
MyOsProject / その他プロジェクト
```

詳細: [consumers.md](consumers.md)
