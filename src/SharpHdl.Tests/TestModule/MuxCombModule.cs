using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// 条件演算子の確認用（#162）。真と偽は同じ幅。条件は 0 が偽、それ以外が真。
/// </summary>
public sealed class MuxComb : Module
{
	public In Sel { get; } = In.UInt(8, "Sel");
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public In C { get; } = In.UInt(8, "C");
	public Out Plain { get; } = Out.UInt(8, "Plain");
	public Out CondAdd { get; } = Out.UInt(8, "CondAdd");
	public Out Sum { get; } = Out.UInt(8, "Sum");
	public Out TrueAdd { get; } = Out.UInt(8, "TrueAdd");
	public Out FalseAdd { get; } = Out.UInt(8, "FalseAdd");
	public Out NotMux { get; } = Out.UInt(8, "NotMux");
	public Out DivOrOne { get; } = Out.UInt(8, "DivOrOne");

	public MuxComb()
	{
		SetPorts([Sel, A, B, C, Plain, CondAdd, Sum, TrueAdd, FalseAdd, NotMux, DivOrOne]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			Plain.Assign(Sel.Mux(A, B));
			CondAdd.Assign((A + B).Mux(C, A));
			Sum.Assign(Sel.Mux(A, B) + C);
			TrueAdd.Assign(Sel.Mux(A + B, C));
			FalseAdd.Assign(Sel.Mux(A, B + C));
			NotMux.Assign(~Sel.Mux(A, B));
			DivOrOne.Assign(Sel.Mux(A / B, Lit.Bits(8, 1)));
		});
	}
}
