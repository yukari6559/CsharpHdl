using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Emit.Visitors;

public class IfRegOutVisitor(HashSet<Signal> regOuts) : IStmtVisitor
{
	public void VisitAssign(AssignStmt assignStmt)
	{
		_ = regOuts.Add(assignStmt.Signal);
	}

	public void VisitIf(IfStmt ifStmt)
	{
		foreach (Stmt thenItem in ifStmt.Then)
		{
			thenItem.Accept(this);
		}
		if (ifStmt.Else != null)
		{
			foreach (Stmt elseItem in ifStmt.Else)
			{
				elseItem.Accept(this);
			}
		}
	}

	public void VisitInstance(InstanceStmt instanceStmt)
	{
		throw IfVisitor.NotAllowedInIf("Instance");
	}

	public void VisitMem(MemStmt memStmt)
	{
		throw IfVisitor.NotAllowedInIf("Mem");
	}

	public void VisitMem2R1W(Mem2R1WStmt mem2R1WStmt)
	{
		throw IfVisitor.NotAllowedInIf("Mem2R1W");
	}

	public void VisitMemWstrb(MemWstrbStmt memWstrbStmt)
	{
		throw IfVisitor.NotAllowedInIf("MemWstrb");
	}

	public void VisitSeqAssign(SeqAssignStmt seqAssignStmt)
	{
		throw IfVisitor.NotAllowedInIf("Seq assign");
	}

	public void VisitSeqBlock(SeqBlockStmt seqBlockStmt)
	{
		throw IfVisitor.NotAllowedInIf("Seq block");
	}

	public void VisitSwitch(SwitchStmt switchStmt)
	{
		throw IfVisitor.NotAllowedInIf("Switch");
	}
}