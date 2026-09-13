using SharpHdl.Core.Model;

namespace SharpHdl.Sim.Runtime;

public class SimWorld
{
	public Dictionary<Signal, ulong> Values = new();
	public ulong Get(Signal signal)
		=> Values[signal];
	
	public void Set(Signal signal, ulong value)
	{
		ulong maxValue;
		if(signal.Width == 64)
			maxValue = ulong.MaxValue;
		else
			maxValue = (1UL << (int)signal.Width) - 1;
		if (Values.ContainsKey(signal) && value <= maxValue)
			Values[signal] = value;
		else if(!Values.ContainsKey(signal))
			throw new KeyNotFoundException();
		else
			throw new WidthMismatchException();
	}
}