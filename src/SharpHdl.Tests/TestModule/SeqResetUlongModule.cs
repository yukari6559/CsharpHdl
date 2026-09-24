using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// Seq reset が ulong（uint 超え）を取れることの確認用（R-C4）。
/// </summary>
public sealed class SeqResetUlong : Module
{
	public In Clk { get; } = In.UInt(1, "clk");
	public In Rst { get; } = In.UInt(1, "rst");
	public Out Q { get; } = Out.UInt(64, "q");

	public SeqResetUlong()
	{
		SetPorts([Clk, Rst, Q]);
	}

	public override void Describe()
	{
		Seq(Clk, Rst, () => Q.Assign(0x1_0000_0000UL, Q + Lit.Bits(64, 1)));
	}
}
