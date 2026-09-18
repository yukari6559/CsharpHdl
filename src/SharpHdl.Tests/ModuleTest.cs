namespace SharpHdl.Tests;

using SharpHdl.Core.Model;
using SharpHdl.Emit;

public class ModuleTests
{
	[Fact]
	public void TestModulePort()
	{
		TestModule1 testmodule1 = new();
		List<Signal> signals = testmodule1.GetPorts().ToList();
		Assert.Equal("Input", signals[0].Name);
		Assert.Equal(1u, signals[0].Width);
		Assert.Equal("Output", signals[1].Name);
		Assert.Equal(1u, signals[1].Width);
	}

	[Fact]
	public void TestModuleExpr()
	{
		TestModule1 testmodule1 = new();
		OpExpr expr = testmodule1.Input + testmodule1.Output;
		Assert.Equal(Op.Plus, expr.Op);
		Assert.Equal(testmodule1.Input, expr.Left);
		Assert.Equal(testmodule1.Output, expr.Right);
	}

	[Fact]
	public void TestAluModuleStmt()
	{
		Alu alu = new();
		alu.Describe();
		List<Stmt> stmts = alu.GetStmts().ToList();
		SwitchStmt switchStmt = (SwitchStmt)stmts[0];
		Assert.Equal(4,switchStmt.Cases.Count);
		Assert.Equal(0u,switchStmt.Cases[0].Value);
		AssignStmt assignStmt = (AssignStmt)switchStmt.Cases[0].Stmts[0];
		Assert.Equal(alu.Y,assignStmt.Signal);
		OpExpr opExpr  = (OpExpr) assignStmt.Expr;
		Assert.Equal(Op.Plus,opExpr.Op);
		Assert.Equal(1u,switchStmt.Cases[1].Value);
		assignStmt = (AssignStmt)switchStmt.Cases[1].Stmts[0];
		Assert.Equal(alu.Y,assignStmt.Signal);
		opExpr  = (OpExpr) assignStmt.Expr;
		Assert.Equal(Op.Minus,opExpr.Op);
		Assert.Equal(2u,switchStmt.Cases[2].Value);
		assignStmt = (AssignStmt)switchStmt.Cases[2].Stmts[0];
		Assert.Equal(alu.Y,assignStmt.Signal);
		opExpr  = (OpExpr) assignStmt.Expr;
		Assert.Equal(Op.And,opExpr.Op);
		Assert.Equal(3u,switchStmt.Cases[3].Value);
		assignStmt = (AssignStmt)switchStmt.Cases[3].Stmts[0];
		Assert.Equal(alu.Y,assignStmt.Signal);
		opExpr  = (OpExpr) assignStmt.Expr;
		Assert.Equal(Op.Or,opExpr.Op);
	}

	[Fact]
	public void TestAluModuleEmitter_EmitsPortFrame()
	{
		Alu alu = new();
		var verilog = VerilogEmitter.Emitter(alu, nameof(Alu));

		const string expected =
			"module Alu(\n" +
			"\tinput wire [31:0] A,\n" +
			"\tinput wire [31:0] B,\n" +
			"\tinput wire [1:0] Op,\n" +
			"\toutput wire [31:0] Y\n" +
			");\n" +
			"endmodule";

		Assert.Equal(expected, verilog);
	}

	[Fact]
	public void TestTestModuleEmitter_EmitsSimpleModule()
	{
		TestModule1 testModule1 = new ();
		testModule1.Describe();
		var verilog = VerilogEmitter.Emitter(testModule1, nameof(TestModule1));

		const string expected =
			"module TestModule1(\n" +
			"\tinput wire Input,\n" +
			"\toutput wire Output\n" +
			");\n" +
			"\tassign Output = Input;\n"+
			"endmodule";
		
		Console.WriteLine(verilog);
		Assert.Equal(expected, verilog);
	}

