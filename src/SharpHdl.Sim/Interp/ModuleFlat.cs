using SharpHdl.Core.Model;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class ModuleFlat
{
	public static SimWorld Build(Module module)
	{
		SimWorld simWorld = new();
		SimInitVisitor simInitVisitor = new(simWorld);
		foreach (Signal signal in module.GetPorts())
		{
			simWorld.Values.Add(signal, 0);
		}
		foreach (Stmt stmt in module.GetStmts())
		{
			stmt.Accept(simInitVisitor);
		}
		return simWorld;
	}
}