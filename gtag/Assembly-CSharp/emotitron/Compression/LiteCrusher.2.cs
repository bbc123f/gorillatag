using System;

namespace emotitron.Compression
{
	// Token: 0x02000342 RID: 834
	[Serializable]
	public abstract class LiteCrusher<T> : LiteCrusher where T : struct
	{
		// Token: 0x06001800 RID: 6144
		public abstract ulong Encode(T val);

		// Token: 0x06001801 RID: 6145
		public abstract T Decode(uint val);

		// Token: 0x06001802 RID: 6146
		public abstract ulong WriteValue(T val, byte[] buffer, ref int bitposition);

		// Token: 0x06001803 RID: 6147
		public abstract void WriteCValue(uint val, byte[] buffer, ref int bitposition);

		// Token: 0x06001804 RID: 6148
		public abstract T ReadValue(byte[] buffer, ref int bitposition);
	}
}
