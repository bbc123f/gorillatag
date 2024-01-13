namespace emotitron.Compression;

public static class BitCounter
{
	public static readonly int[] bitPatternToLog2 = new int[128]
	{
		0, 48, -1, -1, 31, -1, 15, 51, -1, 63,
		5, -1, -1, -1, 19, -1, 23, 28, -1, -1,
		-1, 40, 36, 46, -1, 13, -1, -1, -1, 34,
		-1, 58, -1, 60, 2, 43, 55, -1, -1, -1,
		50, 62, 4, -1, 18, 27, -1, 39, 45, -1,
		-1, 33, 57, -1, 1, 54, -1, 49, -1, 17,
		-1, -1, 32, -1, 53, -1, 16, -1, -1, 52,
		-1, -1, -1, 64, 6, 7, 8, -1, 9, -1,
		-1, -1, 20, 10, -1, -1, 24, -1, 29, -1,
		-1, 21, -1, 11, -1, -1, 41, -1, 25, 37,
		-1, 47, -1, 30, 14, -1, -1, -1, -1, 22,
		-1, -1, 35, 12, -1, -1, -1, 59, 42, -1,
		-1, 61, 3, 26, 38, 44, -1, 56
	};

	public const ulong MULTIPLICATOR = 7783611145303519083uL;

	public static int UsedBitCount(this ulong val)
	{
		val |= val >> 1;
		val |= val >> 2;
		val |= val >> 4;
		val |= val >> 8;
		val |= val >> 16;
		val |= val >> 32;
		return bitPatternToLog2[val * 7783611145303519083L >> 57];
	}

	public static int UsedBitCount(this uint val)
	{
		val |= val >> 1;
		val |= val >> 2;
		val |= val >> 4;
		val |= val >> 8;
		val |= val >> 16;
		return bitPatternToLog2[(long)val * 7783611145303519083L >>> 57];
	}

	public static int UsedBitCount(this int val)
	{
		val |= val >> 1;
		val |= val >> 2;
		val |= val >> 4;
		val |= val >> 8;
		val |= val >> 16;
		return bitPatternToLog2[val * 7783611145303519083L >>> 57];
	}

	public static int UsedBitCount(this ushort val)
	{
		uint num = val;
		num |= num >> 1;
		num |= num >> 2;
		num |= num >> 4;
		num |= num >> 8;
		return bitPatternToLog2[(long)num * 7783611145303519083L >>> 57];
	}

	public static int UsedBitCount(this byte val)
	{
		uint num = val;
		num |= num >> 1;
		num |= num >> 2;
		num |= num >> 4;
		return bitPatternToLog2[(long)num * 7783611145303519083L >>> 57];
	}

	public static int UsedByteCount(this ulong val)
	{
		if (val == 0L)
		{
			return 0;
		}
		if ((val & 0xFF00000000L) != 0L)
		{
			if ((val & 0xFF000000000000L) != 0L)
			{
				if ((val & 0xFF00000000000000uL) != 0L)
				{
					return 8;
				}
				return 7;
			}
			if ((val & 0xFF0000000000L) != 0L)
			{
				return 6;
			}
			return 5;
		}
		if ((val & 0xFF0000) != 0L)
		{
			if ((val & 0xFF000000u) != 0L)
			{
				return 4;
			}
			return 3;
		}
		if ((val & 0xFF00) != 0L)
		{
			return 2;
		}
		return 1;
	}

	public static int UsedByteCount(this uint val)
	{
		if (val == 0)
		{
			return 0;
		}
		if ((val & 0xFF0000u) != 0)
		{
			if ((val & 0xFF000000u) != 0)
			{
				return 4;
			}
			return 3;
		}
		if ((val & 0xFF00u) != 0)
		{
			return 2;
		}
		return 1;
	}

	public static int UsedByteCount(this ushort val)
	{
		if (val == 0)
		{
			return 0;
		}
		if ((val & 0xFF00u) != 0)
		{
			return 2;
		}
		return 1;
	}
}
