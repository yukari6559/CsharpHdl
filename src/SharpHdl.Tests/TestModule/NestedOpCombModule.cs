using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// 入れ子の二項演算が Verilog の優先順位で崩れないことの確認用（#102）。
/// </summary>
public sealed class NestedOpComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public In C { get; } = In.UInt(8, "C");
	public Out XorAnd { get; } = Out.UInt(8, "XorAnd");
	public Out OrAnd { get; } = Out.UInt(8, "OrAnd");
	public Out AndOr { get; } = Out.UInt(8, "AndOr");

	public NestedOpComb()
	{
		SetPorts([A, B, C, XorAnd, OrAnd, AndOr]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			XorAnd.Assign((A ^ B) & C);
			OrAnd.Assign((A | B) & C);
			AndOr.Assign(A & (B | C));
		});
	}
}
