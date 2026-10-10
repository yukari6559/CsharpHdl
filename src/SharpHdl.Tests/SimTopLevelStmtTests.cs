using System.Reflection;
using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;
using SharpHdl.Sim;

namespace SharpHdl.Tests;

public class SimTopLevelStmtTests
{
	private sealed class BogusStmt : Stmt
	{
		public override void Accept(IStmtVisitor stmtVisitor)
		{
			throw new SimUnsupportedException();
		}
	}

	private abstract class ModuleWithInjectedTopStmt : Core.Model.Module
	{
		protected void AddTopStmt(Stmt stmt)
		{
			FieldInfo field = typeof(Core.Model.Module).GetField("Stmts", BindingFlags.Instance | BindingFlags.NonPublic)
				?? throw new InvalidOperationException("Stmts field missing");
			List<Stmt> stmts = (List<Stmt>)(field.GetValue(this)
				?? throw new InvalidOperationException("Stmts is null"));
			stmts.Add(stmt);
		}
	}

	private sealed class ModuleWithUnknownTopStmt : ModuleWithInjectedTopStmt
	{
		public override void Describe()
		{
			AddTopStmt(new BogusStmt());
		}
	}

	private sealed class ModuleWithTopLevelSeqAssign : ModuleWithInjectedTopStmt
	{
		public In A { get; } = In.UInt(1, "a");
		public Out Y { get; } = Out.UInt(1, "y");

		public ModuleWithTopLevelSeqAssign()
		{
			SetPorts([A, Y]);
		}

		public override void Describe()
		{
			AddTopStmt(new SeqAssignStmt(Y, A, 0));
		}
	}

	[Fact]
	public void RunUnknownTopLevelStmtThrowsSimUnsupported()
	{
		_ = Assert.Throws<SimUnsupportedException>(() => new ModuleWithUnknownTopStmt().Run());
	}

	[Fact]
	public void RunTopLevelSeqAssignThrowsSimUnsupported()
	{
		_ = Assert.Throws<SimUnsupportedException>(() => new ModuleWithTopLevelSeqAssign().Run());
	}
}
