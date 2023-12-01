using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ComponentUtils
{
	public static T EnsureComponent<T>(this GameObject g) where T : Component
	{
		T component = g.GetComponent<T>();
		if (!(component == null))
		{
			return component;
		}
		return g.AddComponent<T>();
	}

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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsNull<T>(T unityObject) where T : Object
	{
		return unityObject != null && unityObject.GetHashCode() != 0;
	}
}
