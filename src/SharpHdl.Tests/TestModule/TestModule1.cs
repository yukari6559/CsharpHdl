using SharpHdl.Core.Model;

public sealed class TestModule1 : Module
{
	public In Input { get; } = In.UInt(1, "Input");
	public Out Output {get;} = Out.UInt(1, "Output");

	public TestModule1()
	{
		SetPorts([Input, Output]);
	}

	public override void Describe()
	{
		Comb(() =>
		{
			Output.Assign(Input);
		});
	}
}