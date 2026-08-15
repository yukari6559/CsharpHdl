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
		try
		{
			action.Invoke();
		}
		finally
		{
			CurrentWrite.CurrentModule = null;
			CurrentWrite.CurrentStmts = null;
		}
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
}