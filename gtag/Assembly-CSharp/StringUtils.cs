using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class StringUtils
{
	public static string ToAlphaNumeric(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in s)
		{
			if (char.IsLetterOrDigit(c))
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	public static byte[] ToBytesASCII(this string s)
	{
		return Encoding.ASCII.GetBytes(s);
	}

	public static byte[] ToBytesUTF8(this string s)
	{
		return Encoding.UTF8.GetBytes(s);
	}

	public static byte[] ToBytesUnicode(this string s)
	{
		return Encoding.Unicode.GetBytes(s);
	}

	public static string ComputeSHV2(this string s)
	{
		return Hash128.Compute(s).ToString();
	}

	public static string ToQueryString(this Dictionary<string, string> d)
	{
		if (d == null)
		{
			return null;
		}
		return "?" + string.Join("&", from x in d
		select x.Key + "=" + x.Value);
	}

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

	public const string kForwardSlash = "/";

	public const string kBackSlash = "/";

	public const string kBackTick = "`";

	public const string kMinusDash = "-";

	public const string kPeriod = ".";

	public const string kUnderScore = "_";

	public const string kColon = ":";
}
