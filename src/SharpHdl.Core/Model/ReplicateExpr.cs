using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Core.Model;

public sealed class ReplicateExpr : Expr
{
	public int Count { get; }
	public Expr Expr { get; }
	internal ReplicateExpr(int count, Expr expr)
	{
		Count = count;
		Expr = expr;
	}
	public override T Accept<T>(IExprVisitor<T> exprVisitor)
	{
		return exprVisitor.VisitReplicate(this);
	}

	public override uint GetWidth()
	{
		return (uint)(Count * Expr.GetWidth());
	}
}