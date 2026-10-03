using System.Text;
using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Emit;

public class InstanceVisitor(StringBuilder verilogsb, List<InstanceStmt> instanceStmts) : IStmtVisitor
{
	private readonly HashSet<string> emitted = [];

	public void VisitAssign(AssignStmt assignStmt)
	{
	}

	public void VisitIf(IfStmt ifStmt)
	{
	}

	public void VisitInstance(InstanceStmt instanceStmt)
	{
		instanceStmts.Add(instanceStmt);
		if (!emitted.Add(instanceStmt.ChildModule.GetType().Name))
		{
			return;
		}

		_ = verilogsb.Append(VerilogEmitter.EmitOneModule(instanceStmt.ChildModule, instanceStmt.ChildModule.GetType().Name, [], false));
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
		throw EmitBodyVisitor.SeqAssignAtTopLevel();
	}

	public void VisitSeqBlock(SeqBlockStmt seqBlockStmt)
	{
	}

	public void VisitSwitch(SwitchStmt switchStmt)
	{
	}
}