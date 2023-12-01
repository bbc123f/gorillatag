using System;
using System.Collections.Generic;

public static class ArrayUtils
{
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
