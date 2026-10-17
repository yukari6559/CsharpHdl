using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Core.Model;

public abstract class Expr
{
	public static OpExpr operator +(Expr left, Expr right)
	{
		CheckWidth(left, right);
		return new OpExpr() { Left = left, Right = right, Op = Op.Plus };
	}
	public static OpExpr operator -(Expr left, Expr right)
	{
		CheckWidth(left, right);
		return new OpExpr() { Left = left, Right = right, Op = Op.Minus };
	}
	public static OpExpr operator &(Expr left, Expr right)
	{
		CheckWidth(left, right);
		return new OpExpr() { Left = left, Right = right, Op = Op.And };
	}
	public static OpExpr operator |(Expr left, Expr right)
	{
		CheckWidth(left, right);
		return new OpExpr() { Left = left, Right = right, Op = Op.Or };
	}
	public static OpExpr operator ^(Expr left, Expr right)
	{
		CheckWidth(left, right);
		return new OpExpr() { Left = left, Right = right, Op = Op.Xor };
	}
	public static OpExpr operator <(Expr left, Expr right)
	{
		CheckWidth(left, right);
		return new OpExpr() { Left = left, Right = right, Op = Op.Lt };
	}
	public static OpExpr operator >(Expr left, Expr right)
	{
		CheckWidth(left, right);
		return new OpExpr() { Left = left, Right = right, Op = Op.Gt };
	}
	public static OpExpr operator <=(Expr left, Expr right)
	{
		CheckWidth(left, right);
		return new OpExpr() { Left = left, Right = right, Op = Op.Le };
	}
	public static OpExpr operator >=(Expr left, Expr right)
	{
		CheckWidth(left, right);
		return new OpExpr() { Left = left, Right = right, Op = Op.Ge };
	}
	public Expr Eq(Expr right)
	{
		CheckWidth(this, right);
		return new OpExpr() { Left = this, Right = right, Op = Op.Eq };
	}
	public Expr Neq(Expr right)
	{
		CheckWidth(this, right);
		return new OpExpr() { Left = this, Right = right, Op = Op.Neq };
	}
	public Expr Xnor(Expr right)
	{
		CheckWidth(this, right);
		return new OpExpr() { Left = this, Right = right, Op = Op.Xnor };
	}
	public Expr Concat(params Expr[] exprs)
	{
		Expr[] thisexprs = [.. exprs.Prepend(this)];
		return new ConcatExpr(thisexprs);
	}
	public Expr Slice(uint msb, uint lsb)
	{
		SliceExpr sliceExpr = new(msb, lsb, this);
		return sliceExpr;
	}
	public Expr SignExtend(uint width)
	{
		if (GetWidth() > width)
		{
			throw new WidthMismatchException();
		}

		SignExtendExpr signExtendExpr = new(width, this);
		return signExtendExpr;
	}
	public Expr ZeroExtend(uint width)
	{
		if (GetWidth() > width)
		{
			throw new WidthMismatchException();
		}

		ZeroExtendExpr zeroExtendExpr = new(width, this);
		return zeroExtendExpr;
	}
	public abstract uint GetWidth();
	public abstract T Accept<T>(IExprVisitor<T> exprVisitor);
	private static void CheckWidth(Expr left, Expr right)
	{
		if (right.GetWidth() != left.GetWidth())
		{
			throw new WidthMismatchException($"Operand widths differ: left {left.GetWidth()}, right {right.GetWidth()}.");
		}
	}
}

public sealed class SliceExpr(uint msb, uint lsb, Expr expr) : Expr
{
	public uint LSB { get; private set; } = lsb;
	public uint MSB { get; private set; } = msb;
	public Expr Expr { get; private set; } = expr;

	public override uint GetWidth()
	{
		return MSB - LSB + 1;
	}

	public override T Accept<T>(IExprVisitor<T> exprVisitor)
	{
		return exprVisitor.VisitSlice(this);
	}
}

public sealed class ConcatExpr(params Expr[] expr) : Expr
{
	public Expr[] Exprs { get; private set; } = expr;

	public override uint GetWidth()
	{
		uint len = 0;
		foreach (Expr item in Exprs)
		{
			len += item.GetWidth();
		}

		return len;
	}

	public override T Accept<T>(IExprVisitor<T> exprVisitor)
	{
		return exprVisitor.VisitConcat(this);
	}
}

public sealed class SignExtendExpr(uint width, Expr expr) : Expr
{
	public uint Width { get; private set; } = width;
	public Expr Expr { get; private set; } = expr;

	public override uint GetWidth()
	{
		return Width;
	}

	public override T Accept<T>(IExprVisitor<T> exprVisitor)
	{
		return exprVisitor.VisitSignExtend(this);
	}
}

public sealed class ZeroExtendExpr(uint width, Expr expr) : Expr
{
	public uint Width { get; private set; } = width;
	public Expr Expr { get; private set; } = expr;

	public override uint GetWidth()
	{
		return Width;
	}

	public override T Accept<T>(IExprVisitor<T> exprVisitor)
	{
		return exprVisitor.VisitZeroExtend(this);
	}
}

public static class Lit
{
	public static Expr Bits(uint width, ulong value)
	{
		return new ConstExpr(width, value);
	}
}

public sealed class ConstExpr : Expr
{
	public uint Width { get; private set; }
	public ulong Value { get; private set; }
	public ConstExpr(uint width, ulong value)
	{
		if (width is 0 or > 64)
		{
			throw new WidthMismatchException();
		}
		Width = width;
		Value = value;
	}

	public override uint GetWidth()
	{
		return Width;
	}

	public override T Accept<T>(IExprVisitor<T> exprVisitor)
	{
		return exprVisitor.VisitConst(this);
	}
}