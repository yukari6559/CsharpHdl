using SharpHdl.Core.Model;

namespace Examples.Mem;

/// <summary>
/// Phase 3 Mem 例: 同期 1R1W（深さ 256 × 幅 32）。
/// </summary>
public sealed class SimpleRam : Module
{
	public In Clk { get; } = In.UInt(1, "clk");
	public In We { get; } = In.UInt(1, "we");
	public In Addr { get; } = In.UInt(8, "addr");
	public In Wdata { get; } = In.UInt(32, "wdata");
	public Out Rdata { get; } = Out.UInt(32, "rdata");

	public SimpleRam()
	{
		SetPorts([Clk, We, Addr, Wdata, Rdata]);
	}

	public override void Describe()
	{
		Mem(Clk, depth: 256, width: 32, We, Addr, Wdata, Rdata);
	}
}
