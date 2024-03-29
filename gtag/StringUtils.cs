﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
		return "?" + string.Join("&", d.Select((KeyValuePair<string, string> x) => x.Key + "=" + x.Value));
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

	public static string ToUpperCamelCase(this string input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			return string.Empty;
		}
		string[] array = Regex.Split(input, "[^A-Za-z0-9]+");
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Length > 0)
			{
				array[i] = char.ToUpper(array[i][0]).ToString() + ((array[i].Length > 1) ? array[i].Substring(1).ToLower() : "");
			}
		}
		return string.Join("", array);
	}

	public const string kForwardSlash = "/";

	public const string kBackSlash = "/";

	public const string kBackTick = "`";

	public const string kMinusDash = "-";

	public const string kPeriod = ".";

	public const string kUnderScore = "_";

	public const string kColon = ":";

	[CompilerGenerated]
	[Serializable]
	private sealed class <>c
	{
		// Note: this type is marked as 'beforefieldinit'.
		static <>c()
		{
		}

		public <>c()
		{
		}

		internal string <ToQueryString>b__12_0(KeyValuePair<string, string> x)
		{
			return x.Key + "=" + x.Value;
		}

		public static readonly StringUtils.<>c <>9 = new StringUtils.<>c();

		public static Func<KeyValuePair<string, string>, string> <>9__12_0;
	}
}
