using SharpHdl.Core.Model;

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
			If(En.Eq(Const.UInt(1, 1)),
				then: () =>
				{
					Switch(Op,
						(0, () => Y.Assign(A)),
						(1, () => Y.Assign(B)));
				},
				@else: () => Y.Assign(Const.UInt(8, 0)));
		});
	}
}
