using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// 幅の違う値を ZeroExtend で揃えてから演算・比較する確認用（#105）。
/// </summary>
public sealed class ExtendedOperandComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(4, "B");
	public Out Sum { get; } = Out.UInt(8, "Sum");
	public Out Gt { get; } = Out.UInt(1, "Gt");

	public ExtendedOperandComb()
	{
		SetPorts([A, B, Sum, Gt]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			Sum.Assign(A + B.ZeroExtend(8));
			Gt.Assign(A > B.ZeroExtend(8));
		});
	}
}
