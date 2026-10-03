using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Emit;

public class SeqRegOutVisitor(HashSet<Signal> regOuts) : IStmtVisitor
{
	public void VisitAssign(AssignStmt assignStmt)
	{
		throw SeqVisitor.NotAllowedInSeq("Comb Assign");
	}

	public void VisitIf(IfStmt ifStmt)
	{
		throw SeqVisitor.NotAllowedInSeq("If");
	}

	public void VisitInstance(InstanceStmt instanceStmt)
	{
		throw SeqVisitor.NotAllowedInSeq("Instance");
	}

	public void VisitMem(MemStmt memStmt)
	{
		throw SeqVisitor.NotAllowedInSeq("Mem");
	}

	public void VisitMem2R1W(Mem2R1WStmt mem2R1WStmt)
	{
		throw SeqVisitor.NotAllowedInSeq("Mem2R1W");
	}

	public void VisitMemWstrb(MemWstrbStmt memWstrbStmt)
	{
		throw SeqVisitor.NotAllowedInSeq("MemWstrb");
	}

	public void VisitSeqAssign(SeqAssignStmt seqAssignStmt)
	{
		_ = regOuts.Add(seqAssignStmt.Signal);
	}

	public void VisitSeqBlock(SeqBlockStmt seqBlockStmt)
	{
		throw SeqVisitor.NotAllowedInSeq("Nested Seq block");
	}

	public void VisitSwitch(SwitchStmt switchStmt)
	{
		throw SeqVisitor.NotAllowedInSeq("Switch");
	}
}
