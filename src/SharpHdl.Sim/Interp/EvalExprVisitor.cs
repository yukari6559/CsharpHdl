using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class EvalExprVisitor(SimWorld simWorld) : IExprVisitor<ulong>
{
	public ulong VisitConcat(ConcatExpr concatExpr)
	{
		ulong result = 0;
		int shiftWidth = (int)concatExpr.GetWidth();
		for (int i = 0; i < concatExpr.Exprs.Length; i++)
		{
			shiftWidth -= (int)concatExpr.Exprs[i].GetWidth();
			ulong tmp = concatExpr.Exprs[i].Accept(this);
			tmp <<= shiftWidth;
			result |= tmp;
		}
		return result;
	}

	public ulong VisitConst(ConstExpr constExpr)
	{
		return constExpr.Value & BitUtils.MakeMaskBit(constExpr.Width);
	}

	public ulong VisitOp(OpExpr opExpr)
	{
		ulong maskBit = BitUtils.MakeMaskBit(opExpr.Left.GetWidth());
		return opExpr.Op switch
		{
			Op.Plus => (opExpr.Left.Accept(this) + opExpr.Right.Accept(this)) & maskBit,
			Op.Minus => (opExpr.Left.Accept(this) - opExpr.Right.Accept(this)) & maskBit,
			Op.And => opExpr.Left.Accept(this) & opExpr.Right.Accept(this) & maskBit,
			Op.Or => (opExpr.Left.Accept(this) | opExpr.Right.Accept(this)) & maskBit,
			Op.Eq => (opExpr.Left.Accept(this) == opExpr.Right.Accept(this)) ? 1u : 0u,
			Op.Neq => (opExpr.Left.Accept(this) != opExpr.Right.Accept(this)) ? 1u : 0u,
			Op.Lt => (opExpr.Left.Accept(this) < opExpr.Right.Accept(this)) ? 1u : 0u,
			Op.Gt => (opExpr.Left.Accept(this) > opExpr.Right.Accept(this)) ? 1u : 0u,
			Op.Le => (opExpr.Left.Accept(this) <= opExpr.Right.Accept(this)) ? 1u : 0u,
			Op.Ge => (opExpr.Left.Accept(this) >= opExpr.Right.Accept(this)) ? 1u : 0u,
			_ => throw new SimUnsupportedException()
		};
	}

	public ulong VisitSignal(Signal signal)
	{
		return simWorld.Get(signal);
	}

	public ulong VisitSignExtend(SignExtendExpr signExtendExpr)
	{
		ulong signExtendExprValue = signExtendExpr.Expr.Accept(this);
		ulong tmp = (signExtendExprValue >> ((int)signExtendExpr.Expr.GetWidth() - 1)) & 1;
		return tmp == 0
			? signExtendExprValue & BitUtils.MakeMaskBit(signExtendExpr.Expr.GetWidth())
			: signExtendExprValue | (BitUtils.MakeMaskBit(signExtendExpr.GetWidth()) & ~BitUtils.MakeMaskBit(signExtendExpr.Expr.GetWidth()));
	}

	public ulong VisitSlice(SliceExpr sliceExpr)
	{
		uint width = sliceExpr.MSB - sliceExpr.LSB + 1;
		return (sliceExpr.Expr.Accept(this) >> (int)sliceExpr.LSB) & BitUtils.MakeMaskBit(width);
	}

	public ulong VisitZeroExtend(ZeroExtendExpr zeroExtendExpr)
	{
		return zeroExtendExpr.Expr.Accept(this) & BitUtils.MakeMaskBit(zeroExtendExpr.Expr.GetWidth());
	}
}