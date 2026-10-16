using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// If 内で代入する Wire と 1 bit の Wire の宣言・評価確認用（M4b）。
/// </summary>
public sealed class LocalIfComb : Module
{
	public In Sel { get; } = In.UInt(1, "Sel");
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public Out Y { get; } = Out.UInt(8, "Y");
	public Out Flag { get; } = Out.UInt(1, "Flag");
	public Wire Picked { get; } = Wire.UInt(8, "picked");
	public Wire Valid { get; } = Wire.UInt(1, "valid");

	public LocalIfComb()
	{
		SetPorts([Sel, A, B, Y, Flag]);
		SetLocals([Picked, Valid]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			If(Sel,
				then: () => Picked.Assign(A),
				@else: () => Picked.Assign(B));
		});
		Comb(() => Y.Assign(Picked));
		Comb(() => Valid.Assign(Sel));
		Comb(() => Flag.Assign(Valid));
	}
}
