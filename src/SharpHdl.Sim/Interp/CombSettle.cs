using SharpHdl.Core.Model;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class CombSettle
{
	public bool Settle(List<Stmt> stmts, SimWorld simWorld)
	{
		ulong beforeValue;
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
					beforeValue = evalExpr.Eval(assignStmt.Signal, simWorld);
					simWorld.Set(assignStmt.Signal, evalExpr.Eval(assignStmt.Expr, simWorld));
					if(beforeValue != evalExpr.Eval(assignStmt.Signal, simWorld))
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
			}
		}
		return isUpdateAll;
	}
}