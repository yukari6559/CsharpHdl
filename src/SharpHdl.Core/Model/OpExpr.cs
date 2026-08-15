namespace SharpHdl.Core.Model;

public class OpExpr : Expr
{
	public required Expr Left {get;set;}
	public required Expr Right {get;set;}
	public Op Op {get;set;}
}

public enum Op
{
	Plus
}