using System;
using System.Collections.Generic;

public static class LinqExtensions
{
	public static IEnumerable<T> Self<T>(this T value)
	{
		yield return value;
		yield break;
	}
}
