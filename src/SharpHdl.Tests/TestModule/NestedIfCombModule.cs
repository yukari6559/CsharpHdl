using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// Comb If のネスト（R-E4）確認用。
/// </summary>
public sealed class NestedIfComb : Module
{
	public In Outer { get; } = In.UInt(1, "Outer");
	public In Inner { get; } = In.UInt(1, "Inner");
	public In A { get; } = In.UInt(8, "A");
	public In B { get; } = In.UInt(8, "B");
	public In C { get; } = In.UInt(8, "C");
	public Out Y { get; } = Out.UInt(8, "Y");

	public NestedIfComb()
	{
		SetPorts([Outer, Inner, A, B, C, Y]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			If(Outer,
				then: () => If(Inner,
					then: () => Y.Assign(A),
					@else: () => Y.Assign(B)),
				@else: () => Y.Assign(C));
		});
	}
}
