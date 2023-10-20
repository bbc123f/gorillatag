using System;
using System.Globalization;

namespace emotitron.Compression.HalfFloat
{
	// Token: 0x02000349 RID: 841
	[Serializable]
	public struct Half : IConvertible, IComparable, IComparable<Half>, IEquatable<Half>, IFormattable
	{
		// Token: 0x06001839 RID: 6201 RVA: 0x0008269E File Offset: 0x0008089E
		public Half(float value)
		{
			this.value = HalfUtilities.Pack(value);
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x0600183A RID: 6202 RVA: 0x000826AC File Offset: 0x000808AC
		public ushort RawValue
		{
			get
			{
				return this.value;
			}
		}

		// Token: 0x0600183B RID: 6203 RVA: 0x000826B4 File Offset: 0x000808B4
		public static float[] ConvertToFloat(Half[] values)
		{
			float[] array = new float[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = HalfUtilities.Unpack(values[i].RawValue);
			}
			return array;
		}

		// Token: 0x0600183C RID: 6204 RVA: 0x000826F0 File Offset: 0x000808F0
		public static Half[] ConvertToHalf(float[] values)
		{
			Half[] array = new Half[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Half(values[i]);
			}
			return array;
		}

		// Token: 0x0600183D RID: 6205 RVA: 0x00082724 File Offset: 0x00080924
		public static bool IsInfinity(Half half)
		{
			return half == Half.PositiveInfinity || half == Half.NegativeInfinity;
		}

		// Token: 0x0600183E RID: 6206 RVA: 0x00082740 File Offset: 0x00080940
		public static bool IsNaN(Half half)
		{
			return half == Half.NaN;
		}

		// Token: 0x0600183F RID: 6207 RVA: 0x0008274D File Offset: 0x0008094D
		public static bool IsNegativeInfinity(Half half)
		{
			return half == Half.NegativeInfinity;
		}

		// Token: 0x06001840 RID: 6208 RVA: 0x0008275A File Offset: 0x0008095A
		public static bool IsPositiveInfinity(Half half)
		{
			return half == Half.PositiveInfinity;
		}

		// Token: 0x06001841 RID: 6209 RVA: 0x00082767 File Offset: 0x00080967
		public static bool operator <(Half left, Half right)
		{
			return left < right;
		}

		// Token: 0x06001842 RID: 6210 RVA: 0x00082779 File Offset: 0x00080979
		public static bool operator >(Half left, Half right)
		{
			return left > right;
		}

		// Token: 0x06001843 RID: 6211 RVA: 0x0008278B File Offset: 0x0008098B
		public static bool operator <=(Half left, Half right)
		{
			return left <= right;
		}

		// Token: 0x06001844 RID: 6212 RVA: 0x000827A0 File Offset: 0x000809A0
		public static bool operator >=(Half left, Half right)
		{
			return left >= right;
		}

		// Token: 0x06001845 RID: 6213 RVA: 0x000827B5 File Offset: 0x000809B5
		public static bool operator ==(Half left, Half right)
		{
			return left.Equals(right);
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x000827BF File Offset: 0x000809BF
		public static bool operator !=(Half left, Half right)
		{
			return !left.Equals(right);
		}

		// Token: 0x06001847 RID: 6215 RVA: 0x000827CC File Offset: 0x000809CC
		public static explicit operator Half(float value)
		{
			return new Half(value);
		}

		// Token: 0x06001848 RID: 6216 RVA: 0x000827D4 File Offset: 0x000809D4
		public static implicit operator float(Half value)
		{
			return HalfUtilities.Unpack(value.value);
		}

		// Token: 0x06001849 RID: 6217 RVA: 0x000827E4 File Offset: 0x000809E4
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x0600184A RID: 6218 RVA: 0x00082814 File Offset: 0x00080A14
		public string ToString(string format)
		{
			if (format == null)
			{
				return this.ToString();
			}
			return string.Format(CultureInfo.CurrentCulture, this.ToString(format, CultureInfo.CurrentCulture), Array.Empty<object>());
		}

		// Token: 0x0600184B RID: 6219 RVA: 0x0008285C File Offset: 0x00080A5C
		public string ToString(IFormatProvider formatProvider)
		{
			return string.Format(formatProvider, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x0600184C RID: 6220 RVA: 0x00082888 File Offset: 0x00080A88
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null)
			{
				this.ToString(formatProvider);
			}
			return string.Format(formatProvider, this.ToString(format, formatProvider), Array.Empty<object>());
		}

		// Token: 0x0600184D RID: 6221 RVA: 0x000828C1 File Offset: 0x00080AC1
		public override int GetHashCode()
		{
			return (int)(this.value * 3 / 2 ^ this.value);
		}

		// Token: 0x0600184E RID: 6222 RVA: 0x000828D4 File Offset: 0x00080AD4
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

		// Token: 0x0600184F RID: 6223 RVA: 0x0008292C File Offset: 0x00080B2C
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

		// Token: 0x06001850 RID: 6224 RVA: 0x000829A0 File Offset: 0x00080BA0
		public static bool Equals(ref Half value1, ref Half value2)
		{
			return value1.value == value2.value;
		}

		// Token: 0x06001851 RID: 6225 RVA: 0x000829B0 File Offset: 0x00080BB0
		public bool Equals(Half other)
		{
			return other.value == this.value;
		}

		// Token: 0x06001852 RID: 6226 RVA: 0x000829C0 File Offset: 0x00080BC0
		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != base.GetType()) && this.Equals((Half)obj);
		}

