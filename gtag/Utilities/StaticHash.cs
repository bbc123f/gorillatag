using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Utilities;

public static class StaticHash
{
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(int i)
	{
		uint num = (uint)i;
		num = num + 2127912214 + (num << 12);
		num = num ^ 0xC761C23Cu ^ (num >> 19);
		num = num + 374761393 + (num << 5);
		num = (num + 3550635116u) ^ (num << 9);
		num = (uint)((int)num + -42973499) + (num << 3);
		return (int)num ^ -1252372727 ^ (int)(num >> 16);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(float f)
	{
		SingleInt32 singleInt = default(SingleInt32);
		singleInt.single = f;
		return Calculate(singleInt.int32);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(float f1, float f2)
	{
		int i = Calculate(f1);
		int i2 = Calculate(f2);
		return Combine(i, i2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(float f1, float f2, float f3)
	{
		int i = Calculate(f1);
		int i2 = Calculate(f2);
		int i3 = Calculate(f3);
		return Combine(i, i2, i3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(float f1, float f2, float f3, float f4)
	{
		int i = Calculate(f1);
		int i2 = Calculate(f2);
		int i3 = Calculate(f3);
		int i4 = Calculate(f4);
		return Combine(i, i2, i3, i4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(long l)
	{
		ulong num = (ulong)l;
		num = ~num + (num << 18);
		num ^= num >> 31;
		num *= 21;
		num ^= num >> 11;
		num += num << 6;
		num ^= num >> 22;
		return (int)num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(double d)
	{
		DoubleInt64 doubleInt = default(DoubleInt64);
		doubleInt.@double = d;
		return Calculate(doubleInt.int64);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(double d1, double d2)
	{
		int i = Calculate(d1);
		int i2 = Calculate(d2);
		return Combine(i, i2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(double d1, double d2, double d3)
	{
		int i = Calculate(d1);
		int i2 = Calculate(d2);
		int i3 = Calculate(d3);
		return Combine(i, i2, i3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(double d1, double d2, double d3, double d4)
	{
		int i = Calculate(d1);
		int i2 = Calculate(d2);
		int i3 = Calculate(d3);
		int i4 = Calculate(d4);
		return Combine(i, i2, i3, i4);
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
	public static int Calculate(DateTime dt)
	{
		return Calculate(dt.ToBinary());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(string s)
	{
		if (s == null || s.Length == 0)
		{
			return 0;
		}
		int length = s.Length;
		uint num = (uint)length;
		int num2 = length & 1;
		length >>= 1;
		int num3 = 0;
		while (length > 0)
		{
			num += s[num3];
			uint num4 = ((uint)s[num3 + 1] << 11) ^ num;
			num = (num << 16) ^ num4;
			num3 += 2;
			num += num >> 11;
			length--;
		}
		if (num2 == 1)
		{
			num += s[num3];
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

	public static int Combine(string s1, string s2)
	{
		int i = Calculate(s1);
		int i2 = Calculate(s2);
		return Combine(i, i2);
	}

	public static int Combine(string s1, string s2, string s3)
	{
		int i = Calculate(s1);
		int i2 = Calculate(s2);
		int i3 = Calculate(s3);
		return Combine(i, i2, i3);
	}

	public static int Combine(string s1, string s2, string s3, string s4)
	{
		int i = Calculate(s1);
		int i2 = Calculate(s2);
		int i3 = Calculate(s3);
		int i4 = Calculate(s4);
		return Combine(i, i2, i3, i4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(byte[] bytes)
	{
		if (bytes == null || bytes.Length == 0)
		{
			return 0;
		}
		int num = bytes.Length;
		uint num2 = (uint)num;
		int num3 = num & 1;
		num >>= 1;
		int num4 = 0;
		while (num > 0)
		{
			num2 += bytes[num4];
			uint num5 = (uint)(bytes[num4 + 1] << 11) ^ num2;
			num2 = (num2 << 16) ^ num5;
			num4 += 2;
			num2 += num2 >> 11;
			num--;
		}
		if (num3 == 1)
		{
			num2 += bytes[num4];
			num2 ^= num2 << 11;
			num2 += num2 >> 17;
		}
		num2 ^= num2 << 3;
		num2 += num2 >> 5;
		num2 ^= num2 << 4;
		num2 += num2 >> 17;
		num2 ^= num2 << 25;
		return (int)(num2 + (num2 >> 6));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(int i1, int i2)
	{
		uint num = 3735928567u;
		uint num2 = num;
		uint c = num;
		num += (uint)i1;
		num2 += (uint)i2;
		Finalize(ref num, ref num2, ref c);
		return (int)c;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(int i1, int i2, int i3)
	{
		uint num = 3735928571u;
		uint num2 = num;
		uint num3 = num;
		num += (uint)i1;
		num2 += (uint)i2;
		num3 += (uint)i3;
		Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(int i1, int i2, int i3, int i4)
	{
		uint num = 3735928575u;
		uint num2 = num;
		uint num3 = num;
		num += (uint)i1;
		num2 += (uint)i2;
		num3 += (uint)i3;
		Mix(ref num, ref num2, ref num3);
		num += (uint)i4;
		Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(params int[] values)
	{
		if (values == null)
		{
			return 224428569;
		}
		int num = values.Length;
		uint a = (uint)(-559038737 + (num << 2));
		uint b = a;
		uint c = a;
		int i;
		for (i = 0; num - i > 3; i += 3)
		{
			a += (uint)values[i];
			b += (uint)values[i + 1];
			c += (uint)values[i + 2];
			Mix(ref a, ref b, ref c);
		}
		if (num - i > 2)
		{
			c += (uint)values[i + 2];
		}
		if (num - i > 1)
		{
			b += (uint)values[i + 1];
		}
		if (num - i > 0)
		{
			a += (uint)values[i];
			Finalize(ref a, ref b, ref c);
		}
		return (int)c;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Mix(ref uint a, ref uint b, ref uint c)
	{
		a -= c;
		a ^= Rotate(c, 4);
		c += b;
		b -= a;
		b ^= Rotate(a, 6);
		a += c;
		c -= b;
		c ^= Rotate(b, 8);
		b += a;
		a -= c;
		a ^= Rotate(c, 16);
		c += b;
		b -= a;
		b ^= Rotate(a, 19);
		a += c;
		c -= b;
		c ^= Rotate(b, 4);
		b += a;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Finalize(ref uint a, ref uint b, ref uint c)
	{
		c ^= b;
		c -= Rotate(b, 14);
		a ^= c;
		a -= Rotate(c, 11);
		b ^= a;
		b -= Rotate(a, 25);
		c ^= b;
		c -= Rotate(b, 16);
		a ^= c;
		a -= Rotate(c, 4);
		b ^= a;
		b -= Rotate(a, 14);
		c ^= b;
		c -= Rotate(b, 24);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Rotate(uint x, int k)
	{
		return (x << k) | (x >> 32 - k);
	}
}
