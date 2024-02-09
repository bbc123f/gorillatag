using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GorillaExtensions
{
	public static class GTStringBuilderExtensions
	{
		public static IEnumerable<string> GetChunks(this StringBuilder sb, int max = 16300)
		{
			int end;
			for (int i = 0; i < sb.Length; i = end + 1)
			{
				end = Math.Min(i + max, sb.Length);
				if (end < sb.Length)
				{
					int num = -1;
					for (int j = end - 1; j >= i; j--)
					{
						if (sb[j] == '\n')
						{
							num = j;
							break;
						}
					}
					if (num != -1)
					{
						end = num;
					}
				}
				yield return sb.ToString(i, end - i);
			}
			yield break;
		}

		public static StringBuilder GTAddPath(this StringBuilder stringBuilderToAddTo, GameObject gameObject)
		{
			gameObject.transform.GetPath(ref stringBuilderToAddTo);
			return stringBuilderToAddTo;
		}

		public static StringBuilder GTAddPath(this StringBuilder stringBuilderToAddTo, Transform transform)
		{
			transform.GetPath(ref stringBuilderToAddTo);
			return stringBuilderToAddTo;
		}

		public static StringBuilder GTQuote(this StringBuilder sb, string value)
		{
			return sb.Append('"').Append(value).Append('"');
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b)
		{
			return sb.Append(a).Append(b);
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b, string c)
		{
			return sb.Append(a).Append(b).Append(c);
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b, string c, string d)
		{
			return sb.Append(a).Append(b).Append(c)
				.Append(d);
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b, string c, string d, string e)
		{
			return sb.Append(a).Append(b).Append(c)
				.Append(d)
				.Append(e);
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b, string c, string d, string e, string f)
		{
			return sb.Append(a).Append(b).Append(c)
				.Append(d)
				.Append(e)
				.Append(f);
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b, string c, string d, string e, string f, string g)
		{
			return sb.Append(a).Append(b).Append(c)
				.Append(d)
				.Append(e)
				.Append(f)
				.Append(g);
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h)
		{
			return sb.Append(a).Append(b).Append(c)
				.Append(d)
				.Append(e)
				.Append(f)
				.Append(g)
				.Append(h);
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h, string i)
		{
			return sb.Append(a).Append(b).Append(c)
				.Append(d)
				.Append(e)
				.Append(f)
				.Append(g)
				.Append(h)
				.Append(i);
		}

		public static StringBuilder GTMany(this StringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h, string i, string j)
		{
			return sb.Append(a).Append(b).Append(c)
				.Append(d)
				.Append(e)
				.Append(f)
				.Append(g)
				.Append(h)
				.Append(i)
				.Append(j);
		}
	}
}
