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

	public string VisitSignal(Signal signal)
	{
		return signal.Name;
	}

	public string VisitSignExtend(SignExtendExpr signExtendExpr)
	{
		if (signExtendExpr.GetWidth() - signExtendExpr.Expr.GetWidth() == 0)
		{
			return signExtendExpr.Expr.Accept(this);
		}
		else
		{
			string s = "{{" + $"{signExtendExpr.GetWidth() - signExtendExpr.Expr.GetWidth()}{{" + $"{signExtendExpr.Expr.Accept(this)}[{signExtendExpr.Expr.GetWidth() - 1}]" + "}}, " + $"{signExtendExpr.Expr.Accept(this)}" + "}";
			return s;
		}
	}

	public string VisitSlice(SliceExpr sliceExpr)
	{
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