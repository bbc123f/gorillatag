using System;

namespace emotitron.CompressionTests
{
	// Token: 0x0200034C RID: 844
	public class BasicWriter
	{
		// Token: 0x06001872 RID: 6258 RVA: 0x00083632 File Offset: 0x00081832
		public static void Reset()
		{
			BasicWriter.pos = 0;
		}

		// Token: 0x06001873 RID: 6259 RVA: 0x0008363A File Offset: 0x0008183A
		public static byte[] BasicWrite(byte[] buffer, byte value)
		{
			buffer[BasicWriter.pos] = value;
			BasicWriter.pos++;
			return buffer;
		}

		// Token: 0x06001874 RID: 6260 RVA: 0x00083651 File Offset: 0x00081851
		public static byte BasicRead(byte[] buffer)
		{
			byte result = buffer[BasicWriter.pos];
			BasicWriter.pos++;
			return result;
		}

		// Token: 0x04001968 RID: 6504
		public static int pos;
	}
}
