using SharpHdl.Core.Model;

namespace SharpHdl.Core.Validate;

public static class CheckMultiDrive
{
	public static void Check(List<Stmt> stmts)
	{
		CheckMultiDriveVisitor checkMultiDriveVisitor = new();
		foreach (Stmt stmt in stmts)
		{
			stmt.Accept(checkMultiDriveVisitor);
		}
	}
}