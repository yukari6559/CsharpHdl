using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// 同じ幅への ZeroExtend で入れ子の二項演算の括弧が消えないことの確認用（#148）。
/// </summary>
public sealed class SameWidthExtendComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public In C { get; } = In.UInt(8, "C");
	public Out SubSub { get; } = Out.UInt(8, "SubSub");
	public Out SubMul { get; } = Out.UInt(8, "SubMul");

	public SameWidthExtendComb()
	{
		SetPorts([A, B, C, SubSub, SubMul]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			SubSub.Assign(A - (B - C).ZeroExtend(8));
			SubMul.Assign((A - B).ZeroExtend(8) * C);
		});
	}
}