	[Fact]
	public void TestCounterModuleStmt()
	{
		Counter counter = new();
		counter.Describe();
		List<Stmt> stmts = counter.GetStmts().ToList();

		Assert.Single(stmts);
		SeqBlockStmt block = (SeqBlockStmt)stmts[0];
		Assert.Equal(counter.Clk, block.Clk);
		Assert.Equal(counter.Rst, block.Reset);
		Assert.Single(block.Body);

		SeqAssignStmt assign = (SeqAssignStmt)block.Body[0];
		Assert.Equal(counter.Count, assign.Signal);
		Assert.Equal(0u, assign.ResetValue);
		OpExpr expr = (OpExpr)assign.Expr;
		Assert.Equal(Op.Plus, expr.Op);
		Assert.Equal(counter.Count, expr.Left);
		Assert.Equal(counter.Step, expr.Right);
	}

	[Fact]
	public void TestSeqAssign_OutsideSeq_Throws()
	{
		Counter counter = new();
		Assert.Throws<Exception>(() => counter.Count.Assign(0, counter.Count));
	}

	[Fact]
	public void TestCombAssign_WidthMismatch_Throws()
	{
		BadWidthModule module = new();
		Assert.Throws<WidthMismatchException>(module.DescribeCombMismatch);
	}

	[Fact]
	public void TestSeqAssign_WidthMismatch_Throws()
	{
		BadWidthModule module = new();
		Assert.Throws<WidthMismatchException>(module.DescribeSeqMismatch);
	}

	[Fact]
	public void TestCounterModuleEmitter_EmitsSeq()
	{
		Counter counter = new();
		counter.Describe();
		var verilog = VerilogEmitter.Emitter(counter, nameof(Counter));

		const string expected =
			"module Counter(\n" +
			"\tinput wire clk,\n" +
			"\tinput wire rst,\n" +
			"\tinput wire [7:0] step,\n" +
			"\toutput reg [7:0] count\n" +
			");\n" +
			"\talways @(posedge clk) begin\n" +
			"\t\tif (rst) begin\n" +
			"\t\t\tcount <= 8'd0;\n" +
			"\t\tend else begin\n" +
			"\t\t\tcount <= count + step;\n" +
			"\t\tend\n" +
			"\tend\n" +
			"endmodule";

		Assert.Equal(expected, verilog);
	}

	[Fact]
	public void TestAluModuleEmitter_EmitsAll()
	{
		Alu alu = new();
		alu.Describe();
		var verilog = VerilogEmitter.Emitter(alu, nameof(Alu));

		const string expected =
			"module Alu(\n" +
			"\tinput wire [31:0] A,\n" +
			"\tinput wire [31:0] B,\n" +
			"\tinput wire [1:0] Op,\n" +
			"\toutput wire [31:0] Y\n" +
			");\n" +
			"\tassign Y = (Op == 2'd0) ? (A + B) :\n" +
			"\t\t(Op == 2'd1) ? (A - B) :\n" +
			"\t\t(Op == 2'd2) ? (A & B) :\n" +
			"\t\t(A | B);\n" +
			"endmodule";

		Assert.Equal(expected, verilog);
	}

	[Fact]
	public void TestAluTopModuleStmt()
	{
		AluTop top = new();
		top.Describe();
		List<Stmt> stmts = top.GetStmts().ToList();

		Assert.Single(stmts);
		InstanceStmt instance = (InstanceStmt)stmts[0];
		Assert.Equal("alu", instance.InstanceName);
		Assert.IsType<Alu>(instance.ChildModule);
		Assert.Equal(4, instance.PortConnections.Count);
		Assert.Equal(top.A, instance.PortConnections[0].ParentSignal);
		Assert.Equal(top.Y, instance.PortConnections[3].ParentSignal);
	}

