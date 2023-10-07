using System;
using System.Collections.Generic;

// Token: 0x02000209 RID: 521
public static class ArrayUtils
{
	// Token: 0x06000D2D RID: 3373 RVA: 0x0004D63C File Offset: 0x0004B83C
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

	// Token: 0x06000D2E RID: 3374 RVA: 0x0004D680 File Offset: 0x0004B880
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
