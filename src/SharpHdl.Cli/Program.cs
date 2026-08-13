using SharpHdl.Core;
using SharpHdl.Emit;

if (args is ["--version", ..] or ["version", ..])
{
    Console.WriteLine($"{SharpHdlInfo.Name} {SharpHdlInfo.Version}");
    return 0;
}

// Phase 1 仮: examples/alu のテキストビルダーを直接呼ぶ
if (args is ["emit", "alu", ..] || args is ["emit-alu", ..] || args is ["alu", ..])
{
    string? outPath = null;
    for (var i = 0; i < args.Length - 1; i++)
    {
        if (args[i] is "-o" or "--output")
            outPath = args[i + 1];
    }

    Alu.Describe(outPath);
    return 0;
}

if (args is ["emit", ..])
{
    Console.Error.WriteLine("汎用 emit は未実装です。いまは ALU のみ:");
    Console.Error.WriteLine("  dotnet run --project src/SharpHdl.Cli -- emit alu");
    Console.WriteLine(VerilogEmitter.EmitPlaceholder("Stub"));
    return 1;
}

Console.WriteLine("SharpHdl.Cli — CsharpHdl");
Console.WriteLine("Usage:");
Console.WriteLine("  dotnet run --project src/SharpHdl.Cli -- --version");
Console.WriteLine("  dotnet run --project src/SharpHdl.Cli -- emit alu [-o path]");
Console.WriteLine();
Console.WriteLine("仕様: docs/cli-spec.md / docs/guides/phase-1-alu.md");
return 0;
