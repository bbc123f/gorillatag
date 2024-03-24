using System;
using System.Collections.Generic;

public static class EnumHelper<TEnum> where TEnum : struct, Enum
{
	public static string EnumToName(TEnum e)
	{
		return EnumHelper<TEnum>.gEnumToName[e];
	}

	public static TEnum NameToEnum(string n)
	{
		return EnumHelper<TEnum>.gNameToEnum[n];
	}

	public static int EnumToIndex(TEnum e)
	{
		return EnumHelper<TEnum>.gEnumToIndex[e];
	}

	public static TEnum IndexToEnum(int i)
	{
		return EnumHelper<TEnum>.gIndexToEnum[i];
	}

	public static long EnumToLong(TEnum e)
	{
		return EnumHelper<TEnum>.gEnumToLong[e];
	}

	public static TEnum LongToEnum(long l)
	{
		return EnumHelper<TEnum>.gLongToEnum[l];
	}

	public static TEnum GetValue(int index)
	{
		return EnumHelper<TEnum>.Values[index];
	}

	public static int GetIndex(TEnum value)
	{
		return EnumHelper<TEnum>.gEnumToIndex[value];
	}

	public static string GetName(TEnum value)
	{
		return EnumHelper<TEnum>.gEnumToName[value];
	}

	public static TEnum GetValue(string name)
	{
		return EnumHelper<TEnum>.gNameToEnum[name];
	}

	public static long GetLongValue(TEnum value)
	{
		return EnumHelper<TEnum>.gEnumToLong[value];
	}

	public static TEnum GetValue(long longValue)
	{
		return EnumHelper<TEnum>.gLongToEnum[longValue];
	}

	static EnumHelper()
	{
		int num = EnumHelper<TEnum>.Names.Length;
		EnumHelper<TEnum>.Values = new TEnum[num];
		EnumHelper<TEnum>.LongValues = new long[num];
		EnumHelper<TEnum>.gEnumToName = new Dictionary<TEnum, string>(num);
		EnumHelper<TEnum>.gNameToEnum = new Dictionary<string, TEnum>(num);
		EnumHelper<TEnum>.gEnumToIndex = new Dictionary<TEnum, int>(num);
		EnumHelper<TEnum>.gIndexToEnum = new Dictionary<int, TEnum>(num);
		EnumHelper<TEnum>.gEnumToLong = new Dictionary<TEnum, long>(num);
		EnumHelper<TEnum>.gLongToEnum = new Dictionary<long, TEnum>(num);
		for (int i = 0; i < EnumHelper<TEnum>.Names.Length; i++)
		{
			string text = EnumHelper<TEnum>.Names[i];
			TEnum tenum = Enum.Parse<TEnum>(text);
			int num2 = i;
			long num3 = Convert.ToInt64(tenum);
			EnumHelper<TEnum>.Values[num2] = tenum;
			EnumHelper<TEnum>.LongValues[num2] = num3;
			EnumHelper<TEnum>.gEnumToName[tenum] = text;
			EnumHelper<TEnum>.gNameToEnum[text] = tenum;
			EnumHelper<TEnum>.gEnumToIndex[tenum] = num2;
			EnumHelper<TEnum>.gIndexToEnum[num2] = tenum;
			EnumHelper<TEnum>.gEnumToLong[tenum] = num3;
			EnumHelper<TEnum>.gLongToEnum[num3] = tenum;
		}
	}

	public static readonly string[] Names = Enum.GetNames(typeof(TEnum));

	public static readonly TEnum[] Values;

	public static readonly long[] LongValues;

	private static readonly Dictionary<TEnum, string> gEnumToName;

	private static readonly Dictionary<string, TEnum> gNameToEnum;

	private static readonly Dictionary<TEnum, int> gEnumToIndex;

	private static readonly Dictionary<int, TEnum> gIndexToEnum;

	private static readonly Dictionary<TEnum, long> gEnumToLong;

	private static readonly Dictionary<long, TEnum> gLongToEnum;
}
