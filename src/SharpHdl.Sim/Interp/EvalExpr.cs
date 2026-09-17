using SharpHdl.Core.Model;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;
public class EvalExpr
{
	public ulong Eval(Expr expr, SimWorld simWorld)
	{
		if(expr is Signal signal)
			return simWorld.Get(signal);
		else if(expr is OpExpr opExpr)
		{
			ulong MaskBit;
			if(opExpr.Left.GetWidth() == null)
				throw new Exception();
			else
				MaskBit = BitUtils.MakeMaskBit((uint)opExpr.Left.GetWidth()!);
			if(opExpr.Op == Op.Plus)
			{
				return (Eval(opExpr.Left, simWorld) + Eval(opExpr.Right, simWorld)) & MaskBit;
			}
			else if(opExpr.Op == Op.Minus)
			{
				return (Eval(opExpr.Left, simWorld) - Eval(opExpr.Right, simWorld)) & MaskBit;
			}
			else if(opExpr.Op == Op.And)
			{
				return (Eval(opExpr.Left, simWorld) & Eval(opExpr.Right, simWorld)) & MaskBit;
			}
			else if (opExpr.Op == Op.Or)
			{
				return (Eval(opExpr.Left, simWorld) | Eval(opExpr.Right, simWorld)) & MaskBit;
			}
			else if(opExpr.Op == Op.Eq)
			{
				return (Eval(opExpr.Left, simWorld) == Eval(opExpr.Right, simWorld)) ? 1u : 0u;
			}
			else if(opExpr.Op == Op.Neq)
			{
				return (Eval(opExpr.Left, simWorld) != Eval(opExpr.Right, simWorld)) ? 1u : 0u;
			}
			else if(opExpr.Op == Op.Lt)
			{
				return (Eval(opExpr.Left, simWorld) < Eval(opExpr.Right, simWorld)) ? 1u : 0u;
			}
			else if(opExpr.Op == Op.Gt)
			{
				return (Eval(opExpr.Left, simWorld) > Eval(opExpr.Right, simWorld)) ? 1u : 0u;
			}
			else if(opExpr.Op == Op.Le)
			{
				return (Eval(opExpr.Left, simWorld) <= Eval(opExpr.Right, simWorld)) ? 1u : 0u;
			}
			else if(opExpr.Op == Op.Ge)
			{
				return (Eval(opExpr.Left, simWorld) >= Eval(opExpr.Right, simWorld)) ? 1u : 0u;
			}
			else
				throw new SimUnsupportedException();
		}
		else if(expr is SliceExpr sliceExpr)
		{
			uint width = sliceExpr.MSB - sliceExpr.LSB + 1;
			return (Eval(sliceExpr.Expr, simWorld) >> (int)sliceExpr.LSB) & BitUtils.MakeMaskBit(width);
		}
		else if(expr is ConcatExpr concatExpr)
		{
			ulong result = 0;
			if(concatExpr.GetWidth() == null)
				throw new NullReferenceException();
			int shiftWidth = (int)concatExpr.GetWidth()!;
			for(int i = 0; i < concatExpr.Exprs.Count(); i++)
			{
				shiftWidth -= (int)concatExpr.Exprs[i].GetWidth()!;
				ulong tmp = Eval(concatExpr.Exprs[i], simWorld);
				tmp <<= shiftWidth;
				result |= tmp;
			}
			return result;
		}
		else if(expr is ZeroExtendExpr zeroExtendExpr)
		{
			if(zeroExtendExpr.Expr.GetWidth() == null)
				throw new NullReferenceException();
			return Eval(zeroExtendExpr.Expr, simWorld) & BitUtils.MakeMaskBit((uint)zeroExtendExpr.Expr.GetWidth()!);
		}
		else if(expr is SignExtendExpr signExtendExpr)
		{
			if(signExtendExpr.Expr.GetWidth() == null)
				throw new NullReferenceException();
			ulong tmp = Eval(signExtendExpr.Expr, simWorld) >> ((int)signExtendExpr.Expr.GetWidth()! - 1) & 1;
			if(tmp == 0)
				return Eval(signExtendExpr.Expr, simWorld) & BitUtils.MakeMaskBit((uint)signExtendExpr.Expr.GetWidth()!);
			else
				return Eval(signExtendExpr.Expr, simWorld) | (BitUtils.MakeMaskBit((uint)signExtendExpr.GetWidth()!) & ~BitUtils.MakeMaskBit((uint)signExtendExpr.Expr.GetWidth()!));
		}
		else
			throw new SimUnsupportedException();
	}
}