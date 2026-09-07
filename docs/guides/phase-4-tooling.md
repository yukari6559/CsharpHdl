# Phase 4: 消費者接続・配布・CLI（補助）

## 目的

他リポジトリから **C# として参照して回路を書ける**形にする。  
C# コンパイル（型・API・幅チェック）の恩恵は、ここが本線。

## 本線と脇役

| 役割 | 何をするか |
|------|------------|
| **本線** | 消費者が Core / Emit を参照し、自分の `Module` を書いて Emitter で `.v` を出す |
| **配布** | **nuget.org** に `SharpHdl.Core` / `SharpHdl.Emit` を公開（T1・優先） |
| **脇役** | CLI は examples やデモ用の薄い出口（名前 → 既知 Module → `.v`） |

CLI だけだと「既にビルド済みの例を吐く」だけになり、コンパイルの恩恵は薄い。  
取り込み手順: [consumers.md](../consumers.md)。宿題: [tickets/riscv-sharp-requests.md](../tickets/riscv-sharp-requests.md)。

## やること

1. **T1:** Core / Emit を pack → nuget.org に push → PackageReference のみで消費者ビルドできることを確認  
2. [consumers.md](../consumers.md) を nuget.org 推奨手順に保つ  
3. README Quick Start を nuget.org（または暫定 ProjectReference）と一致させる  
4. [cli-spec.md](../cli-spec.md) の `emit` を **補助**として最小実装（既知 examples 一覧で可）  
5. （任意）生成 `.v` を Verilator でパース確認  

`.gitignore` の `out/` は済み。`artifacts/`（nupkg 出力）も gitignore 推奨。

## 完了条件

- [ ] nuget.org から Core / Emit を参照し、自前 Module を emit できる（T1）  
- [x] README の Quick Start が「ライブラリ参照 → Module → emit」と一致（参照手段は nuget.org へ更新中）  
- [ ] CLI で examples の少なくとも 1 つを `-o` に吐ける（補助・デモ用）  

これで CsharpHdl としては一通り。以降は消費者側の回路記述が主戦場（T2 原語など）。