	[Fact]
	public void TestAluTopModuleEmitter_EmitsHierarchy()
	{
		AluTop top = new();
		top.Describe();
		var verilog = VerilogEmitter.Emitter(top, nameof(AluTop));

		const string expected =
			"module Alu(\n" +
			"\tinput wire [31:0] A,\n" +
			"\tinput wire [31:0] B,\n" +
			"\tinput wire [1:0] Op,\n" +
			"\toutput wire [31:0] Y\n" +
			");\n" +
			"\tassign Y = (Op == 2'd0) ? (A + B) :\n" +
			"\t\t(Op == 2'd1) ? (A - B) :\n" +
			"\t\t(Op == 2'd2) ? (A & B) :\n" +
			"\t\t(A | B);\n" +
			"endmodule" +
			"module AluTop(\n" +
			"\tinput wire [31:0] A,\n" +
			"\tinput wire [31:0] B,\n" +
			"\tinput wire [1:0] Op,\n" +
			"\toutput wire [31:0] Y\n" +
			");\n" +
			"\tAlu alu (\n" +
			"\t\t.A(A),\t\t.B(B),\t\t.Op(Op),\t\t.Y(Y));\n" +
			"endmodule";

		Assert.Equal(expected, verilog);
	}

	[Fact]
	public void TestDualAluTopEmitter_EmitsChildModuleOnce()
	{
		DualAluTop top = new();
		top.Describe();
		var verilog = VerilogEmitter.Emitter(top, nameof(DualAluTop));

		Assert.Equal(1, CountOccurrences(verilog, "module Alu("));
		Assert.Contains("Alu alu0 (", verilog);
		Assert.Contains("Alu alu1 (", verilog);
		Assert.Equal(2, CountOccurrences(verilog, "endmodule"));
	}

	[Fact]
	public void TestSimpleRamModuleStmt()
	{
		SimpleRam ram = new();
		ram.Describe();
		List<Stmt> stmts = ram.GetStmts().ToList();

		Assert.Single(stmts);
		MemStmt mem = Assert.IsType<MemStmt>(stmts[0]);
		Assert.Equal(256u, mem.Depth);
		Assert.Equal(32u, mem.Width);
		Assert.Equal(ram.Clk, mem.Clk);
		Assert.Equal(ram.We, mem.We);
		Assert.Equal(ram.Addr, mem.Addr);
		Assert.Equal(ram.Wdata, mem.Wdata);
		Assert.Equal(ram.Rdata, mem.Rdata);
	}

	[Fact]
	public void TestSimpleRamModuleEmitter_EmitsMem()
	{
		SimpleRam ram = new();
		ram.Describe();
		var verilog = VerilogEmitter.Emitter(ram, nameof(SimpleRam));

		const string expected =
			"module SimpleRam(\n" +
			"\tinput wire clk,\n" +
			"\tinput wire we,\n" +
			"\tinput wire [7:0] addr,\n" +
			"\tinput wire [31:0] wdata,\n" +
			"\toutput reg [31:0] rdata\n" +
			");\n" +
			"reg [31:0] mem [0:255];\n" +
			"always @(posedge clk) begin\n" +
			"\tif (we) mem[addr] <= wdata;\n" +
			"\trdata <= mem[addr];\n" +
			"end\n" +
			"endmodule";

		Assert.Equal(expected, verilog);
	}

	[Fact]
	public void TestByteWriteRamModuleStmt()
	{
		ByteWriteRam ram = new();
		ram.Describe();
		List<Stmt> stmts = ram.GetStmts().ToList();

		Assert.Single(stmts);
		MemWstrbStmt mem = Assert.IsType<MemWstrbStmt>(stmts[0]);
		Assert.Equal(256u, mem.Depth);
		Assert.Equal(32u, mem.Width);
		Assert.Equal(ram.Clk, mem.Clk);
		Assert.Equal(ram.We, mem.We);
		Assert.Equal(ram.Wstrb, mem.Wstrb);
		Assert.Equal(ram.Addr, mem.Addr);
		Assert.Equal(ram.Wdata, mem.Wdata);
		Assert.Equal(ram.Rdata, mem.Rdata);
	}

