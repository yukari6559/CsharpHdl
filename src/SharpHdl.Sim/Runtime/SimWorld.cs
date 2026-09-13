using SharpHdl.Core.Model;

namespace SharpHdl.Sim.Runtime;

public class SimWorld
{
	public Dictionary<Signal, ulong> Values = new();
	public ulong Get(Signal signal)
		=> Values[signal];
	
	public void Set(Signal signal, ulong value)
	{
		ulong maxValue = BitUtils.MakeMaskBit(signal.Width);
		if (Values.ContainsKey(signal) && value <= maxValue)
			Values[signal] = value;
		else if(!Values.ContainsKey(signal))
			throw new KeyNotFoundException();
		else
			throw new WidthMismatchException();
	}
}