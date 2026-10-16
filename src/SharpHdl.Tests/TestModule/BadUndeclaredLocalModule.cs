using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// SetLocal していない Wire / Reg を Describe で使う確認用（M4b）。
/// </summary>
public sealed class BadUndeclaredLocalModule : Module
{
	public In A { get; } = In.UInt(8, "A");
	public Out Y { get; } = Out.UInt(8, "Y");
	public Wire Undeclared { get; } = Wire.UInt(8, "undeclared");
	public Wire Declared { get; } = Wire.UInt(8, "declared");

	public BadUndeclaredLocalModule()
	{
		SetPorts([A, Y]);
	}

	public void DescribeUndeclaredWireLhs()
	{
		Comb(() => Undeclared.Assign(A));
		Comb(() => Y.Assign(Undeclared));
	}

	public void DescribeUndeclaredWireRhs()
	{
		SetLocal(Declared);
		Comb(() => Declared.Assign(Undeclared));
		Comb(() => Y.Assign(Declared));
	}
}
