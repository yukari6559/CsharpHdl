# Phase 4: 消費者接続・配布・CLI（補助）

## 目的

他リポジトリから **C# として参照して回路を書ける**形にする。  
C# コンパイル（型・API・幅チェック）の恩恵は、ここが本線。

## 本線と脇役

| 役割 | 何をするか |
|------|------------|
| **本線** | 消費者が Core / Emit を参照し、自分の `Module` を書いて Emitter で `.v` を出す |
| **脇役** | CLI は examples やデモ用の薄い出口（名前 → 既知 Module → `.v`） |
| **後回し** | NuGet `pack`（隣ディレクトリの ProjectReference で足りるうちは不要） |

CLI だけだと「既にビルド済みの例を吐く」だけになり、コンパイルの恩恵は薄い。  
詳細な取り込み手順は [consumers.md](../consumers.md)。

## やること

1. [consumers.md](../consumers.md) の **プロジェクト参照**手順を確定し、MyOs（または最小の試しプロジェクト）から空に近い `Module` 1 つを emit できることを確認  
2. README Quick Start を上記本線に合わせる  
3. [cli-spec.md](../cli-spec.md) の `emit` を **補助**として最小実装（既知 examples 一覧で可）  
4. 必要になったら `dotnet pack`（私有フィードまたはローカル nupkg）  
5. （任意）生成 `.v` を Verilator でパース確認  

`.gitignore` の `out/` は済み。

## 完了条件

- [ ] 消費者ドキュメント通りに MyOs（または別件）から参照し、自前 Module を emit できる  
- [x] README の Quick Start が実手順（参照 → Module → emit）と一致  
- [ ] CLI で examples の少なくとも 1 つを `-o` に吐ける（補助・デモ用）  

これで CsharpHdl としては一通り。以降は消費者側の回路記述が主戦場。
