namespace SharpHdl.Sim;

public static class BitUtils
{
	public static ulong MakeMaskBit(uint BitWidth)
	{
		return BitWidth == 64 ? ulong.MaxValue : (1UL << (int)BitWidth) - 1;
	}
}