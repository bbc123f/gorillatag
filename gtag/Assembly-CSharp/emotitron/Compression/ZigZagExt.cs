using System;

namespace emotitron.Compression
{
	// Token: 0x02000347 RID: 839
	public static class ZigZagExt
	{
		// Token: 0x06001818 RID: 6168 RVA: 0x00082335 File Offset: 0x00080535
		public static ulong ZigZag(this long s)
		{
			return (ulong)(s << 1 ^ s >> 63);
		}

		// Token: 0x06001819 RID: 6169 RVA: 0x0008233F File Offset: 0x0008053F
		public static long UnZigZag(this ulong u)
		{
			return (long)(u >> 1 ^ -(long)(u & 1UL));
		}

		// Token: 0x0600181A RID: 6170 RVA: 0x0008234A File Offset: 0x0008054A
		public static uint ZigZag(this int s)
		{
			return (uint)(s << 1 ^ s >> 31);
		}

		// Token: 0x0600181B RID: 6171 RVA: 0x00082354 File Offset: 0x00080554
		public static int UnZigZag(this uint u)
		{
			return (int)((ulong)(u >> 1) ^ (ulong)((long)(-(long)(u & 1U))));
		}

		// Token: 0x0600181C RID: 6172 RVA: 0x00082361 File Offset: 0x00080561
		public static ushort ZigZag(this short s)
		{
			return (ushort)((int)s << 1 ^ s >> 15);
		}

		// Token: 0x0600181D RID: 6173 RVA: 0x0008236C File Offset: 0x0008056C
		public static short UnZigZag(this ushort u)
		{
			return (short)(u >> 1 ^ (int)(-(int)((short)(u & 1))));
		}

		// Token: 0x0600181E RID: 6174 RVA: 0x00082378 File Offset: 0x00080578
		public static byte ZigZag(this sbyte s)
		{
			return (byte)((int)s << 1 ^ s >> 7);
		}

		// Token: 0x0600181F RID: 6175 RVA: 0x00082382 File Offset: 0x00080582
		public static sbyte UnZigZag(this byte u)
		{
			return (sbyte)(u >> 1 ^ (int)(-(int)((sbyte)(u & 1))));
		}
	}
}
