using SharpHdl.Core;
using SharpHdl.Emit;

namespace SharpHdl.Tests;

public class SmokeTests
{
    [Fact]
    public void Core_ExposesName()
    {
        Assert.Equal("SharpHdl.Core", SharpHdlInfo.Name);
    }

    [Fact]
    public void Emit_Placeholder_ContainsModule()
    {
        var verilog = VerilogEmitter.EmitPlaceholder("Alu");
        Assert.Contains("module Alu", verilog);
        Assert.Contains("endmodule", verilog);
    }
}
