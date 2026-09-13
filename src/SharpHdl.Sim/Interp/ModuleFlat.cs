using SharpHdl.Core.Model;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim.Interp;

public class ModuleFlat
{
	public SimWorld Build(Module module)
	{
		SimWorld simWorld = new();
		foreach(Signal signal in module.GetPorts())
		{
			simWorld.Values.Add(signal, 0);
		}
		return simWorld;
	}
}