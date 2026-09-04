using SharpHdl.Core.Model;

public sealed class Alu : Module
{
	public In A { get; } = In.UInt(32, "A");
	public In B { get; } = In.UInt(32, "B");
	public In Op { get; } = In.UInt(2, "Op");
	public Out Y { get; } = Out.UInt(32, "Y");

	public Alu()
	{
		SetPorts([A, B, Op, Y]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			Switch(Op,
				(0u, () => Y.Assign(A + B)),
				(1u, () => Y.Assign(A - B)),
				(2u, () => Y.Assign(A & B)),
				(3u, () => Y.Assign(A | B)));
		});
	}
}
