using SharpHdl.Core.Model;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class ModuleFlat
{
	public static SimWorld Build(Module module)
	{
		SimWorld simWorld = new();
		foreach (Signal signal in module.GetPorts())
		{
			simWorld.Values.Add(signal, 0);
		}
		foreach (Stmt stmt in module.GetStmts())
		{
			if (stmt is InstanceStmt instanceStmt)
			{
				foreach (PortConnection connection in instanceStmt.PortConnections)
				{
					simWorld.Alias.Add(connection.ChildPort, connection.ParentSignal);
				}
			}
			else if (stmt is MemStmt memStmt)
			{
				simWorld.MemState[memStmt.Rdata] = new ulong[memStmt.Depth];
			}
			else if (stmt is Mem2R1WStmt mem2R1WStmt)
			{
				ulong[] arr = new ulong[mem2R1WStmt.Depth];
				simWorld.MemState[mem2R1WStmt.Rdata0] = arr;
				simWorld.MemState[mem2R1WStmt.Rdata1] = arr;
			}
			else if (stmt is MemWstrbStmt memWstrbStmt)
			{
				simWorld.MemState[memWstrbStmt.Rdata] = new ulong[memWstrbStmt.Depth];
			}
		}
		return simWorld;
	}
}