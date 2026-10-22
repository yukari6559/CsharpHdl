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
	public static OpExpr operator *(Expr left, Expr right)
	{
		CheckWidth(left, right);
		return new OpExpr() { Left = left, Right = right, Op = Op.Mul };
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
	public static OpExpr operator <<(Expr left, Expr right)
	{
		return new OpExpr() { Left = left, Right = right, Op = Op.Shl };
	}
	public static OpExpr operator >>(Expr left, Expr right)
	{
		return new OpExpr() { Left = left, Right = right, Op = Op.Shr };
	}
	public static OpExpr operator <<(Expr left, int right)
	{
		return new OpExpr() { Left = left, Right = ShiftAmountLit(right), Op = Op.Shl };
	}
	public static OpExpr operator >>(Expr left, int right)
	{
		return new OpExpr() { Left = left, Right = ShiftAmountLit(right), Op = Op.Shr };
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
		if (this is not Signal and not SliceExpr)
		{
			throw new UnsupportedSelectException("Slice is only supported on a signal or a slice of a signal.");
		}
		if (lsb > msb || msb >= GetWidth())
		{
			throw new WidthMismatchException($"Slice [{msb}:{lsb}] is out of range for width {GetWidth()}.");
		}
		if (this is SliceExpr innerSliceExpr)
		{
			return new SliceExpr(innerSliceExpr.LSB + msb, innerSliceExpr.LSB + lsb, innerSliceExpr.Expr);
		}
		SliceExpr sliceExpr = new(msb, lsb, this);
		return sliceExpr;
	}
	public Expr SignExtend(uint width)
	{
		if (this is not Signal and not SliceExpr)
		{
			throw new UnsupportedSelectException("SignExtend is only supported on a signal or a slice of a signal.");
		}
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
		if (GetWidth() == width)
		{
			return this;
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
	private static Expr ShiftAmountLit(int right)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(right);
		uint width = (uint)(32 - System.Numerics.BitOperations.LeadingZeroCount((uint)right));
		if (right == 0)
		{
			width = 1;
		}
		return Lit.Bits(width, (ulong)right);
	}
}

public sealed class SliceExpr : Expr
{
	public uint LSB { get; private set; }
	public uint MSB { get; private set; }
	public Expr Expr { get; private set; }

	internal SliceExpr(uint msb, uint lsb, Expr expr)
	{
		LSB = lsb;
		MSB = msb;
		Expr = expr;
	}

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

public sealed class SignExtendExpr : Expr
{
	public uint Width { get; private set; }
	public Expr Expr { get; private set; }

	internal SignExtendExpr(uint width, Expr expr)
	{
		Width = width;
		Expr = expr;
	}

	public override uint GetWidth()
	{
		return Width;
	}

	public override T Accept<T>(IExprVisitor<T> exprVisitor)
	{
		return exprVisitor.VisitSignExtend(this);
	}
}

public sealed class ZeroExtendExpr : Expr
{
	public uint Width { get; private set; }
	public Expr Expr { get; private set; }

	internal ZeroExtendExpr(uint width, Expr expr)
	{
		Width = width;
		Expr = expr;
	}

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