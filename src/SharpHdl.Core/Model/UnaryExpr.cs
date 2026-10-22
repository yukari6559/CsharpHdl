using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Core.Model;

public sealed class UnaryExpr : Expr
{
	public required Expr Expr { get; set; }
	public required UnaryOp UnaryOp { get; set; }

	public override uint GetWidth()
	{
		return UnaryOp is UnaryOp.LogicNot ? 1 : Expr.GetWidth();
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
	LogicNot
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
			_ => unaryOp.ToString()
		};
	}
}