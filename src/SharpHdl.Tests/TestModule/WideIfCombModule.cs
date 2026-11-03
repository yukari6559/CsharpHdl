using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// 2 ビット以上の If の条件の確認用（#169）。Emit は <c>|x</c> で 1 ビットにし、1 ビットの条件はそのまま。
/// </summary>
public sealed class WideIfComb : Module
{
	public In Sel { get; } = In.UInt(8, "Sel");
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public In En { get; } = In.UInt(1, "En");
	public Out Y { get; } = Out.UInt(8, "Y");
	public Out Z { get; } = Out.UInt(8, "Z");
	public Out W { get; } = Out.UInt(8, "W");

	public WideIfComb()
	{
		SetPorts([Sel, A, B, En, Y, Z, W]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			If(Sel,
				then: () => Y.Assign(A),
				@else: () => Y.Assign(B));
			If(A + B,
				then: () => Z.Assign(A),
				@else: () => Z.Assign(B));
			If(En,
				then: () => W.Assign(A),
				@else: () => W.Assign(B));
		});
	}
}
