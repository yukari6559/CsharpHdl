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
		foreach(var stmt in module.GetStmts())
		{
			if(stmt is InstanceStmt instanceStmt)
			{
				foreach(var connection in instanceStmt.PortConnections)
				{
					simWorld.alias.Add(connection.ChildPort, connection.ParentSignal);
				}
			}
			else if(stmt is MemStmt memStmt)
			{
				simWorld.memState[memStmt.Rdata] = new ulong[memStmt.Depth];
			}
		}
		return simWorld;
	}
}