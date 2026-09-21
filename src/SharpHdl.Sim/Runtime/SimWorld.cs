using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;

namespace SharpHdl.Sim.Runtime;

public class SimWorld
{
	public Dictionary<Signal, ulong> Values { get; set; } = [];
	public Dictionary<Signal, Signal> Alias { get; set; } = [];
	public Dictionary<Signal, ulong[]> MemState { get; set; } = [];
	public ulong Get(Signal signal)
	{
		if (Alias.TryGetValue(signal, out Signal? parent))
		{
			signal = parent;
		}

		return Values[signal];
	}

	public void Set(Signal signal, ulong value)
	{
		if (Alias.TryGetValue(signal, out Signal? parent))
		{
			signal = parent;
		}

		ulong maxValue = BitUtils.MakeMaskBit(signal.Width);
		if (Values.ContainsKey(signal) && value <= maxValue)
		{
			Values[signal] = value;
		}
		else if (!Values.ContainsKey(signal))
		{
			throw new KeyNotFoundException();
		}
		else
		{
			throw new WidthMismatchException();
		}
	}
}