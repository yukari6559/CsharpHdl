namespace SharpHdl.Core.Model;

public enum SignalDirection
{
    Input,
    Output
}
public class Signal : Expr
{
	protected SignalDirection Direction{get;}
	public string Name {get;}
	public uint Width {get;}

	protected Signal(uint width, string name, SignalDirection direction)
	{
		Width = width;
		Name = name;
		Direction = direction;
	}

	public void Assign(Expr expr)
	{
		AssignStmt assignStmt = new (this, expr);
		if(CurrentWrite.CurrentStmts == null)
			throw new Exception();
		CurrentWrite.CurrentStmts.Add(assignStmt);
	}
}

public class In : Signal
{
	private In(uint width, string name, SignalDirection direction) : base(width, name, direction)
	{
	}

	public static In UInt(uint width, string name)
		=> new In(width, name, SignalDirection.Input);
}

public class Out : Signal
{
	private Out(uint width, string name, SignalDirection direction) : base(width, name, direction)
	{
	}
	public static Out UInt(uint width, string name)
		=> new Out(width, name, SignalDirection.Output);
}
