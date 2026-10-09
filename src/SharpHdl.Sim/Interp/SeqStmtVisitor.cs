using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class SeqStmtVisitor(SimWorld simWorld, Signal clk) : IStmtVisitor
{
	private readonly Dictionary<Signal, ulong> nextValues = [];
	public void VisitAssign(AssignStmt assignStmt)
	{
	}

	public void VisitIf(IfStmt ifStmt)
	{
	}

	public void VisitInstance(InstanceStmt instanceStmt)
	{
		foreach (Stmt stmt in instanceStmt.ChildModule.GetStmts())
		{
			stmt.Accept(this);
		}
	}

	public void VisitMem(MemStmt memStmt)
	{
		if (memStmt.Clk == clk)
		{
			ulong nextRdata = simWorld.MemState[memStmt.Rdata][EvalExpr.Eval(memStmt.Addr, simWorld)];
			nextValues[memStmt.Rdata] = nextRdata;
			if (EvalExpr.Eval(memStmt.We, simWorld) == 1)
			{
				simWorld.MemState[memStmt.Rdata][EvalExpr.Eval(memStmt.Addr, simWorld)] = EvalExpr.Eval(memStmt.Wdata, simWorld);
			}
		}
	}

	public void VisitMem2R1W(Mem2R1WStmt mem2R1WStmt)
	{
		if (mem2R1WStmt.Clk == clk && EvalExpr.Eval(mem2R1WStmt.We, simWorld) == 1)
		{
			simWorld.MemState[mem2R1WStmt.Rdata0][EvalExpr.Eval(mem2R1WStmt.Waddr, simWorld)] = EvalExpr.Eval(mem2R1WStmt.Wdata, simWorld);
		}
	}

	public void VisitMemWstrb(MemWstrbStmt memWstrbStmt)
	{
		if (memWstrbStmt.Clk == clk)
		{
			ulong nextRdata = simWorld.MemState[memWstrbStmt.Rdata][EvalExpr.Eval(memWstrbStmt.Addr, simWorld)];
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

	public void VisitSeqAssign(SeqAssignStmt seqAssignStmt)
	{
	}

	public void VisitSeqBlock(SeqBlockStmt seqBlockStmt)
	{
		if (seqBlockStmt.Clk == clk)
		{
			ulong resetValue = EvalExpr.Eval(seqBlockStmt.Reset, simWorld);
			if (resetValue is not 0 and not 1)
			{
				return;
			}
			SeqBlockVisitor seqBlockVisitor = new();
			seqBlockStmt.Body.ForEach(x => x.Accept(seqBlockVisitor));
			foreach (SeqAssignStmt seqAssignStmt in seqBlockVisitor.SeqAssignStmts)
			{
				nextValues[seqAssignStmt.Signal] = resetValue == 1 ? seqAssignStmt.ResetValue : EvalExpr.Eval(seqAssignStmt.Expr, simWorld);
			}
		}
	}

	public void VisitSwitch(SwitchStmt switchStmt)
	{
	}

	public void Commit()
	{
		foreach (KeyValuePair<Signal, ulong> item in nextValues)
		{
			simWorld.Set(item.Key, item.Value);
		}
	}
}