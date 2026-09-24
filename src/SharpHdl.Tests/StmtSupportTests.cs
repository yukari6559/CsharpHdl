using System.Reflection;
using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;
using SharpHdl.Sim;
using SharpHdl.Sim.Interp;

namespace SharpHdl.Tests;

public class StmtSupportTests
{
	private sealed class BogusStmt : Stmt
	{
	}

	private sealed class ModuleWithUnknownTopStmt : Core.Model.Module
	{
		public override void Describe()
		{
			FieldInfo field = typeof(Core.Model.Module).GetField("Stmts", BindingFlags.Instance | BindingFlags.NonPublic)
				?? throw new InvalidOperationException("Stmts field missing");
			List<Stmt> stmts = (List<Stmt>)(field.GetValue(this)
				?? throw new InvalidOperationException("Stmts is null"));
			stmts.Add(new BogusStmt());
		}
	}

	[Fact]
	public void IsSupportedTopLevelAssignStmtIsTrue()
	{
		In a = In.UInt(1, "a");
		Out y = Out.UInt(1, "y");
		AssignStmt stmt = new(y, a);
		Assert.True(StmtSupport.IsSupportedTopLevel(stmt));
	}

	[Fact]
	public void IsSupportedTopLevelUnknownStmtIsFalse()
	{
		Assert.False(StmtSupport.IsSupportedTopLevel(new BogusStmt()));
	}

	[Fact]
	public void RunUnknownTopLevelStmtThrowsSimUnsupported()
	{
		_ = Assert.Throws<SimUnsupportedException>(() => new ModuleWithUnknownTopStmt().Run());
	}
}
