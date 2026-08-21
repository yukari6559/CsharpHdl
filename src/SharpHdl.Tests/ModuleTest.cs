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
}