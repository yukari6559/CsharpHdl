# Phase 1: ALU を生成する

## 目的

最短で「C# → `.v` ファイル」を通す。AST はまだ不要。`StringBuilder` でよい。

## 手順

1. `examples/alu` 用に、C# から次を出力する関数を書く  

```verilog
module Alu(
  input  wire [31:0] A,
  input  wire [31:0] B,
  input  wire [1:0]  Op,
  output wire [31:0] Y
);
  assign Y = (Op == 2'd0) ? (A + B) :
             (Op == 2'd1) ? (A - B) :
             (Op == 2'd2) ? (A & B) :
                           (A | B);
endmodule
```

2. `out/generated/alu.v` に書き出す  
3. Verilator または Icarus でパース確認  

## 完了条件

- [ ] 生成ファイルが存在する  
- [ ] シミュレータがエラーなく読む  
- [ ] （任意）テストベンチで 1+2=3  

→ [phase-2-dsl.md](phase-2-dsl.md)
