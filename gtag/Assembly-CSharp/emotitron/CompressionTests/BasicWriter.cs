using System;

namespace emotitron.CompressionTests
{
	// Token: 0x0200034A RID: 842
	public class BasicWriter
	{
		// Token: 0x06001869 RID: 6249 RVA: 0x0008314A File Offset: 0x0008134A
		public static void Reset()
		{
			BasicWriter.pos = 0;
		}

		// Token: 0x0600186A RID: 6250 RVA: 0x00083152 File Offset: 0x00081352
		public static byte[] BasicWrite(byte[] buffer, byte value)
		{
			buffer[BasicWriter.pos] = value;
			BasicWriter.pos++;
			return buffer;
		}

		// Token: 0x0600186B RID: 6251 RVA: 0x00083169 File Offset: 0x00081369
		public static byte BasicRead(byte[] buffer)
		{
			byte result = buffer[BasicWriter.pos];
			BasicWriter.pos++;
			return result;
		}

		// Token: 0x0400195B RID: 6491
		public static int pos;
	}
}
