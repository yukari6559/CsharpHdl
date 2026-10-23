using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// 全幅の積 MulFull の確認用（#114）。8 × 8 → 16 と、幅の違う 8 × 4 → 12。
/// </summary>
public sealed class MulFullComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public In N { get; } = In.UInt(4, "N");
	public Out Full { get; } = Out.UInt(16, "Full");
	public Out Mixed { get; } = Out.UInt(12, "Mixed");

	public MulFullComb()
	{
		SetPorts([A, B, N, Full, Mixed]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			Full.Assign(A.MulFull(B));
			Mixed.Assign(A.MulFull(N));
		});
	}
}
