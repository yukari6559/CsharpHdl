using System.Globalization;
using System.Text;
using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Emit;

public class SeqVisitor(StringBuilder verilogsb, Signal reset) : IStmtVisitor
{
	public void VisitAssign(AssignStmt assignStmt)
	{
		throw new EmitException("Seq emit: Comb Assign cannot be placed inside a Seq block (always @(posedge)); only Seq assign is allowed.");
	}

	public void VisitIf(IfStmt ifStmt)
	{
		throw new EmitException("Seq emit: If cannot be placed inside a Seq block (always @(posedge)); only Seq assign is allowed.");
	}

	public void VisitInstance(InstanceStmt instanceStmt)
	{
		throw new EmitException("Seq emit: Instance cannot be placed inside a Seq block (always @(posedge)); only Seq assign is allowed.");
	}

	public void VisitMem(MemStmt memStmt)
	{
		throw new EmitException("Seq emit: Mem cannot be placed inside a Seq block (always @(posedge)); only Seq assign is allowed.");
	}

	public void VisitMem2R1W(Mem2R1WStmt mem2R1WStmt)
	{
		throw new EmitException("Seq emit: Mem2R1W cannot be placed inside a Seq block (always @(posedge)); only Seq assign is allowed.");
	}

	public void VisitMemWstrb(MemWstrbStmt memWstrbStmt)
	{
		throw new EmitException("Seq emit: MemWstrb cannot be placed inside a Seq block (always @(posedge)); only Seq assign is allowed.");
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
		throw new EmitException("Seq emit: Nested Seq block cannot be placed inside a Seq block (always @(posedge)); only Seq assign is allowed.");
	}

	public void VisitSwitch(SwitchStmt switchStmt)
	{
		throw new EmitException("Seq emit: Switch cannot be placed inside a Seq block (always @(posedge)); only Seq assign is allowed.");
	}
}