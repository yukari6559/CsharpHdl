using SharpHdl.Core.Model;

/// <summary>Slice 結果幅と左辺幅が合わないときの例外確認用。</summary>
public sealed class BadSliceWidthModule : Module
{
	public In Instr { get; } = In.UInt(32, "instr");
	public Out Narrow { get; } = Out.UInt(8, "narrow");

	public BadSliceWidthModule()
	{
		SetPorts([Instr, Narrow]);
	}

	public void DescribeSliceMismatch()
	{
		Comb(() => Narrow.Assign(Instr.Slice(6, 0)));
	}
}
