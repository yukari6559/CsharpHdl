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
			Op.Mul => (opExpr.Left.Accept(this) * opExpr.Right.Accept(this)) & maskBit,
			Op.Div => EvalDiv(opExpr.Left, opExpr.Right, maskBit),
			Op.Mod => EvalMod(opExpr.Left, opExpr.Right, maskBit),
			Op.And => opExpr.Left.Accept(this) & opExpr.Right.Accept(this) & maskBit,
			Op.LogicAnd => (opExpr.Left.Accept(this) != 0 && opExpr.Right.Accept(this) != 0) ? 1u : 0u,
			Op.Or => (opExpr.Left.Accept(this) | opExpr.Right.Accept(this)) & maskBit,
			Op.LogicOr => (opExpr.Left.Accept(this) != 0 || opExpr.Right.Accept(this) != 0) ? 1u : 0u,
			Op.Xor => (opExpr.Left.Accept(this) ^ opExpr.Right.Accept(this)) & maskBit,
			Op.Xnor => (~(opExpr.Left.Accept(this) ^ opExpr.Right.Accept(this))) & maskBit,
			Op.Eq => (opExpr.Left.Accept(this) == opExpr.Right.Accept(this)) ? 1u : 0u,
			Op.Neq => (opExpr.Left.Accept(this) != opExpr.Right.Accept(this)) ? 1u : 0u,
			Op.Lt => (opExpr.Left.Accept(this) < opExpr.Right.Accept(this)) ? 1u : 0u,
			Op.Gt => (opExpr.Left.Accept(this) > opExpr.Right.Accept(this)) ? 1u : 0u,
			Op.Le => (opExpr.Left.Accept(this) <= opExpr.Right.Accept(this)) ? 1u : 0u,
			Op.Ge => (opExpr.Left.Accept(this) >= opExpr.Right.Accept(this)) ? 1u : 0u,
			Op.Shl => BitUtils.ShiftLeft(opExpr.Left.Accept(this), opExpr.Right.Accept(this), opExpr.GetWidth()),
			Op.Shr => BitUtils.ShiftRight(opExpr.Left.Accept(this), opExpr.Right.Accept(this), opExpr.GetWidth()),
			_ => throw new SimUnsupportedException()
		};
	}

	public ulong VisitUnary(UnaryExpr unaryExpr)
	{
		ulong maskBit = BitUtils.MakeMaskBit(unaryExpr.Expr.GetWidth());
		return unaryExpr.UnaryOp switch
		{
			UnaryOp.Not => ~unaryExpr.Expr.Accept(this) & maskBit,
			UnaryOp.Neg => (0 - unaryExpr.Expr.Accept(this)) & maskBit,
			UnaryOp.LogicNot => unaryExpr.Expr.Accept(this) == 0u ? 1u : 0u,
			UnaryOp.ReduceAnd => (unaryExpr.Expr.Accept(this) & maskBit) == maskBit ? 1u : 0u,
			UnaryOp.ReduceOr => (unaryExpr.Expr.Accept(this) & maskBit) == 0u ? 0u : 1u,
			UnaryOp.ReduceXor => (ulong)System.Numerics.BitOperations.PopCount(unaryExpr.Expr.Accept(this) & maskBit) % 2,
			UnaryOp.ReduceNand => (unaryExpr.Expr.Accept(this) & maskBit) == maskBit ? 0u : 1u,
			UnaryOp.ReduceNor => (unaryExpr.Expr.Accept(this) & maskBit) == 0u ? 1u : 0u,
			UnaryOp.ReduceXnor => (ulong)System.Numerics.BitOperations.PopCount(unaryExpr.Expr.Accept(this) & maskBit) % 2 == 0 ? 1u : 0u,
			_ => throw new SimUnsupportedException(),
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

	private ulong EvalDiv(Expr left, Expr right, ulong maskBit)
	{
		ulong rightValue = right.Accept(this);
		if (rightValue == 0)
		{
			throw new SimException("Division by zero.");
		}
		return (left.Accept(this) / rightValue) & maskBit;
	}

	private ulong EvalMod(Expr left, Expr right, ulong maskBit)
	{
		ulong rightValue = right.Accept(this);
		if (rightValue == 0)
		{
			throw new SimException("Modulo by zero.");
		}
		return (left.Accept(this) % rightValue) & maskBit;
	}
}
