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

public class InstanceStmt : Stmt
{
	public Module ChildModule{get; private set;}
	public string InstanceName{get; private set;}
	public List<PortConnection> PortConnections{get; private set;}
	public InstanceStmt(Module childModule, string instanceName, List<PortConnection> portConnections)
	{
		ChildModule = childModule;
		InstanceName = instanceName;
		PortConnections = portConnections;
	}
}

public class MemStmt : Stmt
{
	public In Clk{get; private set;}
	public uint Depth{get; private set;}
	public uint Width{get; private set;}
	public In We{get; private set;}
	public In Addr{get; private set;}
	public In Wdata{get; private set;}
	public Out Rdata{get; private set;}
	public string? Name{get; private set;}
	public MemStmt(In clk, uint depth, uint width, In we, In addr, In wdata, Out rdata, string? name)
	{
		Clk = clk;
		Depth = depth;
		Width = width;
		We = we;
		Addr = addr;
		Wdata = wdata;
		Rdata = rdata;
		Name = name;
	}
}

public class Mem2R1WStmt : Stmt
{
	public In Clk{get; private set;}
	public uint Depth{get; private set;}
	public uint Width{get; private set;}
	public In We{get; private set;}
	public In Waddr{get; private set;}
	public In Wdata{get; private set;}
	public In Raddr0{get; private set;}
	public Out Rdata0{get; private set;}
	public In Raddr1{get; private set;}
	public Out Rdata1{get; private set;}
	public string? Name{get; private set;}
	public Mem2R1WStmt(In clk, uint depth, uint width, In we, In waddr, In wdata, In raddr0, Out rdata0, In raddr1, Out rdata1, string? name)
	{
		Clk = clk;
		Depth = depth;
		Width = width;
		We = we;
		Waddr = waddr;
		Wdata = wdata;
		Raddr0 = raddr0;
		Rdata0 = rdata0;
		Raddr1 = raddr1;
		Rdata1 = rdata1;
		Name = name;
	}
}

public class MemWstrbStmt : Stmt
{
	public In Clk{get;private set;}
	public uint Depth{get;private set;}
	public uint Width{get;private set;}
	public In We{get;private set;}
	public In Wstrb{get;private set;}
	public In Addr{get;private set;}
	public In Wdata{get;private set;}
	public Out Rdata{get;private set;}
	public string? Name{get; private set;}
	public MemWstrbStmt(In clk, uint depth, uint width, In we, In wstrb, In addr, In wdata, Out rdata, string? name)
	{
		Clk = clk;
		Depth = depth;
		Width = width;
		We = we;
		Wstrb = wstrb;
		Addr = addr;
		Wdata = wdata;
		Rdata = rdata;
		Name = name;
	}
}

public class IfStmt : Stmt
{
	public Expr Cond{get; private set;}
	public List<Stmt> Then{get; private set;}
	public List<Stmt>? Else{get; private set;}
	public IfStmt(Expr cond, List<Stmt> then, List<Stmt>? @else)
	{
		Cond = cond;
		Then = then;
		Else = @else;
	}
}