using System;
using System.Collections.Generic;

// Token: 0x02000216 RID: 534
public static class LinqUtils
{
	// Token: 0x06000D49 RID: 3401 RVA: 0x0004DD3C File Offset: 0x0004BF3C
	public static int IndexOfRef<T>(this IEnumerable<T> source, T value) where T : class
	{
		int num = -1;
		if (source == null)
		{
			return num;
		}
		foreach (T t in source)
		{
			num++;
			if (t == value)
			{
				return num;
			}
		}
		return num;
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x0004DD9C File Offset: 0x0004BF9C
	public static IEnumerable<TSource> DistinctBy<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		HashSet<TResult> set = new HashSet<TResult>();
		foreach (TSource tsource in source)
		{
			TResult item = selector(tsource);
			if (set.Add(item))
			{
				yield return tsource;
			}
		}
		IEnumerator<TSource> enumerator = null;
		yield break;
		yield break;
	}
}
