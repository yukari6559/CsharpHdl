using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;

namespace SharpHdl.Core.Validate;

public static class CheckMultiDrive
{
	public static void Check(List<Stmt> stmts)
	{
		HashSet<Signal> assignedSignal = [];
		foreach (Stmt stmt in stmts)
		{
			if (stmt is AssignStmt assignStmt)
			{
				if (!assignedSignal.Add(assignStmt.Signal))
				{
					throw new MultiDriveException();
				}
			}
			if (stmt is SwitchStmt switchStmt)
			{
				if (switchStmt.Cases.Count > 0 && switchStmt.Cases[0].Stmts.Count > 0 && switchStmt.Cases[0].Stmts[0] is AssignStmt stmt1)
				{
					if (!assignedSignal.Add(stmt1.Signal))
					{
						throw new MultiDriveException();
					}
				}
			}
			if (stmt is IfStmt ifStmt)
			{
				if (ifStmt.Then.Count > 0 && ifStmt.Then[0] is AssignStmt stmt1)
				{
					if (!assignedSignal.Add(stmt1.Signal))
					{
						throw new MultiDriveException();
					}
				}
			}
			if (stmt is SeqBlockStmt seqBlockStmt)
			{
				foreach (Stmt bodystmt in seqBlockStmt.Body)
				{
					if (bodystmt is SeqAssignStmt seqAssignStmt)
					{
						if (!assignedSignal.Add(seqAssignStmt.Signal))
						{
							throw new MultiDriveException();
						}
					}
				}
			}
		}
	}
}