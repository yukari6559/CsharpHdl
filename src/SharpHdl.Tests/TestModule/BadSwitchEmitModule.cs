using SharpHdl.Core.Model;

public sealed class BadSwitchEmitModule : Module
{
	public In Op { get; } = In.UInt(2, "Op");
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public Out Y { get; } = Out.UInt(8, "Y");
	public Out Z { get; } = Out.UInt(8, "Z");

	public BadSwitchEmitModule()
	{
		SetPorts([Op, A, B, Y, Z]);
	}

	public void DescribeEmptyCases()
	{
		Comb(() => Switch(Op));
	}

	public void DescribeEmptyCaseBody()
	{
		Comb(() => Switch(Op,
			(0, () => { }),
			(1, () => Y.Assign(A))));
	}

	public void DescribeMultiAssignCase()
	{
		Comb(() => Switch(Op,
			(0, () =>
			{
				Y.Assign(A);
				Y.Assign(B);
			}),
			(1, () => Y.Assign(A))));
	}

	public void DescribeSignalMismatch()
	{
		Comb(() => Switch(Op,
			(0, () => Y.Assign(A)),
			(1, () => Z.Assign(B))));
	}
}
