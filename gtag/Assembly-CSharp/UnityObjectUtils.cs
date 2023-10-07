using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200022D RID: 557
public static class UnityObjectUtils
{
	// Token: 0x06000DD6 RID: 3542 RVA: 0x00050760 File Offset: 0x0004E960
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNull<T>(T unityObject) where T : Object
	{
		return EqualityComparer<T>.Default.Equals(unityObject, default(T)) || unityObject.GetHashCode() == 0;
	}
}
