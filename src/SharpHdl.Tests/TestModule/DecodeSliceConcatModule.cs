using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>T2b: Slice / Concat（命令フィールド切り出し＋連結）。</summary>
public sealed class DecodeSliceConcat : Module
{
	public In Instr { get; } = In.UInt(32, "instr");
	public Out Opcode { get; } = Out.UInt(7, "opcode");
	public Out Rd { get; } = Out.UInt(5, "rd");
	public Out Word { get; } = Out.UInt(32, "word");

	public DecodeSliceConcat()
	{
		SetPorts([Instr, Opcode, Rd, Word]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			Opcode.Assign(Instr.Slice(6, 0));
			Rd.Assign(Instr.Slice(11, 7));
			Word.Assign(Instr.Slice(31, 16).Concat(Instr.Slice(15, 0)));
		});
	}
}
