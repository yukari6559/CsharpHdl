# Phase 4: CLI・配布・消費者接続

## 目的

他リポジトリから安心して使える形にする。

## やること

1. [cli-spec.md](../cli-spec.md) の `emit` を実装  
2. `dotnet pack` またはプロジェクト参照手順を確定（[consumers.md](../consumers.md)）  
3. MyOsProject から参照し、空の Module 1 つを生成できることを確認  
4. `.gitignore` に `out/` を追加  

## 完了条件

- [ ] CLI で examples が生成できる  
- [ ] 消費者ドキュメント通りに MyOs（または別件）から参照できる  
- [ ] README の Quick Start が実手順と一致  

これで CsharpHdl としては一通り。以降は消費者側の回路記述が主戦場。
