using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Core.Validate;

public class CollectSignalsExprVisitor : IExprVisitor<HashSet<Signal>>
{
	private readonly HashSet<Signal> signals = [];
	public HashSet<Signal> VisitConcat(ConcatExpr concatExpr)
	{
		foreach (Expr expr in concatExpr.Exprs)
		{
			_ = expr.Accept(this);
		}
		return signals;
	}

	public HashSet<Signal> VisitConst(ConstExpr constExpr)
	{
		return signals;
	}

	public HashSet<Signal> VisitOp(OpExpr opExpr)
	{
		_ = opExpr.Left.Accept(this);
		_ = opExpr.Right.Accept(this);
		return signals;
	}

	public HashSet<Signal> VisitUnary(UnaryExpr unaryExpr)
	{
		_ = unaryExpr.Expr.Accept(this);
		return signals;
	}

	public HashSet<Signal> VisitSignal(Signal signal)
	{
		_ = signals.Add(signal);
		return signals;
	}

	public HashSet<Signal> VisitSignExtend(SignExtendExpr signExtendExpr)
	{
		_ = signExtendExpr.Expr.Accept(this);
		return signals;
	}

	public HashSet<Signal> VisitSlice(SliceExpr sliceExpr)
	{
		_ = sliceExpr.Expr.Accept(this);
		return signals;
	}

	public HashSet<Signal> VisitZeroExtend(ZeroExtendExpr zeroExtendExpr)
	{
		_ = zeroExtendExpr.Expr.Accept(this);
		return signals;
	}
}