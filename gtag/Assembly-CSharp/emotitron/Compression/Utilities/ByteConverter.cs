using System;
using System.Runtime.InteropServices;

namespace emotitron.Compression.Utilities
{
	// Token: 0x02000348 RID: 840
	[StructLayout(LayoutKind.Explicit)]
	public struct ByteConverter
	{
		// Token: 0x17000191 RID: 401
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

		// Token: 0x06001821 RID: 6177 RVA: 0x00082400 File Offset: 0x00080600
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

		// Token: 0x06001822 RID: 6178 RVA: 0x00082484 File Offset: 0x00080684
		public static implicit operator ByteConverter(byte val)
		{
			return new ByteConverter
			{
				byte0 = val
			};
		}

		// Token: 0x06001823 RID: 6179 RVA: 0x000824A4 File Offset: 0x000806A4
		public static implicit operator ByteConverter(sbyte val)
		{
			return new ByteConverter
			{
				int8 = val
			};
		}

		// Token: 0x06001824 RID: 6180 RVA: 0x000824C4 File Offset: 0x000806C4
		public static implicit operator ByteConverter(char val)
		{
			return new ByteConverter
			{
				character = val
			};
		}

		// Token: 0x06001825 RID: 6181 RVA: 0x000824E4 File Offset: 0x000806E4
		public static implicit operator ByteConverter(uint val)
		{
			return new ByteConverter
			{
				uint32 = val
			};
		}

		// Token: 0x06001826 RID: 6182 RVA: 0x00082504 File Offset: 0x00080704
		public static implicit operator ByteConverter(int val)
		{
			return new ByteConverter
			{
				int32 = val
			};
		}

		// Token: 0x06001827 RID: 6183 RVA: 0x00082524 File Offset: 0x00080724
		public static implicit operator ByteConverter(ulong val)
		{
			return new ByteConverter
			{
				uint64 = val
			};
		}

		// Token: 0x06001828 RID: 6184 RVA: 0x00082544 File Offset: 0x00080744
		public static implicit operator ByteConverter(long val)
		{
			return new ByteConverter
			{
				int64 = val
			};
		}

		// Token: 0x06001829 RID: 6185 RVA: 0x00082564 File Offset: 0x00080764
		public static implicit operator ByteConverter(float val)
		{
			return new ByteConverter
			{
				float32 = val
			};
		}

		// Token: 0x0600182A RID: 6186 RVA: 0x00082584 File Offset: 0x00080784
		public static implicit operator ByteConverter(double val)
		{
			return new ByteConverter
			{
				float64 = val
			};
		}

		// Token: 0x0600182B RID: 6187 RVA: 0x000825A4 File Offset: 0x000807A4
		public static implicit operator ByteConverter(bool val)
		{
			return new ByteConverter
			{
				int32 = (val ? 1 : 0)
			};
		}

		// Token: 0x0600182C RID: 6188 RVA: 0x000825C8 File Offset: 0x000807C8
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

		// Token: 0x0600182D RID: 6189 RVA: 0x0008263B File Offset: 0x0008083B
		public static implicit operator byte(ByteConverter bc)
		{
			return bc.byte0;
		}

		// Token: 0x0600182E RID: 6190 RVA: 0x00082643 File Offset: 0x00080843
		public static implicit operator sbyte(ByteConverter bc)
		{
			return bc.int8;
		}

		// Token: 0x0600182F RID: 6191 RVA: 0x0008264B File Offset: 0x0008084B
		public static implicit operator char(ByteConverter bc)
		{
			return bc.character;
		}

		// Token: 0x06001830 RID: 6192 RVA: 0x00082653 File Offset: 0x00080853
		public static implicit operator ushort(ByteConverter bc)
		{
			return bc.uint16;
		}

		// Token: 0x06001831 RID: 6193 RVA: 0x0008265B File Offset: 0x0008085B
		public static implicit operator short(ByteConverter bc)
		{
			return bc.int16;
		}

		// Token: 0x06001832 RID: 6194 RVA: 0x00082663 File Offset: 0x00080863
		public static implicit operator uint(ByteConverter bc)
		{
			return bc.uint32;
		}

		// Token: 0x06001833 RID: 6195 RVA: 0x0008266B File Offset: 0x0008086B
		public static implicit operator int(ByteConverter bc)
		{
			return bc.int32;
		}

		// Token: 0x06001834 RID: 6196 RVA: 0x00082673 File Offset: 0x00080873
		public static implicit operator ulong(ByteConverter bc)
		{
			return bc.uint64;
		}

		// Token: 0x06001835 RID: 6197 RVA: 0x0008267B File Offset: 0x0008087B
		public static implicit operator long(ByteConverter bc)
		{
			return bc.int64;
		}

		// Token: 0x06001836 RID: 6198 RVA: 0x00082683 File Offset: 0x00080883
		public static implicit operator float(ByteConverter bc)
		{
			return bc.float32;
		}

		// Token: 0x06001837 RID: 6199 RVA: 0x0008268B File Offset: 0x0008088B
		public static implicit operator double(ByteConverter bc)
		{
			return bc.float64;
		}

		// Token: 0x06001838 RID: 6200 RVA: 0x00082693 File Offset: 0x00080893
		public static implicit operator bool(ByteConverter bc)
		{
			return bc.int32 != 0;
		}

		// Token: 0x0400193B RID: 6459
		[FieldOffset(0)]
		public float float32;

		// Token: 0x0400193C RID: 6460
		[FieldOffset(0)]
		public double float64;

		// Token: 0x0400193D RID: 6461
		[FieldOffset(0)]
		public sbyte int8;

		// Token: 0x0400193E RID: 6462
		[FieldOffset(0)]
		public short int16;

		// Token: 0x0400193F RID: 6463
		[FieldOffset(0)]
		public ushort uint16;

		// Token: 0x04001940 RID: 6464
		[FieldOffset(0)]
		public char character;

		// Token: 0x04001941 RID: 6465
		[FieldOffset(0)]
		public int int32;

		// Token: 0x04001942 RID: 6466
		[FieldOffset(0)]
		public uint uint32;

		// Token: 0x04001943 RID: 6467
		[FieldOffset(0)]
		public long int64;

		// Token: 0x04001944 RID: 6468
		[FieldOffset(0)]
		public ulong uint64;

		// Token: 0x04001945 RID: 6469
		[FieldOffset(0)]
		public byte byte0;

		// Token: 0x04001946 RID: 6470
		[FieldOffset(1)]
		public byte byte1;

		// Token: 0x04001947 RID: 6471
		[FieldOffset(2)]
		public byte byte2;

		// Token: 0x04001948 RID: 6472
		[FieldOffset(3)]
		public byte byte3;

		// Token: 0x04001949 RID: 6473
		[FieldOffset(4)]
		public byte byte4;

		// Token: 0x0400194A RID: 6474
		[FieldOffset(5)]
		public byte byte5;

		// Token: 0x0400194B RID: 6475
		[FieldOffset(6)]
		public byte byte6;

		// Token: 0x0400194C RID: 6476
		[FieldOffset(7)]
		public byte byte7;

		// Token: 0x0400194D RID: 6477
		[FieldOffset(4)]
		public uint uint16_B;
	}
}
