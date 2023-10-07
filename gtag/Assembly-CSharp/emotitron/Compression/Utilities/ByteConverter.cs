using System;
using System.Runtime.InteropServices;

namespace emotitron.Compression.Utilities
{
	// Token: 0x02000346 RID: 838
	[StructLayout(LayoutKind.Explicit)]
	public struct ByteConverter
	{
		// Token: 0x1700018F RID: 399
		public byte this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.byte0;
				case 1:
					return this.byte1;
				case 2:
					return this.byte2;
				case 3:
					return this.byte3;
				case 4:
					return this.byte4;
				case 5:
					return this.byte5;
				case 6:
					return this.byte6;
				case 7:
					return this.byte7;
				default:
					return 0;
				}
			}
		}

		// Token: 0x06001818 RID: 6168 RVA: 0x00081F18 File Offset: 0x00080118
		public static implicit operator ByteConverter(byte[] bytes)
		{
			ByteConverter result = default(ByteConverter);
			int num = bytes.Length;
			result.byte0 = bytes[0];
			if (num > 0)
			{
				result.byte1 = bytes[1];
			}
			if (num > 1)
			{
				result.byte2 = bytes[2];
			}
			if (num > 2)
			{
				result.byte3 = bytes[3];
			}
			if (num > 3)
			{
				result.byte4 = bytes[4];
			}
			if (num > 4)
			{
				result.byte5 = bytes[5];
			}
			if (num > 5)
			{
				result.byte6 = bytes[3];
			}
			if (num > 6)
			{
				result.byte7 = bytes[7];
			}
			return result;
		}

		// Token: 0x06001819 RID: 6169 RVA: 0x00081F9C File Offset: 0x0008019C
		public static implicit operator ByteConverter(byte val)
		{
			return new ByteConverter
			{
				byte0 = val
			};
		}

		// Token: 0x0600181A RID: 6170 RVA: 0x00081FBC File Offset: 0x000801BC
		public static implicit operator ByteConverter(sbyte val)
		{
			return new ByteConverter
			{
				int8 = val
			};
		}

		// Token: 0x0600181B RID: 6171 RVA: 0x00081FDC File Offset: 0x000801DC
		public static implicit operator ByteConverter(char val)
		{
			return new ByteConverter
			{
				character = val
			};
		}

		// Token: 0x0600181C RID: 6172 RVA: 0x00081FFC File Offset: 0x000801FC
		public static implicit operator ByteConverter(uint val)
		{
			return new ByteConverter
			{
				uint32 = val
			};
		}

		// Token: 0x0600181D RID: 6173 RVA: 0x0008201C File Offset: 0x0008021C
		public static implicit operator ByteConverter(int val)
		{
			return new ByteConverter
			{
				int32 = val
			};
		}

		// Token: 0x0600181E RID: 6174 RVA: 0x0008203C File Offset: 0x0008023C
		public static implicit operator ByteConverter(ulong val)
		{
			return new ByteConverter
			{
				uint64 = val
			};
		}

		// Token: 0x0600181F RID: 6175 RVA: 0x0008205C File Offset: 0x0008025C
		public static implicit operator ByteConverter(long val)
		{
			return new ByteConverter
			{
				int64 = val
			};
		}

		// Token: 0x06001820 RID: 6176 RVA: 0x0008207C File Offset: 0x0008027C
		public static implicit operator ByteConverter(float val)
		{
			return new ByteConverter
			{
				float32 = val
			};
		}

		// Token: 0x06001821 RID: 6177 RVA: 0x0008209C File Offset: 0x0008029C
		public static implicit operator ByteConverter(double val)
		{
			return new ByteConverter
			{
				float64 = val
			};
		}

		// Token: 0x06001822 RID: 6178 RVA: 0x000820BC File Offset: 0x000802BC
		public static implicit operator ByteConverter(bool val)
		{
			return new ByteConverter
			{
				int32 = (val ? 1 : 0)
			};
		}

		// Token: 0x06001823 RID: 6179 RVA: 0x000820E0 File Offset: 0x000802E0
		public void ExtractByteArray(byte[] targetArray)
		{
			int num = targetArray.Length;
			targetArray[0] = this.byte0;
			if (num > 0)
			{
				targetArray[1] = this.byte1;
			}
			if (num > 1)
			{
				targetArray[2] = this.byte2;
			}
			if (num > 2)
			{
				targetArray[3] = this.byte3;
			}
			if (num > 3)
			{
				targetArray[4] = this.byte4;
			}
			if (num > 4)
			{
				targetArray[5] = this.byte5;
			}
			if (num > 5)
			{
				targetArray[6] = this.byte6;
			}
			if (num > 6)
			{
				targetArray[7] = this.byte7;
			}
		}

		// Token: 0x06001824 RID: 6180 RVA: 0x00082153 File Offset: 0x00080353
		public static implicit operator byte(ByteConverter bc)
		{
			return bc.byte0;
		}

		// Token: 0x06001825 RID: 6181 RVA: 0x0008215B File Offset: 0x0008035B
		public static implicit operator sbyte(ByteConverter bc)
		{
			return bc.int8;
		}

		// Token: 0x06001826 RID: 6182 RVA: 0x00082163 File Offset: 0x00080363
		public static implicit operator char(ByteConverter bc)
		{
			return bc.character;
		}

		// Token: 0x06001827 RID: 6183 RVA: 0x0008216B File Offset: 0x0008036B
		public static implicit operator ushort(ByteConverter bc)
		{
			return bc.uint16;
		}

		// Token: 0x06001828 RID: 6184 RVA: 0x00082173 File Offset: 0x00080373
		public static implicit operator short(ByteConverter bc)
		{
			return bc.int16;
		}

		// Token: 0x06001829 RID: 6185 RVA: 0x0008217B File Offset: 0x0008037B
		public static implicit operator uint(ByteConverter bc)
		{
			return bc.uint32;
		}

		// Token: 0x0600182A RID: 6186 RVA: 0x00082183 File Offset: 0x00080383
		public static implicit operator int(ByteConverter bc)
		{
			return bc.int32;
		}

		// Token: 0x0600182B RID: 6187 RVA: 0x0008218B File Offset: 0x0008038B
		public static implicit operator ulong(ByteConverter bc)
		{
			return bc.uint64;
		}

		// Token: 0x0600182C RID: 6188 RVA: 0x00082193 File Offset: 0x00080393
		public static implicit operator long(ByteConverter bc)
		{
			return bc.int64;
		}

		// Token: 0x0600182D RID: 6189 RVA: 0x0008219B File Offset: 0x0008039B
		public static implicit operator float(ByteConverter bc)
		{
			return bc.float32;
		}

		// Token: 0x0600182E RID: 6190 RVA: 0x000821A3 File Offset: 0x000803A3
		public static implicit operator double(ByteConverter bc)
		{
			return bc.float64;
		}

		// Token: 0x0600182F RID: 6191 RVA: 0x000821AB File Offset: 0x000803AB
		public static implicit operator bool(ByteConverter bc)
		{
			return bc.int32 != 0;
		}

		// Token: 0x0400192E RID: 6446
		[FieldOffset(0)]
		public float float32;

		// Token: 0x0400192F RID: 6447
		[FieldOffset(0)]
		public double float64;

		// Token: 0x04001930 RID: 6448
		[FieldOffset(0)]
		public sbyte int8;

		// Token: 0x04001931 RID: 6449
		[FieldOffset(0)]
		public short int16;

		// Token: 0x04001932 RID: 6450
		[FieldOffset(0)]
		public ushort uint16;

		// Token: 0x04001933 RID: 6451
		[FieldOffset(0)]
		public char character;

		// Token: 0x04001934 RID: 6452
		[FieldOffset(0)]
		public int int32;

		// Token: 0x04001935 RID: 6453
		[FieldOffset(0)]
		public uint uint32;

		// Token: 0x04001936 RID: 6454
		[FieldOffset(0)]
		public long int64;

		// Token: 0x04001937 RID: 6455
		[FieldOffset(0)]
		public ulong uint64;

		// Token: 0x04001938 RID: 6456
		[FieldOffset(0)]
		public byte byte0;

		// Token: 0x04001939 RID: 6457
		[FieldOffset(1)]
		public byte byte1;

		// Token: 0x0400193A RID: 6458
		[FieldOffset(2)]
		public byte byte2;

		// Token: 0x0400193B RID: 6459
		[FieldOffset(3)]
		public byte byte3;

		// Token: 0x0400193C RID: 6460
		[FieldOffset(4)]
		public byte byte4;

		// Token: 0x0400193D RID: 6461
		[FieldOffset(5)]
		public byte byte5;

		// Token: 0x0400193E RID: 6462
		[FieldOffset(6)]
		public byte byte6;

		// Token: 0x0400193F RID: 6463
		[FieldOffset(7)]
		public byte byte7;

		// Token: 0x04001940 RID: 6464
		[FieldOffset(4)]
		public uint uint16_B;
	}
}
