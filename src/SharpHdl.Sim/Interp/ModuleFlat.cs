using SharpHdl.Core.Model;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class ModuleFlat
{
	public SimWorld Build(Module module)
	{
		SimWorld simWorld = new();
		foreach(Signal signal in module.GetPorts())
		{
			simWorld.Values.Add(signal, 0);
		}
		foreach(InstanceStmt instanceStmt in module.GetStmts().OfType<InstanceStmt>())
		{
			foreach(var connection in instanceStmt.PortConnections)
			{
				simWorld.alias.Add(connection.ChildPort, connection.ParentSignal);
			}
		}
		return simWorld;
	}
}