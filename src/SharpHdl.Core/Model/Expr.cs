using SharpHdl.Core.Exceptions;

namespace SharpHdl.Core.Model;

public abstract class Expr
{
	public static OpExpr operator +(Expr left, Expr right)
	{
		return new OpExpr() { Left = left, Right = right, Op = Op.Plus };
	}
	public static OpExpr operator -(Expr left, Expr right)
	{
		return new OpExpr() { Left = left, Right = right, Op = Op.Minus };
	}
	public static OpExpr operator &(Expr left, Expr right)
	{
		return new OpExpr() { Left = left, Right = right, Op = Op.And };
	}
	public static OpExpr operator |(Expr left, Expr right)
	{
		return new OpExpr() { Left = left, Right = right, Op = Op.Or };
	}
	public static OpExpr operator <(Expr left, Expr right)
	{
		return new OpExpr() { Left = left, Right = right, Op = Op.Lt };
	}
	public static OpExpr operator >(Expr left, Expr right)
	{
		return new OpExpr() { Left = left, Right = right, Op = Op.Gt };
	}
	public static OpExpr operator <=(Expr left, Expr right)
	{
		return new OpExpr() { Left = left, Right = right, Op = Op.Le };
	}
	public static OpExpr operator >=(Expr left, Expr right)
	{
		return new OpExpr() { Left = left, Right = right, Op = Op.Ge };
	}
	public Expr Eq(Expr right)
	{
		return new OpExpr() { Left = this, Right = right, Op = Op.Eq };
	}
	public Expr Neq(Expr right)
	{
		return new OpExpr() { Left = this, Right = right, Op = Op.Neq };
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
}

public class SliceExpr(uint msb, uint lsb, Expr expr) : Expr
{
	public uint LSB { get; private set; } = lsb;
	public uint MSB { get; private set; } = msb;
	public Expr Expr { get; private set; } = expr;

	public override uint GetWidth()
	{
		return MSB - LSB + 1;
	}
}

public class ConcatExpr(params Expr[] expr) : Expr
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
}

public class SignExtendExpr(uint width, Expr expr) : Expr
{
	public uint Width { get; private set; } = width;
	public Expr Expr { get; private set; } = expr;

	public override uint GetWidth()
	{
		return Width;
	}
}

public class ZeroExtendExpr(uint width, Expr expr) : Expr
{
	public uint Width { get; private set; } = width;
	public Expr Expr { get; private set; } = expr;

	public override uint GetWidth()
	{
		return Width;
	}
}

public static class Lit
{
	public static Expr Bits(uint width, ulong value)
	{
		return new ConstExpr(width, value);
	}
}

public class ConstExpr : Expr
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
}