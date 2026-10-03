using System.Globalization;
using System.Text;
using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Emit;

public class EmitBodyVisitor(StringBuilder verilogsb) : IStmtVisitor
{
	public void VisitAssign(AssignStmt assignStmt)
	{
		Expr expr = assignStmt.Expr;
		string s = VerilogEmitter.EmitExpr(expr);
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\tassign {assignStmt.Signal.Name} = {s};\n");
	}

	public void VisitIf(IfStmt ifStmt)
	{
		_ = verilogsb.Append("\talways @(*) begin\n");
		IfVisitor ifVisitor = new(verilogsb, 1);
		ifStmt.Accept(ifVisitor);

		_ = verilogsb.Append("\tend\n");
	}

	public void VisitInstance(InstanceStmt instanceStmt)
	{

	}

	public void VisitMem(MemStmt memStmt)
	{
		string memName = $"mem_{memStmt.Rdata.Name}";
		if (!string.IsNullOrEmpty(memStmt.Name))
		{
			memName = memStmt.Name;
		}
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"reg [{memStmt.Width - 1}:0] {memName} [0:{memStmt.Depth - 1}];\n");
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"always @(posedge {memStmt.Clk.Name}) begin\n");
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\tif ({memStmt.We.Name}) {memName}[{memStmt.Addr.Name}] <= {memStmt.Wdata.Name};\n");
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\t{memStmt.Rdata.Name} <= {memName}[{memStmt.Addr.Name}];\n");
		_ = verilogsb.Append($"end\n");
	}

	public void VisitMem2R1W(Mem2R1WStmt mem2R1WStmt)
	{
		string memName = $"mem_{mem2R1WStmt.Rdata0.Name}";
		if (!string.IsNullOrEmpty(mem2R1WStmt.Name))
		{
			memName = mem2R1WStmt.Name;
		}
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"reg [{mem2R1WStmt.Width - 1}:0] {memName} [0:{mem2R1WStmt.Depth - 1}];\n");
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"always @(posedge {mem2R1WStmt.Clk.Name}) begin\n");
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\tif ({mem2R1WStmt.We.Name}) {memName}[{mem2R1WStmt.Waddr.Name}] <= {mem2R1WStmt.Wdata.Name};\n");
		_ = verilogsb.Append("end\n");
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"assign {mem2R1WStmt.Rdata0.Name} = {memName}[{mem2R1WStmt.Raddr0.Name}];\n");
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"assign {mem2R1WStmt.Rdata1.Name} = {memName}[{mem2R1WStmt.Raddr1.Name}];\n");
	}

	public void VisitMemWstrb(MemWstrbStmt memWstrbStmt)
	{
		string memName = $"mem_{memWstrbStmt.Rdata.Name}";
		if (!string.IsNullOrEmpty(memWstrbStmt.Name))
		{
			memName = memWstrbStmt.Name;
		}
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"reg [{memWstrbStmt.Width - 1}:0] {memName} [0:{memWstrbStmt.Depth - 1}];\n");
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"always @(posedge {memWstrbStmt.Clk.Name}) begin\n");
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\tif ({memWstrbStmt.We.Name}) begin\n");
		uint memLSB = 0;
		uint memMSB = 7;
		for (int i = 0; i < memWstrbStmt.Width / 8; i++)
		{
			_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\t\tif ({memWstrbStmt.Wstrb.Name}[{i}]) {memName}[{memWstrbStmt.Addr.Name}][{memMSB}:{memLSB}] <= {memWstrbStmt.Wdata.Name}[{memMSB}:{memLSB}];\n");
			memLSB += 8;
			memMSB += 8;
		}
		_ = verilogsb.Append("\tend\n");
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\t{memWstrbStmt.Rdata.Name} <= {memName}[{memWstrbStmt.Addr.Name}];\n");
		_ = verilogsb.Append($"end\n");
	}

	public void VisitSeqAssign(SeqAssignStmt seqAssignStmt)
	{
		throw new EmitException("Module body emit: Seq assign must be inside a Seq block, not at module top level.");
	}

	public void VisitSeqBlock(SeqBlockStmt seqBlockStmt)
	{
		SeqVisitor seqVisitor = new(verilogsb, seqBlockStmt.Reset);
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\talways @(posedge {seqBlockStmt.Clk.Name}) begin\n");
		foreach (Stmt bodyItem in seqBlockStmt.Body)
		{
			bodyItem.Accept(seqVisitor);
		}
		_ = verilogsb.Append("\tend\n");
	}

	public void VisitSwitch(SwitchStmt switchStmt)
	{
		Signal? beforeSignal = null;
		AssignStmt assignStmt;
		if (switchStmt.Cases.Count == 0)
		{
			throw new EmitException("Switch emit: Cases must not be empty.");
		}

		for (int i = 0; i < switchStmt.Cases.Count; i++)
		{
			if (switchStmt.Cases[i].Stmts.Count != 1)
			{
				throw new EmitException($"Switch emit: case {i} (value {switchStmt.Cases[i].Value}) must contain exactly one statement, got {switchStmt.Cases[i].Stmts.Count}.");
			}

			if (switchStmt.Cases[i].Stmts[0] is not AssignStmt)
			{
				throw new EmitException($"Switch emit: case {i} (value {switchStmt.Cases[i].Value}) must be a single AssignStmt.");
			}

			assignStmt = (AssignStmt)switchStmt.Cases[i].Stmts[0];
			if (i != 0 && beforeSignal != assignStmt.Signal)
			{
				throw new EmitException($"Switch emit: all cases must assign the same signal (case 0: {beforeSignal!.Name}, case {i}: {assignStmt.Signal.Name}).");
			}
			else if (i == 0)
			{
				_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\tassign {assignStmt.Signal.Name} = ({VerilogEmitter.EmitExpr(switchStmt.Expr)} == {switchStmt.Expr.GetWidth()}'d{switchStmt.Cases[0].Value}) ? ({VerilogEmitter.EmitExpr(assignStmt.Expr)}) :\n");
				beforeSignal = assignStmt.Signal;
				continue;
			}
			else if (i == switchStmt.Cases.Count - 1)
			{
				_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\t\t({VerilogEmitter.EmitExpr(assignStmt.Expr)});\n");
				break;
			}
			_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\t\t({VerilogEmitter.EmitExpr(switchStmt.Expr)} == {switchStmt.Expr.GetWidth()}'d{switchStmt.Cases[i].Value}) ? ({VerilogEmitter.EmitExpr(assignStmt.Expr)}) :\n");
			beforeSignal = assignStmt.Signal;
		}
	}
}