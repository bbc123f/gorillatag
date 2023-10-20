using System;
using System.Runtime.CompilerServices;

// Token: 0x0200005A RID: 90
public static class GTBitOps
{
	// Token: 0x0600019A RID: 410 RVA: 0x0000C328 File Offset: 0x0000A528
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetValueMask(int count)
	{
		return (1 << count) - 1;
	}

	// Token: 0x0600019B RID: 411 RVA: 0x0000C332 File Offset: 0x0000A532
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetClearMask(int index, int valueMask)
	{
		return ~(valueMask << index);
	}

	// Token: 0x0600019C RID: 412 RVA: 0x0000C33B File Offset: 0x0000A53B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetClearMaskByCount(int index, int count)
	{
		return ~((1 << count) - 1 << index);
	}

	// Token: 0x0600019D RID: 413 RVA: 0x0000C34B File Offset: 0x0000A54B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBits(int bits, int index, int valueMask)
	{
		return bits >> index & valueMask;
	}

	// Token: 0x0600019E RID: 414 RVA: 0x0000C355 File Offset: 0x0000A555
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBits(int bits, GTBitOps.BitWriteInfo info)
	{
		return bits >> info.index & info.valueMask;
	}

	// Token: 0x0600019F RID: 415 RVA: 0x0000C369 File Offset: 0x0000A569
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBitsByCount(int bits, int index, int count)
	{
		return bits >> index & (1 << count) - 1;
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x0000C37A File Offset: 0x0000A57A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ReadBit(int bits, int index)
	{
		return (bits >> index & 1) == 1;
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x0000C387 File Offset: 0x0000A587
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBits(ref int bits, GTBitOps.BitWriteInfo info, int value)
	{
		bits = ((bits & info.clearMask) | (value & info.valueMask) << info.index);
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x0000C3A7 File Offset: 0x0000A5A7
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBits(int bits, GTBitOps.BitWriteInfo info, int value)
	{
		GTBitOps.WriteBits(ref bits, info, value);
		return bits;
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x0000C3B3 File Offset: 0x0000A5B3
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBits(ref int bits, int index, int valueMask, int clearMask, int value)
	{
		bits = ((bits & clearMask) | (value & valueMask) << index);
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x0000C3C5 File Offset: 0x0000A5C5
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBits(int bits, int index, int valueMask, int clearMask, int value)
	{
		GTBitOps.WriteBits(ref bits, index, valueMask, clearMask, value);
		return bits;
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x0000C3D4 File Offset: 0x0000A5D4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBitsByCount(ref int bits, int index, int count, int value)
	{
		bits = ((bits & ~((1 << count) - 1 << index)) | (value & (1 << count) - 1) << index);
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x0000C3F9 File Offset: 0x0000A5F9
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBitsByCount(int bits, int index, int count, int value)
	{
		GTBitOps.WriteBitsByCount(ref bits, index, count, value);
		return bits;
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x0000C406 File Offset: 0x0000A606
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBit(ref int bits, int index, bool value)
	{
		bits = ((bits & ~(1 << index)) | (value ? 1 : 0) << index);
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x0000C421 File Offset: 0x0000A621
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBit(int bits, int index, bool value)
	{
		GTBitOps.WriteBit(ref bits, index, value);
		return bits;
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x0000C42D File Offset: 0x0000A62D
	public static string ToBinaryString(int number)
	{
		return Convert.ToString(number, 2).PadLeft(32, '0');
	}

	// Token: 0x0200039C RID: 924
	public readonly struct BitWriteInfo
	{
		// Token: 0x06001AD9 RID: 6873 RVA: 0x00094FA0 File Offset: 0x000931A0
		public BitWriteInfo(int index, int count)
		{
			this.index = index;
			this.valueMask = GTBitOps.GetValueMask(count);
			this.clearMask = GTBitOps.GetClearMask(index, this.valueMask);
		}

		// Token: 0x04001B53 RID: 6995
		public readonly int index;

		// Token: 0x04001B54 RID: 6996
		public readonly int valueMask;

		// Token: 0x04001B55 RID: 6997
		public readonly int clearMask;
	}
}
