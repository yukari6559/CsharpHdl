using SharpHdl.Core.Model;
using SharpHdl.Core.Walk;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class SimCombStmtHandler(SimWorld simWorld, bool isUpdate, bool isUpdateAll) : ICombStmtHandler
{
	private ulong beforeValue0;
	private ulong beforeValue1;
	public bool IsUpdate { get; set; } = isUpdate;
	public bool IsUpdateAll { get; set; } = isUpdateAll;
	public void OnAssign(AssignStmt assignStmt)
	{
		beforeValue0 = EvalExpr.Eval(assignStmt.Signal, simWorld);
		simWorld.Set(assignStmt.Signal, EvalExpr.Eval(assignStmt.Expr, simWorld));
		if (beforeValue0 != EvalExpr.Eval(assignStmt.Signal, simWorld))
		{
			IsUpdate = true;
			IsUpdateAll = true;
		}
	}

	public void OnIf(IfStmt ifStmt)
	{
		if (EvalExpr.Eval(ifStmt.Cond, simWorld) != 0)
		{
			if (CombSettle.Settle(ifStmt.Then, simWorld))
			{
				IsUpdate = true;
				IsUpdateAll = true;
			}
		}
		else if (ifStmt.Else != null)
		{
			if (CombSettle.Settle(ifStmt.Else, simWorld))
			{
				IsUpdate = true;
				IsUpdateAll = true;
			}
		}
	}

	public void OnSwitch(SwitchStmt switchStmt)
	{
		foreach (Case caseItem in switchStmt.Cases)
		{
			if (EvalExpr.Eval(switchStmt.Expr, simWorld) == caseItem.Value)
			{
				if (CombSettle.Settle(caseItem.Stmts, simWorld))
				{
					IsUpdate = true;
					IsUpdateAll = true;
				}
				break;
			}
		}
	}

	public void OnInstance(InstanceStmt instanceStmt)
	{
		if (CombSettle.Settle([.. instanceStmt.ChildModule.GetStmts()], simWorld))
		{
			IsUpdate = true;
			IsUpdateAll = true;
		}
	}

	public void OnMem2R1WStmt(Mem2R1WStmt mem2R1WStmt)
	{
		beforeValue0 = EvalExpr.Eval(mem2R1WStmt.Rdata0, simWorld);
		beforeValue1 = EvalExpr.Eval(mem2R1WStmt.Rdata1, simWorld);
		simWorld.Set(mem2R1WStmt.Rdata0, simWorld.MemState[mem2R1WStmt.Rdata0][EvalExpr.Eval(mem2R1WStmt.Raddr0, simWorld)]);
		simWorld.Set(mem2R1WStmt.Rdata1, simWorld.MemState[mem2R1WStmt.Rdata1][EvalExpr.Eval(mem2R1WStmt.Raddr1, simWorld)]);
		if (beforeValue0 != EvalExpr.Eval(mem2R1WStmt.Rdata0, simWorld) || beforeValue1 != EvalExpr.Eval(mem2R1WStmt.Rdata1, simWorld))
		{
			IsUpdate = true;
			IsUpdateAll = true;
		}
	}
}