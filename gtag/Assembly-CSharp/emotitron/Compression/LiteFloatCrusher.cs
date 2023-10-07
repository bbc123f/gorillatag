using System;
using emotitron.Compression.HalfFloat;
using UnityEngine;

namespace emotitron.Compression
{
	// Token: 0x02000342 RID: 834
	[Serializable]
	public class LiteFloatCrusher : LiteCrusher<float>
	{
		// Token: 0x060017FD RID: 6141 RVA: 0x000817F8 File Offset: 0x0007F9F8
		public LiteFloatCrusher()
		{
			this.compressType = LiteFloatCompressType.Half16;
			this.min = 0f;
			this.max = 1f;
			this.accurateCenter = true;
			LiteFloatCrusher.Recalculate(this.compressType, this.min, this.max, this.accurateCenter, ref this.bits, ref this.encoder, ref this.decoder, ref this.maxCVal);
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x00081874 File Offset: 0x0007FA74
		public LiteFloatCrusher(LiteFloatCompressType compressType, float min, float max, bool accurateCenter)
		{
			this.compressType = compressType;
			this.min = min;
			this.max = max;
			this.accurateCenter = accurateCenter;
			LiteFloatCrusher.Recalculate(compressType, min, max, accurateCenter, ref this.bits, ref this.encoder, ref this.decoder, ref this.maxCVal);
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x000818D8 File Offset: 0x0007FAD8
		public static void Recalculate(LiteFloatCompressType compressType, float min, float max, bool accurateCenter, ref int bits, ref float encoder, ref float decoder, ref ulong maxCVal)
		{
			bits = (int)compressType;
			float num = max - min;
			ulong num2 = (bits == 64) ? ulong.MaxValue : ((1UL << bits) - 1UL);
			if (accurateCenter && num2 != 0UL)
			{
				num2 -= 1UL;
			}
			encoder = ((num == 0f) ? 0f : (num2 / num));
			decoder = ((num2 == 0UL) ? 0f : (num / num2));
			maxCVal = num2;
		}

		// Token: 0x06001800 RID: 6144 RVA: 0x00081940 File Offset: 0x0007FB40
		public override ulong Encode(float val)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				return (ulong)HalfUtilities.Pack(val);
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				return (ulong)val.uint32;
			}
			float num = (val - this.min) * this.encoder + 0.5f;
			if (num < 0f)
			{
				return 0UL;
			}
			ulong num2 = (ulong)num;
			if (num2 <= this.maxCVal)
			{
				return num2;
			}
			return this.maxCVal;
		}

		// Token: 0x06001801 RID: 6145 RVA: 0x000819AC File Offset: 0x0007FBAC
		public override float Decode(uint cval)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				return HalfUtilities.Unpack((ushort)cval);
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				return cval.float32;
			}
			if (cval == 0U)
			{
				return this.min;
			}
			if ((ulong)cval == this.maxCVal)
			{
				return this.max;
			}
			return cval * this.decoder + this.min;
		}

		// Token: 0x06001802 RID: 6146 RVA: 0x00081A10 File Offset: 0x0007FC10
		public override ulong WriteValue(float val, byte[] buffer, ref int bitposition)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				ulong num = (ulong)HalfUtilities.Pack(val);
				buffer.Write(num, ref bitposition, 16);
				return num;
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				ulong num2 = (ulong)val.uint32;
				buffer.Write(num2, ref bitposition, 32);
				return num2;
			}
			ulong num3 = this.Encode(val);
			buffer.Write(num3, ref bitposition, (int)this.compressType);
			return num3;
		}

		// Token: 0x06001803 RID: 6147 RVA: 0x00081A75 File Offset: 0x0007FC75
		public override void WriteCValue(uint cval, byte[] buffer, ref int bitposition)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				buffer.Write((ulong)cval, ref bitposition, 16);
				return;
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				buffer.Write((ulong)cval, ref bitposition, 32);
				return;
			}
			buffer.Write((ulong)cval, ref bitposition, (int)this.compressType);
		}

		// Token: 0x06001804 RID: 6148 RVA: 0x00081AB4 File Offset: 0x0007FCB4
		public override float ReadValue(byte[] buffer, ref int bitposition)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				return HalfUtilities.Unpack((ushort)buffer.Read(ref bitposition, 16));
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				return ((uint)buffer.Read(ref bitposition, 32)).float32;
			}
			uint val = (uint)buffer.Read(ref bitposition, (int)this.compressType);
			return this.Decode(val);
		}

		// Token: 0x06001805 RID: 6149 RVA: 0x00081B10 File Offset: 0x0007FD10
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				base.GetType().Name,
				" ",
				this.compressType.ToString(),
				" mn: ",
				this.min.ToString(),
				" mx: ",
				this.max.ToString(),
				" e: ",
				this.encoder.ToString(),
				" d: ",
				this.decoder.ToString()
			});
		}

		// Token: 0x0400191E RID: 6430
		[SerializeField]
		protected float min;

		// Token: 0x0400191F RID: 6431
		[SerializeField]
		protected float max;

		// Token: 0x04001920 RID: 6432
		[SerializeField]
		public LiteFloatCompressType compressType = LiteFloatCompressType.Half16;

		// Token: 0x04001921 RID: 6433
		[SerializeField]
		private bool accurateCenter = true;

		// Token: 0x04001922 RID: 6434
		[SerializeField]
		private float encoder;

		// Token: 0x04001923 RID: 6435
		[SerializeField]
		private float decoder;

		// Token: 0x04001924 RID: 6436
		[SerializeField]
		private ulong maxCVal;
	}
}
