using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000210 RID: 528
public static class ComponentUtils
{
	// Token: 0x06000D41 RID: 3393 RVA: 0x0004DCDC File Offset: 0x0004BEDC
	public static T EnsureComponent<T>(this GameObject g) where T : Component
	{
		T component = g.GetComponent<T>();
		if (!(component == null))
		{
			return component;
		}
		return g.AddComponent<T>();
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x0004DD08 File Offset: 0x0004BF08
	public static T GetOrAdd<T>(this Component c, ref T instance) where T : Component
	{
		if (!ComponentUtils.IsNull<T>(instance))
		{
			return instance;
		}
		GameObject gameObject = c.gameObject;
		instance = gameObject.GetComponent<T>();
		if (instance != null)
		{
			return instance;
		}
		return instance = gameObject.AddComponent<T>();
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x0004DD68 File Offset: 0x0004BF68
	public static T GetOrAdd<T>(this GameObject g, ref T instance) where T : Component
	{
		if (!ComponentUtils.IsNull<T>(instance))
		{
			return instance;
		}
		instance = g.GetComponent<T>();
		if (instance != null)
		{
			return instance;
		}
		return instance = g.AddComponent<T>();
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x0004DDBE File Offset: 0x0004BFBE
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsNull<T>(T unityObject) where T : Object
	{
		return unityObject != null && unityObject.GetHashCode() != 0;
	}
}
