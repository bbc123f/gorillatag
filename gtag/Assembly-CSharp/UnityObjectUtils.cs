using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200022E RID: 558
public static class UnityObjectUtils
{
	// Token: 0x06000DDC RID: 3548 RVA: 0x000509C0 File Offset: 0x0004EBC0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNull<T>(T unityObject) where T : Object
	{
		return EqualityComparer<T>.Default.Equals(unityObject, default(T)) || unityObject.GetHashCode() == 0;
	}
}
