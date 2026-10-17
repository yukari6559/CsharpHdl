using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

public sealed class XorComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public Out X { get; } = Out.UInt(8, "X");
	public Out Xn { get; } = Out.UInt(8, "Xn");

	public XorComb()
	{
		SetPorts([A, B, X, Xn]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			X.Assign(A ^ B);
			Xn.Assign(A.Xnor(B));
		});
	}
}
