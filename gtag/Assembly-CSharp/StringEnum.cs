using System;
using UnityEngine;

[Serializable]
public class StringEnum<TEnum> where TEnum : struct, Enum
{
	public static implicit operator StringEnum<TEnum>(TEnum e)
	{
		return new StringEnum<TEnum>
		{
			m_EnumValue = e
		};
	}

	public static implicit operator TEnum(StringEnum<TEnum> se)
	{
		return se.m_EnumValue;
	}

	public StringEnum()
	{
	}

	[SerializeField]
	private TEnum m_EnumValue;
}
