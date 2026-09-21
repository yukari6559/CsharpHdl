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
			if (stmt is AssignStmt)
			{

			}
			else if (stmt is SwitchStmt)
			{

			}
			else if (stmt is SeqBlockStmt)
			{

			}
			else if (stmt is InstanceStmt)
			{

			}
			else if (stmt is MemStmt)
			{

			}
			else if (stmt is Mem2R1WStmt mem2R1WStmt)
			{

			}
			else if (stmt is MemWstrbStmt memWstrbStmt)
			{

			}
			else if (stmt is IfStmt)
			{

			}
			else
			{
				throw new SimUnsupportedException();
			}
		}
		simSession.Settle();
		return simSession;
	}
}