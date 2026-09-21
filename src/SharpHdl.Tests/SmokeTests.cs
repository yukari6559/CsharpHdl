using SharpHdl.Core;
using SharpHdl.Emit;
using SharpHdl.Tests.TestModule;

namespace SharpHdl.Tests;

public class SmokeTests
{
	[Fact]
	public void CoreExposesName()
	{
		Assert.Equal("SharpHdl.Core", SharpHdlInfo.Name);
	}

	[Fact]
	public void EmitRealModuleContainsModuleAndEndmodule()
	{
		TestModule1 mod = new();
		mod.Describe();
		string verilog = VerilogEmitter.Emitter(mod, nameof(TestModule1));
		Assert.Contains("module TestModule1", verilog, StringComparison.Ordinal);
		Assert.Contains("endmodule", verilog, StringComparison.Ordinal);
		Assert.Contains("assign Output = Input", verilog, StringComparison.Ordinal);
	}
}
