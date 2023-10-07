using System;

namespace emotitron.Compression
{
	// Token: 0x0200033C RID: 828
	public static class PrimitivePackBitsExt
	{
		// Token: 0x06001776 RID: 6006 RVA: 0x00080AC8 File Offset: 0x0007ECC8
		public static ulong WritePackedBits(this ulong buffer, uint value, ref int bitposition, int bits)
		{
			int bits2 = ((uint)bits).UsedBitCount();
			int num = value.UsedBitCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write((ulong)value, ref bitposition, num);
			return buffer;
		}

		// Token: 0x06001777 RID: 6007 RVA: 0x00080AFC File Offset: 0x0007ECFC
		public static uint WritePackedBits(this uint buffer, ushort value, ref int bitposition, int bits)
		{
			int bits2 = ((uint)bits).UsedBitCount();
			int num = value.UsedBitCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write((ulong)value, ref bitposition, num);
			return buffer;
		}

		// Token: 0x06001778 RID: 6008 RVA: 0x00080B30 File Offset: 0x0007ED30
		public static ushort WritePackedBits(this ushort buffer, byte value, ref int bitposition, int bits)
		{
			int bits2 = ((uint)bits).UsedBitCount();
			int num = value.UsedBitCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write((ulong)value, ref bitposition, num);
			return buffer;
		}

		// Token: 0x06001779 RID: 6009 RVA: 0x00080B64 File Offset: 0x0007ED64
		public static ulong ReadPackedBits(this ulong buffer, ref int bitposition, int bits)
		{
			int bits2 = bits.UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2);
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x0600177A RID: 6010 RVA: 0x00080B8C File Offset: 0x0007ED8C
		public static ulong ReadPackedBits(this uint buffer, ref int bitposition, int bits)
		{
			int bits2 = bits.UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2);
			return (ulong)buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x0600177B RID: 6011 RVA: 0x00080BB4 File Offset: 0x0007EDB4
		public static ulong ReadPackedBits(this ushort buffer, ref int bitposition, int bits)
		{
			int bits2 = bits.UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2);
			return (ulong)buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x0600177C RID: 6012 RVA: 0x00080BDC File Offset: 0x0007EDDC
		public static ulong WriteSignedPackedBits(this ulong buffer, int value, ref int bitposition, int bits)
		{
			uint value2 = (uint)(value << 1 ^ value >> 31);
			buffer = buffer.WritePackedBits(value2, ref bitposition, bits);
			return buffer;
		}

		// Token: 0x0600177D RID: 6013 RVA: 0x00080C00 File Offset: 0x0007EE00
		public static uint WriteSignedPackedBits(this uint buffer, short value, ref int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			buffer = buffer.WritePackedBits((ushort)num, ref bitposition, bits);
			return buffer;
		}

		// Token: 0x0600177E RID: 6014 RVA: 0x00080C24 File Offset: 0x0007EE24
		public static ushort WriteSignedPackedBits(this ushort buffer, sbyte value, ref int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			buffer = buffer.WritePackedBits((byte)num, ref bitposition, bits);
			return buffer;
		}

		// Token: 0x0600177F RID: 6015 RVA: 0x00080C48 File Offset: 0x0007EE48
		public static int ReadSignedPackedBits(this ulong buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06001780 RID: 6016 RVA: 0x00080C6C File Offset: 0x0007EE6C
		public static short ReadSignedPackedBits(this uint buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (short)((int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U)))));
		}

		// Token: 0x06001781 RID: 6017 RVA: 0x00080C90 File Offset: 0x0007EE90
		public static sbyte ReadSignedPackedBits(this ushort buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (sbyte)((int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U)))));
		}
	}
}
