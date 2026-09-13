using SharpHdl.Core.Model;

namespace SharpHdl.Sim;

public class SimSession<T>
{
	public required T Module{get;init;}
	public void Set(Signal signal, ulong value)
	{
		throw new NotImplementedException();
	}
	public ulong Get(Signal signal)
	{
		throw new NotImplementedException();
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