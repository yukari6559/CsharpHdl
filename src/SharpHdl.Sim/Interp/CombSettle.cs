using SharpHdl.Core.Model;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class CombSettle
{
	public static void Settle(List<Stmt> stmts, SimWorld simWorld)
	{
		CombStmtVisitor combStmtVisitor = new(simWorld);
		while (combStmtVisitor.IsUpdate)
		{
			combStmtVisitor.Reset();
			stmts.ForEach(x => x.Accept(combStmtVisitor));
		}
	}
}