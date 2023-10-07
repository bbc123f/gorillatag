using System;
using System.Runtime.CompilerServices;

// Token: 0x0200005A RID: 90
public static class GTBitOps
{
	// Token: 0x0600019A RID: 410 RVA: 0x0000C2E0 File Offset: 0x0000A4E0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetValueMask(int count)
	{
		return (1 << count) - 1;
	}

	// Token: 0x0600019B RID: 411 RVA: 0x0000C2EA File Offset: 0x0000A4EA
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetClearMask(int index, int valueMask)
	{
		return ~(valueMask << index);
	}

	// Token: 0x0600019C RID: 412 RVA: 0x0000C2F3 File Offset: 0x0000A4F3
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetClearMaskByCount(int index, int count)
	{
		return ~((1 << count) - 1 << index);
	}

	// Token: 0x0600019D RID: 413 RVA: 0x0000C303 File Offset: 0x0000A503
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBits(int bits, int index, int valueMask)
	{
		return bits >> index & valueMask;
	}

	// Token: 0x0600019E RID: 414 RVA: 0x0000C30D File Offset: 0x0000A50D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBits(int bits, GTBitOps.BitWriteInfo info)
	{
		return bits >> info.index & info.valueMask;
	}

	// Token: 0x0600019F RID: 415 RVA: 0x0000C321 File Offset: 0x0000A521
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBitsByCount(int bits, int index, int count)
	{
		return bits >> index & (1 << count) - 1;
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x0000C332 File Offset: 0x0000A532
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ReadBit(int bits, int index)
	{
		return (bits >> index & 1) == 1;
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x0000C33F File Offset: 0x0000A53F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBits(ref int bits, GTBitOps.BitWriteInfo info, int value)
	{
		bits = ((bits & info.clearMask) | (value & info.valueMask) << info.index);
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x0000C35F File Offset: 0x0000A55F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBits(int bits, GTBitOps.BitWriteInfo info, int value)
	{
		GTBitOps.WriteBits(ref bits, info, value);
		return bits;
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x0000C36B File Offset: 0x0000A56B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBits(ref int bits, int index, int valueMask, int clearMask, int value)
	{
		bits = ((bits & clearMask) | (value & valueMask) << index);
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x0000C37D File Offset: 0x0000A57D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBits(int bits, int index, int valueMask, int clearMask, int value)
	{
		GTBitOps.WriteBits(ref bits, index, valueMask, clearMask, value);
		return bits;
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x0000C38C File Offset: 0x0000A58C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBitsByCount(ref int bits, int index, int count, int value)
	{
		bits = ((bits & ~((1 << count) - 1 << index)) | (value & (1 << count) - 1) << index);
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x0000C3B1 File Offset: 0x0000A5B1
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBitsByCount(int bits, int index, int count, int value)
	{
		GTBitOps.WriteBitsByCount(ref bits, index, count, value);
		return bits;
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x0000C3BE File Offset: 0x0000A5BE
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBit(ref int bits, int index, bool value)
	{
		bits = ((bits & ~(1 << index)) | (value ? 1 : 0) << index);
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x0000C3D9 File Offset: 0x0000A5D9
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBit(int bits, int index, bool value)
	{
		GTBitOps.WriteBit(ref bits, index, value);
		return bits;
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x0000C3E5 File Offset: 0x0000A5E5
	public static string ToBinaryString(int number)
	{
		return Convert.ToString(number, 2).PadLeft(32, '0');
	}

	// Token: 0x0200039A RID: 922
	public readonly struct BitWriteInfo
	{
		// Token: 0x06001AD0 RID: 6864 RVA: 0x00094AB8 File Offset: 0x00092CB8
		public BitWriteInfo(int index, int count)
		{
			this.index = index;
			this.valueMask = GTBitOps.GetValueMask(count);
			this.clearMask = GTBitOps.GetClearMask(index, this.valueMask);
		}

		// Token: 0x04001B46 RID: 6982
		public readonly int index;

		// Token: 0x04001B47 RID: 6983
		public readonly int valueMask;

		// Token: 0x04001B48 RID: 6984
		public readonly int clearMask;
	}
}
