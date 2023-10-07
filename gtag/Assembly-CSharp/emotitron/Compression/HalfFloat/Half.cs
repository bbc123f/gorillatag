using System;
using System.Globalization;

namespace emotitron.Compression.HalfFloat
{
	// Token: 0x02000347 RID: 839
	[Serializable]
	public struct Half : IConvertible, IComparable, IComparable<Half>, IEquatable<Half>, IFormattable
	{
		// Token: 0x06001830 RID: 6192 RVA: 0x000821B6 File Offset: 0x000803B6
		public Half(float value)
		{
			this.value = HalfUtilities.Pack(value);
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06001831 RID: 6193 RVA: 0x000821C4 File Offset: 0x000803C4
		public ushort RawValue
		{
			get
			{
				return this.value;
			}
		}

		// Token: 0x06001832 RID: 6194 RVA: 0x000821CC File Offset: 0x000803CC
		public static float[] ConvertToFloat(Half[] values)
		{
			float[] array = new float[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = HalfUtilities.Unpack(values[i].RawValue);
			}
			return array;
		}

		// Token: 0x06001833 RID: 6195 RVA: 0x00082208 File Offset: 0x00080408
		public static Half[] ConvertToHalf(float[] values)
		{
			Half[] array = new Half[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Half(values[i]);
			}
			return array;
		}

		// Token: 0x06001834 RID: 6196 RVA: 0x0008223C File Offset: 0x0008043C
		public static bool IsInfinity(Half half)
		{
			return half == Half.PositiveInfinity || half == Half.NegativeInfinity;
		}

		// Token: 0x06001835 RID: 6197 RVA: 0x00082258 File Offset: 0x00080458
		public static bool IsNaN(Half half)
		{
			return half == Half.NaN;
		}

		// Token: 0x06001836 RID: 6198 RVA: 0x00082265 File Offset: 0x00080465
		public static bool IsNegativeInfinity(Half half)
		{
			return half == Half.NegativeInfinity;
		}

		// Token: 0x06001837 RID: 6199 RVA: 0x00082272 File Offset: 0x00080472
		public static bool IsPositiveInfinity(Half half)
		{
			return half == Half.PositiveInfinity;
		}

		// Token: 0x06001838 RID: 6200 RVA: 0x0008227F File Offset: 0x0008047F
		public static bool operator <(Half left, Half right)
		{
			return left < right;
		}

		// Token: 0x06001839 RID: 6201 RVA: 0x00082291 File Offset: 0x00080491
		public static bool operator >(Half left, Half right)
		{
			return left > right;
		}

		// Token: 0x0600183A RID: 6202 RVA: 0x000822A3 File Offset: 0x000804A3
		public static bool operator <=(Half left, Half right)
		{
			return left <= right;
		}

		// Token: 0x0600183B RID: 6203 RVA: 0x000822B8 File Offset: 0x000804B8
		public static bool operator >=(Half left, Half right)
		{
			return left >= right;
		}

		// Token: 0x0600183C RID: 6204 RVA: 0x000822CD File Offset: 0x000804CD
		public static bool operator ==(Half left, Half right)
		{
			return left.Equals(right);
		}

		// Token: 0x0600183D RID: 6205 RVA: 0x000822D7 File Offset: 0x000804D7
		public static bool operator !=(Half left, Half right)
		{
			return !left.Equals(right);
		}

		// Token: 0x0600183E RID: 6206 RVA: 0x000822E4 File Offset: 0x000804E4
		public static explicit operator Half(float value)
		{
			return new Half(value);
		}

		// Token: 0x0600183F RID: 6207 RVA: 0x000822EC File Offset: 0x000804EC
		public static implicit operator float(Half value)
		{
			return HalfUtilities.Unpack(value.value);
		}

		// Token: 0x06001840 RID: 6208 RVA: 0x000822FC File Offset: 0x000804FC
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x06001841 RID: 6209 RVA: 0x0008232C File Offset: 0x0008052C
		public string ToString(string format)
		{
			if (format == null)
			{
				return this.ToString();
			}
			return string.Format(CultureInfo.CurrentCulture, this.ToString(format, CultureInfo.CurrentCulture), Array.Empty<object>());
		}

		// Token: 0x06001842 RID: 6210 RVA: 0x00082374 File Offset: 0x00080574
		public string ToString(IFormatProvider formatProvider)
		{
			return string.Format(formatProvider, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x06001843 RID: 6211 RVA: 0x000823A0 File Offset: 0x000805A0
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null)
			{
				this.ToString(formatProvider);
			}
			return string.Format(formatProvider, this.ToString(format, formatProvider), Array.Empty<object>());
		}

		// Token: 0x06001844 RID: 6212 RVA: 0x000823D9 File Offset: 0x000805D9
		public override int GetHashCode()
		{
			return (int)(this.value * 3 / 2 ^ this.value);
		}

		// Token: 0x06001845 RID: 6213 RVA: 0x000823EC File Offset: 0x000805EC
		public int CompareTo(Half value)
		{
			if (this < value)
			{
				return -1;
			}
			if (this > value)
			{
				return 1;
			}
			if (this != value)
			{
				if (!Half.IsNaN(this))
				{
					return 1;
				}
				if (!Half.IsNaN(value))
				{
					return -1;
				}
			}
			return 0;
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x00082444 File Offset: 0x00080644
		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is Half))
			{
				throw new ArgumentException("The argument value must be a SlimMath.Half.");
			}
			Half half = (Half)value;
			if (this < half)
			{
				return -1;
			}
			if (this > half)
			{
				return 1;
			}
			if (this != half)
			{
				if (!Half.IsNaN(this))
				{
					return 1;
				}
				if (!Half.IsNaN(half))
				{
					return -1;
				}
			}
			return 0;
		}

