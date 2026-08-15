namespace SharpHdl.Tests;

using SharpHdl.Core.Model;

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
}