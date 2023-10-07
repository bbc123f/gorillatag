using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200020F RID: 527
public static class ComponentUtils
{
	// Token: 0x06000D3B RID: 3387 RVA: 0x0004DA7C File Offset: 0x0004BC7C
	public static T EnsureComponent<T>(this GameObject g) where T : Component
	{
		T component = g.GetComponent<T>();
		if (!(component == null))
		{
			return component;
		}
		return g.AddComponent<T>();
	}

	// Token: 0x06000D3C RID: 3388 RVA: 0x0004DAA8 File Offset: 0x0004BCA8
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

	// Token: 0x06000D3D RID: 3389 RVA: 0x0004DB08 File Offset: 0x0004BD08
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

	// Token: 0x06000D3E RID: 3390 RVA: 0x0004DB5E File Offset: 0x0004BD5E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsNull<T>(T unityObject) where T : Object
	{
		return unityObject != null && unityObject.GetHashCode() != 0;
	}
}
