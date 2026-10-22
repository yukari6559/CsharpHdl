using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Emit.Visitors;

public class EmitExprVisitor : IExprVisitor<string>
{
	public string VisitConcat(ConcatExpr concatExpr)
	{
		string s = "{";
		for (int i = 0; i < concatExpr.Exprs.Length; i++)
		{
			if (i == concatExpr.Exprs.Length - 1)
			{
				s += $"{concatExpr.Exprs[i].Accept(this)}";
			}
			else
			{
				s += $"{concatExpr.Exprs[i].Accept(this)}, ";
			}
		}
		s += "}";
		return s;
	}

	public string VisitConst(ConstExpr constExpr)
	{
		ulong masked = constExpr.Width != 64
				? constExpr.Value & ((1UL << (int)constExpr.Width) - 1)
				: constExpr.Width == 64 ? constExpr.Value : throw new WidthMismatchException();
		string s = $"{constExpr.Width}'d{masked}";
		return s;
	}

	public string VisitOp(OpExpr opExpr)
	{
		string s = "";
		if (opExpr.Left is OpExpr)
		{
			s += "(" + opExpr.Left.Accept(this) + ")" + " ";
		}
		else
		{
			s += opExpr.Left.Accept(this) + " ";
		}
		s += opExpr.Op.ToCustomString() + " ";
		if (opExpr.Right is OpExpr)
		{
			s += "(" + opExpr.Right.Accept(this) + ")";
		}
		else
		{
			s += opExpr.Right.Accept(this);
		}
		return s;
	}

	public string VisitUnary(UnaryExpr unaryExpr)
	{
		string s = "";
		if (unaryExpr.UnaryOp is UnaryOp.LogicNot && unaryExpr.Expr.GetWidth() != 1)
		{
			s += "~|";
		}
		else
		{
			s += unaryExpr.UnaryOp.ToCustomString();
		}
		if (unaryExpr.Expr is OpExpr or UnaryExpr)
		{
			s += "(" + unaryExpr.Expr.Accept(this) + ")";
		}
		else
		{
			s += unaryExpr.Expr.Accept(this);
		}
		return s;
	}

	public string VisitSignal(Signal signal)
	{
		return signal.Name;
	}

	public string VisitSignExtend(SignExtendExpr signExtendExpr)
	{
		string signBit;
		if (signExtendExpr.GetWidth() - signExtendExpr.Expr.GetWidth() == 0)
		{
			return signExtendExpr.Expr.Accept(this);
		}

		string repeatCount = $"{signExtendExpr.GetWidth() - signExtendExpr.Expr.GetWidth()}";

		if (signExtendExpr.Expr is SliceExpr sliceExpr)
		{
			if (sliceExpr.Expr.GetWidth() == 1)
			{
				signBit = $"{sliceExpr.Expr.Accept(this)}";
			}
			else
			{
				signBit = $"{sliceExpr.Expr.Accept(this)}[{sliceExpr.MSB}]";
			}
		}
		else
		{
			if (signExtendExpr.Expr.GetWidth() == 1)
			{
				signBit = signExtendExpr.Expr.Accept(this);
			}
			else
			{
				signBit = $"{signExtendExpr.Expr.Accept(this)}[{signExtendExpr.Expr.GetWidth() - 1}]";
			}
		}
		return "{" + "{" + $"{repeatCount}" + "{" + $"{signBit}" + "}" + "}" + ", " + $"{signExtendExpr.Expr.Accept(this)}" + "}";
	}

	public string VisitSlice(SliceExpr sliceExpr)
	{
		if (sliceExpr.LSB == 0 && sliceExpr.MSB == sliceExpr.Expr.GetWidth() - 1)
		{
			return sliceExpr.Expr.Accept(this);
		}
		return sliceExpr.Expr.Accept(this) + $"[{sliceExpr.MSB}:{sliceExpr.LSB}]";
	}

	public string VisitZeroExtend(ZeroExtendExpr zeroExtendExpr)
	{
		if (zeroExtendExpr.GetWidth() - zeroExtendExpr.Expr.GetWidth() == 0)
		{
			return zeroExtendExpr.Expr.Accept(this);
		}
		else
		{
			string s = "{{" + $"{zeroExtendExpr.GetWidth() - zeroExtendExpr.Expr.GetWidth()}" + "{1'b0}}, " + $"{zeroExtendExpr.Expr.Accept(this)}}}";
			return s;
		}
	}
}