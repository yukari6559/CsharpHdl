using System.Collections.ObjectModel;

namespace SharpHdl.Core.Model;

public class Module
{
	private List<Signal> Ports = [];
	private List<Stmt> Stmts = [];
	public void SetPorts(List<Signal> ports)
	{
		Ports.AddRange(ports);
	}
	public void SetPort(Signal port)
	{
		Ports.Add(port);
	}
	public ReadOnlyCollection<Signal> GetPorts()
	{
		return Ports.AsReadOnly();
	}
	public ReadOnlyCollection<Stmt> GetStmts()
	{
		return Stmts.AsReadOnly();
	}

	public virtual void Describe()
	{
		
	}

	public void Comb(Action action)
	{
		CurrentWrite.Push(this, Stmts, ModuleType.Comb);
		try
		{
			action.Invoke();
		}
		finally
		{
			CurrentWrite.Pop();
		}
	}

	public void Seq(In clk, In reset, Action action)
	{
		List<Stmt> seqBlockStmts = new();
		CurrentWrite.Push(this, seqBlockStmts, ModuleType.Seq);
		try
		{
			action.Invoke();
		}
		finally
		{
			CurrentWrite.Pop();
		}
		Stmts.Add(new SeqBlockStmt(clk, reset, seqBlockStmts));
	}

	public void Switch(In op, params (uint value, Action action)[] casesParam)
	{
		List<Stmt> parent = CurrentWrite.CurrentStmts!;
		List<Case> cases = new();
		try
		{
			foreach(var (value,action) in casesParam)
			{
				List<Stmt> branchstmts = new ();
				cases.Add(new Case(value, branchstmts));
				CurrentWrite.CurrentStmts = branchstmts;
				action.Invoke();
				CurrentWrite.CurrentStmts = parent;
			}
			parent.Add(new SwitchStmt(op,cases));
		}
		finally
		{
			CurrentWrite.CurrentStmts = parent;
		}
	}

	public void Instance(Module childModule, string instanceName, params (Signal ChildPort, Signal parentSignal)[] connections)
	{
		var portConnections = connections.Select(c => new PortConnection{ChildPort = c.ChildPort, ParentSignal = c.parentSignal}).ToList();
		Stmts.Add(new InstanceStmt(childModule, instanceName, portConnections));
	}

	public void Mem(In clk, uint depth, uint width, In we, In addr, In wdata, Out rdata)
	{
		if (!(we.Width == 1) || !(wdata.Width == width) || !(rdata.Width == width) || !(depth >= 1) || !(addr.Width == (int)Math.Ceiling(Math.Log(depth, 2))) || !(clk.Width == 1))
			throw new WidthMismatchException();
		Stmts.Add(new MemStmt(clk, depth, width, we, addr, wdata, rdata));
	}

	public void Mem(In clk, uint depth, uint width, In we, In waddr, In wdata, In raddr0, Out rdata0, In raddr1, Out rdata1)
	{
		if (!(we.Width == 1) || !(wdata.Width == width) || !(rdata0.Width == width) || !(rdata1.Width == width) || !(depth >= 1) || !(raddr0.Width == (int)Math.Ceiling(Math.Log(depth, 2))) || !(raddr1.Width == (int)Math.Ceiling(Math.Log(depth, 2))) || !(waddr.Width == (int)Math.Ceiling(Math.Log(depth, 2))) || !(clk.Width == 1))
			throw new WidthMismatchException();
		Stmts.Add(new Mem2R1WStmt(clk, depth, width, we, waddr, wdata, raddr0, rdata0, raddr1, rdata1));
	}
	public void Mem(In clk, uint depth, uint width, In we, In wstrb, In addr, In wdata, Out rdata)
	{
		if (!(we.Width == 1) || !(width % 8 == 0) || !(wstrb.Width == width / 8) || !(wdata.Width == width) || !(rdata.Width == width) || !(depth >= 1) || !(addr.Width == (int)Math.Ceiling(Math.Log(depth, 2))) || !(clk.Width == 1))
			throw new WidthMismatchException();
		Stmts.Add(new MemWstrbStmt(clk, depth, width, we, wstrb, addr, wdata, rdata));
	}
	public void If(Expr cond, Action then, Action? @else = null)
	{
		List<Stmt> parent = CurrentWrite.CurrentStmts!;
		List<Stmt> thenstmts = new ();
		List<Stmt>? elsestmts = null;
		CurrentWrite.CurrentStmts = thenstmts;
		try
		{
			then.Invoke();
			if(@else != null)
			{
				elsestmts = new();
				CurrentWrite.CurrentStmts = elsestmts;
				@else.Invoke();
			}
			parent.Add(new IfStmt(cond,thenstmts,elsestmts));
		}
		finally
		{
			CurrentWrite.CurrentStmts = parent;
		}
	}
}
