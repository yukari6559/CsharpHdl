using SharpHdl.Core.Model;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class CombSettle
{
	public static bool Settle(List<Stmt> stmts, SimWorld simWorld)
	{
		ulong beforeValue0;
		ulong beforeValue1;
		bool isUpdate = true;
		bool isUpdateAll = false;
		while (isUpdate)
		{
			isUpdate = false;
			foreach (Stmt stmt in stmts)
			{
				if (stmt is AssignStmt assignStmt)
				{
					beforeValue0 = EvalExpr.Eval(assignStmt.Signal, simWorld);
					simWorld.Set(assignStmt.Signal, EvalExpr.Eval(assignStmt.Expr, simWorld));
					if (beforeValue0 != EvalExpr.Eval(assignStmt.Signal, simWorld))
					{
						isUpdate = true;
						isUpdateAll = true;
					}
				}
				if (stmt is SwitchStmt switchStmt)
				{
					foreach (Case caseItem in switchStmt.Cases)
					{
						if (EvalExpr.Eval(switchStmt.Expr, simWorld) == caseItem.Value)
						{
							if (Settle(caseItem.Stmts, simWorld))
							{
								isUpdate = true;
								isUpdateAll = true;
							}
							break;
						}
					}
				}
				if (stmt is InstanceStmt instanceStmt)
				{
					if (Settle([.. instanceStmt.ChildModule.GetStmts()], simWorld))
					{
						isUpdate = true;
						isUpdateAll = true;
					}
				}
				if (stmt is Mem2R1WStmt mem2R1WStmt)
				{
					beforeValue0 = EvalExpr.Eval(mem2R1WStmt.Rdata0, simWorld);
					beforeValue1 = EvalExpr.Eval(mem2R1WStmt.Rdata1, simWorld);
					simWorld.Set(mem2R1WStmt.Rdata0, simWorld.MemState[mem2R1WStmt.Rdata0][EvalExpr.Eval(mem2R1WStmt.Raddr0, simWorld)]);
					simWorld.Set(mem2R1WStmt.Rdata1, simWorld.MemState[mem2R1WStmt.Rdata1][EvalExpr.Eval(mem2R1WStmt.Raddr1, simWorld)]);
					if (beforeValue0 != EvalExpr.Eval(mem2R1WStmt.Rdata0, simWorld) || beforeValue1 != EvalExpr.Eval(mem2R1WStmt.Rdata1, simWorld))
					{
						isUpdate = true;
						isUpdateAll = true;
					}
				}
				if (stmt is IfStmt ifStmt)
				{

					if (EvalExpr.Eval(ifStmt.Cond, simWorld) != 0)
					{
						if (Settle(ifStmt.Then, simWorld))
						{
							isUpdate = true;
							isUpdateAll = true;
						}
					}
					else if (ifStmt.Else != null)
					{
						if (Settle(ifStmt.Else, simWorld))
						{
							isUpdate = true;
							isUpdateAll = true;
						}
					}
				}
			}
		}
		return isUpdateAll;
	}
}