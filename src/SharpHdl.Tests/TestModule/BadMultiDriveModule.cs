using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// 同一信号への複数駆動（R-C7）の確認用。
/// </summary>
public sealed class BadMultiDriveModule : Module
{
	public In Clk { get; } = In.UInt(1, "clk");
	public In Rst { get; } = In.UInt(1, "rst");
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public Out Y { get; } = Out.UInt(8, "Y");
	public Out Count { get; } = Out.UInt(8, "count");

	public BadMultiDriveModule()
	{
		SetPorts([Clk, Rst, A, B, Y, Count]);
	}

	public void DescribeDualCombAssign()
	{
		Comb(() =>
		{
			Y.Assign(A);
			Y.Assign(B);
		});
	}

	public void DescribeDualSeqAssign()
	{
		Seq(Clk, Rst, () =>
		{
			Count.Assign(0, Count + Lit.Bits(8, 1));
			Count.Assign(0, A);
		});
	}

	public void DescribeAssignAndSwitch()
	{
		Comb(() =>
		{
			Y.Assign(A);
			Switch(A,
				(0u, () => Y.Assign(B)),
				(1u, () => Y.Assign(B)));
		});
	}
}
