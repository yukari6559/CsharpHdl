using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

public sealed class IfThenOnlyComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public In C { get; } = In.UInt(8, "C");
	public Out Y { get; } = Out.UInt(8, "Y");

	public IfThenOnlyComb()
	{
		SetPorts([A, B, C, Y]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			If(A.Eq(B), then: () => Y.Assign(C));
		});
	}
}
