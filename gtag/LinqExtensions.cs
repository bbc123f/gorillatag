using System;
using System.Collections.Generic;

public static class LinqExtensions
{
	public static IEnumerable<TSource> DistinctBy<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		HashSet<TResult> set = new HashSet<TResult>();
		foreach (TSource item2 in source)
		{
			TResult item = selector(item2);
			if (set.Add(item))
			{
				yield return item2;
			}
		}
	}
}
