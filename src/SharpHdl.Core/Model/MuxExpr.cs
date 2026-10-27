using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Core.Model;

public sealed class MuxExpr : Expr
{
	public Expr Left { get; private set; }
	public Expr Right { get; private set; }
	public Expr Cond { get; private set; }
	internal MuxExpr(Expr left, Expr right, Expr cond)
	{
		Left = left;
		Right = right;
		Cond = cond;
	}
	public override T Accept<T>(IExprVisitor<T> exprVisitor)
	{
		return exprVisitor.VisitMux(this);
	}

	public override uint GetWidth()
	{
		return Left.GetWidth();
	}
}