using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>T2d: 1R1W + バイトイネーブル（width 32 / wstrb 4）。</summary>
public sealed class ByteWriteRam : Module
{
	public In Clk { get; } = In.UInt(1, "clk");
	public In We { get; } = In.UInt(1, "we");
	public In Wstrb { get; } = In.UInt(4, "wstrb");
	public In Addr { get; } = In.UInt(8, "addr");
	public In Wdata { get; } = In.UInt(32, "wdata");
	public Out Rdata { get; } = Out.UInt(32, "rdata");

	public ByteWriteRam()
	{
		SetPorts([Clk, We, Wstrb, Addr, Wdata, Rdata]);
	}

	public override void Describe()
	{
		Mem(Clk, depth: 256, width: 32, We, Wstrb, Addr, Wdata, Rdata);
	}
}
