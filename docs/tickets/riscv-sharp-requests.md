# RISC-Sharp からの要望（宿題チケット）

消費者: [RISC-Sharp](../../RISC-Sharp)（RV64 CPU / FPGA / 自作 OS）  
方針: ISA は消費者側。CsharpHdl は原語だけ足す。

## T1 — nuget.org に pack / 公開する

**優先度:** 高（RISC-Sharp は参照方式を **nuget.org** にしたい）

- [x] `SharpHdl.Core` / `SharpHdl.Emit` を `dotnet pack` できる（パッケージメタ付き）
- [x] nuget.org へ push し、公開ページから取れる（`0.1.0`）
- [x] [consumers.md](../consumers.md) に nuget.org での取り込み手順がある
- [ ] 消費者が ProjectReference なし（PackageReference のみ）でビルドできる

公開パッケージ:

- https://www.nuget.org/packages/SharpHdl.Core/0.1.0
- https://www.nuget.org/packages/SharpHdl.Emit/0.1.0

（索引反映前は検索に出なくても、URL・版指定の restore は可能なことが多い）

**配布先:** nuget.org（公開）。  
公開後は RISC-Sharp 側を PackageReference に切り替えて上記の残り1項を閉じる。

## T2 — RV64 CPU 向け原語（Phase 1 前）

**優先度:** 高（無いとコア記述が DSL をフォークし始める）

| ID | 原語 | 完了の目安 |
|----|------|------------|
| T2a | 64bit の Reg / Wire（幅パラメータで 64 が自然に書ける） | 幅 64 の Module が emit できる |
| T2b | ビットスライス / 連結 | 命令デコードでフィールドが取れる |
| T2c | 2 読み 1 書きレジスタファイル（または同等の Mem／ポート） | x0〜x31 相当が書ける |
| T2d | バイト／部分書き込み | LB/SB 等のメモリが書ける |
| T2e | 符号拡張 | 即値・ロードの符号拡張が書ける |

実装詳細は CsharpHdl 側の裁量。RISC-Sharp は「使えること」だけ見る。

## 受け取り方

- チケット消化 → RISC-Sharp の `docs/phase-0-contract.md` の宿題表を更新
- nuget.org 公開後 → RISC-Sharp の参照を ProjectReference から PackageReference へ
