using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Token: 0x02000229 RID: 553
public static class StaticHash
{
	// Token: 0x06000DA9 RID: 3497 RVA: 0x0004FEFC File Offset: 0x0004E0FC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(int i)
	{
		uint num = (uint)(i + 2127912214 + (i << 12));
		num = (num ^ 3345072700U ^ num >> 19);
		num = num + 374761393U + (num << 5);
		num = (num + 3550635116U ^ num << 9);
		num = num + 4251993797U + (num << 3);
		return (int)(num ^ 3042594569U ^ num >> 16);
	}

	// Token: 0x06000DAA RID: 3498 RVA: 0x0004FF58 File Offset: 0x0004E158
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(float f)
	{
		return StaticHash.Calculate(new StaticHash.SingleInt32
		{
			single = f
		}.int32);
	}

	// Token: 0x06000DAB RID: 3499 RVA: 0x0004FF80 File Offset: 0x0004E180
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(float f1, float f2)
	{
		int i = StaticHash.Calculate(f1);
		int i2 = StaticHash.Calculate(f2);
		return StaticHash.Combine(i, i2);
	}

	// Token: 0x06000DAC RID: 3500 RVA: 0x0004FFA0 File Offset: 0x0004E1A0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(float f1, float f2, float f3)
	{
		int i = StaticHash.Calculate(f1);
		int i2 = StaticHash.Calculate(f2);
		int i3 = StaticHash.Calculate(f3);
		return StaticHash.Combine(i, i2, i3);
	}

