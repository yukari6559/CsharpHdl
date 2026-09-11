namespace SharpHdl.Core.Model;

public class Expr
{
	public static OpExpr operator +(Expr left, Expr right)
	{
		return new OpExpr(){Left = left,Right = right,Op = Op.Plus};
	}
	public static OpExpr operator -(Expr left, Expr right)
	{
		return new OpExpr(){Left = left,Right = right,Op = Op.Minus};
	}
	public static OpExpr operator &(Expr left, Expr right)
	{
		return new OpExpr(){Left = left,Right = right,Op = Op.And};
	}
	public static OpExpr operator |(Expr left, Expr right)
	{
		return new OpExpr(){Left = left,Right = right,Op = Op.Or};
	}
	public Expr Concat(params Expr[] exprs)
	{
		Expr[] thisexprs = exprs.Prepend(this).ToArray();
		return new ConcatExpr(thisexprs);
	}
	public Expr Slice(uint msb, uint lsb)
	{
		SliceExpr sliceExpr = new(msb,lsb,this);
		return sliceExpr;
	}
	public Expr SignExtend(uint width)
	{
		if(this.GetWidth() > width || this.GetWidth() == null)
			throw new WidthMismatchException();
		SignExtendExpr signExtendExpr = new(width, this);
		return signExtendExpr;
	}
	public Expr ZeroExtend(uint width)
	{
		if(this.GetWidth() > width || this.GetWidth() == null)
			throw new WidthMismatchException();
		ZeroExtendExpr zeroExtendExpr = new(width, this);
		return zeroExtendExpr;
	}
}

public static class ExprExtension
{
	public static uint? GetWidth(this object obj)
	{
		switch (obj)
		{
			case Signal signal:
				return signal.Width;
			case OpExpr opExpr:
				return opExpr.Left.GetWidth();
			case SliceExpr sliceExpr:
				return sliceExpr.MSB - sliceExpr.LSB + 1;
			case ConcatExpr concatExpr:
				uint len = 0;
				foreach(var item in concatExpr.Expr)
				{
					if(item.GetWidth() == null)
						return null;
					len += (uint)item.GetWidth()!;
				}
				return len;
			case SignExtendExpr signExtendExpr:
				return signExtendExpr.Width;
			case ZeroExtendExpr zeroExtendExpr:
				return zeroExtendExpr.Width;
		}
		return null;
	}
}

public class SliceExpr : Expr
{
	public uint LSB{get;private set;}
	public uint MSB{get;private set;}
	public Expr Expr{get;private set;}
	public SliceExpr(uint msb, uint lsb, Expr expr)
	{
		MSB = msb;
		LSB = lsb;
		Expr = expr;
	}
}

public class ConcatExpr : Expr
{
	public Expr[] Expr{get;private set;}
	public ConcatExpr(params Expr[] expr)
	{
		Expr = expr;
	}
}

public class SignExtendExpr : Expr
{
	public uint Width{get;private set;}
	public Expr Expr{get;private set;}
	public SignExtendExpr(uint width, Expr expr)
	{
		Width = width;
		Expr = expr;
	}
}

public class ZeroExtendExpr : Expr
{
	public uint Width{get;private set;}
	public Expr Expr{get;private set;}
	public ZeroExtendExpr(uint width, Expr expr)
	{
		Width = width;
		Expr = expr;
	}
}