namespace SharpHdl.Core.Model;

public static class CurrentWrite
{
	[ThreadStatic]
	private static List<CurrentWriteData>? CurrentWriteDatas;
	public static Module? CurrentModule
	{
		get => CurrentWriteDatas == null || CurrentWriteDatas.Count == 0 ? null : CurrentWriteDatas[^1].CurrentModule;
		set
		{
			if (CurrentWriteDatas == null || CurrentWriteDatas.Count == 0)
			{
				throw new InvalidOperationException();
			}

			CurrentWriteDatas[^1].CurrentModule = value;
		}
	}
	public static List<Stmt>? CurrentStmts
	{
		get => CurrentWriteDatas == null || CurrentWriteDatas.Count == 0 ? null : CurrentWriteDatas[^1].CurrentStmts;
		set
		{
			if (CurrentWriteDatas == null || CurrentWriteDatas.Count == 0)
			{
				throw new InvalidOperationException();
			}

			CurrentWriteDatas[^1].CurrentStmts = value;
		}
	}
	public static ModuleType? ModuleType
	{
		get => CurrentWriteDatas == null || CurrentWriteDatas.Count == 0 ? null : CurrentWriteDatas[^1].ModuleType;
		set
		{
			if (CurrentWriteDatas == null || CurrentWriteDatas.Count == 0)
			{
				throw new InvalidOperationException();
			}

			CurrentWriteDatas[^1].ModuleType = value;
		}
	}
	public static void Push(Module module, List<Stmt> stmts, ModuleType moduleType)
	{
		CurrentWriteDatas ??= [];
		CurrentWriteDatas.Add(new CurrentWriteData { CurrentModule = module, CurrentStmts = stmts, ModuleType = moduleType });
	}
	public static void Pop()
	{
		if (CurrentWriteDatas == null || CurrentWriteDatas.Count == 0)
		{
			throw new InvalidOperationException();
		}

		CurrentWriteDatas.RemoveAt(CurrentWriteDatas.Count - 1);
	}
}

public class CurrentWriteData
{
	public Module? CurrentModule { get; set; }
	public List<Stmt>? CurrentStmts { get; set; }
	public ModuleType? ModuleType { get; set; }
}

public enum ModuleType
{
	Comb,
	Seq
}