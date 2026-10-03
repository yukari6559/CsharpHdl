using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Emit.Visitors;

public class RegOutVisitor(HashSet<Signal> regOuts) : IStmtVisitor
{
	public void VisitAssign(AssignStmt assignStmt)
	{
	}

	public void VisitIf(IfStmt ifStmt)
	{
		IfRegOutVisitor ifRegOutVisitor = new(regOuts);
		ifStmt.Accept(ifRegOutVisitor);
	}

	public void VisitInstance(InstanceStmt instanceStmt)
	{
	}

	public void VisitMem(MemStmt memStmt)
	{
		_ = regOuts.Add(memStmt.Rdata);
	}

	public void VisitMem2R1W(Mem2R1WStmt mem2R1WStmt)
	{
	}

	public void VisitMemWstrb(MemWstrbStmt memWstrbStmt)
	{
		_ = regOuts.Add(memWstrbStmt.Rdata);
	}

	public void VisitSeqAssign(SeqAssignStmt seqAssignStmt)
	{
		throw EmitBodyVisitor.SeqAssignAtTopLevel();
	}

	public void VisitSeqBlock(SeqBlockStmt seqBlockStmt)
	{
		SeqRegOutVisitor seqRegOutVisitor = new(regOuts);
		foreach (Stmt body in seqBlockStmt.Body)
		{
			body.Accept(seqRegOutVisitor);
		}
	}

	public void VisitSwitch(SwitchStmt switchStmt)
	{
	}
}