using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// Slice の畳み込み、Slice の SignExtend、1 bit 信号の選択が正しい Verilog になることの確認用（#104）。
/// </summary>
public sealed class SliceSelectComb : Module
{
	public In Instr { get; } = In.UInt(32, "Instr");
	public In F { get; } = In.UInt(1, "F");
	public Out Imm { get; } = Out.UInt(64, "Imm");
	public Out Folded { get; } = Out.UInt(4, "Folded");
	public Out Top { get; } = Out.UInt(8, "Top");
	public Out Full { get; } = Out.UInt(32, "Full");
	public Out FExt { get; } = Out.UInt(8, "FExt");
	public Out FSlice { get; } = Out.UInt(1, "FSlice");

	public SliceSelectComb()
	{
		SetPorts([Instr, F, Imm, Folded, Top, Full, FExt, FSlice]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			Imm.Assign(Instr.Slice(31, 20).SignExtend(64));
			Folded.Assign(Instr.Slice(15, 8).Slice(3, 0));
			Top.Assign(Instr.Slice(31, 31).SignExtend(8));
			Full.Assign(Instr.Slice(31, 0));
			FExt.Assign(F.SignExtend(8));
			FSlice.Assign(F.Slice(0, 0));
		});
	}
}
