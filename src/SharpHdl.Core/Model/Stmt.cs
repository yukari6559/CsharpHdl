namespace SharpHdl.Core.Model;
public class Case
{
	public uint Value {get;private set;}
	public List<Stmt> Stmts {get;private set;}
	public Case(uint value, List<Stmt> stmts)
	{
		Value = value;
		Stmts = stmts;
	}
}
public class Stmt
{
	
}

public class AssignStmt : Stmt
{
	public Signal Signal{get;private set;}
	public Expr Expr{get;private set;}

	public AssignStmt(Signal signal, Expr expr)
	{
		Signal = signal;
		Expr = expr;
	}
}

public class SwitchStmt : Stmt
{
	public Signal Signal{get;private set;}
	public List<Case> Cases{get;private set;}
	public SwitchStmt(Signal signal, List<Case> cases)
	{
		Signal = signal;
		Cases = cases;
	}
}

public class SeqBlockStmt : Stmt
{
	public In Clk{get; private set;}
	public In Reset{get; private set;}
	public List<Stmt> Body{get; private set;}
	public SeqBlockStmt(In clk, In reset, List<Stmt> body)
	{
		Clk = clk;
		Reset = reset;
		Body = body;
	}
}

public class SeqAssignStmt : Stmt
{
	public Signal Signal{get; private set;}
	public Expr Expr{get; private set;}
	public uint ResetValue{get;private set;}
	public SeqAssignStmt(Signal signal, Expr expr, uint resetValue)
	{
		Signal = signal;
		Expr = expr;
		ResetValue = resetValue;
	}
}