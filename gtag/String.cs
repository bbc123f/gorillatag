using System;
using UnityEngine;

[Serializable]
public class String<TEnum> where TEnum : struct, Enum
{
	public static implicit operator String<TEnum>(TEnum e)
	{
		return new String<TEnum>
		{
			m_EnumValue = e
		};
	}

	public static implicit operator TEnum(String<TEnum> se)
	{
		return se.m_EnumValue;
	}

	public String()
	{
	}

	[SerializeField]
	private TEnum m_EnumValue;
}