		// Token: 0x06001847 RID: 6215 RVA: 0x000824B8 File Offset: 0x000806B8
		public static bool Equals(ref Half value1, ref Half value2)
		{
			return value1.value == value2.value;
		}

		// Token: 0x06001848 RID: 6216 RVA: 0x000824C8 File Offset: 0x000806C8
		public bool Equals(Half other)
		{
			return other.value == this.value;
		}

		// Token: 0x06001849 RID: 6217 RVA: 0x000824D8 File Offset: 0x000806D8
		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != base.GetType()) && this.Equals((Half)obj);
		}

		// Token: 0x0600184A RID: 6218 RVA: 0x0008250A File Offset: 0x0008070A
		public TypeCode GetTypeCode()
		{
			return Type.GetTypeCode(typeof(Half));
		}

		// Token: 0x0600184B RID: 6219 RVA: 0x0008251B File Offset: 0x0008071B
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		// Token: 0x0600184C RID: 6220 RVA: 0x0008252D File Offset: 0x0008072D
		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		// Token: 0x0600184D RID: 6221 RVA: 0x0008253F File Offset: 0x0008073F
		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.Char.");
		}

		// Token: 0x0600184E RID: 6222 RVA: 0x0008254B File Offset: 0x0008074B
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.DateTime.");
		}

		// Token: 0x0600184F RID: 6223 RVA: 0x00082557 File Offset: 0x00080757
		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this);
		}

		// Token: 0x06001850 RID: 6224 RVA: 0x00082569 File Offset: 0x00080769
		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}

		// Token: 0x06001851 RID: 6225 RVA: 0x0008257B File Offset: 0x0008077B
		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}

		// Token: 0x06001852 RID: 6226 RVA: 0x0008258D File Offset: 0x0008078D
		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}

		// Token: 0x06001853 RID: 6227 RVA: 0x0008259F File Offset: 0x0008079F
		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}

		// Token: 0x06001854 RID: 6228 RVA: 0x000825B1 File Offset: 0x000807B1
		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this);
		}

		// Token: 0x06001855 RID: 6229 RVA: 0x000825C3 File Offset: 0x000807C3
		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return this;
		}

		// Token: 0x06001856 RID: 6230 RVA: 0x000825D0 File Offset: 0x000807D0
		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return ((IConvertible)this).ToType(type, provider);
		}

		// Token: 0x06001857 RID: 6231 RVA: 0x000825EA File Offset: 0x000807EA
		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}

		// Token: 0x06001858 RID: 6232 RVA: 0x000825FC File Offset: 0x000807FC
		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x0008260E File Offset: 0x0008080E
		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this);
		}

		// Token: 0x04001941 RID: 6465
		private ushort value;

		// Token: 0x04001942 RID: 6466
		public const int PrecisionDigits = 3;

		// Token: 0x04001943 RID: 6467
		public const int MantissaBits = 11;

		// Token: 0x04001944 RID: 6468
		public const int MaximumDecimalExponent = 4;

		// Token: 0x04001945 RID: 6469
		public const int MaximumBinaryExponent = 15;

		// Token: 0x04001946 RID: 6470
		public const int MinimumDecimalExponent = -4;

		// Token: 0x04001947 RID: 6471
		public const int MinimumBinaryExponent = -14;

		// Token: 0x04001948 RID: 6472
		public const int ExponentRadix = 2;

		// Token: 0x04001949 RID: 6473
		public const int AdditionRounding = 1;

		// Token: 0x0400194A RID: 6474
		public static readonly Half Epsilon = new Half(0.0004887581f);

		// Token: 0x0400194B RID: 6475
		public static readonly Half MaxValue = new Half(65504f);

		// Token: 0x0400194C RID: 6476
		public static readonly Half MinValue = new Half(6.103516E-05f);

		// Token: 0x0400194D RID: 6477
		public static readonly Half NaN = new Half(float.NaN);

		// Token: 0x0400194E RID: 6478
		public static readonly Half NegativeInfinity = new Half(float.NegativeInfinity);

		// Token: 0x0400194F RID: 6479
		public static readonly Half PositiveInfinity = new Half(float.PositiveInfinity);
	}
}
