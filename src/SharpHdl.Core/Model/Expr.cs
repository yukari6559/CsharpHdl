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
		}
		return null;
	}
}