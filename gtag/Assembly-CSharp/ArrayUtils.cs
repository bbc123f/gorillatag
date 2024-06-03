using System;
using System.Collections.Generic;

public static class ArrayUtils
{
	public static T[] Copy<T>(T[] source)
	{
		if (source == null)
		{
			return null;
		}
		if (source.Length == 0)
		{
			return Array.Empty<T>();
		}
		T[] array = new T[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			array[i] = source[i];
		}
		return array;
	}

	public static int IndexOfRef<T>(this List<T> array, T value) where T : class
	{
		if (array == null || array.Count == 0)
		{
			return -1;
		}
		for (int i = 0; i < array.Count; i++)
		{
			if (array[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	public static int IndexOfRef<T>(this T[] array, T value) where T : class
	{
		if (array == null || array.Length == 0)
		{
			return -1;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == value)
			{
				return i;
			}
		}
		return -1;
	}
}
