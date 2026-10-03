using System.Globalization;
using System.Text;
using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;
using SharpHdl.Core.Validate;

namespace SharpHdl.Emit;

/// <summary>
/// Verilog emitter の入口プレースホルダ。実装は docs/verilog-emit-spec.md に従う。
/// </summary>
public static class VerilogEmitter
{
	public static string Emitter(Module top, string topName)
	{
		List<InstanceStmt> instanceStmts = [];

		return EmitOneModule(top, topName, instanceStmts, true);
	}

	public static string EmitExpr(Expr expr)
	{

		if (expr is Signal signal)
		{
			return signal.Name;
		}
		else if (expr is OpExpr opExpr)
		{
			return EmitExpr(opExpr.Left) + " " + opExpr.Op.ToCustomString() + " " + EmitExpr(opExpr.Right);
		}
		else if (expr is SliceExpr sliceExpr)
		{
			return EmitExpr(sliceExpr.Expr) + $"[{sliceExpr.MSB}:{sliceExpr.LSB}]";
		}
		else if (expr is ConcatExpr concatExpr)
		{
			string s = "{";
			for (int i = 0; i < concatExpr.Exprs.Length; i++)
			{
				if (i == concatExpr.Exprs.Length - 1)
				{
					s += $"{EmitExpr(concatExpr.Exprs[i])}";
				}
				else
				{
					s += $"{EmitExpr(concatExpr.Exprs[i])}, ";
				}
			}
			s += "}";
			return s;
		}
		else if (expr is SignExtendExpr signExtendExpr)
		{
			if (signExtendExpr.GetWidth() - signExtendExpr.Expr.GetWidth() == 0)
			{
				return EmitExpr(signExtendExpr.Expr);
			}
			else
			{
				string s = "{{" + $"{signExtendExpr.GetWidth() - signExtendExpr.Expr.GetWidth()}{{" + $"{EmitExpr(signExtendExpr.Expr)}[{signExtendExpr.Expr.GetWidth() - 1}]" + "}}, " + $"{EmitExpr(signExtendExpr.Expr)}" + "}";
				return s;
			}
		}
		else if (expr is ZeroExtendExpr zeroExtendExpr)
		{
			if (zeroExtendExpr.GetWidth() - zeroExtendExpr.Expr.GetWidth() == 0)
			{
				return EmitExpr(zeroExtendExpr.Expr);
			}
			else
			{
				string s = "{{" + $"{zeroExtendExpr.GetWidth() - zeroExtendExpr.Expr.GetWidth()}" + "{1'b0}}, " + $"{EmitExpr(zeroExtendExpr.Expr)}}}";
				return s;
			}
		}
		else if (expr is ConstExpr constExpr)
		{
			ulong masked = constExpr.Width != 64
				? constExpr.Value & ((1UL << (int)constExpr.Width) - 1)
				: constExpr.Width == 64 ? constExpr.Value : throw new WidthMismatchException();
			string s = $"{constExpr.Width}'d{masked}";
			return s;
		}
		else
		{
			throw new EmitException();
		}
	}

	public static HashSet<Signal> CollectRegOuts(List<Stmt> stmts, HashSet<Signal> regOuts)
	{
		RegOutVisitor regOutVisitor = new(regOuts);
		foreach (Stmt item in stmts)
		{
			item.Accept(regOutVisitor);
		}
		return regOuts;
	}

	public static void WireDefine(List<Signal> signals, StringBuilder verilogsb, HashSet<Signal> regOuts)
	{
		for (int i = 0; i < signals.Count; i++)
		{
			Signal item = signals[i];
			if (item.Width == 1)
			{
				if (item.Direction == SignalDirection.Input)
				{
					_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\tinput wire {item.Name}");
				}
				else if (regOuts.Contains(item))
				{
					_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\toutput reg [{item.Width - 1}:0] {item.Name}");
				}
				else if (item.Direction == SignalDirection.Output)
				{
					_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\toutput wire {item.Name}");
				}
			}
			else if (item.Direction == SignalDirection.Input)
			{
				_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\tinput wire [{item.Width - 1}:0] {item.Name}");
			}
			else if (regOuts.Contains(item))
			{
				_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\toutput reg [{item.Width - 1}:0] {item.Name}");
			}
			else if (item.Direction == SignalDirection.Output)
			{
				_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\toutput wire [{item.Width - 1}:0] {item.Name}");
			}

			_ = i + 1 != signals.Count ? verilogsb.Append(",\n") : verilogsb.Append('\n');
		}
		_ = verilogsb.Append(");\n");
	}

	public static void EmitModuleBody(List<Stmt> stmts, StringBuilder verilogsb)
	{
		CheckMultiDrive.Check(stmts);
		EmitBodyVisitor emitBodyVisitor = new(verilogsb);
		foreach (Stmt item in stmts)
		{
			item.Accept(emitBodyVisitor);
		}
	}

	public static void CollectInstances(List<Stmt> stmts, List<InstanceStmt> instanceStmts, StringBuilder verilogsb)
	{
		InstanceVisitor instanceVisitor = new(verilogsb, instanceStmts);
		foreach (Stmt item in stmts)
		{
			item.Accept(instanceVisitor);
		}
	}

	public static void EmitInstanceLines(List<InstanceStmt> instanceStmts, StringBuilder verilogsb)
	{
		foreach (InstanceStmt item in instanceStmts)
		{
			_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\t{item.ChildModule.GetType().Name} {item.InstanceName} (\n");
			for (int i = 0; i < item.PortConnections.Count; i++)
			{
				_ = i == item.PortConnections.Count - 1
					? verilogsb.Append(CultureInfo.InvariantCulture, $"\t\t.{item.PortConnections[i].ChildPort.Name}({item.PortConnections[i].ParentSignal.Name})")
					: verilogsb.Append(CultureInfo.InvariantCulture, $"\t\t.{item.PortConnections[i].ChildPort.Name}({item.PortConnections[i].ParentSignal.Name}),");
			}
			_ = verilogsb.Append(");\n");
		}
	}

	public static string EmitOneModule(Module module, string moduleName, List<InstanceStmt> instanceStmts, bool emitHierarchy)
	{
		StringBuilder verilogsb = new();

		List<Signal> signals = [.. module.GetPorts()];
		List<Stmt> stmts = [.. module.GetStmts()];
		HashSet<Signal> regOuts = [];

		if (emitHierarchy)
		{
			CollectInstances(stmts, instanceStmts, verilogsb);
		}

		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"module {moduleName}(\n");
		_ = CollectRegOuts(stmts, regOuts);

		WireDefine(signals, verilogsb, regOuts);

		if (emitHierarchy)
		{
			EmitInstanceLines(instanceStmts, verilogsb);
		}

		EmitModuleBody(stmts, verilogsb);

		_ = verilogsb.Append("endmodule");

		return verilogsb.ToString();
	}
}