	[Fact]
	public void TestByteWriteRamModuleEmitter_EmitsByteEnables()
	{
		ByteWriteRam ram = new();
		ram.Describe();
		var verilog = VerilogEmitter.Emitter(ram, nameof(ByteWriteRam));

		const string expected =
			"module ByteWriteRam(\n" +
			"\tinput wire clk,\n" +
			"\tinput wire we,\n" +
			"\tinput wire [3:0] wstrb,\n" +
			"\tinput wire [7:0] addr,\n" +
			"\tinput wire [31:0] wdata,\n" +
			"\toutput reg [31:0] rdata\n" +
			");\n" +
			"reg [31:0] mem [0:255];\n" +
			"always @(posedge clk) begin\n" +
			"\tif (we) begin\n" +
			"\t\tif (wstrb[0]) mem[addr][7:0] <= wdata[7:0];\n" +
			"\t\tif (wstrb[1]) mem[addr][15:8] <= wdata[15:8];\n" +
			"\t\tif (wstrb[2]) mem[addr][23:16] <= wdata[23:16];\n" +
			"\t\tif (wstrb[3]) mem[addr][31:24] <= wdata[31:24];\n" +
			"\tend\n" +
			"\trdata <= mem[addr];\n" +
			"end\n" +
			"endmodule";

		Assert.Equal(expected, verilog);
	}

	[Fact]
	public void TestMemWstrb_WidthMismatch_Throws()
	{
		BadMemWstrbWidthModule module = new();
		Assert.Throws<WidthMismatchException>(module.DescribeWstrbMismatch);
	}

	[Fact]
	public void TestRegFile2R1WModuleStmt()
	{
		RegFile2R1W rf = new();
		rf.Describe();
		List<Stmt> stmts = rf.GetStmts().ToList();

		Assert.Single(stmts);
		Mem2R1WStmt mem = Assert.IsType<Mem2R1WStmt>(stmts[0]);
		Assert.Equal(32u, mem.Depth);
		Assert.Equal(64u, mem.Width);
		Assert.Equal(rf.Clk, mem.Clk);
		Assert.Equal(rf.We, mem.We);
		Assert.Equal(rf.Waddr, mem.Waddr);
		Assert.Equal(rf.Wdata, mem.Wdata);
		Assert.Equal(rf.Raddr0, mem.Raddr0);
		Assert.Equal(rf.Rdata0, mem.Rdata0);
		Assert.Equal(rf.Raddr1, mem.Raddr1);
		Assert.Equal(rf.Rdata1, mem.Rdata1);
	}

	[Fact]
	public void TestRegFile2R1WModuleEmitter_EmitsAsyncReads()
	{
		RegFile2R1W rf = new();
		rf.Describe();
		var verilog = VerilogEmitter.Emitter(rf, nameof(RegFile2R1W));

		const string expected =
			"module RegFile2R1W(\n" +
			"\tinput wire clk,\n" +
			"\tinput wire we,\n" +
			"\tinput wire [4:0] waddr,\n" +
			"\tinput wire [63:0] wdata,\n" +
			"\tinput wire [4:0] raddr0,\n" +
			"\toutput wire [63:0] rdata0,\n" +
			"\tinput wire [4:0] raddr1,\n" +
			"\toutput wire [63:0] rdata1\n" +
			");\n" +
			"reg [63:0] mem [0:31];\n" +
			"always @(posedge clk) begin\n" +
			"\tif (we) mem[waddr] <= wdata;\n" +
			"end\n" +
			"assign rdata0 = mem[raddr0];\n" +
			"assign rdata1 = mem[raddr1];\n" +
			"endmodule";

		Assert.Equal(expected, verilog);
	}

	[Fact]
	public void TestMem2R1W_WidthMismatch_Throws()
	{
		BadMem2R1WWidthModule module = new();
		Assert.Throws<WidthMismatchException>(module.DescribeRaddr1Mismatch);
	}

	[Fact]
	public void TestWidth64PassModule_PortsAre64()
	{
		Width64Pass mod = new();
		List<Signal> ports = mod.GetPorts().ToList();
		Assert.Equal(64u, ports[0].Width);
		Assert.Equal(64u, ports[1].Width);
	}

