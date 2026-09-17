using SharpHdl.Core.Model;

public sealed class IfMuxComb : Module
{
	public In SelA { get; } = In.UInt(8, "SelA");
	public In SelB { get; } = In.UInt(8, "SelB");
	public In C { get; } = In.UInt(8, "C");
	public In D { get; } = In.UInt(8, "D");
	public Out Y { get; } = Out.UInt(8, "Y");

	public IfMuxComb()
	{
		SetPorts([SelA, SelB, C, D, Y]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			If(SelA.Eq(SelB),
				then: () => Y.Assign(C),
				@else: () => Y.Assign(D));
		});
	}
}