	// Token: 0x06000DAD RID: 3501 RVA: 0x0004FFC8 File Offset: 0x0004E1C8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(float f1, float f2, float f3, float f4)
	{
		int i = StaticHash.Calculate(f1);
		int i2 = StaticHash.Calculate(f2);
		int i3 = StaticHash.Calculate(f3);
		int i4 = StaticHash.Calculate(f4);
		return StaticHash.Combine(i, i2, i3, i4);
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x0004FFF8 File Offset: 0x0004E1F8
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

	// Token: 0x06000DAF RID: 3503 RVA: 0x00050034 File Offset: 0x0004E234
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(long l1, long l2)
	{
		int i = StaticHash.Calculate(l1);
		int i2 = StaticHash.Calculate(l2);
		return StaticHash.Combine(i, i2);
	}

	// Token: 0x06000DB0 RID: 3504 RVA: 0x00050054 File Offset: 0x0004E254
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(long l1, long l2, long l3)
	{
		int i = StaticHash.Calculate(l1);
		int i2 = StaticHash.Calculate(l2);
		int i3 = StaticHash.Calculate(l3);
		return StaticHash.Combine(i, i2, i3);
	}

	// Token: 0x06000DB1 RID: 3505 RVA: 0x0005007C File Offset: 0x0004E27C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(long l1, long l2, long l3, long l4)
	{
		int i = StaticHash.Calculate(l1);
		int i2 = StaticHash.Calculate(l2);
		int i3 = StaticHash.Calculate(l3);
		int i4 = StaticHash.Calculate(l4);
		return StaticHash.Combine(i, i2, i3, i4);
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x000500AC File Offset: 0x0004E2AC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(double d)
	{
		return StaticHash.Calculate(new StaticHash.DoubleInt64
		{
			@double = d
		}.int64);
	}

	// Token: 0x06000DB3 RID: 3507 RVA: 0x000500D4 File Offset: 0x0004E2D4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(double d1, double d2)
	{
		int i = StaticHash.Calculate(d1);
		int i2 = StaticHash.Calculate(d2);
		return StaticHash.Combine(i, i2);
	}

	// Token: 0x06000DB4 RID: 3508 RVA: 0x000500F4 File Offset: 0x0004E2F4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(double d1, double d2, double d3)
	{
		int i = StaticHash.Calculate(d1);
		int i2 = StaticHash.Calculate(d2);
		int i3 = StaticHash.Calculate(d3);
		return StaticHash.Combine(i, i2, i3);
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x0005011C File Offset: 0x0004E31C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(double d1, double d2, double d3, double d4)
	{
		int i = StaticHash.Calculate(d1);
		int i2 = StaticHash.Calculate(d2);
		int i3 = StaticHash.Calculate(d3);
		int i4 = StaticHash.Calculate(d4);
		return StaticHash.Combine(i, i2, i3, i4);
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x0005014C File Offset: 0x0004E34C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(bool b)
	{
		if (!b)
		{
			return 1800329511;
		}
		return -1266253386;
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x0005015C File Offset: 0x0004E35C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(bool b1, bool b2)
	{
		int i = StaticHash.Calculate(b1);
		int i2 = StaticHash.Calculate(b2);
		return StaticHash.Combine(i, i2);
	}

	// Token: 0x06000DB8 RID: 3512 RVA: 0x0005017C File Offset: 0x0004E37C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(bool b1, bool b2, bool b3)
	{
		int i = StaticHash.Calculate(b1);
		int i2 = StaticHash.Calculate(b2);
		int i3 = StaticHash.Calculate(b3);
		return StaticHash.Combine(i, i2, i3);
	}

	// Token: 0x06000DB9 RID: 3513 RVA: 0x000501A4 File Offset: 0x0004E3A4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(bool b1, bool b2, bool b3, bool b4)
	{
		int i = StaticHash.Calculate(b1);
		int i2 = StaticHash.Calculate(b2);
		int i3 = StaticHash.Calculate(b3);
		int i4 = StaticHash.Calculate(b4);
		return StaticHash.Combine(i, i2, i3, i4);
	}

	// Token: 0x06000DBA RID: 3514 RVA: 0x000501D4 File Offset: 0x0004E3D4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Calculate(DateTime dt)
	{
		return StaticHash.Calculate(dt.ToBinary());
	}

	// Token: 0x06000DBB RID: 3515 RVA: 0x000501E4 File Offset: 0x0004E3E4
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
			num = (num << 16 ^ num4);
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

	// Token: 0x06000DBC RID: 3516 RVA: 0x0005028C File Offset: 0x0004E48C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(string s1, string s2)
	{
		int i = StaticHash.Calculate(s1);
		int i2 = StaticHash.Calculate(s2);
		return StaticHash.Combine(i, i2);
	}

	// Token: 0x06000DBD RID: 3517 RVA: 0x000502AC File Offset: 0x0004E4AC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(string s1, string s2, string s3)
	{
		int i = StaticHash.Calculate(s1);
		int i2 = StaticHash.Calculate(s2);
		int i3 = StaticHash.Calculate(s3);
		return StaticHash.Combine(i, i2, i3);
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x000502D4 File Offset: 0x0004E4D4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(string s1, string s2, string s3, string s4)
	{
		int i = StaticHash.Calculate(s1);
		int i2 = StaticHash.Calculate(s2);
		int i3 = StaticHash.Calculate(s3);
		int i4 = StaticHash.Calculate(s4);
		return StaticHash.Combine(i, i2, i3, i4);
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x00050304 File Offset: 0x0004E504
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
			uint num4 = (uint)((int)bytes[num3 + 1] << 11 ^ (int)num);
			num = (num << 16 ^ num4);
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

	// Token: 0x06000DC0 RID: 3520 RVA: 0x00050398 File Offset: 0x0004E598
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(int i1, int i2)
	{
		uint num = 3735928567U;
		uint num2 = num;
		uint result = num;
		num += (uint)i1;
		num2 += (uint)i2;
		StaticHash.Finalize(ref num, ref num2, ref result);
		return (int)result;
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x000503C4 File Offset: 0x0004E5C4
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

	// Token: 0x06000DC2 RID: 3522 RVA: 0x000503F4 File Offset: 0x0004E5F4
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

	// Token: 0x06000DC3 RID: 3523 RVA: 0x00050434 File Offset: 0x0004E634
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

	// Token: 0x06000DC4 RID: 3524 RVA: 0x000504D0 File Offset: 0x0004E6D0
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

	// Token: 0x06000DC5 RID: 3525 RVA: 0x00050584 File Offset: 0x0004E784
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

	// Token: 0x06000DC6 RID: 3526 RVA: 0x00050623 File Offset: 0x0004E823
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Rotate(uint x, int k)
	{
		return x << k | x >> 32 - k;
	}

	// Token: 0x0200047D RID: 1149
	[StructLayout(LayoutKind.Explicit)]
	private struct SingleInt32
	{
		// Token: 0x04001EB6 RID: 7862
		[FieldOffset(0)]
		public float single;

		// Token: 0x04001EB7 RID: 7863
		[FieldOffset(0)]
		public int int32;
	}

	// Token: 0x0200047E RID: 1150
	[StructLayout(LayoutKind.Explicit)]
	private struct DoubleInt64
	{
		// Token: 0x04001EB8 RID: 7864
		[FieldOffset(0)]
		public double @double;

		// Token: 0x04001EB9 RID: 7865
		[FieldOffset(0)]
		public long int64;
	}
}