		// Token: 0x06001853 RID: 6227 RVA: 0x000829F2 File Offset: 0x00080BF2
		public TypeCode GetTypeCode()
		{
			return Type.GetTypeCode(typeof(Half));
		}

		// Token: 0x06001854 RID: 6228 RVA: 0x00082A03 File Offset: 0x00080C03
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		// Token: 0x06001855 RID: 6229 RVA: 0x00082A15 File Offset: 0x00080C15
		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		// Token: 0x06001856 RID: 6230 RVA: 0x00082A27 File Offset: 0x00080C27
		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.Char.");
		}

		// Token: 0x06001857 RID: 6231 RVA: 0x00082A33 File Offset: 0x00080C33
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.DateTime.");
		}

		// Token: 0x06001858 RID: 6232 RVA: 0x00082A3F File Offset: 0x00080C3F
		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this);
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x00082A51 File Offset: 0x00080C51
		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}

		// Token: 0x0600185A RID: 6234 RVA: 0x00082A63 File Offset: 0x00080C63
		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}

		// Token: 0x0600185B RID: 6235 RVA: 0x00082A75 File Offset: 0x00080C75
		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}

		// Token: 0x0600185C RID: 6236 RVA: 0x00082A87 File Offset: 0x00080C87
		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}

		// Token: 0x0600185D RID: 6237 RVA: 0x00082A99 File Offset: 0x00080C99
		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this);
		}

		// Token: 0x0600185E RID: 6238 RVA: 0x00082AAB File Offset: 0x00080CAB
		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return this;
		}

		// Token: 0x0600185F RID: 6239 RVA: 0x00082AB8 File Offset: 0x00080CB8
		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return ((IConvertible)this).ToType(type, provider);
		}

		// Token: 0x06001860 RID: 6240 RVA: 0x00082AD2 File Offset: 0x00080CD2
		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}

		// Token: 0x06001861 RID: 6241 RVA: 0x00082AE4 File Offset: 0x00080CE4
		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}

		// Token: 0x06001862 RID: 6242 RVA: 0x00082AF6 File Offset: 0x00080CF6
		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this);
		}

		// Token: 0x0400194E RID: 6478
		private ushort value;

		// Token: 0x0400194F RID: 6479
		public const int PrecisionDigits = 3;

		// Token: 0x04001950 RID: 6480
		public const int MantissaBits = 11;

		// Token: 0x04001951 RID: 6481
		public const int MaximumDecimalExponent = 4;

		// Token: 0x04001952 RID: 6482
		public const int MaximumBinaryExponent = 15;

		// Token: 0x04001953 RID: 6483
		public const int MinimumDecimalExponent = -4;

		// Token: 0x04001954 RID: 6484
		public const int MinimumBinaryExponent = -14;

		// Token: 0x04001955 RID: 6485
		public const int ExponentRadix = 2;

		// Token: 0x04001956 RID: 6486
		public const int AdditionRounding = 1;

		// Token: 0x04001957 RID: 6487
		public static readonly Half Epsilon = new Half(0.0004887581f);

		// Token: 0x04001958 RID: 6488
		public static readonly Half MaxValue = new Half(65504f);

		// Token: 0x04001959 RID: 6489
		public static readonly Half MinValue = new Half(6.103516E-05f);

		// Token: 0x0400195A RID: 6490
		public static readonly Half NaN = new Half(float.NaN);

		// Token: 0x0400195B RID: 6491
		public static readonly Half NegativeInfinity = new Half(float.NegativeInfinity);

		// Token: 0x0400195C RID: 6492
		public static readonly Half PositiveInfinity = new Half(float.PositiveInfinity);
	}
}
