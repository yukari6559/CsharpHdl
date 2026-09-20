using SharpHdl.Core.Model;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class EvalExpr
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:条件式に変換します", Justification = "<保留中>")]
	public static ulong Eval(Expr expr, SimWorld simWorld)
	{
		if (expr is Signal signal)
		{
			return simWorld.Get(signal);
		}
		else if (expr is OpExpr opExpr)
		{
			ulong MaskBit = BitUtils.MakeMaskBit(opExpr.Left.GetWidth());
			if (opExpr.Op == Op.Plus)
			{
				return (Eval(opExpr.Left, simWorld) + Eval(opExpr.Right, simWorld)) & MaskBit;
			}
			else if (opExpr.Op == Op.Minus)
			{
				return (Eval(opExpr.Left, simWorld) - Eval(opExpr.Right, simWorld)) & MaskBit;
			}
			else if (opExpr.Op == Op.And)
			{
				return Eval(opExpr.Left, simWorld) & Eval(opExpr.Right, simWorld) & MaskBit;
			}
			else if (opExpr.Op == Op.Or)
			{
				return (Eval(opExpr.Left, simWorld) | Eval(opExpr.Right, simWorld)) & MaskBit;
			}
			else if (opExpr.Op == Op.Eq)
			{
				return (Eval(opExpr.Left, simWorld) == Eval(opExpr.Right, simWorld)) ? 1u : 0u;
			}
			else if (opExpr.Op == Op.Neq)
			{
				return (Eval(opExpr.Left, simWorld) != Eval(opExpr.Right, simWorld)) ? 1u : 0u;
			}
			else if (opExpr.Op == Op.Lt)
			{
				return (Eval(opExpr.Left, simWorld) < Eval(opExpr.Right, simWorld)) ? 1u : 0u;
			}
			else if (opExpr.Op == Op.Gt)
			{
				return (Eval(opExpr.Left, simWorld) > Eval(opExpr.Right, simWorld)) ? 1u : 0u;
			}
			else if (opExpr.Op == Op.Le)
			{
				return (Eval(opExpr.Left, simWorld) <= Eval(opExpr.Right, simWorld)) ? 1u : 0u;
			}
			else if (opExpr.Op == Op.Ge)
			{
				return (Eval(opExpr.Left, simWorld) >= Eval(opExpr.Right, simWorld)) ? 1u : 0u;
			}
			else
			{
				throw new SimUnsupportedException();
			}
		}
		else if (expr is SliceExpr sliceExpr)
		{
			uint width = sliceExpr.MSB - sliceExpr.LSB + 1;
			return (Eval(sliceExpr.Expr, simWorld) >> (int)sliceExpr.LSB) & BitUtils.MakeMaskBit(width);
		}
		else if (expr is ConcatExpr concatExpr)
		{
			ulong result = 0;
			int shiftWidth = (int)concatExpr.GetWidth();
			for (int i = 0; i < concatExpr.Exprs.Length; i++)
			{
				shiftWidth -= (int)concatExpr.Exprs[i].GetWidth();
				ulong tmp = Eval(concatExpr.Exprs[i], simWorld);
				tmp <<= shiftWidth;
				result |= tmp;
			}
			return result;
		}
		else if (expr is ZeroExtendExpr zeroExtendExpr)
		{
			return Eval(zeroExtendExpr.Expr, simWorld) & BitUtils.MakeMaskBit(zeroExtendExpr.Expr.GetWidth());
		}
		else if (expr is SignExtendExpr signExtendExpr)
		{
			ulong tmp = (Eval(signExtendExpr.Expr, simWorld) >> ((int)signExtendExpr.Expr.GetWidth() - 1)) & 1;
			if (tmp == 0)
			{
				return Eval(signExtendExpr.Expr, simWorld) & BitUtils.MakeMaskBit(signExtendExpr.Expr.GetWidth());
			}
			else
			{
				return Eval(signExtendExpr.Expr, simWorld) | (BitUtils.MakeMaskBit(signExtendExpr.GetWidth()) & ~BitUtils.MakeMaskBit(signExtendExpr.Expr.GetWidth()));
			}
		}
		else if (expr is ConstExpr constExpr)
		{
			return constExpr.Value & BitUtils.MakeMaskBit(constExpr.Width);
		}
		else
		{
			throw new SimUnsupportedException();
		}
	}
}