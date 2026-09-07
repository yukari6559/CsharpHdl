# CsharpHdl ドキュメント索引

実装は人間が行う。ここは仕様と手順の入口。

## 読む順

| 順 | 文書 | 目的 |
|----|------|------|
| 1 | [getting-started.md](getting-started.md) | 何を作るか・最初の週 |
| 2 | [ROADMAP.md](ROADMAP.md) | フェーズ全体（Phase 0–4） |
| 2b | [long-term-plan.md](long-term-plan.md) | 長期計画（Phase A–H: VSCode / FPGA / 合成探究） |
| 3 | [dsl-spec.md](dsl-spec.md) | DSL の契約（API イメージ） |
| 4 | [verilog-emit-spec.md](verilog-emit-spec.md) | 生成 Verilog の約束 |
| 5 | [guides/phase-0-foundation.md](guides/phase-0-foundation.md) | 実装開始 |

## 仕様（Contract）

| 文書 | 内容 |
|------|------|
| [dsl-spec.md](dsl-spec.md) | Module / Signal / Comb / Seq 等 |
| [verilog-emit-spec.md](verilog-emit-spec.md) | 出力 Verilog の形 |
| [cli-spec.md](cli-spec.md) | CLI の入出力 |
| [testing.md](testing.md) | テスト戦略 |

## ガイド

| 文書 | 内容 |
|------|------|
| [guides/README.md](guides/README.md) | ガイド一覧 |
| [guides/phase-0-foundation.md](guides/phase-0-foundation.md) | 方針固め |
| [guides/phase-1-alu.md](guides/phase-1-alu.md) | 文字列 or AST で ALU 生成 |
| [guides/phase-2-dsl.md](guides/phase-2-dsl.md) | DSL + Emitter |
| [guides/phase-2-how-to-think.md](guides/phase-2-how-to-think.md) | Phase 2 の考え方・小さな課題とヒント（コードなし） |
| [guides/phase-3-cpu-ready.md](guides/phase-3-cpu-ready.md) | CPU 向け機能（Mem / 階層） |
| [guides/phase-4-tooling.md](guides/phase-4-tooling.md) | nuget.org 配布（T1）・消費者 emit・CLI 補助 |

## 長期計画

| 文書 | 内容 |
|------|------|
| [long-term-plan.md](long-term-plan.md) | VSCode 統合・ピン Attribute・マルチバックエンド・合成探究 |
| [ROADMAP.md](ROADMAP.md) § Phase 5+ | 長期計画の概要（本計画へのリンク） |

## 運用

| 文書 | 内容 |
|------|------|
| [phase-checklist.md](phase-checklist.md) | チェックリスト |
| [repo-layout.md](repo-layout.md) | ディレクトリ |
| [glossary.md](glossary.md) | 用語 |
| [consumers.md](consumers.md) | nuget.org / 参照での使い方 |
| [tickets/riscv-sharp-requests.md](tickets/riscv-sharp-requests.md) | RISC-Sharp 宿題（T1 nuget.org 等） |
| [integration-log.md](integration-log.md) | 記録 |

## 迷ったら

```
「Reg はどう書く？」     → dsl-spec.md
「assign になる条件は？」 → verilog-emit-spec.md
「今週何を作る？」       → guides/phase-N-*.md
「Signal で止まった」    → guides/phase-2-how-to-think.md
「MyOs / RISC-Sharp からどう使う？」 → consumers.md
「宿題チケットは？」 → tickets/riscv-sharp-requests.md
```
