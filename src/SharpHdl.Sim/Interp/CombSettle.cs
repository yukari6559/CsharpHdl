using SharpHdl.Core.Model;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class CombSettle
{
	public bool Settle(List<Stmt> stmts, SimWorld simWorld)
	{
		ulong beforeValue0;
		ulong beforeValue1;
		bool isUpdate = true;
		bool isUpdateAll = false;
		EvalExpr evalExpr = new();
		while(isUpdate)
		{
			isUpdate = false;
			foreach(var stmt in stmts)
			{
				if(stmt is AssignStmt assignStmt)
				{
					beforeValue0 = evalExpr.Eval(assignStmt.Signal, simWorld);
					simWorld.Set(assignStmt.Signal, evalExpr.Eval(assignStmt.Expr, simWorld));
					if(beforeValue0 != evalExpr.Eval(assignStmt.Signal, simWorld))
					{
						isUpdate = true;
						isUpdateAll = true;
					}
				}
				if(stmt is SwitchStmt switchStmt)
				{
					foreach(var caseItem in switchStmt.Cases)
					{
						if(evalExpr.Eval(switchStmt.Signal, simWorld) == caseItem.Value)
						{
							if(Settle(caseItem.Stmts, simWorld) == true)
							{
								isUpdate = true;
								isUpdateAll = true;
							}
							break;
						}
					}
				}
				if(stmt is InstanceStmt instanceStmt)
				{
					if(Settle(instanceStmt.ChildModule.GetStmts().ToList(), simWorld))
					{
						isUpdate = true;
						isUpdateAll = true;
					}
				}
				if(stmt is Mem2R1WStmt mem2R1WStmt)
				{
					beforeValue0 = evalExpr.Eval(mem2R1WStmt.Rdata0, simWorld);
					beforeValue1 = evalExpr.Eval(mem2R1WStmt.Rdata1, simWorld);
					simWorld.Set(mem2R1WStmt.Rdata0, simWorld.memState[mem2R1WStmt.Rdata0][evalExpr.Eval(mem2R1WStmt.Raddr0, simWorld)]);
					simWorld.Set(mem2R1WStmt.Rdata1, simWorld.memState[mem2R1WStmt.Rdata1][evalExpr.Eval(mem2R1WStmt.Raddr1, simWorld)]);
					if(beforeValue0 != evalExpr.Eval(mem2R1WStmt.Rdata0, simWorld) || beforeValue1 != evalExpr.Eval(mem2R1WStmt.Rdata1, simWorld))
					{
						isUpdate = true;
						isUpdateAll = true;
					}
				}
				if(stmt is IfStmt ifStmt)
				{
					
					if(evalExpr.Eval(ifStmt.Cond, simWorld) != 0)
					{
						if(Settle(ifStmt.Then, simWorld))
						{
							isUpdate = true;
							isUpdateAll = true;
						}
					}
					else if(ifStmt.Else != null)
					{
						if(Settle(ifStmt.Else, simWorld))
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