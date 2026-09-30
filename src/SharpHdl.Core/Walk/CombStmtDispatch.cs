using SharpHdl.Core.Model;

namespace SharpHdl.Core.Walk;

public class CombStmtDispatch
{
	public static void WalkComb(IEnumerable<Stmt> stmts, ICombStmtHandler combStmtHandler)
	{
		foreach (Stmt stmt in stmts)
		{
			if (stmt is AssignStmt assignStmt)
			{
				combStmtHandler.OnAssign(assignStmt);
			}
			else if (stmt is SwitchStmt switchStmt)
			{
				combStmtHandler.OnSwitch(switchStmt);
			}
			else if (stmt is IfStmt ifStmt)
			{
				combStmtHandler.OnIf(ifStmt);
			}
			else if (stmt is InstanceStmt instanceStmt)
			{
				combStmtHandler.OnInstance(instanceStmt);
			}
			else if (stmt is Mem2R1WStmt mem2R1WStmt)
			{
				combStmtHandler.OnMem2R1WStmt(mem2R1WStmt);
			}
		}
	}
}