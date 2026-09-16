using SharpHdl.Core.Model;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class SeqAdvance
{
	public void Advance(List<Stmt> stmts, SimWorld simWorld, Signal clk)
	{
		Dictionary<Signal, ulong> nextValues = new();
		EvalExpr evalExpr = new();
		foreach(var stmt in stmts)
		{
			if(stmt is SeqBlockStmt seqBlockStmt && seqBlockStmt.Clk == clk)
			{
				if(evalExpr.Eval(seqBlockStmt.Reset, simWorld) == 1)
				{
					foreach(SeqAssignStmt innerStmt in seqBlockStmt.Body)
					{
						nextValues[innerStmt.Signal] = innerStmt.ResetValue;
					}
				}
				else if(evalExpr.Eval(seqBlockStmt.Reset, simWorld) == 0)
				{
					foreach(SeqAssignStmt innerStmt in seqBlockStmt.Body)
					{
						nextValues[innerStmt.Signal] = evalExpr.Eval(innerStmt.Expr, simWorld);
					}
				}
			}
			if(stmt is InstanceStmt instanceStmt)
			{
				Advance(instanceStmt.ChildModule.GetStmts().ToList(), simWorld, clk);
			}
		}
		foreach(var item in nextValues)
		{
			simWorld.Set(item.Key, item.Value);
		}
	}
}