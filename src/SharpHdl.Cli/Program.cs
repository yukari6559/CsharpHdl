using SharpHdl.Core;
using SharpHdl.Emit;

if (args is ["--version", ..] or ["version", ..])
{
    Console.WriteLine($"{SharpHdlInfo.Name} {SharpHdlInfo.Version}");
    return 0;
}

if (args is ["emit", ..])
{
    Console.Error.WriteLine("emit は未実装です。docs/cli-spec.md / docs/guides/phase-1-alu.md を参照してください。");
    Console.WriteLine(VerilogEmitter.EmitPlaceholder("Stub"));
    return 1;
}

Console.WriteLine("SharpHdl.Cli — CsharpHdl");
Console.WriteLine("Usage:");
Console.WriteLine("  dotnet run --project src/SharpHdl.Cli -- --version");
Console.WriteLine("  dotnet run --project src/SharpHdl.Cli -- emit <input> -o <output.v>");
Console.WriteLine();
Console.WriteLine("仕様: docs/cli-spec.md");
return 0;
