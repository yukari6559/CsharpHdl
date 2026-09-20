namespace SharpHdl.Core.Model;

public class OpExpr : Expr
{
	public required Expr Left { get; set; }
	public required Expr Right { get; set; }
	public Op Op { get; set; }

	public override uint GetWidth()
	{
		return Op is Op.Eq or Op.Neq or Op.Lt or Op.Gt or Op.Le or Op.Ge ? 1 : Left.GetWidth();
	}
}

public enum Op
{
	Plus,
	Minus,
	And,
	Or,
	Eq,
	Neq,
	Lt,
	Gt,
	Le,
	Ge
}

public static class OpExtension
{
	public static string ToCustomString(this Op op)
	{
		return op switch
		{
			Op.Plus => "+",
			Op.Minus => "-",
			Op.And => "&",
			Op.Or => "|",
			Op.Eq => "==",
			Op.Neq => "!=",
			Op.Lt => "<",
			Op.Gt => ">",
			Op.Le => "<=",
			Op.Ge => ">=",
			_ => op.ToString()
		};
	}
}