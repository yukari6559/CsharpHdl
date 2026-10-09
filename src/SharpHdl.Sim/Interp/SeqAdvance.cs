using SharpHdl.Core.Model;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class SeqAdvance
{
	public static void Advance(List<Stmt> stmts, SimWorld simWorld, Signal clk)
	{
		SeqStmtVisitor seqStmtVisitor = new(simWorld, clk);
		foreach (Stmt stmt in stmts)
		{
			stmt.Accept(seqStmtVisitor);
		}
		seqStmtVisitor.Commit();
	}
}