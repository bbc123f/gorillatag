using System;

namespace emotitron.Compression
{
	// Token: 0x02000337 RID: 823
	public static class ArraySerializeExt
	{
		// Token: 0x060016F7 RID: 5879 RVA: 0x0007F44C File Offset: 0x0007D64C
		public static void Zero(this byte[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060016F8 RID: 5880 RVA: 0x0007F46C File Offset: 0x0007D66C
		public static void Zero(this byte[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060016F9 RID: 5881 RVA: 0x0007F490 File Offset: 0x0007D690
		public static void Zero(this byte[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060016FA RID: 5882 RVA: 0x0007F4B4 File Offset: 0x0007D6B4
		public static void Zero(this ushort[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060016FB RID: 5883 RVA: 0x0007F4D4 File Offset: 0x0007D6D4
		public static void Zero(this ushort[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060016FC RID: 5884 RVA: 0x0007F4F8 File Offset: 0x0007D6F8
		public static void Zero(this ushort[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060016FD RID: 5885 RVA: 0x0007F51C File Offset: 0x0007D71C
		public static void Zero(this uint[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x060016FE RID: 5886 RVA: 0x0007F53C File Offset: 0x0007D73C
		public static void Zero(this uint[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x060016FF RID: 5887 RVA: 0x0007F560 File Offset: 0x0007D760
		public static void Zero(this uint[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x06001700 RID: 5888 RVA: 0x0007F584 File Offset: 0x0007D784
		public static void Zero(this ulong[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x06001701 RID: 5889 RVA: 0x0007F5A4 File Offset: 0x0007D7A4
		public static void Zero(this ulong[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x06001702 RID: 5890 RVA: 0x0007F5C8 File Offset: 0x0007D7C8
		public static void Zero(this ulong[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x06001703 RID: 5891 RVA: 0x0007F5EC File Offset: 0x0007D7EC
		public static void WriteSigned(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06001704 RID: 5892 RVA: 0x0007F60C File Offset: 0x0007D80C
		public static void WriteSigned(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06001705 RID: 5893 RVA: 0x0007F62C File Offset: 0x0007D82C
		public static void WriteSigned(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06001706 RID: 5894 RVA: 0x0007F64C File Offset: 0x0007D84C
		public static void WriteSigned(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong value2 = (ulong)(value << 1 ^ value >> 63);
			buffer.Write(value2, ref bitposition, bits);
		}

		// Token: 0x06001707 RID: 5895 RVA: 0x0007F66C File Offset: 0x0007D86C
		public static void WriteSigned(this uint[] buffer, long value, ref int bitposition, int bits)
		{
			ulong value2 = (ulong)(value << 1 ^ value >> 63);
			buffer.Write(value2, ref bitposition, bits);
		}

		// Token: 0x06001708 RID: 5896 RVA: 0x0007F68C File Offset: 0x0007D88C
		public static void WriteSigned(this ulong[] buffer, long value, ref int bitposition, int bits)
		{
			ulong value2 = (ulong)(value << 1 ^ value >> 63);
			buffer.Write(value2, ref bitposition, bits);
		}

		// Token: 0x06001709 RID: 5897 RVA: 0x0007F6AC File Offset: 0x0007D8AC
		public static int ReadSigned(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x0600170A RID: 5898 RVA: 0x0007F6D0 File Offset: 0x0007D8D0
		public static int ReadSigned(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x0600170B RID: 5899 RVA: 0x0007F6F4 File Offset: 0x0007D8F4
		public static int ReadSigned(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x0600170C RID: 5900 RVA: 0x0007F718 File Offset: 0x0007D918
		public static long ReadSigned64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)(num >> 1 ^ -(long)(num & 1UL));
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x0007F738 File Offset: 0x0007D938
		public static long ReadSigned64(this uint[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)(num >> 1 ^ -(long)(num & 1UL));
		}

		// Token: 0x0600170E RID: 5902 RVA: 0x0007F758 File Offset: 0x0007D958
		public static long ReadSigned64(this ulong[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)(num >> 1 ^ -(long)(num & 1UL));
		}

		// Token: 0x0600170F RID: 5903 RVA: 0x0007F777 File Offset: 0x0007D977
		public static void WriteFloat(this byte[] buffer, float value, ref int bitposition)
		{
			buffer.Write((ulong)value.uint32, ref bitposition, 32);
		}

		// Token: 0x06001710 RID: 5904 RVA: 0x0007F78E File Offset: 0x0007D98E
		public static float ReadFloat(this byte[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 32);
		}

		// Token: 0x06001711 RID: 5905 RVA: 0x0007F7A4 File Offset: 0x0007D9A4
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

		// Token: 0x06001712 RID: 5906 RVA: 0x0007F800 File Offset: 0x0007DA00
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

		// Token: 0x06001713 RID: 5907 RVA: 0x0007F860 File Offset: 0x0007DA60
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

		// Token: 0x06001714 RID: 5908 RVA: 0x0007F8AC File Offset: 0x0007DAAC
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

		// Token: 0x06001715 RID: 5909 RVA: 0x0007F8F8 File Offset: 0x0007DAF8
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

		// Token: 0x06001716 RID: 5910 RVA: 0x0007F99C File Offset: 0x0007DB9C
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

		// Token: 0x06001717 RID: 5911 RVA: 0x0007FA30 File Offset: 0x0007DC30
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

		// Token: 0x06001718 RID: 5912 RVA: 0x0007FAC0 File Offset: 0x0007DCC0
		public static void WriteBool(this ulong[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06001719 RID: 5913 RVA: 0x0007FAD2 File Offset: 0x0007DCD2
		public static void WriteBool(this uint[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x0600171A RID: 5914 RVA: 0x0007FAE4 File Offset: 0x0007DCE4
		public static void WriteBool(this byte[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x0600171B RID: 5915 RVA: 0x0007FAF8 File Offset: 0x0007DCF8
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

		// Token: 0x0600171C RID: 5916 RVA: 0x0007FB54 File Offset: 0x0007DD54
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

		// Token: 0x0600171D RID: 5917 RVA: 0x0007FBB0 File Offset: 0x0007DDB0
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

		// Token: 0x0600171E RID: 5918 RVA: 0x0007FC0A File Offset: 0x0007DE0A
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this byte[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600171F RID: 5919 RVA: 0x0007FC14 File Offset: 0x0007DE14
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this uint[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x0007FC1E File Offset: 0x0007DE1E
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this ulong[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x0007FC28 File Offset: 0x0007DE28
		public static uint ReadUInt32(this byte[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06001722 RID: 5922 RVA: 0x0007FC33 File Offset: 0x0007DE33
		public static uint ReadUInt32(this uint[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06001723 RID: 5923 RVA: 0x0007FC3E File Offset: 0x0007DE3E
		public static uint ReadUInt32(this ulong[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06001724 RID: 5924 RVA: 0x0007FC49 File Offset: 0x0007DE49
		public static ushort ReadUInt16(this byte[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06001725 RID: 5925 RVA: 0x0007FC54 File Offset: 0x0007DE54
		public static ushort ReadUInt16(this uint[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06001726 RID: 5926 RVA: 0x0007FC5F File Offset: 0x0007DE5F
		public static ushort ReadUInt16(this ulong[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06001727 RID: 5927 RVA: 0x0007FC6A File Offset: 0x0007DE6A
		public static byte ReadByte(this byte[] buffer, ref int bitposition, int bits = 8)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06001728 RID: 5928 RVA: 0x0007FC75 File Offset: 0x0007DE75
		public static byte ReadByte(this uint[] buffer, ref int bitposition, int bits = 32)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06001729 RID: 5929 RVA: 0x0007FC80 File Offset: 0x0007DE80
		public static byte ReadByte(this ulong[] buffer, ref int bitposition, int bits)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x0600172A RID: 5930 RVA: 0x0007FC8B File Offset: 0x0007DE8B
		public static bool ReadBool(this ulong[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x0600172B RID: 5931 RVA: 0x0007FC9C File Offset: 0x0007DE9C
		public static bool ReadBool(this uint[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x0600172C RID: 5932 RVA: 0x0007FCAD File Offset: 0x0007DEAD
		public static bool ReadBool(this byte[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x0600172D RID: 5933 RVA: 0x0007FCBE File Offset: 0x0007DEBE
		public static char ReadChar(this ulong[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x0600172E RID: 5934 RVA: 0x0007FCCA File Offset: 0x0007DECA
		public static char ReadChar(this uint[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x0600172F RID: 5935 RVA: 0x0007FCD6 File Offset: 0x0007DED6
		public static char ReadChar(this byte[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x06001730 RID: 5936 RVA: 0x0007FCE4 File Offset: 0x0007DEE4
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

		// Token: 0x06001731 RID: 5937 RVA: 0x0007FD2C File Offset: 0x0007DF2C
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

		// Token: 0x06001732 RID: 5938 RVA: 0x0007FD6C File Offset: 0x0007DF6C
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

		// Token: 0x06001733 RID: 5939 RVA: 0x0007FDAC File Offset: 0x0007DFAC
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

		// Token: 0x06001734 RID: 5940 RVA: 0x0007FDEC File Offset: 0x0007DFEC
		public static ulong IndexAsUInt64(this byte[] buffer, int index)
		{
			int num = index << 3;
			return (ulong)buffer[num] | (ulong)buffer[num + 1] << 8 | (ulong)buffer[num + 2] << 16 | (ulong)buffer[num + 3] << 24 | (ulong)buffer[num + 4] << 32 | (ulong)buffer[num + 5] << 40 | (ulong)buffer[num + 6] << 48 | (ulong)buffer[num + 7] << 56;
		}

		// Token: 0x06001735 RID: 5941 RVA: 0x0007FE48 File Offset: 0x0007E048
		public static ulong IndexAsUInt64(this uint[] buffer, int index)
		{
			int num = index << 1;
			return (ulong)buffer[num] | (ulong)buffer[num + 1] << 32;
		}

		// Token: 0x06001736 RID: 5942 RVA: 0x0007FE68 File Offset: 0x0007E068
		public static uint IndexAsUInt32(this byte[] buffer, int index)
		{
			int num = index << 3;
			return (uint)((int)buffer[num] | (int)buffer[num + 1] << 8 | (int)buffer[num + 2] << 16 | (int)buffer[num + 3] << 24);
		}

		// Token: 0x06001737 RID: 5943 RVA: 0x0007FE98 File Offset: 0x0007E098
		public static uint IndexAsUInt32(this ulong[] buffer, int index)
		{
			int num = index >> 1;
			int num2 = (index & 1) << 5;
			return (uint)((byte)(buffer[num] >> num2));
		}

		// Token: 0x06001738 RID: 5944 RVA: 0x0007FEB8 File Offset: 0x0007E0B8
		public static byte IndexAsUInt8(this ulong[] buffer, int index)
		{
			int num = index >> 3;
			int num2 = (index & 7) << 3;
			return (byte)(buffer[num] >> num2);
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x0007FED8 File Offset: 0x0007E0D8
		public static byte IndexAsUInt8(this uint[] buffer, int index)
		{
			int num = index >> 3;
			int num2 = (index & 3) << 3;
			return (byte)((ulong)buffer[num] >> num2);
		}

		// Token: 0x04001900 RID: 6400
		private const string bufferOverrunMsg = "Byte buffer length exceeded by write or read. Dataloss will occur. Likely due to a Read/Write mismatch.";
	}
}
