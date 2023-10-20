using System;
using System.Collections.Generic;

// Token: 0x0200020A RID: 522
public static class ArrayUtils
{
	// Token: 0x06000D33 RID: 3379 RVA: 0x0004D89C File Offset: 0x0004BA9C
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

	// Token: 0x06000D34 RID: 3380 RVA: 0x0004D8E0 File Offset: 0x0004BAE0
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
