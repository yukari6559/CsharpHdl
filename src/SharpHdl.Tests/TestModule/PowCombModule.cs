using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// 定数べき乗の確認用（#160）。指数は定数だけ。結果は底の幅。
/// </summary>
public sealed class PowComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public Out Pow3 { get; } = Out.UInt(8, "Pow3");
	public Out Pow5 { get; } = Out.UInt(8, "Pow5");
	public Out Pow6 { get; } = Out.UInt(8, "Pow6");
	public Out Pow8 { get; } = Out.UInt(8, "Pow8");
	public Out Pow0 { get; } = Out.UInt(8, "Pow0");
	public Out PowLit { get; } = Out.UInt(8, "PowLit");

	public PowComb()
	{
		SetPorts([A, Pow3, Pow5, Pow6, Pow8, Pow0, PowLit]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			Pow3.Assign(A.Pow(3));
			Pow5.Assign(A.Pow(5));
			Pow6.Assign(A.Pow(6));
			Pow8.Assign(A.Pow(8));
			Pow0.Assign(A.Pow(0));
			PowLit.Assign(A.Pow(Lit.Bits(4, 3)));
		});
	}
}
