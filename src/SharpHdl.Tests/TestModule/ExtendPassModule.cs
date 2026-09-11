using SharpHdl.Core.Model;

/// <summary>T2e: 8→32 の SignExtend / ZeroExtend。</summary>
public sealed class ExtendPass : Module
{
	public In ByteIn { get; } = In.UInt(8, "byte_in");
	public Out SignedOut { get; } = Out.UInt(32, "signed_out");
	public Out ZeroOut { get; } = Out.UInt(32, "zero_out");

	public ExtendPass()
	{
		SetPorts([ByteIn, SignedOut, ZeroOut]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			SignedOut.Assign(ByteIn.SignExtend(32));
			ZeroOut.Assign(ByteIn.ZeroExtend(32));
		});
	}
}
