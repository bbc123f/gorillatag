using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static class StaticHash
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(int i)
	{
		uint num = (uint)(i + 2127912214 + (i << 12));
		num = num ^ 3345072700U ^ (num >> 19);
		num = num + 374761393U + (num << 5);
		num = (num + 3550635116U) ^ (num << 9);
		num = num + 4251993797U + (num << 3);
		return (int)(num ^ 3042594569U ^ (num >> 16));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(float f)
	{
		return StaticHash.Calculate(new StaticHash.SingleInt32
		{
			single = f
		}.int32);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(float f1, float f2)
	{
		int num = StaticHash.Calculate(f1);
		int num2 = StaticHash.Calculate(f2);
		return StaticHash.Combine(num, num2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(float f1, float f2, float f3)
	{
		int num = StaticHash.Calculate(f1);
		int num2 = StaticHash.Calculate(f2);
		int num3 = StaticHash.Calculate(f3);
		return StaticHash.Combine(num, num2, num3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(float f1, float f2, float f3, float f4)
	{
		int num = StaticHash.Calculate(f1);
		int num2 = StaticHash.Calculate(f2);
		int num3 = StaticHash.Calculate(f3);
		int num4 = StaticHash.Calculate(f4);
		return StaticHash.Combine(num, num2, num3, num4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(long l)
	{
		ulong num = (ulong)(~(ulong)l + (l << 18));
		num ^= num >> 31;
		num *= 21UL;
		num ^= num >> 11;
		num += num << 6;
		num ^= num >> 22;
		return (int)num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(long l1, long l2)
	{
		int num = StaticHash.Calculate(l1);
		int num2 = StaticHash.Calculate(l2);
		return StaticHash.Combine(num, num2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(long l1, long l2, long l3)
	{
		int num = StaticHash.Calculate(l1);
		int num2 = StaticHash.Calculate(l2);
		int num3 = StaticHash.Calculate(l3);
		return StaticHash.Combine(num, num2, num3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(long l1, long l2, long l3, long l4)
	{
		int num = StaticHash.Calculate(l1);
		int num2 = StaticHash.Calculate(l2);
		int num3 = StaticHash.Calculate(l3);
		int num4 = StaticHash.Calculate(l4);
		return StaticHash.Combine(num, num2, num3, num4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(double d)
	{
		return StaticHash.Calculate(new StaticHash.DoubleInt64
		{
			@double = d
		}.int64);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(double d1, double d2)
	{
		int num = StaticHash.Calculate(d1);
		int num2 = StaticHash.Calculate(d2);
		return StaticHash.Combine(num, num2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(double d1, double d2, double d3)
	{
		int num = StaticHash.Calculate(d1);
		int num2 = StaticHash.Calculate(d2);
		int num3 = StaticHash.Calculate(d3);
		return StaticHash.Combine(num, num2, num3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(double d1, double d2, double d3, double d4)
	{
		int num = StaticHash.Calculate(d1);
		int num2 = StaticHash.Calculate(d2);
		int num3 = StaticHash.Calculate(d3);
		int num4 = StaticHash.Calculate(d4);
		return StaticHash.Combine(num, num2, num3, num4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(bool b)
	{
		if (!b)
		{
			return 1800329511;
		}
		return -1266253386;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(bool b1, bool b2)
	{
		int num = StaticHash.Calculate(b1);
		int num2 = StaticHash.Calculate(b2);
		return StaticHash.Combine(num, num2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(bool b1, bool b2, bool b3)
	{
		int num = StaticHash.Calculate(b1);
		int num2 = StaticHash.Calculate(b2);
		int num3 = StaticHash.Calculate(b3);
		return StaticHash.Combine(num, num2, num3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(bool b1, bool b2, bool b3, bool b4)
	{
		int num = StaticHash.Calculate(b1);
		int num2 = StaticHash.Calculate(b2);
		int num3 = StaticHash.Calculate(b3);
		int num4 = StaticHash.Calculate(b4);
		return StaticHash.Combine(num, num2, num3, num4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(DateTime dt)
	{
		return StaticHash.Calculate(dt.ToBinary());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(string s)
	{
		if (s == null || s.Length == 0)
		{
			return 0;
		}
		int i = s.Length;
		uint num = (uint)i;
		int num2 = i & 1;
		i >>= 1;
		int num3 = 0;
		while (i > 0)
		{
			num += (uint)s[num3];
			uint num4 = (uint)((uint)s[num3 + 1] << 11) ^ num;
			num = (num << 16) ^ num4;
			num3 += 2;
			num += num >> 11;
			i--;
		}
		if (num2 == 1)
		{
			num += (uint)s[num3];
			num ^= num << 11;
			num += num >> 17;
		}
		num ^= num << 3;
		num += num >> 5;
		num ^= num << 4;
		num += num >> 17;
		num ^= num << 25;
		return (int)(num + (num >> 6));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(string s1, string s2)
	{
		int num = StaticHash.Calculate(s1);
		int num2 = StaticHash.Calculate(s2);
		return StaticHash.Combine(num, num2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(string s1, string s2, string s3)
	{
		int num = StaticHash.Calculate(s1);
		int num2 = StaticHash.Calculate(s2);
		int num3 = StaticHash.Calculate(s3);
		return StaticHash.Combine(num, num2, num3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(string s1, string s2, string s3, string s4)
	{
		int num = StaticHash.Calculate(s1);
		int num2 = StaticHash.Calculate(s2);
		int num3 = StaticHash.Calculate(s3);
		int num4 = StaticHash.Calculate(s4);
		return StaticHash.Combine(num, num2, num3, num4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(byte[] bytes)
	{
		if (bytes == null || bytes.Length == 0)
		{
			return 0;
		}
		int i = bytes.Length;
		uint num = (uint)i;
		int num2 = i & 1;
		i >>= 1;
		int num3 = 0;
		while (i > 0)
		{
			num += (uint)bytes[num3];
			uint num4 = (uint)(((int)bytes[num3 + 1] << 11) ^ (int)num);
			num = (num << 16) ^ num4;
			num3 += 2;
			num += num >> 11;
			i--;
		}
		if (num2 == 1)
		{
			num += (uint)bytes[num3];
			num ^= num << 11;
			num += num >> 17;
		}
		num ^= num << 3;
		num += num >> 5;
		num ^= num << 4;
		num += num >> 17;
		num ^= num << 25;
		return (int)(num + (num >> 6));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(int i1, int i2)
	{
		uint num = 3735928567U;
		uint num2 = num;
		uint num3 = num;
		num += (uint)i1;
		num2 += (uint)i2;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(int i1, int i2, int i3)
	{
		uint num = 3735928571U;
		uint num2 = num;
		uint num3 = num;
		num += (uint)i1;
		num2 += (uint)i2;
		num3 += (uint)i3;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(int i1, int i2, int i3, int i4)
	{
		uint num = 3735928575U;
		uint num2 = num;
		uint num3 = num;
		num += (uint)i1;
		num2 += (uint)i2;
		num3 += (uint)i3;
		StaticHash.Mix(ref num, ref num2, ref num3);
		num += (uint)i4;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(int[] values)
	{
		if (values == null || values.Length == 0)
		{
			return 224428569;
		}
		int num = values.Length;
		uint num2 = (uint)(-559038737 + (num << 2));
		uint num3 = num2;
		uint num4 = num2;
		int num5 = 0;
		while (num - num5 > 3)
		{
			num2 += (uint)values[num5];
			num3 += (uint)values[num5 + 1];
			num4 += (uint)values[num5 + 2];
			StaticHash.Mix(ref num2, ref num3, ref num4);
			num5 += 3;
		}
		if (num - num5 > 2)
		{
			num4 += (uint)values[num5 + 2];
		}
		if (num - num5 > 1)
		{
			num3 += (uint)values[num5 + 1];
		}
		if (num - num5 > 0)
		{
			num2 += (uint)values[num5];
			StaticHash.Finalize(ref num2, ref num3, ref num4);
		}
		return (int)num4;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Mix(ref uint a, ref uint b, ref uint c)
	{
		a -= c;
		a ^= StaticHash.Rotate(c, 4);
		c += b;
		b -= a;
		b ^= StaticHash.Rotate(a, 6);
		a += c;
		c -= b;
		c ^= StaticHash.Rotate(b, 8);
		b += a;
		a -= c;
		a ^= StaticHash.Rotate(c, 16);
		c += b;
		b -= a;
		b ^= StaticHash.Rotate(a, 19);
		a += c;
		c -= b;
		c ^= StaticHash.Rotate(b, 4);
		b += a;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Finalize(ref uint a, ref uint b, ref uint c)
	{
		c ^= b;
		c -= StaticHash.Rotate(b, 14);
		a ^= c;
		a -= StaticHash.Rotate(c, 11);
		b ^= a;
		b -= StaticHash.Rotate(a, 25);
		c ^= b;
		c -= StaticHash.Rotate(b, 16);
		a ^= c;
		a -= StaticHash.Rotate(c, 4);
		b ^= a;
		b -= StaticHash.Rotate(a, 14);
		c ^= b;
		c -= StaticHash.Rotate(b, 24);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Rotate(uint x, int k)
	{
		return (x << k) | (x >> 32 - k);
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct SingleInt32
	{
		[FieldOffset(0)]
		public float single;

		[FieldOffset(0)]
		public int int32;
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct DoubleInt64
	{
		[FieldOffset(0)]
		public double @double;

		[FieldOffset(0)]
		public long int64;
	}
}
