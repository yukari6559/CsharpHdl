# はじめに

**CsharpHdl** は、C# でハードウェアを記述し Verilog を出すための **独立プロジェクト** です。  
OS 自作（MyOsProject）とはフォルダもリポジトリも別です。先にこちらを育て、後から複数案件で再利用します。

## 1 週間の目標

- [ ] INDEX / ROADMAP / dsl-spec を通読
- [ ] 「普通の C# ≠ 回路」を説明できる
- [ ] Phase 0 の完了条件を満たす
- [ ] （任意）空の .NET ライブラリを `src/SharpHdl.Core` に作る

## なぜ独立か

| 理由 | 説明 |
|------|------|
| 再利用 | MyOs 以外の CPU / TT 提出 / IP でも使う |
| 依存の向き | OS → HDL ツール。HDL が OS を知らない |
| 開発順序 | ジェネレータを先に安定させ、消費側は後から接続 |

## 成果物のイメージ（完成時）

本線は **nuget.org の Core / Emit を参照**し、自分の `Module` を書いて Emitter で `.v` を出すこと（[consumers.md](consumers.md)）。  
公開前は暫定で ProjectReference 可。C# の型チェックや幅エラーは、そのビルド時に効く。

```bash
# 補助: リポジトリ内 examples を CLI で吐く（デモ用）
dotnet run --project src/SharpHdl.Cli -- emit alu -o out/generated/alu.v
```

生成された `.v` を Verilator / yosys に渡す。

## 次

→ [guides/phase-0-foundation.md](guides/phase-0-foundation.md)  
Phase 4 / T1 なら → [guides/phase-4-tooling.md](guides/phase-4-tooling.md) / [consumers.md](consumers.md) / [tickets/riscv-sharp-requests.md](tickets/riscv-sharp-requests.md)
