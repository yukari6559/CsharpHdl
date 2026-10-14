using System.Collections.ObjectModel;
using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;
using SharpHdl.Tests.TestModule;

namespace SharpHdl.Tests;

public class LocalSignalTests
{
	[Fact]
	public void LocalsAreRegisteredAndAssignableInTheirContext()
	{
		LocalSignalModule module = new();
		module.DescribeDatapath();

		Assert.Equal([module.Sum, module.Acc], module.GetLocals());
		Assert.DoesNotContain(module.Sum, module.GetPorts());
		Assert.DoesNotContain(module.Acc, module.GetPorts());

		ReadOnlyCollection<Stmt> stmts = module.GetStmts();
		Assert.Equal(3, stmts.Count);
		AssignStmt sumAssign = Assert.IsType<AssignStmt>(stmts[0]);
		Assert.Same(module.Sum, sumAssign.Signal);
		SeqBlockStmt seqBlock = Assert.IsType<SeqBlockStmt>(stmts[1]);
		SeqAssignStmt accAssign = Assert.IsType<SeqAssignStmt>(Assert.Single(seqBlock.Body));
		Assert.Same(module.Acc, accAssign.Signal);
		Assert.Same(module.Sum, accAssign.Expr);
	}

	[Fact]
	public void LocalsHaveInternalDirection()
	{
		Assert.Equal(SignalDirection.Internal, Wire.UInt(4, "w").Direction);
		Assert.Equal(SignalDirection.Internal, Reg.UInt(4, "r").Direction);
	}

	[Fact]
	public void WireAssignInSeqThrows()
	{
		LocalSignalModule module = new();
		_ = Assert.Throws<DescribeContextException>(module.DescribeWireInSeq);
	}

	[Fact]
	public void RegAssignInCombThrows()
	{
		LocalSignalModule module = new();
		_ = Assert.Throws<DescribeContextException>(module.DescribeRegInComb);
	}

	[Fact]
	public void InputAssignInCombThrows()
	{
		LocalSignalModule module = new();
		_ = Assert.Throws<DescribeContextException>(module.DescribeInputInComb);
	}

	[Fact]
	public void InputAssignInSeqThrows()
	{
		LocalSignalModule module = new();
		_ = Assert.Throws<DescribeContextException>(module.DescribeInputInSeq);
	}

	[Fact]
	public void SetLocalWithPortSignalThrows()
	{
		Module module = new();
		_ = Assert.Throws<SignalDeclarationException>(() => module.SetLocal(Out.UInt(1, "y")));
		_ = Assert.Throws<SignalDeclarationException>(() => module.SetLocals([Wire.UInt(1, "w"), In.UInt(1, "a")]));
		Assert.Empty(module.GetLocals());
	}

	[Fact]
	public void SetPortWithLocalSignalThrows()
	{
		Module module = new();
		_ = Assert.Throws<SignalDeclarationException>(() => module.SetPort(Wire.UInt(1, "w")));
		_ = Assert.Throws<SignalDeclarationException>(() => module.SetPorts([In.UInt(1, "a"), Reg.UInt(1, "r")]));
		Assert.Empty(module.GetPorts());
	}

	[Fact]
	public void DuplicateNameBetweenPortAndLocalThrows()
	{
		Module module = new();
		module.SetPort(In.UInt(1, "x"));
		_ = Assert.Throws<SignalDeclarationException>(() => module.SetLocal(Wire.UInt(1, "x")));
		_ = Assert.Throws<SignalDeclarationException>(() => module.SetLocals([Reg.UInt(1, "x")]));

		Module reversed = new();
		reversed.SetLocal(Wire.UInt(1, "x"));
		_ = Assert.Throws<SignalDeclarationException>(() => reversed.SetPort(In.UInt(1, "x")));
		_ = Assert.Throws<SignalDeclarationException>(() => reversed.SetPorts([Out.UInt(1, "x")]));
	}

	[Fact]
	public void DuplicateNameWithinSameKindThrows()
	{
		Module module = new();
		module.SetPort(In.UInt(1, "a"));
		_ = Assert.Throws<SignalDeclarationException>(() => module.SetPort(Out.UInt(1, "a")));
		_ = Assert.Throws<SignalDeclarationException>(() => module.SetPorts([In.UInt(1, "b"), Out.UInt(1, "b")]));
		_ = Assert.Throws<SignalDeclarationException>(() => module.SetLocals([Wire.UInt(1, "w"), Reg.UInt(1, "w")]));
	}

	[Fact]
	public void FailedRegistrationDoesNotReserveNames()
	{
		Module module = new();
		_ = Assert.Throws<SignalDeclarationException>(() => module.SetPort(Wire.UInt(1, "x")));
		module.SetLocal(Wire.UInt(1, "x"));

		_ = Assert.Throws<SignalDeclarationException>(() => module.SetLocal(Out.UInt(1, "y")));
		module.SetPort(Out.UInt(1, "y"));

		_ = Assert.Throws<SignalDeclarationException>(() => module.SetLocals([Wire.UInt(1, "p"), Out.UInt(1, "q")]));
		module.SetLocals([Wire.UInt(1, "p"), Reg.UInt(1, "q")]);

		_ = Assert.Throws<SignalDeclarationException>(() => module.SetPorts([In.UInt(1, "s"), Reg.UInt(1, "t")]));
		module.SetPorts([In.UInt(1, "s"), Out.UInt(1, "t")]);

		Assert.Equal(["y", "s", "t"], module.GetPorts().Select(p => p.Name));
		Assert.Equal(["x", "p", "q"], module.GetLocals().Select(l => l.Name));
	}
}
