# Phase 2: DSL + Emitter

## 目的

[dsl-spec.md](../dsl-spec.md) に沿った API と、[verilog-emit-spec.md](../verilog-emit-spec.md) に沿った Emitter を実装する。

**進め方が分からないとき** → [phase-2-how-to-think.md](phase-2-how-to-think.md)（§2.5: 小さな課題とヒント）

## 実装するもの

| 部品 | 場所 |
|------|------|
| `Module`, `Signal`, `Comb`, `Seq` | `SharpHdl.Core` |
| `EmitVerilog` | `SharpHdl.Emit` |
| ALU / カウンタ例 | `examples/` |
| スナップショットテスト | `SharpHdl.Tests` |

## 完了条件

- [ ] DSL で ALU を記述し、Phase 1 と同等の Verilog が出る  
- [ ] 同期カウンタ例が動く  
- [ ] 幅不一致がエラーになる  

→ [phase-3-cpu-ready.md](phase-3-cpu-ready.md)
