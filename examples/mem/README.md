# examples/mem

Phase 3 の同期 1R1W メモリ例。

- `SimpleRamModule.cs` — `Module.Mem(depth: 256, width: 32, ...)`

```bash
dotnet test src/SharpHdl.Tests --filter SimpleRam
```

生成イメージ:

```verilog
module SimpleRam(
	input wire clk,
	input wire we,
	input wire [7:0] addr,
	input wire [31:0] wdata,
	output reg [31:0] rdata
);
reg [31:0] mem [0:255];
always @(posedge clk) begin
	if (we) mem[addr] <= wdata;
	rdata <= mem[addr];
end
endmodule
```
