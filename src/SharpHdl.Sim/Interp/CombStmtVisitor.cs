using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class CombStmtVisitor(SimWorld simWorld) : IStmtVisitor
{
	private ulong beforeValue0;
	private ulong beforeValue1;
	public bool IsUpdate { get; private set; } = true;
	public void Reset()
	{
		IsUpdate = false;
	}
	public void VisitAssign(AssignStmt assignStmt)
	{
		beforeValue0 = EvalExpr.Eval(assignStmt.Signal, simWorld);
		simWorld.Set(assignStmt.Signal, EvalExpr.Eval(assignStmt.Expr, simWorld));
		if (beforeValue0 != EvalExpr.Eval(assignStmt.Signal, simWorld))
		{
			IsUpdate = true;
		}
	}

	public void VisitIf(IfStmt ifStmt)
	{
		if (EvalExpr.Eval(ifStmt.Cond, simWorld) != 0)
		{
			ifStmt.Then.ForEach(x => x.Accept(this));
		}
		else
		{
			ifStmt.Else?.ForEach(x => x.Accept(this));
		}
	}

	public void VisitInstance(InstanceStmt instanceStmt)
	{
		foreach (Stmt s in instanceStmt.ChildModule.GetStmts())
		{
			s.Accept(this);
		}
	}

	public void VisitMem(MemStmt memStmt)
	{
	}

	public void VisitMem2R1W(Mem2R1WStmt mem2R1WStmt)
	{
		beforeValue0 = EvalExpr.Eval(mem2R1WStmt.Rdata0, simWorld);
		beforeValue1 = EvalExpr.Eval(mem2R1WStmt.Rdata1, simWorld);
		simWorld.Set(mem2R1WStmt.Rdata0, simWorld.MemState[mem2R1WStmt.Rdata0][EvalExpr.Eval(mem2R1WStmt.Raddr0, simWorld)]);
		simWorld.Set(mem2R1WStmt.Rdata1, simWorld.MemState[mem2R1WStmt.Rdata1][EvalExpr.Eval(mem2R1WStmt.Raddr1, simWorld)]);
		if (beforeValue0 != EvalExpr.Eval(mem2R1WStmt.Rdata0, simWorld) || beforeValue1 != EvalExpr.Eval(mem2R1WStmt.Rdata1, simWorld))
		{
			IsUpdate = true;
		}
	}

	public void VisitMemWstrb(MemWstrbStmt memWstrbStmt)
	{
	}

	public void VisitSeqAssign(SeqAssignStmt seqAssignStmt)
	{
	}

	public void VisitSeqBlock(SeqBlockStmt seqBlockStmt)
	{
	}

	public void VisitSwitch(SwitchStmt switchStmt)
	{
		foreach (Case caseItem in switchStmt.Cases)
		{
			if (EvalExpr.Eval(switchStmt.Expr, simWorld) == caseItem.Value)
			{
				caseItem.Stmts.ForEach(x => x.Accept(this));
				break;
			}
		}
	}
}