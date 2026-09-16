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
			else if(stmt is Mem2R1WStmt mem2R1WStmt)
			{
				ulong[] arr = new ulong[mem2R1WStmt.Depth];
				simWorld.memState[mem2R1WStmt.Rdata0] = arr;
				simWorld.memState[mem2R1WStmt.Rdata1] = arr;
			}
		}
		return simWorld;
	}
}