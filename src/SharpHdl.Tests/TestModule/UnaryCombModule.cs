using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// 単項演算 ~ / - / ! の Emit と Sim の確認用（#101）。
/// </summary>
public sealed class UnaryComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public In En { get; } = In.UInt(1, "En");
	public Out NotA { get; } = Out.UInt(8, "NotA");
	public Out NegA { get; } = Out.UInt(8, "NegA");
	public Out LNotA { get; } = Out.UInt(1, "LNotA");
	public Out NotAnd { get; } = Out.UInt(8, "NotAnd");
	public Out NegNeg { get; } = Out.UInt(8, "NegNeg");
	public Out NotAAndB { get; } = Out.UInt(8, "NotAAndB");
	public Out NotLow { get; } = Out.UInt(4, "NotLow");
	public Out LNotEn { get; } = Out.UInt(1, "LNotEn");

	public UnaryComb()
	{
		SetPorts([A, B, En, NotA, NegA, LNotA, NotAnd, NegNeg, NotAAndB, NotLow, LNotEn]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			NotA.Assign(~A);
			NegA.Assign(-A);
			LNotA.Assign(!A);
			NotAnd.Assign(~(A & B));
			NegNeg.Assign(-(-A));
			NotAAndB.Assign(~A & B);
			NotLow.Assign(~A.Slice(3, 0));
			LNotEn.Assign(!En);
		});
	}
}
