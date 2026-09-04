using SharpHdl.Core.Model;

public sealed class DualAluTop : Module
{
	readonly Alu _alu0 = new();
	readonly Alu _alu1 = new();

	public In A0 { get; } = In.UInt(32, "A0");
	public In B0 { get; } = In.UInt(32, "B0");
	public In Op0 { get; } = In.UInt(2, "Op0");
	public Out Y0 { get; } = Out.UInt(32, "Y0");
	public In A1 { get; } = In.UInt(32, "A1");
	public In B1 { get; } = In.UInt(32, "B1");
	public In Op1 { get; } = In.UInt(2, "Op1");
	public Out Y1 { get; } = Out.UInt(32, "Y1");

	public DualAluTop()
	{
		SetPorts([A0, B0, Op0, Y0, A1, B1, Op1, Y1]);
	}

	public override void Describe()
	{
		_alu0.Describe();
		_alu1.Describe();
		Instance(_alu0, "alu0",
			(_alu0.A, A0), (_alu0.B, B0), (_alu0.Op, Op0), (_alu0.Y, Y0));
		Instance(_alu1, "alu1",
			(_alu1.A, A1), (_alu1.B, B1), (_alu1.Op, Op1), (_alu1.Y, Y1));
	}
}
