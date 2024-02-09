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

	public static bool GetComponentAndSetFieldIfNullElseLogAndDisable<T>(this Behaviour c, ref T fieldRef, string fieldName, string fieldTypeName, string msgSuffix = "Disabling.", [CallerMemberName] string caller = "__UNKNOWN_CALLER__") where T : Component
	{
		if (c.GetComponentAndSetFieldIfNullElseLog(ref fieldRef, fieldName, fieldTypeName, msgSuffix, caller))
		{
			return true;
		}
		c.enabled = false;
		return false;
	}

	public static bool GetComponentAndSetFieldIfNullElseLog<T>(this Behaviour c, ref T fieldRef, string fieldName, string fieldTypeName, string msgSuffix = "", [CallerMemberName] string caller = "__UNKNOWN_CALLER__") where T : Component
	{
		if (fieldRef != null)
		{
			return true;
		}
		fieldRef = c.GetComponent<T>();
		if (fieldRef != null)
		{
			return true;
		}
		Debug.LogError(string.Concat(new string[] { caller, ": Could not find ", fieldTypeName, " \"", fieldName, "\" on \"", c.name, "\". ", msgSuffix }), c);
		return false;
	}

	public static bool DisableIfNull<T>(this Behaviour c, T fieldRef, string fieldName, string fieldTypeName, [CallerMemberName] string caller = "__UNKNOWN_CALLER__") where T : Object
	{
		if (fieldRef != null)
		{
			return true;
		}
		c.enabled = false;
		return false;
	}
}
