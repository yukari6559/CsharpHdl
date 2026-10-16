using SharpHdl.Core.Model;
using SharpHdl.Core.Validate;
using SharpHdl.Sim.Interp;

namespace SharpHdl.Sim;

public static class ModuleSimExtensions
{
	public static SimSession<T> Run<T>(this T module) where T : Module
	{
		SimSession<T> simSession = new() { Module = module };
		module.Describe();
		CheckMultiDrive.Check([.. module.GetStmts()]);
		CheckUndeclaredLocals.Check(module);
		simSession.SimWorld = ModuleFlat.Build(module);
		simSession.Settle();
		return simSession;
	}
}