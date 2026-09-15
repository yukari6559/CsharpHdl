using SharpHdl.Core.Model;
using SharpHdl.Sim.Interp;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim;

public static class ModuleSimExtensions
{
	public static SimSession<T> Run<T>(this T module) where T : Module
	{
		ModuleFlat moduleFlat = new();
		SimSession<T> simSession = new(){Module = module};
		module.Describe();
		simSession.simWorld = moduleFlat.Build(module);
		foreach(var stmts in module.GetStmts())
		{
			if(stmts is AssignStmt)
			{
				
			}
			else if(stmts is SwitchStmt)
			{
				
			}
			else if(stmts is SeqBlockStmt)
			{
				
			}
			else
				throw new SimUnsupportedException();
		}
		simSession.Settle();
		return simSession;
	}
}