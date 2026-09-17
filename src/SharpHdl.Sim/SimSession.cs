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
	public void LoadMem(Signal keySignal, ReadOnlySpan<byte> values)
	{
		simWorld.memState.TryGetValue(keySignal, out var currentMem);
		if(currentMem == null)
			throw new NullReferenceException();
		if(keySignal.Width % 8 != 0)
			throw new WidthMismatchException();
		if(values.Length > currentMem.Length * (keySignal.Width / 8))
			throw new WidthMismatchException();
		ulong[] words = new ulong[currentMem.Length];
		for(int i = 0; i < currentMem.Length; i++)
		{
			ulong word = 0;
			for(int j = 0; j < keySignal.Width / 8; j++)
			{
				ulong b = 0;
				ulong offset = (ulong)(i * (keySignal.Width / 8) + j);
				if(offset < (ulong)values.Length)
					b = values[(int)offset];
				else
					b = 0;
				word |= ((ulong)b) << (8 * j);
				words[i] = word;
			}
		}
		LoadMem(keySignal, words);
	}
	public void LoadMem(Signal keySignal, ReadOnlySpan<ulong> words)
	{
		simWorld.memState.TryGetValue(keySignal, out var value);
		if(value == null)
			throw new NullReferenceException();
		if(words.Length > value.Length)
			throw new WidthMismatchException();
		
		for(int i = 0; i < words.Length; i++)
		{
			value[i] = words[i] & BitUtils.MakeMaskBit(keySignal.Width);
		}
	}
	public ulong PeekMem(Signal keySignal, int index)
	{
		simWorld.memState.TryGetValue(keySignal, out var value);
		if(value == null)
			throw new NullReferenceException();
		if(index < 0 || index >= value.Length)
			throw new WidthMismatchException();
		return value[index];
	}
	public void PokeMem(Signal keySignal, int index, ulong value)
	{
		simWorld.memState.TryGetValue(keySignal, out var currentValue);
		if(currentValue == null)
			throw new NullReferenceException();
		if(index < 0 || index >= currentValue.Length)
			throw new WidthMismatchException();
		currentValue[index] = value & BitUtils.MakeMaskBit(keySignal.Width);
	}
}