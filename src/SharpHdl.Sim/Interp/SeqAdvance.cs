using SharpHdl.Core.Model;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class SeqAdvance
{
	public static void Advance(List<Stmt> stmts, SimWorld simWorld, Signal clk)
	{
		Dictionary<Signal, ulong> nextValues = [];
		ulong nextRdata;
		foreach (Stmt stmt in stmts)
		{
			if (stmt is SeqBlockStmt seqBlockStmt && seqBlockStmt.Clk == clk)
			{
				if (EvalExpr.Eval(seqBlockStmt.Reset, simWorld) == 1)
				{
					foreach (SeqAssignStmt innerStmt in seqBlockStmt.Body.Cast<SeqAssignStmt>())
					{
						nextValues[innerStmt.Signal] = innerStmt.ResetValue;
					}
				}
				else if (EvalExpr.Eval(seqBlockStmt.Reset, simWorld) == 0)
				{
					foreach (SeqAssignStmt innerStmt in seqBlockStmt.Body.Cast<SeqAssignStmt>())
					{
						nextValues[innerStmt.Signal] = EvalExpr.Eval(innerStmt.Expr, simWorld);
					}
				}
			}
			if (stmt is InstanceStmt instanceStmt)
			{
				Advance([.. instanceStmt.ChildModule.GetStmts()], simWorld, clk);
			}
			if (stmt is MemStmt memStmt && memStmt.Clk == clk)
			{
				nextRdata = simWorld.MemState[memStmt.Rdata][EvalExpr.Eval(memStmt.Addr, simWorld)];
				nextValues[memStmt.Rdata] = nextRdata;
				if (EvalExpr.Eval(memStmt.We, simWorld) == 1)
				{
					simWorld.MemState[memStmt.Rdata][EvalExpr.Eval(memStmt.Addr, simWorld)] = EvalExpr.Eval(memStmt.Wdata, simWorld);
				}
			}
			if (stmt is Mem2R1WStmt mem2R1WStmt && mem2R1WStmt.Clk == clk)
			{
				if (EvalExpr.Eval(mem2R1WStmt.We, simWorld) == 1)
				{
					simWorld.MemState[mem2R1WStmt.Rdata0][EvalExpr.Eval(mem2R1WStmt.Waddr, simWorld)] = EvalExpr.Eval(mem2R1WStmt.Wdata, simWorld);
				}
			}
			if (stmt is MemWstrbStmt memWstrbStmt && memWstrbStmt.Clk == clk)
			{
				nextRdata = simWorld.MemState[memWstrbStmt.Rdata][EvalExpr.Eval(memWstrbStmt.Addr, simWorld)];
				if (EvalExpr.Eval(memWstrbStmt.We, simWorld) == 1)
				{
					ulong word = simWorld.MemState[memWstrbStmt.Rdata][EvalExpr.Eval(memWstrbStmt.Addr, simWorld)];
					for (int i = 0; i < memWstrbStmt.Width / 8; i++)
					{
						if (((EvalExpr.Eval(memWstrbStmt.Wstrb, simWorld) >> i) & 1) == 1)
						{
							ulong mask = 0xFFul << (8 * i);
							word = (word & ~mask) | (EvalExpr.Eval(memWstrbStmt.Wdata, simWorld) & mask);
						}
					}
					simWorld.MemState[memWstrbStmt.Rdata][EvalExpr.Eval(memWstrbStmt.Addr, simWorld)] = word;
				}
				nextValues[memWstrbStmt.Rdata] = nextRdata;
			}
		}
		foreach (KeyValuePair<Signal, ulong> item in nextValues)
		{
			simWorld.Set(item.Key, item.Value);
		}
	}
}