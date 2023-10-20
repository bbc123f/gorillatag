using System;

namespace emotitron.Compression
{
	// Token: 0x02000339 RID: 825
	public static class ArraySerializeExt
	{
		// Token: 0x06001700 RID: 5888 RVA: 0x0007F934 File Offset: 0x0007DB34
		public static void Zero(this byte[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x06001701 RID: 5889 RVA: 0x0007F954 File Offset: 0x0007DB54
		public static void Zero(this byte[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x06001702 RID: 5890 RVA: 0x0007F978 File Offset: 0x0007DB78
		public static void Zero(this byte[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x06001703 RID: 5891 RVA: 0x0007F99C File Offset: 0x0007DB9C
		public static void Zero(this ushort[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x06001704 RID: 5892 RVA: 0x0007F9BC File Offset: 0x0007DBBC
		public static void Zero(this ushort[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x06001705 RID: 5893 RVA: 0x0007F9E0 File Offset: 0x0007DBE0
		public static void Zero(this ushort[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x06001706 RID: 5894 RVA: 0x0007FA04 File Offset: 0x0007DC04
		public static void Zero(this uint[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x06001707 RID: 5895 RVA: 0x0007FA24 File Offset: 0x0007DC24
		public static void Zero(this uint[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x06001708 RID: 5896 RVA: 0x0007FA48 File Offset: 0x0007DC48
		public static void Zero(this uint[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x06001709 RID: 5897 RVA: 0x0007FA6C File Offset: 0x0007DC6C
		public static void Zero(this ulong[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x0600170A RID: 5898 RVA: 0x0007FA8C File Offset: 0x0007DC8C
		public static void Zero(this ulong[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x0600170B RID: 5899 RVA: 0x0007FAB0 File Offset: 0x0007DCB0
		public static void Zero(this ulong[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x0600170C RID: 5900 RVA: 0x0007FAD4 File Offset: 0x0007DCD4
		public static void WriteSigned(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x0007FAF4 File Offset: 0x0007DCF4
		public static void WriteSigned(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600170E RID: 5902 RVA: 0x0007FB14 File Offset: 0x0007DD14
		public static void WriteSigned(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600170F RID: 5903 RVA: 0x0007FB34 File Offset: 0x0007DD34
		public static void WriteSigned(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong value2 = (ulong)(value << 1 ^ value >> 63);
			buffer.Write(value2, ref bitposition, bits);
		}

		// Token: 0x06001710 RID: 5904 RVA: 0x0007FB54 File Offset: 0x0007DD54
		public static void WriteSigned(this uint[] buffer, long value, ref int bitposition, int bits)
		{
			ulong value2 = (ulong)(value << 1 ^ value >> 63);
			buffer.Write(value2, ref bitposition, bits);
		}

		// Token: 0x06001711 RID: 5905 RVA: 0x0007FB74 File Offset: 0x0007DD74
		public static void WriteSigned(this ulong[] buffer, long value, ref int bitposition, int bits)
		{
			ulong value2 = (ulong)(value << 1 ^ value >> 63);
			buffer.Write(value2, ref bitposition, bits);
		}

		// Token: 0x06001712 RID: 5906 RVA: 0x0007FB94 File Offset: 0x0007DD94
		public static int ReadSigned(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06001713 RID: 5907 RVA: 0x0007FBB8 File Offset: 0x0007DDB8
		public static int ReadSigned(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06001714 RID: 5908 RVA: 0x0007FBDC File Offset: 0x0007DDDC
		public static int ReadSigned(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06001715 RID: 5909 RVA: 0x0007FC00 File Offset: 0x0007DE00
		public static long ReadSigned64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)(num >> 1 ^ -(long)(num & 1UL));
		}

		// Token: 0x06001716 RID: 5910 RVA: 0x0007FC20 File Offset: 0x0007DE20
		public static long ReadSigned64(this uint[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)(num >> 1 ^ -(long)(num & 1UL));
		}

		// Token: 0x06001717 RID: 5911 RVA: 0x0007FC40 File Offset: 0x0007DE40
		public static long ReadSigned64(this ulong[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)(num >> 1 ^ -(long)(num & 1UL));
		}

		// Token: 0x06001718 RID: 5912 RVA: 0x0007FC5F File Offset: 0x0007DE5F
		public static void WriteFloat(this byte[] buffer, float value, ref int bitposition)
		{
			buffer.Write((ulong)value.uint32, ref bitposition, 32);
		}

		// Token: 0x06001719 RID: 5913 RVA: 0x0007FC76 File Offset: 0x0007DE76
		public static float ReadFloat(this byte[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 32);
		}

		// Token: 0x0600171A RID: 5914 RVA: 0x0007FC8C File Offset: 0x0007DE8C
		public static void Append(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int i = bitposition & 7;
			int num = bitposition >> 3;
			ulong num2 = (1UL << i) - 1UL;
			ulong num3 = ((ulong)buffer[num] & num2) | value << i;
			buffer[num] = (byte)num3;
			for (i = 8 - i; i < bits; i += 8)
			{
				num++;
				buffer[num] = (byte)(value >> i);
			}
			bitposition += bits;
		}

		// Token: 0x0600171B RID: 5915 RVA: 0x0007FCE8 File Offset: 0x0007DEE8
		public static void Append(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int i = bitposition & 31;
			int num = bitposition >> 5;
			ulong num2 = (1UL << i) - 1UL;
			ulong num3 = ((ulong)buffer[num] & num2) | value << i;
			buffer[num] = (uint)num3;
			for (i = 32 - i; i < bits; i += 32)
			{
				num++;
				buffer[num] = (uint)(value >> i);
			}
			bitposition += bits;
		}

		// Token: 0x0600171C RID: 5916 RVA: 0x0007FD48 File Offset: 0x0007DF48
		public static void Append(this uint[] buffer, uint value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 31;
			int num2 = bitposition >> 5;
			ulong num3 = (1UL << num) - 1UL;
			ulong num4 = ((ulong)buffer[num2] & num3) | (ulong)value << num;
			buffer[num2] = (uint)num4;
			buffer[num2 + 1] = (uint)(num4 >> 32);
			bitposition += bits;
		}

		// Token: 0x0600171D RID: 5917 RVA: 0x0007FD94 File Offset: 0x0007DF94
		public static void Append(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			ulong num3 = (1UL << num) - 1UL;
			ulong num4 = (buffer[num2] & num3) | value << num;
			buffer[num2] = num4;
			buffer[num2 + 1] = value >> 64 - num;
			bitposition += bits;
		}

		// Token: 0x0600171E RID: 5918 RVA: 0x0007FDE0 File Offset: 0x0007DFE0
		public static void Write(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 7;
			int num2 = bitposition >> 3;
			int i = num + bits;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			buffer[num2] = (byte)(((ulong)buffer[num2] & ~num4) | (num5 & num4));
			num = 8 - num;
			for (i -= 8; i > 8; i -= 8)
			{
				num2++;
				num5 = value >> num;
				buffer[num2] = (byte)num5;
				num += 8;
			}
			if (i > 0)
			{
				num2++;
				num4 = num3 >> num;
				num5 = value >> num;
				buffer[num2] = (byte)(((ulong)buffer[num2] & ~num4) | (num5 & num4));
			}
			bitposition += bits;
		}

		// Token: 0x0600171F RID: 5919 RVA: 0x0007FE84 File Offset: 0x0007E084
		public static void Write(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 31;
			int num2 = bitposition >> 5;
			int i = num + bits;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			buffer[num2] = (uint)(((ulong)buffer[num2] & ~num4) | (num5 & num4));
			num = 32 - num;
			for (i -= 32; i > 32; i -= 32)
			{
				num2++;
				num4 = num3 >> num;
				num5 = value >> num;
				buffer[num2] = (uint)(((ulong)buffer[num2] & ~num4) | (num5 & num4));
				num += 32;
			}
			bitposition += bits;
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x0007FF18 File Offset: 0x0007E118
		public static void Write(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			int i = num + bits;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			buffer[num2] = ((buffer[num2] & ~num4) | (num5 & num4));
			num = 64 - num;
			for (i -= 64; i > 64; i -= 64)
			{
				num2++;
				num4 = num3 >> num;
				num5 = value >> num;
				buffer[num2] = ((buffer[num2] & ~num4) | (num5 & num4));
				num += 64;
			}
			bitposition += bits;
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x0007FFA8 File Offset: 0x0007E1A8
		public static void WriteBool(this ulong[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06001722 RID: 5922 RVA: 0x0007FFBA File Offset: 0x0007E1BA
		public static void WriteBool(this uint[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06001723 RID: 5923 RVA: 0x0007FFCC File Offset: 0x0007E1CC
		public static void WriteBool(this byte[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06001724 RID: 5924 RVA: 0x0007FFE0 File Offset: 0x0007E1E0
		public static ulong Read(this byte[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 7;
			int num = bitposition >> 3;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = (ulong)buffer[num] >> i;
			for (i = 8 - i; i < bits; i += 8)
			{
				num++;
				num3 |= (ulong)buffer[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06001725 RID: 5925 RVA: 0x0008003C File Offset: 0x0007E23C
		public static ulong Read(this uint[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 31;
			int num = bitposition >> 5;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = (ulong)buffer[num] >> i;
			for (i = 32 - i; i < bits; i += 32)
			{
				num++;
				num3 |= (ulong)buffer[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06001726 RID: 5926 RVA: 0x00080098 File Offset: 0x0007E298
		public static ulong Read(this ulong[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 63;
			int num = bitposition >> 6;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = buffer[num] >> i;
			for (i = 64 - i; i < bits; i += 64)
			{
				num++;
				num3 |= buffer[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06001727 RID: 5927 RVA: 0x000800F2 File Offset: 0x0007E2F2
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this byte[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06001728 RID: 5928 RVA: 0x000800FC File Offset: 0x0007E2FC
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this uint[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06001729 RID: 5929 RVA: 0x00080106 File Offset: 0x0007E306
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this ulong[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600172A RID: 5930 RVA: 0x00080110 File Offset: 0x0007E310
		public static uint ReadUInt32(this byte[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600172B RID: 5931 RVA: 0x0008011B File Offset: 0x0007E31B
		public static uint ReadUInt32(this uint[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600172C RID: 5932 RVA: 0x00080126 File Offset: 0x0007E326
		public static uint ReadUInt32(this ulong[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600172D RID: 5933 RVA: 0x00080131 File Offset: 0x0007E331
		public static ushort ReadUInt16(this byte[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600172E RID: 5934 RVA: 0x0008013C File Offset: 0x0007E33C
		public static ushort ReadUInt16(this uint[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600172F RID: 5935 RVA: 0x00080147 File Offset: 0x0007E347
		public static ushort ReadUInt16(this ulong[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06001730 RID: 5936 RVA: 0x00080152 File Offset: 0x0007E352
		public static byte ReadByte(this byte[] buffer, ref int bitposition, int bits = 8)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06001731 RID: 5937 RVA: 0x0008015D File Offset: 0x0007E35D
		public static byte ReadByte(this uint[] buffer, ref int bitposition, int bits = 32)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06001732 RID: 5938 RVA: 0x00080168 File Offset: 0x0007E368
		public static byte ReadByte(this ulong[] buffer, ref int bitposition, int bits)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06001733 RID: 5939 RVA: 0x00080173 File Offset: 0x0007E373
		public static bool ReadBool(this ulong[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x06001734 RID: 5940 RVA: 0x00080184 File Offset: 0x0007E384
		public static bool ReadBool(this uint[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x06001735 RID: 5941 RVA: 0x00080195 File Offset: 0x0007E395
		public static bool ReadBool(this byte[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x06001736 RID: 5942 RVA: 0x000801A6 File Offset: 0x0007E3A6
		public static char ReadChar(this ulong[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x06001737 RID: 5943 RVA: 0x000801B2 File Offset: 0x0007E3B2
		public static char ReadChar(this uint[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x06001738 RID: 5944 RVA: 0x000801BE File Offset: 0x0007E3BE
		public static char ReadChar(this byte[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x000801CC File Offset: 0x0007E3CC
		public static void ReadOutSafe(this ulong[] source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = srcStartPos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 64) ? 64 : i);
				ulong value = source.Read(ref num, num2);
				target.Write(value, ref bitposition, num2);
			}
			bitposition += bits;
		}

		// Token: 0x0600173A RID: 5946 RVA: 0x00080214 File Offset: 0x0007E414
		public static void ReadOutSafe(this ulong[] source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = srcStartPos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 64) ? 64 : i);
				ulong value = source.Read(ref num, num2);
				target.Write(value, ref bitposition, num2);
			}
		}

		// Token: 0x0600173B RID: 5947 RVA: 0x00080254 File Offset: 0x0007E454
		public static void ReadOutSafe(this byte[] source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = srcStartPos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 8) ? 8 : i);
				ulong value = source.Read(ref num, num2);
				target.Write(value, ref bitposition, num2);
			}
		}

		// Token: 0x0600173C RID: 5948 RVA: 0x00080294 File Offset: 0x0007E494
		public static void ReadOutSafe(this byte[] source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = srcStartPos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 8) ? 8 : i);
				ulong value = source.Read(ref num, num2);
				target.Write(value, ref bitposition, num2);
			}
		}

		// Token: 0x0600173D RID: 5949 RVA: 0x000802D4 File Offset: 0x0007E4D4
		public static ulong IndexAsUInt64(this byte[] buffer, int index)
		{
			int num = index << 3;
			return (ulong)buffer[num] | (ulong)buffer[num + 1] << 8 | (ulong)buffer[num + 2] << 16 | (ulong)buffer[num + 3] << 24 | (ulong)buffer[num + 4] << 32 | (ulong)buffer[num + 5] << 40 | (ulong)buffer[num + 6] << 48 | (ulong)buffer[num + 7] << 56;
		}

		// Token: 0x0600173E RID: 5950 RVA: 0x00080330 File Offset: 0x0007E530
		public static ulong IndexAsUInt64(this uint[] buffer, int index)
		{
			int num = index << 1;
			return (ulong)buffer[num] | (ulong)buffer[num + 1] << 32;
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x00080350 File Offset: 0x0007E550
		public static uint IndexAsUInt32(this byte[] buffer, int index)
		{
			int num = index << 3;
			return (uint)((int)buffer[num] | (int)buffer[num + 1] << 8 | (int)buffer[num + 2] << 16 | (int)buffer[num + 3] << 24);
		}

		// Token: 0x06001740 RID: 5952 RVA: 0x00080380 File Offset: 0x0007E580
		public static uint IndexAsUInt32(this ulong[] buffer, int index)
		{
			int num = index >> 1;
			int num2 = (index & 1) << 5;
			return (uint)((byte)(buffer[num] >> num2));
		}

		// Token: 0x06001741 RID: 5953 RVA: 0x000803A0 File Offset: 0x0007E5A0
		public static byte IndexAsUInt8(this ulong[] buffer, int index)
		{
			int num = index >> 3;
			int num2 = (index & 7) << 3;
			return (byte)(buffer[num] >> num2);
		}

		// Token: 0x06001742 RID: 5954 RVA: 0x000803C0 File Offset: 0x0007E5C0
		public static byte IndexAsUInt8(this uint[] buffer, int index)
		{
			int num = index >> 3;
			int num2 = (index & 3) << 3;
			return (byte)((ulong)buffer[num] >> num2);
		}

		// Token: 0x0400190D RID: 6413
		private const string bufferOverrunMsg = "Byte buffer length exceeded by write or read. Dataloss will occur. Likely due to a Read/Write mismatch.";
	}
}
