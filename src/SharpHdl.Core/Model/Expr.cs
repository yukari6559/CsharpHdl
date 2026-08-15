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