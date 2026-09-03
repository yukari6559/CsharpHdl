using SharpHdl.Core.Model;

public sealed class BadWidthModule : Module
{
	public In Clk { get; } = In.UInt(1, "clk");
	public In Rst { get; } = In.UInt(1, "rst");
	public In Wide { get; } = In.UInt(32, "wide");
	public Out Narrow { get; } = Out.UInt(8, "narrow");

	public BadWidthModule()
	{
		SetPorts([Clk, Rst, Wide, Narrow]);
	}

	public void DescribeCombMismatch()
	{
		Comb(() => Narrow.Assign(Wide));
	}

	public void DescribeSeqMismatch()
	{
		Seq(Clk, Rst, () => Narrow.Assign(0, Wide));
	}
}
