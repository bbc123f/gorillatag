using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class EnumData<TEnum> where TEnum : struct, Enum
{
	public static EnumData<TEnum> Shared
	{
		[CompilerGenerated]
		get
		{
			return EnumData<TEnum>.<Shared>k__BackingField;
		}
	} = new EnumData<TEnum>();

	public EnumData()
	{
		this.Names = Enum.GetNames(typeof(TEnum));
		int num = this.Names.Length;
		this.Values = new TEnum[num];
		this.LongValues = new long[num];
		this.EnumToName = new Dictionary<TEnum, string>(num);
		this.NameToEnum = new Dictionary<string, TEnum>(num);
		this.EnumToIndex = new Dictionary<TEnum, int>(num);
		this.IndexToEnum = new Dictionary<int, TEnum>(num);
		this.EnumToLong = new Dictionary<TEnum, long>(num);
		this.LongToEnum = new Dictionary<long, TEnum>(num);
		for (int i = 0; i < this.Names.Length; i++)
		{
			string text = this.Names[i];
			TEnum tenum = Enum.Parse<TEnum>(text);
			int num2 = i;
			long num3 = Convert.ToInt64(tenum);
			this.Values[num2] = tenum;
			this.LongValues[num2] = num3;
			this.EnumToName[tenum] = text;
			this.NameToEnum[text] = tenum;
			this.EnumToIndex[tenum] = num2;
			this.IndexToEnum[num2] = tenum;
			this.EnumToLong[tenum] = num3;
			this.LongToEnum[num3] = tenum;
		}
	}

	// Note: this type is marked as 'beforefieldinit'.
	static EnumData()
	{
	}

	[CompilerGenerated]
	private static readonly EnumData<TEnum> <Shared>k__BackingField;

	public readonly string[] Names;

	public readonly TEnum[] Values;

	public readonly long[] LongValues;

	public readonly Dictionary<TEnum, string> EnumToName;

	public readonly Dictionary<string, TEnum> NameToEnum;

	public readonly Dictionary<TEnum, int> EnumToIndex;

	public readonly Dictionary<int, TEnum> IndexToEnum;

	public readonly Dictionary<TEnum, long> EnumToLong;

	public readonly Dictionary<long, TEnum> LongToEnum;
}
