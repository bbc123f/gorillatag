using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Utilities
{
	// Token: 0x02000286 RID: 646
	public static class StringUtils
	{
		// Token: 0x060010AE RID: 4270 RVA: 0x00058E09 File Offset: 0x00057009
		public static byte[] ToBytesASCII(this string s)
		{
			return Encoding.ASCII.GetBytes(s);
		}

		// Token: 0x060010AF RID: 4271 RVA: 0x00058E16 File Offset: 0x00057016
		public static byte[] ToBytesUTF8(this string s)
		{
			return Encoding.UTF8.GetBytes(s);
		}

		// Token: 0x060010B0 RID: 4272 RVA: 0x00058E23 File Offset: 0x00057023
		public static byte[] ToBytesUnicode(this string s)
		{
			return Encoding.Unicode.GetBytes(s);
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x00058E30 File Offset: 0x00057030
		public static string ComputeSHV2(this string s)
		{
			return Hash128.Compute(s).ToString();
		}

		// Token: 0x060010B2 RID: 4274 RVA: 0x00058E51 File Offset: 0x00057051
		public static string ToQueryString(this Dictionary<string, string> d)
		{
			if (d == null)
			{
				return null;
			}
			return "?" + string.Join("&", from x in d
			select x.Key + "=" + x.Value);
		}

		// Token: 0x060010B3 RID: 4275 RVA: 0x00058E94 File Offset: 0x00057094
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

		// Token: 0x04001250 RID: 4688
		public const string kForwardSlash = "/";

		// Token: 0x04001251 RID: 4689
		public const string kBackSlash = "/";

		// Token: 0x04001252 RID: 4690
		public const string kBackTick = "`";

		// Token: 0x04001253 RID: 4691
		public const string kMinusDash = "-";

		// Token: 0x04001254 RID: 4692
		public const string kPeriod = ".";

		// Token: 0x04001255 RID: 4693
		public const string kUnderScore = "_";

		// Token: 0x04001256 RID: 4694
		public const string kColon = ":";
	}
}
