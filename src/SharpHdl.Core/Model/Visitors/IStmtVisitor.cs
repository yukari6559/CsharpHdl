namespace SharpHdl.Core.Model.Visitors;

public interface IStmtVisitor
{
	void VisitAssign(AssignStmt assignStmt);
	void VisitSwitch(SwitchStmt switchStmt);
	void VisitSeqBlock(SeqBlockStmt seqBlockStmt);
	void VisitSeqAssign(SeqAssignStmt seqAssignStmt);
	void VisitInstance(InstanceStmt instanceStmt);
	void VisitMem(MemStmt memStmt);
	void VisitMem2R1W(Mem2R1WStmt mem2R1WStmt);
	void VisitMemWstrb(MemWstrbStmt memWstrbStmt);
	void VisitIf(IfStmt ifStmt);
}