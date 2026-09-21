using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;
using SharpHdl.Sim.Interp;
using SharpHdl.Sim.Runtime;

namespace SharpHdl.Sim;

public class SimSession<T> where T : Module
{
	public required T Module { get; init; }
	public SimWorld SimWorld { get; set; } = new();
	public void Set(Signal signal, ulong value)
	{
		SimWorld.Set(signal, value);
	}
	public ulong Get(Signal signal)
	{
		return SimWorld.Get(signal);
	}
	public void Settle()
	{
		_ = CombSettle.Settle([.. Module.GetStmts()], SimWorld);
	}
	public void Advance(In clk)
	{
		SeqAdvance.Advance([.. Module.GetStmts()], SimWorld, clk);
		Settle();
	}
	public void AdvanceWhile(In clk, int maxCycles, Func<SimSession<T>, bool> cont)
	{
		int count = 0;
		while (cont(this))
		{
			if (maxCycles <= count)
			{
				throw new SimException("Max cycles exceeded");
			}

			Advance(clk);
			count++;
		}
	}
	public void Advance(In clk, int n)
	{
		for (int i = 0; i < n; i++)
		{
			Advance(clk);
		}
	}
	public void LoadMem(Signal keySignal, ReadOnlySpan<byte> values)
	{
		_ = SimWorld.MemState.TryGetValue(keySignal, out ulong[]? currentMem);
		if (currentMem == null)
		{
			throw new SimException();
		}

		if (keySignal.Width % 8 != 0)
		{
			throw new WidthMismatchException();
		}

		if (values.Length > currentMem.Length * (keySignal.Width / 8))
		{
			throw new WidthMismatchException();
		}

		ulong[] words = new ulong[currentMem.Length];
		for (int i = 0; i < currentMem.Length; i++)
		{
			ulong word = 0;
			for (int j = 0; j < keySignal.Width / 8; j++)
			{
				ulong b;
				ulong offset = (ulong)((i * (keySignal.Width / 8)) + j);
				b = offset < (ulong)values.Length ? values[(int)offset] : (ulong)0;

				word |= b << (8 * j);
				words[i] = word;
			}
		}
		LoadMem(keySignal, words);
	}
	public void LoadMem(Signal keySignal, ReadOnlySpan<ulong> words)
	{
		_ = SimWorld.MemState.TryGetValue(keySignal, out ulong[]? value);
		if (value == null)
		{
			throw new SimException();
		}

		if (words.Length > value.Length)
		{
			throw new WidthMismatchException();
		}

		for (int i = 0; i < words.Length; i++)
		{
			value[i] = words[i] & BitUtils.MakeMaskBit(keySignal.Width);
		}
	}
	public ulong PeekMem(Signal keySignal, int index)
	{
		_ = SimWorld.MemState.TryGetValue(keySignal, out ulong[]? value);
		return value == null
			? throw new SimException()
			: index < 0 || index >= value.Length ? throw new WidthMismatchException() : value[index];
	}
	public void PokeMem(Signal keySignal, int index, ulong value)
	{
		_ = SimWorld.MemState.TryGetValue(keySignal, out ulong[]? currentValue);
		if (currentValue == null)
		{
			throw new SimException();
		}

		if (index < 0 || index >= currentValue.Length)
		{
			throw new SimException();
		}

		currentValue[index] = value & BitUtils.MakeMaskBit(keySignal.Width);
	}
}