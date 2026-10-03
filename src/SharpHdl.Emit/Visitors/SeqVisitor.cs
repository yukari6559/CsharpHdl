using System.Globalization;
using System.Text;
using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Emit.Visitors;

public class SeqVisitor(StringBuilder verilogsb, Signal reset) : IStmtVisitor
{
	public void VisitAssign(AssignStmt assignStmt)
	{
		throw NotAllowedInSeq("Comb Assign");
	}

	public void VisitIf(IfStmt ifStmt)
	{
		throw NotAllowedInSeq("If");
	}

	public void VisitInstance(InstanceStmt instanceStmt)
	{
		throw NotAllowedInSeq("Instance");
	}

	public void VisitMem(MemStmt memStmt)
	{
		throw NotAllowedInSeq("Mem");
	}

	public void VisitMem2R1W(Mem2R1WStmt mem2R1WStmt)
	{
		throw NotAllowedInSeq("Mem2R1W");
	}

	public void VisitMemWstrb(MemWstrbStmt memWstrbStmt)
	{
		throw NotAllowedInSeq("MemWstrb");
	}

	public void VisitSeqAssign(SeqAssignStmt seqAssignStmt)
	{
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\t\tif ({reset.Name}) begin\n");
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\t\t\t{seqAssignStmt.Signal.Name} <= {seqAssignStmt.Signal.Width}'d{seqAssignStmt.ResetValue};\n");
		_ = verilogsb.Append("\t\tend else begin\n");
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\t\t\t{seqAssignStmt.Signal.Name} <= {VerilogEmitter.EmitExpr(seqAssignStmt.Expr)};\n");
		_ = verilogsb.Append("\t\tend\n");
	}

	public void VisitSeqBlock(SeqBlockStmt seqBlockStmt)
	{
		throw NotAllowedInSeq("Nested Seq block");
	}

	public void VisitSwitch(SwitchStmt switchStmt)
	{
		throw NotAllowedInSeq("Switch");
	}

	internal static EmitException NotAllowedInSeq(string stmtKind)
	{
		return new EmitException($"Seq emit: {stmtKind} cannot be placed inside a Seq block (always @(posedge)); only Seq assign is allowed.");
	}
}