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
		CurrentWrite.CurrentModule = this;
		CurrentWrite.CurrentStmts = Stmts;
		CurrentWrite.ModuleType = ModuleType.Comb;
		try
		{
			action.Invoke();
		}
		finally
		{
			CurrentWrite.CurrentModule = null;
			CurrentWrite.CurrentStmts = null;
			CurrentWrite.ModuleType = null;
		}
	}

	public void Seq(In clk, In reset, Action action)
	{
		CurrentWrite.CurrentModule = this;
		List<Stmt> seqBlockStmts = new();
		CurrentWrite.CurrentStmts = seqBlockStmts;
		CurrentWrite.ModuleType = ModuleType.Seq;
		try
		{
			action.Invoke();
		}
		finally
		{
			CurrentWrite.CurrentModule = null;
			CurrentWrite.CurrentStmts = null;
			CurrentWrite.ModuleType = null;
		}
		Stmts.Add(new SeqBlockStmt(clk, reset, seqBlockStmts));
	}

	public void Switch(In op, params (uint value, Action action)[] casesParam)
	{
		CurrentWrite.CurrentModule = this;
		CurrentWrite.CurrentStmts = Stmts;
		List<Stmt> parent = CurrentWrite.CurrentStmts;
		List<Case> cases = new();
		foreach(var (value,action) in casesParam)
		{
			List<Stmt> branchstmts = new ();
			cases.Add(new Case(value, branchstmts));
			CurrentWrite.CurrentStmts = branchstmts;
			action.Invoke();
			CurrentWrite.CurrentStmts = parent;
		}
		parent.Add(new SwitchStmt(op,cases));
		CurrentWrite.CurrentStmts = Stmts;
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
}