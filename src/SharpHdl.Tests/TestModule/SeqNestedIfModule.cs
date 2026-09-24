using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// Seq body に If を置き、Sim が未対応を明示例外にできることの検証用。
/// </summary>
public sealed class SeqNestedIf : Module
{
	public In Clk { get; } = In.UInt(1, "clk");
	public In Rst { get; } = In.UInt(1, "rst");
	public In En { get; } = In.UInt(1, "en");
	public Out Count { get; } = Out.UInt(8, "count");

	public SeqNestedIf()
	{
		SetPorts([Clk, Rst, En, Count]);
	}

	public override void Describe()
	{
		Seq(Clk, Rst, () =>
		{
			If(En, () => Count.Assign(0, Count + Lit.Bits(8, 1)));
		});
	}
}
