namespace SharpHdl.Core.Model;

public static class CurrentWrite
{
	public static Module? CurrentModule{get;set;}
	public static List<Stmt>? CurrentStmts{get;set;}
	public static ModuleType? ModuleType {get;set;}
}

public enum ModuleType
{
	Comb,
	Seq
}