using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Sim.Interp;

public class SeqBlockVisitor : IStmtVisitor
{
	private readonly List<SeqAssignStmt> seqAssignStmts = [];
	public IReadOnlyCollection<SeqAssignStmt> SeqAssignStmts => seqAssignStmts;
	public void VisitAssign(AssignStmt assignStmt)
	{
		throw new SimUnsupportedException();
	}

	public void VisitIf(IfStmt ifStmt)
	{
		throw new SimUnsupportedException();
	}

	public void VisitInstance(InstanceStmt instanceStmt)
	{
		throw new SimUnsupportedException();
	}

	public void VisitMem(MemStmt memStmt)
	{
		throw new SimUnsupportedException();
	}

	public void VisitMem2R1W(Mem2R1WStmt mem2R1WStmt)
	{
		throw new SimUnsupportedException();
	}

	public void VisitMemWstrb(MemWstrbStmt memWstrbStmt)
	{
		throw new SimUnsupportedException();
	}

	public void VisitSeqAssign(SeqAssignStmt seqAssignStmt)
	{
		seqAssignStmts.Add(seqAssignStmt);
	}

	public void VisitSeqBlock(SeqBlockStmt seqBlockStmt)
	{
		throw new SimUnsupportedException();
	}

	public void VisitSwitch(SwitchStmt switchStmt)
	{
		throw new SimUnsupportedException();
	}
}