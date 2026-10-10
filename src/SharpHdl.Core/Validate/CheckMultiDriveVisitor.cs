using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Core.Validate;

public class CheckMultiDriveVisitor : IStmtVisitor
{
	private readonly HashSet<Signal> assignedSignal = [];

	public void VisitAssign(AssignStmt assignStmt)
	{
		if (!assignedSignal.Add(assignStmt.Signal))
		{
			throw new MultiDriveException();
		}
	}

	public void VisitIf(IfStmt ifStmt)
	{
		HashSet<Signal> ifAssignedSignal = [];

		CheckMultiDriveVisitor checkMultiDriveVisitor = new();
		foreach (Stmt stmt in ifStmt.Then)
		{
			stmt.Accept(checkMultiDriveVisitor);
		}
		ifAssignedSignal.UnionWith(checkMultiDriveVisitor.assignedSignal);

		if (ifStmt.Else is not null)
		{
			checkMultiDriveVisitor = new();
			foreach (Stmt stmt in ifStmt.Else)
			{
				stmt.Accept(checkMultiDriveVisitor);
			}
			ifAssignedSignal.UnionWith(checkMultiDriveVisitor.assignedSignal);
		}

		foreach (Signal item in ifAssignedSignal)
		{
			if (!assignedSignal.Add(item))
			{
				throw new MultiDriveException();
			}
		}
	}

	public void VisitInstance(InstanceStmt instanceStmt)
	{
	}

	public void VisitMem(MemStmt memStmt)
	{
	}

	public void VisitMem2R1W(Mem2R1WStmt mem2R1WStmt)
	{
	}

	public void VisitMemWstrb(MemWstrbStmt memWstrbStmt)
	{
	}

	public void VisitSeqAssign(SeqAssignStmt seqAssignStmt)
	{
		if (!assignedSignal.Add(seqAssignStmt.Signal))
		{
			throw new MultiDriveException();
		}
	}

	public void VisitSeqBlock(SeqBlockStmt seqBlockStmt)
	{
		foreach (Stmt bodystmt in seqBlockStmt.Body)
		{
			bodystmt.Accept(this);
		}
	}

	public void VisitSwitch(SwitchStmt switchStmt)
	{
		HashSet<Signal> switchAssignedSignal = [];

		foreach (Case caseStmt in switchStmt.Cases)
		{
			CheckMultiDriveVisitor checkMultiDriveVisitor = new();
			foreach (Stmt item in caseStmt.Stmts)
			{
				item.Accept(checkMultiDriveVisitor);
			}
			switchAssignedSignal.UnionWith(checkMultiDriveVisitor.assignedSignal);
		}

		foreach (Signal item in switchAssignedSignal)
		{
			if (!assignedSignal.Add(item))
			{
				throw new MultiDriveException();
			}
		}
	}
}