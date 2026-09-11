using SharpHdl.Core.Model;

/// <summary>拡張先幅が元より狭いときの例外確認用。</summary>
public sealed class BadExtendWidthModule : Module
{
	public In Wide { get; } = In.UInt(16, "wide");
	public Out Narrow { get; } = Out.UInt(8, "narrow");

	public BadExtendWidthModule()
	{
		SetPorts([Wide, Narrow]);
	}

	public void DescribeSignShrink()
	{
		Comb(() => Narrow.Assign(Wide.SignExtend(8)));
	}

	public void DescribeZeroShrink()
	{
		Comb(() => Narrow.Assign(Wide.ZeroExtend(8)));
	}
}
