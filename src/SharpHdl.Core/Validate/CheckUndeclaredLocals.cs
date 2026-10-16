using SharpHdl.Core.Model;

namespace SharpHdl.Core.Validate;

public static class CheckUndeclaredLocals
{
	public static void Check(Module module)
	{
		CheckUndeclaredLocalsVisitor checkUndeclaredLocalsVisitor = new(module);
		foreach (Stmt stmt in module.GetStmts())
		{
			stmt.Accept(checkUndeclaredLocalsVisitor);
		}
	}
}