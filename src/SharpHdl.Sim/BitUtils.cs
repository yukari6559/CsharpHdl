namespace SharpHdl.Sim;

public static class BitUtils
{
	public static ulong MakeMaskBit(uint BitWidth)
	{
		return BitWidth == 64 ? ulong.MaxValue : (1UL << (int)BitWidth) - 1;
	}

	public static ulong ShiftLeft(ulong value, ulong amount, uint width)
	{
		if (amount >= width)
		{
			return 0;
		}
		return (value << (int)amount) & MakeMaskBit(width);
	}
	public static ulong ShiftRight(ulong value, ulong amount, uint width)
	{
		if (amount >= width)
		{
			return 0;
		}
		return (value >> (int)amount) & MakeMaskBit(width);
	}
}