	[Fact]
	public void TestWidth64PassModuleEmitter_Emits63to0()
	{
		Width64Pass mod = new();
		mod.Describe();
		var verilog = VerilogEmitter.Emitter(mod, nameof(Width64Pass));

		const string expected =
			"module Width64Pass(\n" +
			"\tinput wire [63:0] A,\n" +
			"\toutput wire [63:0] Y\n" +
			");\n" +
			"\tassign Y = A;\n" +
			"endmodule";

		Assert.Equal(expected, verilog);
	}

	[Fact]
	public void TestSlice_GetWidth_IsInclusive()
	{
		In instr = In.UInt(32, "instr");
		Assert.Equal(7u, instr.Slice(6, 0).GetWidth());
		Assert.Equal(5u, instr.Slice(11, 7).GetWidth());
	}

	[Fact]
	public void TestConcat_GetWidth_SumsParts()
	{
		In hi = In.UInt(16, "hi");
		In lo = In.UInt(16, "lo");
		Assert.Equal(32u, hi.Concat(lo).GetWidth());
	}

	[Fact]
	public void TestSliceAssign_WidthMismatch_Throws()
	{
		BadSliceWidthModule module = new();
		Assert.Throws<WidthMismatchException>(module.DescribeSliceMismatch);
	}

	[Fact]
	public void TestDecodeSliceConcatModuleStmt()
	{
		DecodeSliceConcat mod = new();
		mod.Describe();
		List<Stmt> stmts = mod.GetStmts().ToList();
		Assert.Equal(3, stmts.Count);

		AssignStmt opcode = Assert.IsType<AssignStmt>(stmts[0]);
		SliceExpr opcodeSlice = Assert.IsType<SliceExpr>(opcode.Expr);
		Assert.Equal(6u, opcodeSlice.MSB);
		Assert.Equal(0u, opcodeSlice.LSB);
		Assert.Equal(mod.Instr, opcodeSlice.Expr);

		AssignStmt rd = Assert.IsType<AssignStmt>(stmts[1]);
		SliceExpr rdSlice = Assert.IsType<SliceExpr>(rd.Expr);
		Assert.Equal(11u, rdSlice.MSB);
		Assert.Equal(7u, rdSlice.LSB);

		AssignStmt word = Assert.IsType<AssignStmt>(stmts[2]);
		ConcatExpr concat = Assert.IsType<ConcatExpr>(word.Expr);
		Assert.Equal(2, concat.Exprs.Length);
		Assert.IsType<SliceExpr>(concat.Exprs[0]);
		Assert.IsType<SliceExpr>(concat.Exprs[1]);
	}

	[Fact]
	public void TestDecodeSliceConcatModuleEmitter_EmitsPartSelectAndConcat()
	{
		DecodeSliceConcat mod = new();
		mod.Describe();
		var verilog = VerilogEmitter.Emitter(mod, nameof(DecodeSliceConcat));

		const string expected =
			"module DecodeSliceConcat(\n" +
			"\tinput wire [31:0] instr,\n" +
			"\toutput wire [6:0] opcode,\n" +
			"\toutput wire [4:0] rd,\n" +
			"\toutput wire [31:0] word\n" +
			");\n" +
			"\tassign opcode = instr[6:0];\n" +
			"\tassign rd = instr[11:7];\n" +
			"\tassign word = {instr[31:16], instr[15:0]};\n" +
			"endmodule";

		Assert.Equal(expected, verilog);
	}

	[Fact]
	public void TestSignExtend_GetWidth_IsTarget()
	{
		In b = In.UInt(8, "b");
		Assert.Equal(32u, b.SignExtend(32).GetWidth());
	}

	[Fact]
	public void TestZeroExtend_GetWidth_IsTarget()
	{
		In b = In.UInt(8, "b");
		Assert.Equal(32u, b.ZeroExtend(32).GetWidth());
	}

	[Fact]
	public void TestExtend_Shrink_Throws()
	{
		BadExtendWidthModule module = new();
		Assert.Throws<WidthMismatchException>(module.DescribeSignShrink);
		Assert.Throws<WidthMismatchException>(module.DescribeZeroShrink);
	}

