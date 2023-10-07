using System;

namespace emotitron.Compression
{
	// Token: 0x02000338 RID: 824
	public static class ArraySerializeUnsafe
	{
		// Token: 0x0600173A RID: 5946 RVA: 0x0007FEFC File Offset: 0x0007E0FC
		public unsafe static void WriteSigned(ulong* buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(buffer, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600173B RID: 5947 RVA: 0x0007FF1C File Offset: 0x0007E11C
		public unsafe static void AppendSigned(ulong* buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Append(buffer, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600173C RID: 5948 RVA: 0x0007FF3C File Offset: 0x0007E13C
		public unsafe static void AddSigned(this int value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Append(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600173D RID: 5949 RVA: 0x0007FF5C File Offset: 0x0007E15C
		public unsafe static void AddSigned(this short value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Append(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600173E RID: 5950 RVA: 0x0007FF7C File Offset: 0x0007E17C
		public unsafe static void AddSigned(this sbyte value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Append(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x0007FF9C File Offset: 0x0007E19C
		public unsafe static void InjectSigned(this int value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06001740 RID: 5952 RVA: 0x0007FFBC File Offset: 0x0007E1BC
		public unsafe static void InjectSigned(this short value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06001741 RID: 5953 RVA: 0x0007FFDC File Offset: 0x0007E1DC
		public unsafe static void InjectSigned(this sbyte value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06001742 RID: 5954 RVA: 0x0007FFFC File Offset: 0x0007E1FC
		public unsafe static void PokeSigned(this int value, ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06001743 RID: 5955 RVA: 0x00080020 File Offset: 0x0007E220
		public unsafe static void PokeSigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x00080044 File Offset: 0x0007E244
		public unsafe static void PokeSigned(this sbyte value, ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06001745 RID: 5957 RVA: 0x00080068 File Offset: 0x0007E268
		public unsafe static int ReadSigned(ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x0008008C File Offset: 0x0007E28C
		public unsafe static int PeekSigned(ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06001747 RID: 5959 RVA: 0x000800B0 File Offset: 0x0007E2B0
		public unsafe static void Append(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			ulong num3 = (1UL << num) - 1UL;
			ulong num4 = (uPtr[num2] & num3) | value << num;
			uPtr[num2] = num4;
			uPtr[num2 + 1] = num4 >> 64 - num;
			bitposition += bits;
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x00080108 File Offset: 0x0007E308
		public unsafe static void Write(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			uPtr[num2] = ((uPtr[num2] & ~num4) | (num5 & num4));
			num = 64 - num;
			if (num < bits)
			{
				num4 = num3 >> num;
				num5 = value >> num;
				num2++;
				uPtr[num2] = ((uPtr[num2] & ~num4) | (num5 & num4));
			}
			bitposition += bits;
		}

		// Token: 0x06001749 RID: 5961 RVA: 0x0008018C File Offset: 0x0007E38C
		public unsafe static ulong Read(ulong* uPtr, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 63;
			int num = bitposition >> 6;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = uPtr[num] >> i;
			for (i = 64 - i; i < bits; i += 64)
			{
				num++;
				num3 |= uPtr[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x0600174A RID: 5962 RVA: 0x000801F0 File Offset: 0x0007E3F0
		public unsafe static ulong Read(ulong* uPtr, int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 63;
			int num = bitposition >> 6;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = uPtr[num] >> i;
			for (i = 64 - i; i < bits; i += 64)
			{
				num++;
				num3 |= uPtr[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x0600174B RID: 5963 RVA: 0x0008024F File Offset: 0x0007E44F
		public unsafe static void Add(this ulong value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, value, ref bitposition, bits);
		}

		// Token: 0x0600174C RID: 5964 RVA: 0x0008025B File Offset: 0x0007E45B
		public unsafe static void Add(this uint value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600174D RID: 5965 RVA: 0x00080268 File Offset: 0x0007E468
		public unsafe static void Add(this ushort value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600174E RID: 5966 RVA: 0x00080275 File Offset: 0x0007E475
		public unsafe static void Add(this byte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600174F RID: 5967 RVA: 0x00080282 File Offset: 0x0007E482
		public unsafe static void AddUnsigned(this long value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06001750 RID: 5968 RVA: 0x0008028E File Offset: 0x0007E48E
		public unsafe static void AddUnsigned(this int value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06001751 RID: 5969 RVA: 0x0008029B File Offset: 0x0007E49B
		public unsafe static void AddUnsigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06001752 RID: 5970 RVA: 0x000802A8 File Offset: 0x0007E4A8
		public unsafe static void AddUnsigned(this sbyte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06001753 RID: 5971 RVA: 0x000802B5 File Offset: 0x0007E4B5
		public unsafe static void Inject(this ulong value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, bits);
		}

		// Token: 0x06001754 RID: 5972 RVA: 0x000802C0 File Offset: 0x0007E4C0
		public unsafe static void Inject(this uint value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06001755 RID: 5973 RVA: 0x000802CC File Offset: 0x0007E4CC
		public unsafe static void Inject(this ushort value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06001756 RID: 5974 RVA: 0x000802D8 File Offset: 0x0007E4D8
		public unsafe static void Inject(this byte value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06001757 RID: 5975 RVA: 0x000802E4 File Offset: 0x0007E4E4
		public unsafe static void InjectUnsigned(this long value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06001758 RID: 5976 RVA: 0x000802EF File Offset: 0x0007E4EF
		public unsafe static void InjectUnsigned(this int value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06001759 RID: 5977 RVA: 0x000802FB File Offset: 0x0007E4FB
		public unsafe static void InjectUnsigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x0600175A RID: 5978 RVA: 0x00080308 File Offset: 0x0007E508
		public unsafe static void InjectUnsigned(this sbyte value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x0600175B RID: 5979 RVA: 0x00080314 File Offset: 0x0007E514
		public unsafe static void Poke(this ulong value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, bits);
		}

		// Token: 0x0600175C RID: 5980 RVA: 0x00080320 File Offset: 0x0007E520
		public unsafe static void Poke(this uint value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600175D RID: 5981 RVA: 0x0008032D File Offset: 0x0007E52D
		public unsafe static void Poke(this ushort value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600175E RID: 5982 RVA: 0x0008033A File Offset: 0x0007E53A
		public unsafe static void Poke(this byte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x0600175F RID: 5983 RVA: 0x00080347 File Offset: 0x0007E547
		public unsafe static void InjectUnsigned(this long value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06001760 RID: 5984 RVA: 0x00080353 File Offset: 0x0007E553
		public unsafe static void InjectUnsigned(this int value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06001761 RID: 5985 RVA: 0x00080360 File Offset: 0x0007E560
		public unsafe static void PokeUnsigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06001762 RID: 5986 RVA: 0x0008036D File Offset: 0x0007E56D
		public unsafe static void PokeUnsigned(this sbyte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06001763 RID: 5987 RVA: 0x0008037C File Offset: 0x0007E57C
		public unsafe static void ReadOutUnsafe(ulong* sourcePtr, int sourcePos, ulong* targetPtr, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 64) ? 64 : i);
				ulong value = ArraySerializeUnsafe.Read(sourcePtr, ref num, num2);
				ArraySerializeUnsafe.Write(targetPtr, value, ref targetPos, num2);
			}
			targetPos += bits;
		}

		// Token: 0x06001764 RID: 5988 RVA: 0x000803C4 File Offset: 0x0007E5C4
		public unsafe static void ReadOutUnsafe(this ulong[] source, int sourcePos, byte[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (ulong[] array = source)
			{
				ulong* uPtr;
				if (source == null || array.Length == 0)
				{
					uPtr = null;
				}
				else
				{
					uPtr = &array[0];
				}
				fixed (byte[] array2 = target)
				{
					byte* ptr;
					if (target == null || array2.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array2[0];
					}
					ulong* uPtr2 = (ulong*)ptr;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06001765 RID: 5989 RVA: 0x00080450 File Offset: 0x0007E650
		public unsafe static void ReadOutUnsafe(this ulong[] source, int sourcePos, uint[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (ulong[] array = source)
			{
				ulong* uPtr;
				if (source == null || array.Length == 0)
				{
					uPtr = null;
				}
				else
				{
					uPtr = &array[0];
				}
				fixed (uint[] array2 = target)
				{
					uint* ptr;
					if (target == null || array2.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array2[0];
					}
					ulong* uPtr2 = (ulong*)ptr;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06001766 RID: 5990 RVA: 0x000804DC File Offset: 0x0007E6DC
		public unsafe static void ReadOutUnsafe(this ulong[] source, int sourcePos, ulong[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (ulong[] array = source)
			{
				ulong* uPtr;
				if (source == null || array.Length == 0)
				{
					uPtr = null;
				}
				else
				{
					uPtr = &array[0];
				}
				fixed (ulong[] array2 = target)
				{
					ulong* uPtr2;
					if (target == null || array2.Length == 0)
					{
						uPtr2 = null;
					}
					else
					{
						uPtr2 = &array2[0];
					}
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06001767 RID: 5991 RVA: 0x00080564 File Offset: 0x0007E764
		public unsafe static void ReadOutUnsafe(this uint[] source, int sourcePos, byte[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (uint[] array = source)
			{
				uint* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (byte[] array2 = target)
				{
					byte* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* uPtr = (ulong*)ptr;
					ulong* uPtr2 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06001768 RID: 5992 RVA: 0x000805F4 File Offset: 0x0007E7F4
		public unsafe static void ReadOutUnsafe(this uint[] source, int sourcePos, uint[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (uint[] array = source)
			{
				uint* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (uint[] array2 = target)
				{
					uint* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* uPtr = (ulong*)ptr;
					ulong* uPtr2 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06001769 RID: 5993 RVA: 0x00080684 File Offset: 0x0007E884
		public unsafe static void ReadOutUnsafe(this uint[] source, int sourcePos, ulong[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (uint[] array = source)
			{
				uint* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (ulong[] array2 = target)
				{
					ulong* uPtr;
					if (target == null || array2.Length == 0)
					{
						uPtr = null;
					}
					else
					{
						uPtr = &array2[0];
					}
					ulong* uPtr2 = (ulong*)ptr;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr2, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x0600176A RID: 5994 RVA: 0x00080710 File Offset: 0x0007E910
		public unsafe static void ReadOutUnsafe(this byte[] source, int sourcePos, ulong[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (byte[] array = source)
			{
				byte* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (ulong[] array2 = target)
				{
					ulong* uPtr;
					if (target == null || array2.Length == 0)
					{
						uPtr = null;
					}
					else
					{
						uPtr = &array2[0];
					}
					ulong* uPtr2 = (ulong*)ptr;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr2, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x0600176B RID: 5995 RVA: 0x0008079C File Offset: 0x0007E99C
		public unsafe static void ReadOutUnsafe(this byte[] source, int sourcePos, uint[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (byte[] array = source)
			{
				byte* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (uint[] array2 = target)
				{
					uint* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* uPtr = (ulong*)ptr;
					ulong* uPtr2 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x0600176C RID: 5996 RVA: 0x0008082C File Offset: 0x0007EA2C
		public unsafe static void ReadOutUnsafe(this byte[] source, int sourcePos, byte[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (byte[] array = source)
			{
				byte* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (byte[] array2 = target)
				{
					byte* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* uPtr = (ulong*)ptr;
					ulong* uPtr2 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x04001901 RID: 6401
		private const string bufferOverrunMsg = "Byte buffer overrun. Dataloss will occur.";
	}
}
