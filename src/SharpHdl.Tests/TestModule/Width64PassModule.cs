using SharpHdl.Core.Model;

/// <summary>T2a: 幅 64 が UInt パラメータで自然に書けることの確認用。</summary>
public sealed class Width64Pass : Module
{
	public In A { get; } = In.UInt(64, "A");
	public Out Y { get; } = Out.UInt(64, "Y");

	public Width64Pass()
	{
		SetPorts([A, Y]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			Y.Assign(A);
		});
	}
}
