# Phase 0: 方針固め

## 目的

「C# HDL とは何か」「何を生成するか」を固定する。コードは最小でよい。

## やること

1. [dsl-spec.md](../dsl-spec.md) と [verilog-emit-spec.md](../verilog-emit-spec.md) を読む  
2. 手書きで ALU の Verilog を 20 行ほど書いてみる（生成の正解イメージ）  
3. Comb と Seq の違いを自分の言葉で書く  
4. （推奨）ソリューションは作成済み。確認:
   ```bash
   cd CsharpHdl
   dotnet build CsharpHdl.slnx
   ```

## 完了条件

- [x] 普通の C# が FPGA に載らない理由を説明できる  
- [x] 生成物は Verilog であると説明できる  
- [x] MyOs など消費者は別リポジトリであると説明できる  

→ [phase-1-alu.md](phase-1-alu.md)
