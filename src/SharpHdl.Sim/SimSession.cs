using SharpHdl.Core.Model;
using SharpHdl.Sim.Interp;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim;

public class SimSession<T> where T : Module
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
		CombSettle combSettle = new();
		combSettle.Settle(Module.GetStmts().ToList(), simWorld);
	}
	public void Advance(In clk)
	{
		SeqAdvance seqAdvance = new();
		seqAdvance.Advance(Module.GetStmts().ToList(), simWorld, clk);
		Settle();
	}
	public void Advance(In clk, int n)
	{
		for(int i = 0; i < n; i++)
		{
			Advance(clk);
		}
	}
}