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

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:条件式に変換します", Justification = "<保留中>")]
	public ulong VisitOp(OpExpr opExpr)
	{
		ulong MaskBit = BitUtils.MakeMaskBit(opExpr.Left.GetWidth());
		if (opExpr.Op == Op.Plus)
		{
			return (opExpr.Left.Accept(this) + opExpr.Right.Accept(this)) & MaskBit;
		}
		else if (opExpr.Op == Op.Minus)
		{
			return (opExpr.Left.Accept(this) - opExpr.Right.Accept(this)) & MaskBit;
		}
		else if (opExpr.Op == Op.And)
		{
			return opExpr.Left.Accept(this) & opExpr.Right.Accept(this) & MaskBit;
		}
		else if (opExpr.Op == Op.Or)
		{
			return (opExpr.Left.Accept(this) | opExpr.Right.Accept(this)) & MaskBit;
		}
		else if (opExpr.Op == Op.Eq)
		{
			return (opExpr.Left.Accept(this) == opExpr.Right.Accept(this)) ? 1u : 0u;
		}
		else if (opExpr.Op == Op.Neq)
		{
			return (opExpr.Left.Accept(this) != opExpr.Right.Accept(this)) ? 1u : 0u;
		}
		else if (opExpr.Op == Op.Lt)
		{
			return (opExpr.Left.Accept(this) < opExpr.Right.Accept(this)) ? 1u : 0u;
		}
		else if (opExpr.Op == Op.Gt)
		{
			return (opExpr.Left.Accept(this) > opExpr.Right.Accept(this)) ? 1u : 0u;
		}
		else if (opExpr.Op == Op.Le)
		{
			return (opExpr.Left.Accept(this) <= opExpr.Right.Accept(this)) ? 1u : 0u;
		}
		else if (opExpr.Op == Op.Ge)
		{
			return (opExpr.Left.Accept(this) >= opExpr.Right.Accept(this)) ? 1u : 0u;
		}
		else
		{
			throw new SimUnsupportedException();
		}
	}

	public ulong VisitSignal(Signal signal)
	{
		return simWorld.Get(signal);
	}

	public ulong VisitSignExtend(SignExtendExpr signExtendExpr)
	{
		ulong tmp = (signExtendExpr.Expr.Accept(this) >> ((int)signExtendExpr.Expr.GetWidth() - 1)) & 1;
		return tmp == 0
			? signExtendExpr.Expr.Accept(this) & BitUtils.MakeMaskBit(signExtendExpr.Expr.GetWidth())
			: signExtendExpr.Expr.Accept(this) | (BitUtils.MakeMaskBit(signExtendExpr.GetWidth()) & ~BitUtils.MakeMaskBit(signExtendExpr.Expr.GetWidth()));
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