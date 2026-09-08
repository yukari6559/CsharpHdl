using SharpHdl.Core.Model;

/// <summary>T2c: 同期書き・非同期読みの 2R1W（depth 32 / width 64）。</summary>
public sealed class RegFile2R1W : Module
{
	public In Clk { get; } = In.UInt(1, "clk");
	public In We { get; } = In.UInt(1, "we");
	public In Waddr { get; } = In.UInt(5, "waddr");
	public In Wdata { get; } = In.UInt(64, "wdata");
	public In Raddr0 { get; } = In.UInt(5, "raddr0");
	public Out Rdata0 { get; } = Out.UInt(64, "rdata0");
	public In Raddr1 { get; } = In.UInt(5, "raddr1");
	public Out Rdata1 { get; } = Out.UInt(64, "rdata1");

	public RegFile2R1W()
	{
		SetPorts([Clk, We, Waddr, Wdata, Raddr0, Rdata0, Raddr1, Rdata1]);
	}

	public override void Describe()
	{
		Mem(Clk, depth: 32, width: 64, We, Waddr, Wdata, Raddr0, Rdata0, Raddr1, Rdata1);
	}
}
