using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class UnityObjectUtils
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Object AsNull(this Object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (!(obj == null))
		{
			return obj;
		}
		return null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SafeDestroy(this Object obj)
	{
		Object.Destroy(obj);
	}
}
