namespace SharpHdl.Core.Model;

public static class CurrentWrite
{
	[ThreadStatic]
	static Module? _CurrentModule;
	[ThreadStatic]
	static List<Stmt>? _CurrentStmts;
	[ThreadStatic]
	static ModuleType? _ModuleType;

	public static Module? CurrentModule
	{
		get => _CurrentModule;
		set => _CurrentModule = value; 
	}
	public static List<Stmt>? CurrentStmts
	{
		get => _CurrentStmts;
		set => _CurrentStmts = value;
	}
	public static ModuleType? ModuleType
	{
		get => _ModuleType;
		set => _ModuleType = value;
	}
}

public enum ModuleType
{
	Comb,
	Seq
}