using SharpHdl.Core.Model;

namespace SharpHdl.Sim.Interp;

public static class StmtSupport
{
	private static readonly HashSet<Type> Allowed =
	[
		typeof(AssignStmt),
		typeof(SwitchStmt),
		typeof(SeqBlockStmt),
		typeof(InstanceStmt),
		typeof(MemStmt),
		typeof(Mem2R1WStmt),
		typeof(MemWstrbStmt),
		typeof(IfStmt),
	];
	public static bool IsSupportedTopLevel(Stmt stmt)
	{
		return Allowed.Contains(stmt.GetType());
	}
}