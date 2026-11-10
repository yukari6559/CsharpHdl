using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// 初期値付き ROM の確認用（SHDL-10）。表が深さより短い ROM と、配列名を渡した ROM。読み出しは同期。
/// </summary>
public sealed class SmallRom : Module
{
	public In Clk { get; } = In.UInt(1, "clk");
	public In Phase { get; } = In.UInt(2, "Phase");
	public Out Sine { get; } = Out.UInt(8, "Sine");
	public Out Tri { get; } = Out.UInt(8, "Tri");

	public SmallRom()
	{
		SetPorts([Clk, Phase, Sine, Tri]);
	}

	public override void Describe()
	{
		Rom(Clk, 4, 8, Phase, [10, 20, 30], Sine);
		Rom(Clk, 4, 8, Phase, [1, 2, 3, 4], Tri, "wave_table");
	}
}
