using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;
using SharpHdl.Sim.Interp;

namespace SharpHdl.Sim;

public static class ModuleSimExtensions
{
	public static SimSession<T> Run<T>(this T module) where T : Module
	{
		SimSession<T> simSession = new() { Module = module };
		module.Describe();
		simSession.SimWorld = ModuleFlat.Build(module);
		foreach (Stmt stmt in module.GetStmts())
		{
			if (!StmtSupport.IsSupportedTopLevel(stmt))
			{
				throw new SimUnsupportedException();
			}
		}
		simSession.Settle();
		return simSession;
	}
}