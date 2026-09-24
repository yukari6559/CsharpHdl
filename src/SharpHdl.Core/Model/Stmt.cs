namespace SharpHdl.Core.Model;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:識別子はキーワードと同一にすることはできません", Justification = "<保留中>")]
public class Case(uint value, List<Stmt> stmts)
{
	public uint Value { get; private set; } = value;
	public List<Stmt> Stmts { get; private set; } = stmts;
}
public class Stmt
{

}

public class AssignStmt(Signal signal, Expr expr) : Stmt
{
	public Signal Signal { get; private set; } = signal;
	public Expr Expr { get; private set; } = expr;
}

public class SwitchStmt(Expr expr, List<Case> cases) : Stmt
{
	public Expr Expr { get; private set; } = expr;
	public List<Case> Cases { get; private set; } = cases;
}

public class SeqBlockStmt(In clk, In reset, List<Stmt> body) : Stmt
{
	public In Clk { get; private set; } = clk;
	public In Reset { get; private set; } = reset;
	public List<Stmt> Body { get; private set; } = body;
}

public class SeqAssignStmt(Signal signal, Expr expr, uint resetValue) : Stmt
{
	public Signal Signal { get; private set; } = signal;
	public Expr Expr { get; private set; } = expr;
	public uint ResetValue { get; private set; } = resetValue;
}

public class InstanceStmt(Module childModule, string instanceName, List<PortConnection> portConnections) : Stmt
{
	public Module ChildModule { get; private set; } = childModule;
	public string InstanceName { get; private set; } = instanceName;
	public List<PortConnection> PortConnections { get; private set; } = portConnections;
}

public class MemStmt(In clk, uint depth, uint width, In we, In addr, In wdata, Out rdata, string? name) : Stmt
{
	public In Clk { get; private set; } = clk;
	public uint Depth { get; private set; } = depth;
	public uint Width { get; private set; } = width;
	public In We { get; private set; } = we;
	public In Addr { get; private set; } = addr;
	public In Wdata { get; private set; } = wdata;
	public Out Rdata { get; private set; } = rdata;
	public string? Name { get; private set; } = name;
}

public class Mem2R1WStmt(In clk, uint depth, uint width, In we, In waddr, In wdata, In raddr0, Out rdata0, In raddr1, Out rdata1, string? name) : Stmt
{
	public In Clk { get; private set; } = clk;
	public uint Depth { get; private set; } = depth;
	public uint Width { get; private set; } = width;
	public In We { get; private set; } = we;
	public In Waddr { get; private set; } = waddr;
	public In Wdata { get; private set; } = wdata;
	public In Raddr0 { get; private set; } = raddr0;
	public Out Rdata0 { get; private set; } = rdata0;
	public In Raddr1 { get; private set; } = raddr1;
	public Out Rdata1 { get; private set; } = rdata1;
	public string? Name { get; private set; } = name;
}

public class MemWstrbStmt(In clk, uint depth, uint width, In we, In wstrb, In addr, In wdata, Out rdata, string? name) : Stmt
{
	public In Clk { get; private set; } = clk;
	public uint Depth { get; private set; } = depth;
	public uint Width { get; private set; } = width;
	public In We { get; private set; } = we;
	public In Wstrb { get; private set; } = wstrb;
	public In Addr { get; private set; } = addr;
	public In Wdata { get; private set; } = wdata;
	public Out Rdata { get; private set; } = rdata;
	public string? Name { get; private set; } = name;
}

public class IfStmt(Expr cond, List<Stmt> then, List<Stmt>? @else) : Stmt
{
	public Expr Cond { get; private set; } = cond;
	public List<Stmt> Then { get; private set; } = then;
	public List<Stmt>? Else { get; private set; } = @else;
}