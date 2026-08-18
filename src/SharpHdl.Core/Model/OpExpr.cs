namespace SharpHdl.Core.Model;

public class OpExpr : Expr
{
	public required Expr Left {get;set;}
	public required Expr Right {get;set;}
	public Op Op {get;set;}
}

public enum Op
{
	Plus,
	Minus,
	And,
	Or
}

public static class OpExtension
{
	public static string ToCustomString(this Op op) => op switch
	{
		Op.Plus => "+",
		Op.Minus => "-",
		Op.And => "&",
		Op.Or => "|",
		_ => op.ToString()
	};
}