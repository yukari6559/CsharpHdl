using SharpHdl.Core.Model;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class EvalExpr
{
	public static ulong Eval(Expr expr, SimWorld simWorld)
	{
		EvalExprVisitor evalExprVisitor = new(simWorld);
		return expr.Accept(evalExprVisitor);
	}
}