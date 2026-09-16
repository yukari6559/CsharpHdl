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
		foreach(var stmt in module.GetStmts())
		{
			if(stmt is AssignStmt)
			{
				
			}
			else if(stmt is SwitchStmt)
			{
				
			}
			else if(stmt is SeqBlockStmt)
			{
				
			}
			else if(stmt is InstanceStmt)
			{
				
			}
			else
				throw new SimUnsupportedException();
		}
		simSession.Settle();
		return simSession;
	}
}