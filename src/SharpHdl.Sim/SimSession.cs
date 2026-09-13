using SharpHdl.Core.Model;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim;

public class SimSession<T>
{
	public required T Module{get;init;}
	public SimWorld simWorld = new();
	public void Set(Signal signal, ulong value)
	{
		simWorld.Set(signal, value);
	}
	public ulong Get(Signal signal)
	{
		return simWorld.Get(signal);
	}
	public void Settle()
	{
		throw new NotImplementedException();
	}
	public void Advance(In clk)
	{
		throw new NotImplementedException();
	}
	public void Advance(In clk, int n)
	{
		throw new NotImplementedException();
	}
}