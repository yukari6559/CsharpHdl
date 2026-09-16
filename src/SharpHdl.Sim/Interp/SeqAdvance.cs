using SharpHdl.Core.Model;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class SeqAdvance
{
	public void Advance(List<Stmt> stmts, SimWorld simWorld, Signal clk)
	{
		Dictionary<Signal, ulong> nextValues = new();
		EvalExpr evalExpr = new();
		ulong nextRdata;
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
			if(stmt is MemStmt memStmt && memStmt.Clk == clk)
			{
				nextRdata = (simWorld.memState[memStmt.Rdata])[evalExpr.Eval(memStmt.Addr, simWorld)];
				nextValues[memStmt.Rdata] = nextRdata;
				if(evalExpr.Eval(memStmt.We, simWorld) == 1)
				{
					simWorld.memState[memStmt.Rdata][evalExpr.Eval(memStmt.Addr, simWorld)] = evalExpr.Eval(memStmt.Wdata, simWorld);
				}
			}
			if(stmt is Mem2R1WStmt mem2R1WStmt && mem2R1WStmt.Clk == clk)
			{
				if(evalExpr.Eval(mem2R1WStmt.We, simWorld) == 1)
				{
					simWorld.memState[mem2R1WStmt.Rdata0][evalExpr.Eval(mem2R1WStmt.Waddr, simWorld)] = evalExpr.Eval(mem2R1WStmt.Wdata, simWorld);
				}
			}
			if(stmt is MemWstrbStmt memWstrbStmt && memWstrbStmt.Clk == clk)
			{
				nextRdata = simWorld.memState[memWstrbStmt.Rdata][evalExpr.Eval(memWstrbStmt.Addr, simWorld)];
				if(evalExpr.Eval(memWstrbStmt.We, simWorld) == 1)
				{
					var word = simWorld.memState[memWstrbStmt.Rdata][evalExpr.Eval(memWstrbStmt.Addr,simWorld)];
					for(int i = 0; i < memWstrbStmt.Width / 8; i++)
					{
						if(((evalExpr.Eval(memWstrbStmt.Wstrb, simWorld) >> i) & 1) == 1)
						{
							ulong mask = 0xFFul << (8*i);
							word = (word & ~mask) | (evalExpr.Eval(memWstrbStmt.Wdata,simWorld) & mask);
						}
					}
					simWorld.memState[memWstrbStmt.Rdata][evalExpr.Eval(memWstrbStmt.Addr,simWorld)] = word;
				}
				nextValues[memWstrbStmt.Rdata] = nextRdata; 
			}
		}
		foreach(var item in nextValues)
		{
			simWorld.Set(item.Key, item.Value);
		}
	}
}