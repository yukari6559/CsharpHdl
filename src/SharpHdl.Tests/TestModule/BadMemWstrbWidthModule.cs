using SharpHdl.Core.Model;

/// <summary>Mem wstrb の幅不一致確認用。</summary>
public sealed class BadMemWstrbWidthModule : Module
{
	public In Clk { get; } = In.UInt(1, "clk");
	public In We { get; } = In.UInt(1, "we");
	public In Wstrb { get; } = In.UInt(3, "wstrb");
	public In Addr { get; } = In.UInt(8, "addr");
	public In Wdata { get; } = In.UInt(32, "wdata");
	public Out Rdata { get; } = Out.UInt(32, "rdata");

	public BadMemWstrbWidthModule()
	{
		SetPorts([Clk, We, Wstrb, Addr, Wdata, Rdata]);
	}

	public void DescribeWstrbMismatch()
	{
		Mem(Clk, depth: 256, width: 32, We, Wstrb, Addr, Wdata, Rdata);
	}
}
