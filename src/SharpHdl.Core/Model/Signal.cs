namespace SharpHdl.Core.Model;

public enum SignalDirection
{
    Input,
    Output
}
public class Signal : Expr
{
	public SignalDirection Direction{get;private set;}
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
		if(expr.GetWidth() != Width)
			throw new WidthMismatchException();
		AssignStmt assignStmt = new (this, expr);
		if(CurrentWrite.CurrentStmts == null)
			throw new Exception();
		if(CurrentWrite.ModuleType == ModuleType.Comb)
			CurrentWrite.CurrentStmts.Add(assignStmt);
		else
			throw new Exception();
	}

	public void Assign(uint resetValue, Expr next)
	{
		if(next.GetWidth() != Width)
			throw new WidthMismatchException();
		SeqAssignStmt assignStmt = new(this, next, resetValue);
		if(CurrentWrite.CurrentStmts == null)
			throw new Exception();
		if(CurrentWrite.ModuleType == ModuleType.Seq)
			CurrentWrite.CurrentStmts.Add(assignStmt);
		else
			throw new Exception();
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
