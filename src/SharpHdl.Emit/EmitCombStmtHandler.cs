using System.Globalization;
using System.Text;
using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;
using SharpHdl.Core.Walk;

namespace SharpHdl.Emit;

public class EmitCombStmtHandler(StringBuilder verilogsb) : ICombStmtHandler
{
	public void OnAssign(AssignStmt assignStmt)
	{
		Expr expr = assignStmt.Expr;
		string s = VerilogEmitter.EmitExpr(expr);
		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\tassign {assignStmt.Signal.Name} = {s};\n");
	}

	public void OnIf(IfStmt ifStmt)
	{
		_ = verilogsb.Append("\talways @(*) begin\n");
		VerilogEmitter.EmitOneIf(verilogsb, ifStmt, 1);

		_ = verilogsb.Append("\tend\n");
	}

	public void OnInstance(InstanceStmt instanceStmt)
	{

	}

	public void OnMem2R1WStmt(Mem2R1WStmt mem2R1WStmt)
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

	public void OnSwitch(SwitchStmt switchStmt)
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