using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Core.Model;

public sealed class UnaryExpr : Expr
{
	public Expr Expr { get; private set; }
	public UnaryOp UnaryOp { get; private set; }

	internal UnaryExpr(Expr expr, UnaryOp unaryOp)
	{
		Expr = expr;
		UnaryOp = unaryOp;
	}

	public override uint GetWidth()
	{
		return UnaryOp is UnaryOp.Not or UnaryOp.Neg ? Expr.GetWidth() : 1;
	}

	public override T Accept<T>(IExprVisitor<T> exprVisitor)
	{
		return exprVisitor.VisitUnary(this);
	}
}

public enum UnaryOp
{
	Not,
	Neg,
	LogicNot,
	ReduceAnd,
	ReduceOr,
	ReduceXor,
	ReduceNand,
	ReduceNor,
	ReduceXnor
}

public static class UnaryOpExtension
{
	public static string ToCustomString(this UnaryOp unaryOp)
	{
		return unaryOp switch
		{
			UnaryOp.Not => "~",
			UnaryOp.Neg => "-",
			UnaryOp.LogicNot => "!",
			UnaryOp.ReduceAnd => "&",
			UnaryOp.ReduceOr => "|",
			UnaryOp.ReduceXor => "^",
			UnaryOp.ReduceNand => "~&",
			UnaryOp.ReduceNor => "~|",
			UnaryOp.ReduceXnor => "~^",
			_ => unaryOp.ToString()
		};
	}
}