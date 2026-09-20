using SharpHdl.Core;
using SharpHdl.Emit;

namespace SharpHdl.Tests;

public class SmokeTests
{
	[Fact]
	public void CoreExposesName()
	{
		Assert.Equal("SharpHdl.Core", SharpHdlInfo.Name);
	}

	[Fact]
	public void EmitPlaceholderContainsModule()
	{
		string verilog = VerilogEmitter.EmitPlaceholder("Alu");
		Assert.Contains("module Alu", verilog);
		Assert.Contains("endmodule", verilog);
	}
}
