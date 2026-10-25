using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// 符号なし除算・剰余の確認用（#157）。左右は同じ幅、結果は左の幅。
/// </summary>
public sealed class DivComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public Out Quot { get; } = Out.UInt(8, "Quot");
	public Out Rem { get; } = Out.UInt(8, "Rem");
	public Out Quot3 { get; } = Out.UInt(8, "Quot3");
	public Out Rem3 { get; } = Out.UInt(8, "Rem3");

	public DivComb()
	{
		SetPorts([A, B, Quot, Rem, Quot3, Rem3]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			Quot.Assign(A / B);
			Rem.Assign(A % B);
			Quot3.Assign(A / 3);
			Rem3.Assign(A % 3);
		});
	}

	public void DescribeRemainderSignalZero()
	{
		Comb(() => Rem.Assign(A % B));
	}

	public void DescribeQuotientExprZero()
	{
		Comb(() => Quot.Assign(A / (B - B)));
	}
}
