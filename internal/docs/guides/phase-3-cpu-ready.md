# Phase 3: CPU / SoC 向け機能

## 目的

消費者が CPU コアを書ける最低限を足す。**特定 ISA はここには置かない。**

## 階層モジュール（完了）

| 部品 | 場所 |
|------|------|
| `PortConnection`, `InstanceStmt` | `SharpHdl.Core` |
| `Module.Instance(...)` | `SharpHdl.Core` |
| 子→親連結 + インスタンス行 emit | `SharpHdl.Emit` |
| `AluTop` / `DualAluTop` テスト | `SharpHdl.Tests` |
| 階層例 | `examples/alu-top/` |

### 完了条件（階層）

- [x] モジュール階層とポート接続  
- [x] 階層 2 段の例が emit できる（`AluTop`・`DualAluTop`）  

## Mem 1R1W（完了・emit）

| 部品 | 場所 |
|------|------|
| `MemStmt` / `Module.Mem` | `SharpHdl.Core` |
| 同期 `reg` 配列 + `always @(posedge)` | `SharpHdl.Emit` |
| seq/Mem 出力の `output reg` 判定 | `SharpHdl.Emit` |
| `SimpleRam` テスト | `SharpHdl.Tests` |
| 例 | `examples/mem/` |

### 完了条件（Mem）

- [x] 同期 1R1W を DSL で記述し emit できる  
- [x] 幅チェック（`we`/`addr`/`wdata`/`rdata`・`depth`/`clk`）  
- [ ] Verilator 等でのシミュレーション — Phase 4 回帰で可  

## 追加機能（任意・後回し）

- [ ] ビットスライス / 連結（必要なら）  
- [ ] 複数ファイルまたは連結出力  

## 例

`examples/` に「汎用」なだけのもの:

- レジスタファイル風（深さ・幅パラメータ）→ `examples/mem`
- フェッチ無しのステートマシン雛形  

SimpleRISC 本体は MyOsProject 側。

## 完了条件（フェーズ全体）

- [x] 階層 2 段の例が emit できる  
- [x] Mem 例が emit できる（シミュレーションは Phase 4）  

→ 次: [phase-4-tooling.md](phase-4-tooling.md)
