using SharpHdl.Core.Model;

public sealed class BadWidthCombModule : Module
{
	public In Wide { get; } = In.UInt(32, "wide");
	public Out Narrow { get; } = Out.UInt(8, "narrow");

	public BadWidthCombModule()
	{
		SetPorts([Wide, Narrow]);
	}

	public override void Describe()
	{
		Comb(() => Narrow.Assign(Wide));
	}
}

public sealed class BadWidthSeqModule : Module
{
	public In Clk { get; } = In.UInt(1, "clk");
	public In Rst { get; } = In.UInt(1, "rst");
	public In Wide { get; } = In.UInt(32, "wide");
	public Out Narrow { get; } = Out.UInt(8, "narrow");

	public BadWidthSeqModule()
	{
		SetPorts([Clk, Rst, Wide, Narrow]);
	}

	public override void Describe()
	{
		Seq(Clk, Rst, () => Narrow.Assign(0, Wide));
	}
}
