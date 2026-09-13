namespace SharpHdl.Sim;
public static class BitUtils
{
	public static ulong MakeMaskBit(uint BitWidth)
	{
		if(BitWidth == 64)
			return ulong.MaxValue;
		return (1UL << (int)BitWidth) - 1;
	}
}