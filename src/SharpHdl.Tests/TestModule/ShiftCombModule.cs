using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// 論理シフト（定数量・可変量）の確認用（#9 M2b）。
/// </summary>
public sealed class ShiftComb : Module
{
	public In A { get; } = In.UInt(8, "A");
	public In Amt { get; } = In.UInt(4, "Amt");
	public In W { get; } = In.UInt(32, "W");
	public In Amt7 { get; } = In.UInt(7, "Amt7");
	public Out ShlConst { get; } = Out.UInt(8, "ShlConst");
	public Out ShrConst { get; } = Out.UInt(8, "ShrConst");
	public Out ShlVar { get; } = Out.UInt(8, "ShlVar");
	public Out ShrVar { get; } = Out.UInt(8, "ShrVar");
	public Out WShr { get; } = Out.UInt(32, "WShr");
	public Out WShlVar { get; } = Out.UInt(32, "WShlVar");
	public Out Nested { get; } = Out.UInt(8, "Nested");
	public Out ShlZero { get; } = Out.UInt(8, "ShlZero");

	public ShiftComb()
	{
		SetPorts([A, Amt, W, Amt7, ShlConst, ShrConst, ShlVar, ShrVar, WShr, WShlVar, Nested, ShlZero]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			ShlConst.Assign(A << 2);
			ShrConst.Assign(A >> 3);
			ShlVar.Assign(A << Amt);
			ShrVar.Assign(A >> Amt);
			WShr.Assign(W >> 1);
			WShlVar.Assign(W << Amt7);
			Nested.Assign((A << 1) + A);
			ShlZero.Assign(A << 0);
		});
	}
}
