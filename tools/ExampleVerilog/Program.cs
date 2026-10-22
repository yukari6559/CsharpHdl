using SharpHdl.Core.Model;
using SharpHdl.Emit;

// Emits every Module of the example compiled in (-p:Example=<dir>) as <prefix>-<Type>.v,
// so CI can check that the examples still build and lint their Verilog.
if (args.Length < 2)
{
	Console.Error.WriteLine("usage: dotnet run --project tools/ExampleVerilog -p:Example=<dir> -- <outDir> <prefix>");
	return 2;
}

string outDir = args[0];
string prefix = args[1];
_ = Directory.CreateDirectory(outDir);

Type[] moduleTypes = [.. typeof(Program).Assembly.GetTypes()
	.Where(t => t.IsSubclassOf(typeof(Module)) && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) is not null)
	.OrderBy(t => t.Name, StringComparer.Ordinal)];

if (moduleTypes.Length == 0)
{
	Console.Error.WriteLine($"no Module with a parameterless constructor in example '{prefix}'");
	return 1;
}

foreach (Type type in moduleTypes)
{
	Module module = (Module)Activator.CreateInstance(type)!;
	module.Describe();
	string path = Path.Combine(outDir, $"{prefix}-{type.Name}.v");
	File.WriteAllText(path, VerilogEmitter.Emitter(module, type.Name) + "\n");
	Console.WriteLine(path);
}

return 0;
