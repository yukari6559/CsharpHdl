using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class SimInitVisitor(SimWorld simWorld) : IStmtVisitor
{
	public void VisitAssign(AssignStmt assignStmt)
	{
	}

	public void VisitIf(IfStmt ifStmt)
	{
	}

	public void VisitInstance(InstanceStmt instanceStmt)
	{
		foreach (PortConnection connection in instanceStmt.PortConnections)
		{
			simWorld.Alias.Add(connection.ChildPort, connection.ParentSignal);
		}
	}

	public void VisitMem(MemStmt memStmt)
	{
		simWorld.MemState[memStmt.Rdata] = new ulong[memStmt.Depth];
	}

	public void VisitMem2R1W(Mem2R1WStmt mem2R1WStmt)
	{
		ulong[] arr = new ulong[mem2R1WStmt.Depth];
		simWorld.MemState[mem2R1WStmt.Rdata0] = arr;
		simWorld.MemState[mem2R1WStmt.Rdata1] = arr;
	}

	public void VisitMemWstrb(MemWstrbStmt memWstrbStmt)
	{
		simWorld.MemState[memWstrbStmt.Rdata] = new ulong[memWstrbStmt.Depth];
	}

	public void VisitSeqAssign(SeqAssignStmt seqAssignStmt)
	{
	}

	public void VisitSeqBlock(SeqBlockStmt seqBlockStmt)
	{
	}

	public void VisitSwitch(SwitchStmt switchStmt)
	{
	}
}