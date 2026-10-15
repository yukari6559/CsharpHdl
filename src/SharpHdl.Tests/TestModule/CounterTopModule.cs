using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

public sealed class CounterTop : Module
{
	private readonly Counter _counter = new();

	public In Clk { get; } = In.UInt(1, "clk");
	public In Rst { get; } = In.UInt(1, "rst");
	public In Step { get; } = In.UInt(8, "step");
	public Out Count { get; } = Out.UInt(8, "count");

	public CounterTop()
	{
		SetPorts([Clk, Rst, Step, Count]);
	}

	public override void Describe()
	{
		_counter.Describe();
		Instance(_counter, "counter",
			(_counter.Clk, Clk), (_counter.Rst, Rst), (_counter.Step, Step), (_counter.Count, Count));
	}
}
