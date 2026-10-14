using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// モジュール内ローカル信号（Wire / Reg）の宣言と代入文脈の確認用（M4a）。
/// </summary>
public sealed class LocalSignalModule : Module
{
	public In Clk { get; } = In.UInt(1, "clk");
	public In Rst { get; } = In.UInt(1, "rst");
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public Out Y { get; } = Out.UInt(8, "Y");
	public Wire Sum { get; } = Wire.UInt(8, "sum");
	public Reg Acc { get; } = Reg.UInt(8, "acc");

	public LocalSignalModule()
	{
		SetPorts([Clk, Rst, A, B, Y]);
		SetLocals([Sum, Acc]);
	}

	public void DescribeDatapath()
	{
		Comb(() => Sum.Assign(A + B));
		Seq(Clk, Rst, () => Acc.Assign(0, Sum));
		Comb(() => Y.Assign(Acc));
	}

	public void DescribeWireInSeq()
	{
		Seq(Clk, Rst, () => Sum.Assign(0, A));
	}

	public void DescribeRegInComb()
	{
		Comb(() => Acc.Assign(A));
	}

	public void DescribeInputInComb()
	{
		Comb(() => A.Assign(B));
	}

	public void DescribeInputInSeq()
	{
		Seq(Clk, Rst, () => A.Assign(0, B));
	}
}
