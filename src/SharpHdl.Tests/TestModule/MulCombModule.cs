using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// 乗算（同じ幅どうし、上位は切り捨て）と、ZeroExtend で広げた全幅の積の確認用（#9 M2a）。
/// </summary>
public sealed class MulComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public In C { get; } = In.UInt(8, "C");
	public In Q { get; } = In.UInt(64, "Q");
	public In R { get; } = In.UInt(64, "R");
	public Out Low { get; } = Out.UInt(8, "Low");
	public Out Full { get; } = Out.UInt(16, "Full");
	public Out Nested { get; } = Out.UInt(8, "Nested");
	public Out Q64 { get; } = Out.UInt(64, "Q64");

	public MulComb()
	{
		SetPorts([A, B, C, Q, R, Low, Full, Nested, Q64]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			Low.Assign(A * B);
			Full.Assign(A.ZeroExtend(16) * B.ZeroExtend(16));
			Nested.Assign((A * B) + C);
			Q64.Assign(Q * R);
		});
	}

	public void DescribeForgotWiden()
	{
		Comb(() => Full.Assign(A * B));
	}
}
