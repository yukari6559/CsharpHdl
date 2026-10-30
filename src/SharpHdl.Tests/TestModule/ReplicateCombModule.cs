using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// 繰り返し連結の確認用（#165）。回数は定数だけ。結果は回数と中の幅の積。
/// </summary>
public sealed class ReplicateComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public In Bit { get; } = In.UInt(1, "Bit");
	public In N { get; } = In.UInt(4, "N");
	public Out Twice { get; } = Out.UInt(16, "Twice");
	public Out Fill { get; } = Out.UInt(8, "Fill");
	public Out Once { get; } = Out.UInt(8, "Once");
	public Out LitCount { get; } = Out.UInt(32, "LitCount");
	public Out Mixed { get; } = Out.UInt(16, "Mixed");
	public Out Sum { get; } = Out.UInt(16, "Sum");
	public Out NotRep { get; } = Out.UInt(16, "NotRep");

	public ReplicateComb()
	{
		SetPorts([A, Bit, N, Twice, Fill, Once, LitCount, Mixed, Sum, NotRep]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			Twice.Assign(A.Replicate(2));
			Fill.Assign(Bit.Replicate(8));
			Once.Assign(A.Replicate(1));
			LitCount.Assign(A.Replicate(Lit.Bits(8, 260)));
			Mixed.Assign(N.Replicate(2).Concat(A));
			Sum.Assign(A.Replicate(2) + A.Replicate(2));
			NotRep.Assign(~A.Replicate(2));
		});
	}
}
