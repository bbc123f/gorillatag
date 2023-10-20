using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Utilities
{
	// Token: 0x02000288 RID: 648
	public static class StringUtils
	{
		// Token: 0x060010B5 RID: 4277 RVA: 0x000591E5 File Offset: 0x000573E5
		public static byte[] ToBytesASCII(this string s)
		{
			return Encoding.ASCII.GetBytes(s);
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x000591F2 File Offset: 0x000573F2
		public static byte[] ToBytesUTF8(this string s)
		{
			return Encoding.UTF8.GetBytes(s);
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x000591FF File Offset: 0x000573FF
		public static byte[] ToBytesUnicode(this string s)
		{
			return Encoding.Unicode.GetBytes(s);
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x0005920C File Offset: 0x0005740C
		public static string ComputeSHV2(this string s)
		{
			return Hash128.Compute(s).ToString();
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x0005922D File Offset: 0x0005742D
		public static string ToQueryString(this Dictionary<string, string> d)
		{
			if (d == null)
			{
				return null;
			}
			return "?" + string.Join("&", from x in d
			select x.Key + "=" + x.Value);
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x00059270 File Offset: 0x00057470
		public static string Combine(string separator, params string[] values)
		{
			if (values == null || values.Length == 0)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = !string.IsNullOrEmpty(separator);
			for (int i = 0; i < values.Length; i++)
			{
				if (flag)
				{
					stringBuilder.Append(separator);
				}
				stringBuilder.Append(values);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0400125D RID: 4701
		public const string kForwardSlash = "/";

		// Token: 0x0400125E RID: 4702
		public const string kBackSlash = "/";

		// Token: 0x0400125F RID: 4703
		public const string kBackTick = "`";

		// Token: 0x04001260 RID: 4704
		public const string kMinusDash = "-";

		// Token: 0x04001261 RID: 4705
		public const string kPeriod = ".";

		// Token: 0x04001262 RID: 4706
		public const string kUnderScore = "_";

		// Token: 0x04001263 RID: 4707
		public const string kColon = ":";
	}
}
