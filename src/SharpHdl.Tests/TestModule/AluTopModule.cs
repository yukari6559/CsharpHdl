using SharpHdl.Core.Model;

public sealed class AluTop : Module
{
	readonly Alu _alu = new();

	public In A { get; } = In.UInt(32, "A");
	public In B { get; } = In.UInt(32, "B");
	public In Op { get; } = In.UInt(2, "Op");
	public Out Y { get; } = Out.UInt(32, "Y");

	public AluTop()
	{
		SetPorts([A, B, Op, Y]);
	}

	public override void Describe()
	{
		_alu.Describe();
		Instance(_alu, "alu",
			(_alu.A, A), (_alu.B, B), (_alu.Op, Op), (_alu.Y, Y));
	}
}
