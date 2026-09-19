namespace SharpHdl.Core.Model;

public static class CurrentWrite
{
	[ThreadStatic]
	public static List<CurrentWriteData>? CurrentWriteDatas;
	public static Module? CurrentModule
	{
		get
		{
			if(CurrentWriteDatas == null || CurrentWriteDatas.Count == 0)
				return null;
			return CurrentWriteDatas[^1]._CurrentModule;
		}
		set
		{
			if(CurrentWriteDatas == null || CurrentWriteDatas.Count == 0)
				throw new Exception();
			CurrentWriteDatas![^1]._CurrentModule = value;
		}
	}
	public static List<Stmt>? CurrentStmts
	{
		get
		{
			if(CurrentWriteDatas == null || CurrentWriteDatas.Count == 0)
				return null;
			return CurrentWriteDatas[^1]._CurrentStmts;
		}
		set
		{
			if(CurrentWriteDatas == null || CurrentWriteDatas.Count == 0)
				throw new Exception();
			CurrentWriteDatas![^1]._CurrentStmts = value;
		}
	}
	public static ModuleType? ModuleType
	{
		get
		{
			if(CurrentWriteDatas == null || CurrentWriteDatas.Count == 0)
				return null;
			return CurrentWriteDatas[^1]._ModuleType;
		}
		set
		{
			if(CurrentWriteDatas == null || CurrentWriteDatas.Count == 0)
				throw new Exception();
			CurrentWriteDatas![^1]._ModuleType = value;
		}
	}
	public static void Push(Module module, List<Stmt> stmts, ModuleType moduleType)
	{
		CurrentWriteDatas ??= new();
		CurrentWriteDatas.Add(new CurrentWriteData{_CurrentModule = module, _CurrentStmts = stmts, _ModuleType = moduleType});
	}
	public static void Pop()
	{
		if(CurrentWriteDatas == null || CurrentWriteDatas.Count == 0)
			throw new Exception();
		CurrentWriteDatas.RemoveAt(CurrentWriteDatas.Count - 1);
	}
}

public class CurrentWriteData
{
	public Module? _CurrentModule{get;set;}
	public List<Stmt>? _CurrentStmts{get;set;}
	public ModuleType? _ModuleType{get;set;}
}

public enum ModuleType
{
	Comb,
	Seq
}