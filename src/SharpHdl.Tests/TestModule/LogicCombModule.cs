using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// 論理積・論理和の確認用（#154）。同じ幅の AND と、幅の違う OR、入れ子の括弧。
/// </summary>
public sealed class LogicComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public In N { get; } = In.UInt(4, "N");
	public Out AndAB { get; } = Out.UInt(1, "AndAB");
	public Out OrAN { get; } = Out.UInt(1, "OrAN");
	public Out AndOfOr { get; } = Out.UInt(1, "AndOfOr");
	public Out OrOfAnd { get; } = Out.UInt(1, "OrOfAnd");

	public LogicComb()
	{
		SetPorts([A, B, N, AndAB, OrAN, AndOfOr, OrOfAnd]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			AndAB.Assign(A.LogicAnd(B));
			OrAN.Assign(A.LogicOr(N));
			AndOfOr.Assign(A.LogicAnd(B).LogicOr(N));
			OrOfAnd.Assign(A.LogicOr(B).LogicAnd(N));
		});
	}
}