	[Fact]
	public void TestExtendPassModuleStmt()
	{
		ExtendPass mod = new();
		mod.Describe();
		List<Stmt> stmts = mod.GetStmts().ToList();
		Assert.Equal(2, stmts.Count);

		AssignStmt signed = Assert.IsType<AssignStmt>(stmts[0]);
		SignExtendExpr sext = Assert.IsType<SignExtendExpr>(signed.Expr);
		Assert.Equal(32u, sext.Width);
		Assert.Equal(mod.ByteIn, sext.Expr);

		AssignStmt zero = Assert.IsType<AssignStmt>(stmts[1]);
		ZeroExtendExpr zext = Assert.IsType<ZeroExtendExpr>(zero.Expr);
		Assert.Equal(32u, zext.Width);
		Assert.Equal(mod.ByteIn, zext.Expr);
	}

	[Fact]
	public void TestExtendPassModuleEmitter_EmitsReplication()
	{
		ExtendPass mod = new();
		mod.Describe();
		var verilog = VerilogEmitter.Emitter(mod, nameof(ExtendPass));

		const string expected =
			"module ExtendPass(\n" +
			"\tinput wire [7:0] byte_in,\n" +
			"\toutput wire [31:0] signed_out,\n" +
			"\toutput wire [31:0] zero_out\n" +
			");\n" +
			"\tassign signed_out = {{24{byte_in[7]}}, byte_in};\n" +
			"\tassign zero_out = {{24{1'b0}}, byte_in};\n" +
			"endmodule";

		Assert.Equal(expected, verilog);
	}

	[Fact]
	public void TestIfMuxCombModuleEmitter_EmitsAlwaysIfElse()
	{
		IfMuxComb mod = new();
		mod.Describe();
		var verilog = VerilogEmitter.Emitter(mod, nameof(IfMuxComb));

		Assert.Contains("output reg [7:0] Y", verilog, StringComparison.Ordinal);
		Assert.DoesNotContain("output wire [7:0] Y", verilog, StringComparison.Ordinal);
		Assert.Contains("always @(*) begin", verilog, StringComparison.Ordinal);
		Assert.Contains("if (SelA == SelB) begin", verilog, StringComparison.Ordinal);
		Assert.Contains("Y = C;", verilog, StringComparison.Ordinal);
		Assert.Contains("else begin", verilog, StringComparison.Ordinal);
		Assert.Contains("Y = D;", verilog, StringComparison.Ordinal);
	}

	[Fact]
	public void TestIfThenOnlyCombModuleEmitter_OmitsElse()
	{
		IfThenOnlyComb mod = new();
		mod.Describe();
		var verilog = VerilogEmitter.Emitter(mod, nameof(IfThenOnlyComb));

		Assert.Contains("output reg [7:0] Y", verilog, StringComparison.Ordinal);
		Assert.DoesNotContain("output wire [7:0] Y", verilog, StringComparison.Ordinal);
		Assert.Contains("always @(*) begin", verilog, StringComparison.Ordinal);
		Assert.Contains("if (A == B) begin", verilog, StringComparison.Ordinal);
		Assert.Contains("Y = C;", verilog, StringComparison.Ordinal);
		Assert.DoesNotContain("else begin", verilog, StringComparison.Ordinal);
	}

	[Fact]
	public void TestConstCombModuleEmitter_EmitsSizedDecimals()
	{
		ConstComb mod = new();
		mod.Describe();
		var verilog = VerilogEmitter.Emitter(mod, nameof(ConstComb));

		Assert.Contains("8'd5", verilog, StringComparison.Ordinal);
		Assert.Contains("8'd10", verilog, StringComparison.Ordinal);
		Assert.Contains("8'd3", verilog, StringComparison.Ordinal);
		Assert.Contains("8'd44", verilog, StringComparison.Ordinal);
		Assert.DoesNotContain("8'd300", verilog, StringComparison.Ordinal);
	}

	private static int CountOccurrences(string text, string value)
	{
		int count = 0;
		int index = 0;
		while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
		{
			count++;
			index += value.Length;
		}
		return count;
	}
}