using SharpHdl.Core.Model;

public sealed class Counter : Module
{
	public In Clk { get; } = In.UInt(1, "clk");
	public In Rst { get; } = In.UInt(1, "rst");
	public In Step { get; } = In.UInt(8, "step");
	public Out Count { get; } = Out.UInt(8, "count");

	public Counter()
	{
		SetPorts([Clk, Rst, Step, Count]);
	}

	public override void Describe()
	{
		Seq(Clk, Rst, () => Count.Assign(0, Count + Step));
	}
}
