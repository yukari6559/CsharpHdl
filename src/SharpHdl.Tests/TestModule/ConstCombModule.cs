using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

public sealed class ConstComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public Out Imm { get; } = Out.UInt(8, "Lit");
	public Out Sum { get; } = Out.UInt(8, "Sum");
	public Out IsThree { get; } = Out.UInt(1, "IsThree");
	public Out Masked { get; } = Out.UInt(8, "Masked");

	public ConstComb()
	{
		SetPorts([A, Imm, Sum, IsThree, Masked]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			Imm.Assign(Lit.Bits(8, 5));
			Sum.Assign(A + Lit.Bits(8, 10));
			IsThree.Assign(A.Eq(Lit.Bits(8, 3)));
			Masked.Assign(Lit.Bits(8, 300));
		});
	}
}
