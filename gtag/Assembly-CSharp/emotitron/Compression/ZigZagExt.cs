using System;

namespace emotitron.Compression
{
	// Token: 0x02000345 RID: 837
	public static class ZigZagExt
	{
		// Token: 0x0600180F RID: 6159 RVA: 0x00081E4D File Offset: 0x0008004D
		public static ulong ZigZag(this long s)
		{
			return (ulong)(s << 1 ^ s >> 63);
		}

		// Token: 0x06001810 RID: 6160 RVA: 0x00081E57 File Offset: 0x00080057
		public static long UnZigZag(this ulong u)
		{
			return (long)(u >> 1 ^ -(long)(u & 1UL));
		}

		// Token: 0x06001811 RID: 6161 RVA: 0x00081E62 File Offset: 0x00080062
		public static uint ZigZag(this int s)
		{
			return (uint)(s << 1 ^ s >> 31);
		}

		// Token: 0x06001812 RID: 6162 RVA: 0x00081E6C File Offset: 0x0008006C
		public static int UnZigZag(this uint u)
		{
			return (int)((ulong)(u >> 1) ^ (ulong)((long)(-(long)(u & 1U))));
		}

		// Token: 0x06001813 RID: 6163 RVA: 0x00081E79 File Offset: 0x00080079
		public static ushort ZigZag(this short s)
		{
			return (ushort)((int)s << 1 ^ s >> 15);
		}

		// Token: 0x06001814 RID: 6164 RVA: 0x00081E84 File Offset: 0x00080084
		public static short UnZigZag(this ushort u)
		{
			return (short)(u >> 1 ^ (int)(-(int)((short)(u & 1))));
		}

		// Token: 0x06001815 RID: 6165 RVA: 0x00081E90 File Offset: 0x00080090
		public static byte ZigZag(this sbyte s)
		{
			return (byte)((int)s << 1 ^ s >> 7);
		}

		// Token: 0x06001816 RID: 6166 RVA: 0x00081E9A File Offset: 0x0008009A
		public static sbyte UnZigZag(this byte u)
		{
			return (sbyte)(u >> 1 ^ (int)(-(int)((sbyte)(u & 1))));
		}
	}
}
