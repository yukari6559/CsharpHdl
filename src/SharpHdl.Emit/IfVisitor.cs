using System.Globalization;
using System.Text;
using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Emit;

public class IfVisitor(StringBuilder verilogsb, int depth) : IStmtVisitor
{
	public void VisitAssign(AssignStmt assignStmt)
	{
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"{string.Concat(Enumerable.Repeat("\t", depth))}{assignStmt.Signal.Name} = {VerilogEmitter.EmitExpr(assignStmt.Expr)};\n");
	}

	public void VisitIf(IfStmt ifStmt)
	{
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"{string.Concat(Enumerable.Repeat("\t", depth))}if ({VerilogEmitter.EmitExpr(ifStmt.Cond)}) begin\n");

		IfVisitor nestIfVisitor = new(verilogsb, depth + 1);

		foreach (Stmt thenItem in ifStmt.Then)
		{
			thenItem.Accept(nestIfVisitor);
		}
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"{string.Concat(Enumerable.Repeat("\t", depth))}end\n");
		if (ifStmt.Else != null)
		{
			_ = verilogsb.Append(CultureInfo.InvariantCulture, $"{string.Concat(Enumerable.Repeat("\t", depth))}else begin\n");
			foreach (Stmt elseItem in ifStmt.Else)
			{
				elseItem.Accept(nestIfVisitor);
			}
			_ = verilogsb.Append(CultureInfo.InvariantCulture, $"{string.Concat(Enumerable.Repeat("\t", depth))}end\n");
		}
	}

	public void VisitInstance(InstanceStmt instanceStmt)
	{
		throw new EmitException("If emit: Instance cannot be placed inside a Comb If (always @(*)); only Assign and nested If are allowed.");
	}

	public void VisitMem(MemStmt memStmt)
	{
		throw new EmitException("If emit: Mem cannot be placed inside a Comb If (always @(*)); only Assign and nested If are allowed.");
	}

	public void VisitMem2R1W(Mem2R1WStmt mem2R1WStmt)
	{
		throw new EmitException("If emit: Mem2R1W cannot be placed inside a Comb If (always @(*)); only Assign and nested If are allowed.");
	}

	public void VisitMemWstrb(MemWstrbStmt memWstrbStmt)
	{
		throw new EmitException("If emit: MemWstrb cannot be placed inside a Comb If (always @(*)); only Assign and nested If are allowed.");
	}

	public void VisitSeqAssign(SeqAssignStmt seqAssignStmt)
	{
		throw new EmitException("If emit: Seq assign cannot be placed inside a Comb If (always @(*)); only Assign and nested If are allowed.");
	}

	public void VisitSeqBlock(SeqBlockStmt seqBlockStmt)
	{
		throw new EmitException("If emit: Seq block cannot be placed inside a Comb If (always @(*)); only Assign and nested If are allowed.");
	}

	public void VisitSwitch(SwitchStmt switchStmt)
	{
		throw new EmitException("If emit: Switch cannot be placed inside a Comb If (always @(*)); only Assign and nested If are allowed.");
	}
}