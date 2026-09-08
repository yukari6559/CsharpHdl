# RISC-Sharp からの要望（宿題チケット）

消費者: [RISC-Sharp](../../RISC-Sharp)（RV64 CPU / FPGA / 自作 OS）  
方針: ISA は消費者側。CsharpHdl は原語だけ足す。

## T1 — nuget.org に pack / 公開する — **完了（クローズ）**

**優先度:** 高だった（RISC-Sharp は参照方式を nuget.org にしたい）

- [x] `SharpHdl.Core` / `SharpHdl.Emit` を `dotnet pack` できる（パッケージメタ付き）
- [x] nuget.org へ push し、公開ページから取れる（`0.1.0`）
- [x] [consumers.md](../consumers.md) に nuget.org での取り込み手順がある
- [x] 消費者が ProjectReference なし（PackageReference のみ）でビルドできる

公開パッケージ:

- https://www.nuget.org/packages/SharpHdl.Core/0.1.0
- https://www.nuget.org/packages/SharpHdl.Emit/0.1.0

**配布先:** nuget.org（公開）。手順の正本: [consumers.md](../consumers.md) の「C. NuGet（nuget.org）」。

RISC-Sharp 側の宿題表（`docs/phase-0-contract.md`）もクローズ済みに更新すること。

## T2 — RV64 CPU 向け原語（Phase 1 前）— **次**

**優先度:** 高（無いとコア記述が DSL をフォークし始める）

| ID | 原語 | 完了の目安 | 状態 |
|----|------|------------|------|
| T2a | 64bit の Reg / Wire（幅パラメータで 64 が自然に書ける） | 幅 64 の Module が emit できる | **完了**（`Width64Pass` テスト） |
| T2b | ビットスライス / 連結 | 命令デコードでフィールドが取れる | **着手中** — メソッド API（`Slice` / `Concat`）。仕様: [dsl-spec.md](../dsl-spec.md) |
| T2c | 2 読み 1 書きレジスタファイル（または同等の Mem／ポート） | x0〜x31 相当が書ける | 未 |
| T2d | バイト／部分書き込み | LB/SB 等のメモリが書ける | 未 |
| T2e | 符号拡張 | 即値・ロードの符号拡張が書ける | 未 |

実装詳細は CsharpHdl 側の裁量。RISC-Sharp は「使えること」だけ見る。

## 受け取り方

- チケット消化 → RISC-Sharp の `docs/phase-0-contract.md` の宿題表を更新
- 参照は nuget.org の PackageReference（T1 完了）
