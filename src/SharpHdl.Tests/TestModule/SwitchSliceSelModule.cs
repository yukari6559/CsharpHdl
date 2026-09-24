using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// Switch セレクタに Slice Expr を使う例（R-C3）。
/// </summary>
public sealed class SwitchSliceSel : Module
{
	public In Instr { get; } = In.UInt(8, "Instr");
	public Out Y { get; } = Out.UInt(8, "Y");

	public SwitchSliceSel()
	{
		SetPorts([Instr, Y]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			Switch(Instr.Slice(1, 0),
				(0u, () => Y.Assign(Lit.Bits(8, 10))),
				(1u, () => Y.Assign(Lit.Bits(8, 20))),
				(2u, () => Y.Assign(Lit.Bits(8, 30))),
				(3u, () => Y.Assign(Lit.Bits(8, 40))));
		});
	}
}
