using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// リダクション 6 種の Emit と Sim の確認用（#101）。
/// </summary>
public sealed class ReduceComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public In En { get; } = In.UInt(1, "En");
	public Out AndR { get; } = Out.UInt(1, "AndR");
	public Out OrR { get; } = Out.UInt(1, "OrR");
	public Out XorR { get; } = Out.UInt(1, "XorR");
	public Out NandR { get; } = Out.UInt(1, "NandR");
	public Out NorR { get; } = Out.UInt(1, "NorR");
	public Out XnorR { get; } = Out.UInt(1, "XnorR");
	public Out NotAndR { get; } = Out.UInt(1, "NotAndR");
	public Out EnOr { get; } = Out.UInt(1, "EnOr");
	public Out OrOfAnd { get; } = Out.UInt(1, "OrOfAnd");
	public Out LowXor { get; } = Out.UInt(1, "LowXor");

	public ReduceComb()
	{
		SetPorts([A, B, En, AndR, OrR, XorR, NandR, NorR, XnorR, NotAndR, EnOr, OrOfAnd, LowXor]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			AndR.Assign(A.ReduceAnd());
			OrR.Assign(A.ReduceOr());
			XorR.Assign(A.ReduceXor());
			NandR.Assign(A.ReduceNand());
			NorR.Assign(A.ReduceNor());
			XnorR.Assign(A.ReduceXnor());
			NotAndR.Assign(~A.ReduceAnd());
			EnOr.Assign(En & A.ReduceOr());
			OrOfAnd.Assign((A & B).ReduceOr());
			LowXor.Assign(A.Slice(3, 0).ReduceXor());
		});
	}
}
