using System;
using System.Collections.Generic;

public static class LinqUtils
{
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
