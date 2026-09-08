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
		}
		return null;
	}
}