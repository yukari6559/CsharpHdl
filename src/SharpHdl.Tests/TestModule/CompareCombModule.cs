using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

public sealed class CompareComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public Out Eq { get; } = Out.UInt(1, "Eq");
	public Out Neq { get; } = Out.UInt(1, "Neq");
	public Out Lt { get; } = Out.UInt(1, "Lt");
	public Out Le { get; } = Out.UInt(1, "Le");
	public Out Gt { get; } = Out.UInt(1, "Gt");
	public Out Ge { get; } = Out.UInt(1, "Ge");

	public CompareComb()
	{
		SetPorts([A, B, Eq, Neq, Lt, Le, Gt, Ge]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			Eq.Assign(A.Eq(B));
			Neq.Assign(A.Neq(B));
			Lt.Assign(A < B);
			Le.Assign(A <= B);
			Gt.Assign(A > B);
			Ge.Assign(A >= B);
		});
	}
}
