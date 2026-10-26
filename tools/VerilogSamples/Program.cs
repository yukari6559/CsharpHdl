using SharpHdl.Core.Model;
using SharpHdl.Emit;
using SharpHdl.Tests.TestModule;

// Emits the test modules that describe valid hardware so CI can lint the Verilog
// with external tools. Modules that are expected to throw (Bad*) and Core-only shapes
// that Emit rejects by design (NestedSwitchInIf, SeqNestedIf) are not listed.
string outDir = args.Length > 0 ? args[0] : "artifacts/verilog";
_ = Directory.CreateDirectory(outDir);

(string Name, Func<Module> Build)[] samples =
[
	(nameof(Alu), () => Described(new Alu())),
	(nameof(AluTop), () => Described(new AluTop())),
	(nameof(DualAluTop), () => Described(new DualAluTop())),
	(nameof(DeepTop), () => Described(new DeepTop())),
	(nameof(Counter), () => Described(new Counter())),
	(nameof(CounterTop), () => Described(new CounterTop())),
	(nameof(SeqResetUlong), () => Described(new SeqResetUlong())),
	(nameof(IfMuxComb), () => Described(new IfMuxComb())),
	(nameof(IfThenOnlyComb), () => Described(new IfThenOnlyComb())),
	(nameof(NestedIfComb), () => Described(new NestedIfComb())),
	(nameof(CompareComb), () => Described(new CompareComb())),
	(nameof(ConstComb), () => Described(new ConstComb())),
	(nameof(DecodeSliceConcat), () => Described(new DecodeSliceConcat())),
	(nameof(SwitchSliceSel), () => Described(new SwitchSliceSel())),
	(nameof(ExtendPass), () => Described(new ExtendPass())),
	(nameof(Width64Pass), () => Described(new Width64Pass())),
	(nameof(SimpleRam), () => Described(new SimpleRam())),
	(nameof(NamedSimpleRam), () => Described(new NamedSimpleRam())),
	(nameof(ByteWriteRam), () => Described(new ByteWriteRam())),
	(nameof(RegFile2R1W), () => Described(new RegFile2R1W())),
	(nameof(XorComb), () => Described(new XorComb())),
	(nameof(NestedOpComb), () => Described(new NestedOpComb())),
	(nameof(SameWidthExtendComb), () => Described(new SameWidthExtendComb())),
	(nameof(UnaryComb), () => Described(new UnaryComb())),
	(nameof(ReduceComb), () => Described(new ReduceComb())),
	(nameof(ExtendedOperandComb), () => Described(new ExtendedOperandComb())),
	(nameof(SliceSelectComb), () => Described(new SliceSelectComb())),
	(nameof(ShiftComb), () => Described(new ShiftComb())),
	(nameof(MulComb), () => Described(new MulComb())),
	(nameof(DivComb), () => Described(new DivComb())),
	(nameof(PowComb), () => Described(new PowComb())),
	(nameof(MulFullComb), () => Described(new MulFullComb())),
	(nameof(LogicComb), () => Described(new LogicComb())),
	(nameof(LocalIfComb), () => Described(new LocalIfComb())),
	(nameof(LocalSignalTop), () => Described(new LocalSignalTop())),
	(nameof(LocalSignalModule), () =>
	{
		LocalSignalModule module = new();
		module.DescribeDatapath();
		return module;
	}),
];

foreach ((string name, Func<Module> build) in samples)
{
	string path = Path.Combine(outDir, name + ".v");
	File.WriteAllText(path, VerilogEmitter.Emitter(build(), name) + "\n");
	Console.WriteLine(path);
}

static Module Described(Module module)
{
	module.Describe();
	return module;
}
