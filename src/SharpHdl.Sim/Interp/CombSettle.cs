using SharpHdl.Core.Model;
using SharpHdl.Core.Walk;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class CombSettle
{
	public static bool Settle(List<Stmt> stmts, SimWorld simWorld)
	{
		SimCombStmtHandler simCombStmtHandler = new(simWorld, true, false);
		while (simCombStmtHandler.IsUpdate)
		{
			simCombStmtHandler.IsUpdate = false;
			CombStmtDispatch.WalkComb(stmts, simCombStmtHandler);
		}
		return simCombStmtHandler.IsUpdateAll;
	}
}