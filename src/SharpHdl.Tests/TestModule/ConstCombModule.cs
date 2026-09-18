using SharpHdl.Core.Model;

public sealed class ConstComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public Out Lit { get; } = Out.UInt(8, "Lit");
	public Out Sum { get; } = Out.UInt(8, "Sum");
	public Out IsThree { get; } = Out.UInt(1, "IsThree");
	public Out Masked { get; } = Out.UInt(8, "Masked");

	public ConstComb()
	{
		SetPorts([A, Lit, Sum, IsThree, Masked]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			Lit.Assign(Const.UInt(8, 5));
			Sum.Assign(A + Const.UInt(8, 10));
			IsThree.Assign(A.Eq(Const.UInt(8, 3)));
			Masked.Assign(Const.UInt(8, 300));
		});
	}
}
