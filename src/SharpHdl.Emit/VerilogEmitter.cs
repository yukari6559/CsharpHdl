using System.Globalization;
using System.Text;
using SharpHdl.Core.Model;
using SharpHdl.Core.Validate;
using SharpHdl.Emit.Visitors;

namespace SharpHdl.Emit;

/// <summary>
/// Verilog emitter の入口プレースホルダ。実装は docs/verilog-emit-spec.md に従う。
/// </summary>
public static class VerilogEmitter
{
	public static string Emitter(Module top, string topName)
	{
		HashSet<string> emittedModules = [];
		List<InstanceStmt> instanceStmts = [];

		return EmitOneModule(top, topName, instanceStmts, emittedModules);
	}

	public static string EmitExpr(Expr expr)
	{
		EmitExprVisitor exprVisitor = new();
		return expr.Accept(exprVisitor);
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

	public static void LocalWireDefine(List<Signal> localSignals, StringBuilder verilogsb, HashSet<Signal> regOuts)
	{
		for (int i = 0; i < localSignals.Count; i++)
		{
			if (localSignals[i].Width == 1)
			{
				if (localSignals[i] is Reg reg)
				{
					_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\treg {reg.Name};\n");
				}
				if (localSignals[i] is Wire wire)
				{
					_ = regOuts.Contains(localSignals[i])
						? verilogsb.Append(CultureInfo.InvariantCulture, $"\treg {wire.Name};\n")
						: verilogsb.Append(CultureInfo.InvariantCulture, $"\twire {wire.Name};\n");
				}
			}
			else
			{
				if (localSignals[i] is Reg reg)
				{
					_ = verilogsb.Append(CultureInfo.InvariantCulture, $"\treg [{reg.Width - 1}:0] {reg.Name};\n");
				}
				if (localSignals[i] is Wire wire)
				{
					_ = regOuts.Contains(localSignals[i])
						? verilogsb.Append(CultureInfo.InvariantCulture, $"\treg [{wire.Width - 1}:0] {wire.Name};\n")
						: verilogsb.Append(CultureInfo.InvariantCulture, $"\twire [{wire.Width - 1}:0] {wire.Name};\n");
				}
			}
		}
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

	public static void CollectInstances(List<Stmt> stmts, List<InstanceStmt> instanceStmts, StringBuilder verilogsb, HashSet<string> emittedModules)
	{
		InstanceVisitor instanceVisitor = new(verilogsb, instanceStmts, emittedModules);
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
					? verilogsb.Append(CultureInfo.InvariantCulture, $"\t\t.{item.PortConnections[i].ChildPort.Name}({item.PortConnections[i].ParentSignal.Name})\n")
					: verilogsb.Append(CultureInfo.InvariantCulture, $"\t\t.{item.PortConnections[i].ChildPort.Name}({item.PortConnections[i].ParentSignal.Name}),\n");
			}
			_ = verilogsb.Append("\t);\n");
		}
	}

	public static string EmitOneModule(Module module, string moduleName, List<InstanceStmt> instanceStmts, HashSet<string> emittedModules)
	{
		StringBuilder verilogsb = new();

		List<Signal> signals = [.. module.GetPorts()];
		List<Signal> localSignals = [.. module.GetLocals()];
		List<Stmt> stmts = [.. module.GetStmts()];
		HashSet<Signal> regOuts = [];

		CheckUndeclaredLocals.Check(module);

		CollectInstances(stmts, instanceStmts, verilogsb, emittedModules);

		_ = verilogsb.Append(CultureInfo.InvariantCulture, $"module {moduleName}(\n");
		_ = CollectRegOuts(stmts, regOuts);

		WireDefine(signals, verilogsb, regOuts);
		LocalWireDefine(localSignals, verilogsb, regOuts);

		EmitInstanceLines(instanceStmts, verilogsb);

		EmitModuleBody(stmts, verilogsb);

		_ = verilogsb.Append("endmodule");

		return verilogsb.ToString();
	}
}
