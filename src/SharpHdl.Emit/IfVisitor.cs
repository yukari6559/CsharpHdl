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
		throw NotAllowedInIf("Instance");
	}

	public void VisitMem(MemStmt memStmt)
	{
		throw NotAllowedInIf("Mem");
	}

	public void VisitMem2R1W(Mem2R1WStmt mem2R1WStmt)
	{
		throw NotAllowedInIf("Mem2R1W");
	}

	public void VisitMemWstrb(MemWstrbStmt memWstrbStmt)
	{
		throw NotAllowedInIf("MemWstrb");
	}

	public void VisitSeqAssign(SeqAssignStmt seqAssignStmt)
	{
		throw NotAllowedInIf("Seq assign");
	}

	public void VisitSeqBlock(SeqBlockStmt seqBlockStmt)
	{
		throw NotAllowedInIf("Seq block");
	}

	public void VisitSwitch(SwitchStmt switchStmt)
	{
		throw NotAllowedInIf("Switch");
	}

	internal static EmitException NotAllowedInIf(string stmtKind)
	{
		return new EmitException($"If emit: {stmtKind} cannot be placed inside a Comb If (always @(*)); only Assign and nested If are allowed.");
	}
}