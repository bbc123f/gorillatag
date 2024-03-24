using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ComponentUtils
{
	public static T AddComponent<T>(this Component c) where T : Component
	{
		return c.gameObject.AddComponent<T>();
	}

	public static void GetOrAddComponent<T>(this Component c, out T result) where T : Component
	{
		if (!c.TryGetComponent<T>(out result))
		{
			result = c.gameObject.AddComponent<T>();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsNull<T>(T unityObject) where T : Object
	{
		return unityObject == null || unityObject.GetHashCode() == 0;
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

	public static Hash128 ComputeStaticHash128(Component c, string k)
	{
		return ComponentUtils.ComputeStaticHash128(c, StaticHash.Calculate(k));
	}

	public static Hash128 ComputeStaticHash128(Component c, int k = 0)
	{
		if (c == null)
		{
			return default(Hash128);
		}
		Transform transform = c.transform;
		Component[] components = c.gameObject.GetComponents(typeof(Component));
		uint[] array = ComponentUtils.kHashBits;
		int siblingIndex = transform.GetSiblingIndex();
		int num = components.Length;
		int num2 = 0;
		while (num2 < num && c != components[num2])
		{
			num2++;
		}
		int num3 = StaticHash.Combine(k + 2, 1);
		int num4 = StaticHash.Combine(siblingIndex + 4, num3);
		int num5 = StaticHash.Combine(num + 8, num4);
		int num6 = StaticHash.Combine(num2 + 16, num5);
		array[0] = (uint)num3;
		array[1] = (uint)num4;
		array[2] = (uint)num5;
		array[3] = (uint)num6;
		SRand srand = new SRand(StaticHash.Combine(num3, num4, num5, num6));
		srand.Shuffle<uint>(array);
		Hash128 hash = new Hash128(array[0], array[1], array[2], array[3]);
		Hash128 hash2 = Hash128.Compute(c.GetType().FullName);
		Hash128 hash3 = TransformUtils.ComputePathHash(transform);
		Hash128 hash4 = transform.localToWorldMatrix.QuantizedHash128();
		HashUtilities.AppendHash(ref hash2, ref hash);
		HashUtilities.AppendHash(ref hash3, ref hash);
		HashUtilities.AppendHash(ref hash4, ref hash);
		return hash;
	}

	// Note: this type is marked as 'beforefieldinit'.
	static ComponentUtils()
	{
	}

	private static readonly uint[] kHashBits = new uint[4];
}
