using SharpHdl.Core.Model;

namespace SharpHdl.Sim.Runtime;

public class SimWorld
{
	public Dictionary<Signal, ulong> Values = new();
	public Dictionary<Signal, Signal> alias = new();
	public Dictionary<Signal, ulong[]> memState = new();
	public ulong Get(Signal signal)
	{
		if(alias.TryGetValue(signal, out var parent))
			signal = parent;
		return Values[signal];
	}
	
	public void Set(Signal signal, ulong value)
	{
		if(alias.TryGetValue(signal, out var parent))
			signal = parent;
		ulong maxValue = BitUtils.MakeMaskBit(signal.Width);
		if (Values.ContainsKey(signal) && value <= maxValue)
		{
			Values[signal] = value;
		}
		else if(!Values.ContainsKey(signal))
			throw new KeyNotFoundException();
		else
			throw new WidthMismatchException();
	}
}