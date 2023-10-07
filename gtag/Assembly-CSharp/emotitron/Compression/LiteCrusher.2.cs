using System;

namespace emotitron.Compression
{
	// Token: 0x02000340 RID: 832
	[Serializable]
	public abstract class LiteCrusher<T> : LiteCrusher where T : struct
	{
		// Token: 0x060017F7 RID: 6135
		public abstract ulong Encode(T val);

		// Token: 0x060017F8 RID: 6136
		public abstract T Decode(uint val);

		// Token: 0x060017F9 RID: 6137
		public abstract ulong WriteValue(T val, byte[] buffer, ref int bitposition);

		// Token: 0x060017FA RID: 6138
		public abstract void WriteCValue(uint val, byte[] buffer, ref int bitposition);

		// Token: 0x060017FB RID: 6139
		public abstract T ReadValue(byte[] buffer, ref int bitposition);
	}
}
