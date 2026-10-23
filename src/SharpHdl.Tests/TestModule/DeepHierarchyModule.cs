using SharpHdl.Core.Model;

namespace SharpHdl.Tests.TestModule;

/// <summary>
/// 3 段の階層の Emit の確認用（#146）。DeepTop → DeepMid → DeepLeaf で、DeepTop も DeepLeaf を直接使う。
/// </summary>
public sealed class DeepLeaf : Module
{
	public In A { get; } = In.UInt(8, "A");
	public Out Y { get; } = Out.UInt(8, "Y");

	public DeepLeaf()
	{
		SetPorts([A, Y]);
	}

	public override void Describe()
	{
		Comb(() => Y.Assign(~A));
	}
}

public sealed class DeepMid : Module
{
	private readonly DeepLeaf _leaf = new();

	public In A { get; } = In.UInt(8, "A");
	public Out Y { get; } = Out.UInt(8, "Y");

	public DeepMid()
	{
		SetPorts([A, Y]);
	}

	public override void Describe()
	{
		_leaf.Describe();
		Instance(_leaf, "leaf", (_leaf.A, A), (_leaf.Y, Y));
	}
}

public sealed class DeepTop : Module
{
	private readonly DeepMid _mid = new();
	private readonly DeepLeaf _leaf = new();

	public In A { get; } = In.UInt(8, "A");
	public Out Y { get; } = Out.UInt(8, "Y");
	public Out Z { get; } = Out.UInt(8, "Z");

	public DeepTop()
	{
		SetPorts([A, Y, Z]);
	}

	public override void Describe()
	{
		_mid.Describe();
		_leaf.Describe();
		Instance(_mid, "mid", (_mid.A, A), (_mid.Y, Y));
		Instance(_leaf, "leaf", (_leaf.A, A), (_leaf.Y, Z));
	}
}
