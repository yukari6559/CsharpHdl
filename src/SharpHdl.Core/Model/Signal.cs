using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Core.Model;

public enum SignalDirection
{
	Input,
	Output,
	Internal
}
public class Signal : Expr
{
	public SignalDirection Direction { get; private set; }
	public string Name { get; }
	public uint Width { get; }
	protected virtual bool CanAssignComb => false;
	protected virtual bool CanAssignSeq => false;

	protected Signal(uint width, string name, SignalDirection direction)
	{
		Width = width;
		Name = name;
		Direction = direction;
	}

	public override T Accept<T>(IExprVisitor<T> exprVisitor)
	{
		return exprVisitor.VisitSignal(this);
	}

	public void Assign(Expr expr)
	{
		if (!CanAssignComb)
		{
			throw new DescribeContextException();
		}

		if (expr.GetWidth() != Width)
		{
			throw new WidthMismatchException();
		}

		AssignStmt assignStmt = new(this, expr);
		if (CurrentWrite.CurrentStmts == null)
		{
			throw new DescribeContextException();
		}

		if (CurrentWrite.ModuleType == ModuleType.Comb)
		{
			CurrentWrite.CurrentStmts.Add(assignStmt);
		}
		else
		{
			throw new DescribeContextException();
		}
	}

	public void Assign(ulong resetValue, Expr next)
	{
		if (!CanAssignSeq)
		{
			throw new DescribeContextException();
		}

		if (next.GetWidth() != Width)
		{
			throw new WidthMismatchException();
		}

		SeqAssignStmt assignStmt = new(this, next, resetValue);
		if (CurrentWrite.CurrentStmts == null)
		{
			throw new DescribeContextException();
		}

		if (CurrentWrite.ModuleType == ModuleType.Seq)
		{
			CurrentWrite.CurrentStmts.Add(assignStmt);
		}
		else
		{
			throw new DescribeContextException();
		}
	}
	public override uint GetWidth()
	{
		return Width;
	}
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:識別子はキーワードと同一にすることはできません", Justification = "<保留中>")]
public sealed class In : Signal
{
	private In(uint width, string name, SignalDirection direction) : base(width, name, direction)
	{
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:識別子に型名が含まれます", Justification = "<保留中>")]
	public static In UInt(uint width, string name)
	{
		return new In(width, name, SignalDirection.Input);
	}
}

public sealed class Out : Signal
{
	protected override bool CanAssignComb => true;
	protected override bool CanAssignSeq => true;

	private Out(uint width, string name, SignalDirection direction) : base(width, name, direction)
	{
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:識別子に型名が含まれます", Justification = "<保留中>")]
	public static Out UInt(uint width, string name)
	{
		return new Out(width, name, SignalDirection.Output);
	}
}

public sealed class Reg : Signal
{
	protected override bool CanAssignSeq => true;

	private Reg(uint width, string name, SignalDirection direction) : base(width, name, direction)
	{
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:識別子に型名が含まれます", Justification = "<保留中>")]
	public static Reg UInt(uint width, string name)
	{
		return new Reg(width, name, SignalDirection.Internal);
	}
}

public sealed class Wire : Signal
{
	protected override bool CanAssignComb => true;

	private Wire(uint width, string name, SignalDirection direction) : base(width, name, direction)
	{
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:識別子に型名が含まれます", Justification = "<保留中>")]
	public static Wire UInt(uint width, string name)
	{
		return new Wire(width, name, SignalDirection.Internal);
	}
}
