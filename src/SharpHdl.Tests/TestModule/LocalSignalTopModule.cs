using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// ローカル信号を持つ子モジュールを 1 階層インスタンス化する確認用（M4b）。
/// </summary>
public sealed class LocalSignalTop : Module
{
	public LocalSignalModule Child { get; } = new();

	public In Clk { get; } = In.UInt(1, "clk");
	public In Rst { get; } = In.UInt(1, "rst");
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public Out Y { get; } = Out.UInt(8, "Y");

	public LocalSignalTop()
	{
		SetPorts([Clk, Rst, A, B, Y]);
	}

	public override void Describe()
	{
		Child.DescribeDatapath();
		Instance(Child, "child",
			(Child.Clk, Clk), (Child.Rst, Rst), (Child.A, A), (Child.B, B), (Child.Y, Y));
	}
}
