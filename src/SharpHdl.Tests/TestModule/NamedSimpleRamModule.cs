using SharpHdl.Core.Model;

public sealed class NamedSimpleRam : Module
{
	public In Clk { get; } = In.UInt(1, "clk");
	public In We { get; } = In.UInt(1, "we");
	public In Addr { get; } = In.UInt(8, "addr");
	public In Wdata { get; } = In.UInt(32, "wdata");
	public Out Rdata { get; } = Out.UInt(32, "rdata");

	public NamedSimpleRam()
	{
		SetPorts([Clk, We, Addr, Wdata, Rdata]);
	}

	public override void Describe()
	{
		Mem(Clk, depth: 256, width: 32, We, Addr, Wdata, Rdata, name: "dmem");
	}
}
