using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class UnityObjectUtils
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNull<T>(T unityObject) where T : Object
	{
		return EqualityComparer<T>.Default.Equals(unityObject, default(T)) || unityObject.GetHashCode() == 0;
	}
}
