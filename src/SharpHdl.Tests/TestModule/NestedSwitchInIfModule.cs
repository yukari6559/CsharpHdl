using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

public sealed class NestedSwitchInIf : Module
{
	public In En { get; } = In.UInt(1, "En");
	public In Op { get; } = In.UInt(1, "Op");
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public Out Y { get; } = Out.UInt(8, "Y");

	public NestedSwitchInIf()
	{
		SetPorts([En, Op, A, B, Y]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			If(En.Eq(Lit.Bits(1, 1)),
				then: () =>
				{
					Switch(Op,
						(0, () => Y.Assign(A)),
						(1, () => Y.Assign(B)));
				},
				@else: () => Y.Assign(Lit.Bits(8, 0)));
		});
	}
}
