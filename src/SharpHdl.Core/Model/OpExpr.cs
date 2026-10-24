using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Core.Model;

public sealed class OpExpr : Expr
{
	public Expr Left { get; private set; }
	public Expr Right { get; private set; }
	public Op Op { get; private set; }

	internal OpExpr(Expr left, Expr right, Op op)
	{
		Left = left;
		Right = right;
		Op = op;
	}

	public override uint GetWidth()
	{
		return Op is Op.Eq or Op.Neq or Op.Lt or Op.Gt or Op.Le or Op.Ge or Op.LogicAnd or Op.LogicOr ? 1 : Left.GetWidth();
	}

	public override T Accept<T>(IExprVisitor<T> exprVisitor)
	{
		return exprVisitor.VisitOp(this);
	}
}

public enum Op
{
	Plus,
	Minus,
	Mul,
	And,
	LogicAnd,
	Or,
	LogicOr,
	Xor,
	Xnor,
	Eq,
	Neq,
	Lt,
	Gt,
	Le,
	Ge,
	Shl,
	Shr
}

public static class OpExtension
{
	public static string ToCustomString(this Op op)
	{
		return op switch
		{
			Op.Plus => "+",
			Op.Minus => "-",
			Op.Mul => "*",
			Op.And => "&",
			Op.LogicAnd => "&&",
			Op.Or => "|",
			Op.LogicOr => "||",
			Op.Xor => "^",
			Op.Xnor => "~^",
			Op.Eq => "==",
			Op.Neq => "!=",
			Op.Lt => "<",
			Op.Gt => ">",
			Op.Le => "<=",
			Op.Ge => ">=",
			Op.Shl => "<<",
			Op.Shr => ">>",
			_ => op.ToString()
		};
	}
}