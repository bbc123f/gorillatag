using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaExtensions;

public static class GTExt
{
	public enum ParityOptions
	{
		XFlip,
		YFlip,
		ZFlip,
		AllFlip
	}

	public static T GetComponentInHierarchy<T>(this Scene scene, bool includeInactive = true) where T : Component
	{
		GameObject[] rootGameObjects = scene.GetRootGameObjects();
		foreach (GameObject gameObject in rootGameObjects)
		{
			T component = gameObject.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(includeInactive);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				component = componentsInChildren[j].GetComponent<T>();
				if (component != null)
				{
					return component;
				}
			}
		}
		return null;
	}

	public static List<T> GetComponentsInHierarchy<T>(this Scene scene, bool includeInactive = true, int capacity = 64)
	{
		List<T> list = new List<T>(capacity);
		GameObject[] rootGameObjects = scene.GetRootGameObjects();
		for (int i = 0; i < rootGameObjects.Length; i++)
		{
			T[] componentsInChildren = rootGameObjects[i].GetComponentsInChildren<T>(includeInactive);
			list.AddRange(componentsInChildren);
		}
		return list;
	}

	public static List<UnityEngine.Object> GetComponentsInHierarchy(this Scene scene, Type type, bool includeInactive = true, int capacity = 64)
	{
		List<UnityEngine.Object> list = new List<UnityEngine.Object>(capacity);
		GameObject[] rootGameObjects = scene.GetRootGameObjects();
		for (int i = 0; i < rootGameObjects.Length; i++)
		{
			Component[] componentsInChildren = rootGameObjects[i].GetComponentsInChildren(type, includeInactive);
			list.AddRange(componentsInChildren);
		}
		return list;
	}

	public static List<GameObject> GetGameObjectsInHierarchy(this Scene scene, bool includeInactive = true, int capacity = 64)
	{
		return scene.GetComponentsInHierarchy<GameObject>(includeInactive, capacity);
	}

	public static T GetComponentWithRegex<T>(this Component root, string regexString) where T : Component
	{
		T[] componentsInChildren = root.GetComponentsInChildren<T>();
		Regex regex = new Regex(regexString);
		T[] array = componentsInChildren;
		foreach (T val in array)
		{
			if (regex.IsMatch(val.name))
			{
				return val;
			}
		}
		return null;
	}

	private static List<T> GetComponentsWithRegex_Internal<T>(IEnumerable<T> allComponents, string regexString, bool includeInactive, int capacity = 64) where T : Component
	{
		List<T> foundComponents = new List<T>(capacity);
		Regex regex = new Regex(regexString);
		GetComponentsWithRegex_Internal(allComponents, regex, ref foundComponents);
		return foundComponents;
	}

	private static void GetComponentsWithRegex_Internal<T>(IEnumerable<T> allComponents, Regex regex, ref List<T> foundComponents) where T : Component
	{
		foreach (T allComponent in allComponents)
		{
			string name = allComponent.name;
			if (regex.IsMatch(name))
			{
				foundComponents.Add(allComponent);
			}
		}
	}

	public static List<T> GetComponentsWithRegex<T>(this Scene scene, string regexString, bool includeInactive, int capacity) where T : Component
	{
		return GetComponentsWithRegex_Internal(scene.GetComponentsInHierarchy<T>(includeInactive, capacity), regexString, includeInactive, capacity);
	}

	public static List<T> GetComponentsWithRegex<T>(this Component root, string regexString, bool includeInactive, int capacity) where T : Component
	{
		return GetComponentsWithRegex_Internal(root.GetComponentsInChildren<T>(includeInactive), regexString, includeInactive, capacity);
	}

	public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string regexString, bool includeInactive = true, int capacity = 64)
	{
		List<Transform> componentsWithRegex = scene.GetComponentsWithRegex<Transform>(regexString, includeInactive, capacity);
		List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
		foreach (Transform item in componentsWithRegex)
		{
			list.Add(item.gameObject);
		}
		return list;
	}

	public static void GetComponentsWithRegex_Internal<T>(this List<T> allComponents, Regex[] regexes, int maxCount, ref List<T> foundComponents) where T : Component
	{
		if (maxCount == 0)
		{
			return;
		}
		int num = 0;
		foreach (T allComponent in allComponents)
		{
			for (int i = 0; i < regexes.Length; i++)
			{
				if (regexes[i].IsMatch(allComponent.name))
				{
					foundComponents.Add(allComponent);
					num++;
					if (maxCount > 0 && num >= maxCount)
					{
						return;
					}
				}
			}
		}
	}

	public static List<T> GetComponentsWithRegex<T>(this Scene scene, string[] regexStrings, bool includeInactive = true, int maxCount = -1, int capacity = 64) where T : Component
	{
		List<T> componentsInHierarchy = scene.GetComponentsInHierarchy<T>(includeInactive, capacity);
		List<T> foundComponents = new List<T>(componentsInHierarchy.Count);
		Regex[] array = new Regex[regexStrings.Length];
		for (int i = 0; i < regexStrings.Length; i++)
		{
			array[i] = new Regex(regexStrings[i]);
		}
		componentsInHierarchy.GetComponentsWithRegex_Internal(array, maxCount, ref foundComponents);
		return foundComponents;
	}

	public static List<T> GetComponentsWithRegex<T>(this Scene scene, string[] regexStrings, string[] excludeRegexStrings, bool includeInactive = true, int maxCount = -1) where T : Component
	{
		List<T> componentsInHierarchy = scene.GetComponentsInHierarchy<T>(includeInactive);
		List<T> list = new List<T>(componentsInHierarchy.Count);
		if (maxCount == 0)
		{
			return list;
		}
		int num = 0;
		foreach (T item in componentsInHierarchy)
		{
			bool flag = false;
			foreach (string pattern in regexStrings)
			{
				if (flag || !Regex.IsMatch(item.name, pattern))
				{
					continue;
				}
				foreach (string pattern2 in excludeRegexStrings)
				{
					if (!flag)
					{
						flag = Regex.IsMatch(item.name, pattern2);
					}
				}
				if (!flag)
				{
					list.Add(item);
					num++;
					if (maxCount > 0 && num >= maxCount)
					{
						return list;
					}
				}
			}
		}
		return list;
	}

	public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string[] regexStrings, bool includeInactive = true, int maxCount = -1)
	{
		List<Transform> componentsWithRegex = scene.GetComponentsWithRegex<Transform>(regexStrings, includeInactive, maxCount);
		List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
		foreach (Transform item in componentsWithRegex)
		{
			list.Add(item.gameObject);
		}
		return list;
	}

	public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string[] regexStrings, string[] excludeRegexStrings, bool includeInactive = true, int maxCount = -1)
	{
		List<Transform> componentsWithRegex = scene.GetComponentsWithRegex<Transform>(regexStrings, excludeRegexStrings, includeInactive, maxCount);
		List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
		foreach (Transform item in componentsWithRegex)
		{
			list.Add(item.gameObject);
		}
		return list;
	}

	public static List<T> GetComponentsByName<T>(this Transform xform, string name, bool includeInactive = true) where T : Component
	{
		T[] componentsInChildren = xform.GetComponentsInChildren<T>(includeInactive);
		List<T> list = new List<T>(componentsInChildren.Length);
		T[] array = componentsInChildren;
		foreach (T val in array)
		{
			if (val.name == name)
			{
				list.Add(val);
			}
		}
		return list;
	}

	public static string GetPath(this Transform transform)
	{
		string text = transform.name;
		while (transform.parent != null)
		{
			transform = transform.parent;
			text = transform.name + "/" + text;
		}
		return "/" + text;
	}

	public static string GetPath(this Transform transform, int maxDepth)
	{
		string text = transform.name;
		int num = 0;
		while (transform.parent != null && num < maxDepth)
		{
			transform = transform.parent;
			text = transform.name + "/" + text;
			num++;
		}
		return "/" + text;
	}

	public static string GetPath(this Transform transform, Transform stopper)
	{
		string text = transform.name;
		while (transform.parent != null && transform.parent != stopper)
		{
			transform = transform.parent;
			text = transform.name + "/" + text;
		}
		return "/" + text;
	}

	public static string GetPath(this GameObject gameObject)
	{
		return gameObject.transform.GetPath();
	}

	public static string GetPath(this GameObject gameObject, int limit)
	{
		return gameObject.transform.GetPath(limit);
	}

	public static List<GameObject> GetGameObjectsInHierarchy(this Scene scene, string name, bool includeInactive = true)
	{
		List<GameObject> list = new List<GameObject>();
		GameObject[] rootGameObjects = scene.GetRootGameObjects();
		foreach (GameObject gameObject in rootGameObjects)
		{
			if (gameObject.name.Contains(name))
			{
				list.Add(gameObject);
			}
			Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(includeInactive);
			foreach (Transform transform in componentsInChildren)
			{
				if (transform.name.Contains(name))
				{
					list.Add(transform.gameObject);
				}
			}
		}
		return list;
	}

	public static string GetComponentPath(this Component component)
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		component.GetComponentPath(stringBuilder);
		return stringBuilder.ToString();
	}

	public static string GetComponentPath<T>(this T component) where T : Component
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		component.GetComponentPath(stringBuilder);
		return stringBuilder.ToString();
	}

	public static void GetComponentPath<T>(this T component, StringBuilder strBuilder) where T : Component
	{
		Transform transform = component.transform;
		int length = strBuilder.Length;
		strBuilder.Append("/->/");
		Type typeFromHandle = typeof(T);
		strBuilder.Append(typeFromHandle.Name);
		Component[] components = transform.GetComponents(typeFromHandle);
		if (components.Length > 1)
		{
			strBuilder.Append("#");
			strBuilder.Append(Array.IndexOf(components, component));
		}
		while (transform != null)
		{
			strBuilder.Insert(length, transform.name);
			strBuilder.Insert(length, "/");
			transform = transform.parent;
		}
	}

	public static T GetComponentByPath<T>(this GameObject root, string path) where T : Component
	{
		string[] array = path.Split(new string[1] { "/->/" }, StringSplitOptions.None);
		if (array.Length < 2)
		{
			return null;
		}
		string[] array2 = array[0].Split(new string[1] { "/" }, StringSplitOptions.RemoveEmptyEntries);
		Transform transform = root.transform;
		for (int i = 1; i < array2.Length; i++)
		{
			string n = array2[i];
			transform = transform.Find(n);
			if (transform == null)
			{
				return null;
			}
		}
		Type type = Type.GetType(array[1].Split('#')[0]);
		if (type == null)
		{
			return null;
		}
		Component component = transform.GetComponent(type);
		if (component == null)
		{
			return null;
		}
		return component as T;
	}

	public static T GetOrAddComponent<T>(this GameObject gameObject, ref T component) where T : Component
	{
		if (component == null)
		{
			component = gameObject.GetOrAddComponent<T>();
		}
		return component;
	}

	public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
	{
		if (!gameObject.TryGetComponent<T>(out var component))
		{
			return gameObject.AddComponent<T>();
		}
		return component;
	}

	public static void SetLossyScale(this Transform transform, Vector3 scale)
	{
		scale = transform.InverseTransformVector(scale);
		Debug.Log(scale);
		Vector3 lossyScale = transform.lossyScale;
		Debug.Log(scale);
		transform.localScale = new Vector3(scale.x / lossyScale.x, scale.y / lossyScale.y, scale.z / lossyScale.z);
	}

	public static void ForEachBackwards<T>(this List<T> list, Action<T> action)
	{
		for (int num = list.Count - 1; num >= 0; num--)
		{
			T obj = list[num];
			try
			{
				action(obj);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	public static void SafeForEachBackwards<T>(this List<T> list, Action<T> action)
	{
		for (int num = list.Count - 1; num >= 0; num--)
		{
			T obj = list[num];
			try
			{
				action(obj);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	public static bool CompareAs255Unclamped(this Color a, Color b)
	{
		int num = (int)(a.r * 255f);
		int num2 = (int)(a.g * 255f);
		int num3 = (int)(a.b * 255f);
		int num4 = (int)(a.a * 255f);
		int num5 = (int)(b.r * 255f);
		int num6 = (int)(b.g * 255f);
		int num7 = (int)(b.b * 255f);
		int num8 = (int)(b.a * 255f);
		if (num == num5 && num2 == num6 && num3 == num7)
		{
			return num4 == num8;
		}
		return false;
	}

	public static Quaternion QuaternionFromToVec(Vector3 toVector, Vector3 fromVector)
	{
		Vector3 vector = Vector3.Cross(fromVector, toVector);
		Debug.Log(vector);
		Debug.Log(vector.magnitude);
		Debug.Log(Vector3.Dot(fromVector, toVector) + 1f);
		Quaternion quaternion = new Quaternion(vector.x, vector.y, vector.z, 1f + Vector3.Dot(toVector, fromVector));
		Debug.Log(quaternion);
		Debug.Log(quaternion.eulerAngles);
		Debug.Log(quaternion.normalized);
		return quaternion.normalized;
	}

	public static Vector3 Position(this Matrix4x4 matrix)
	{
		float m = matrix.m03;
		float m2 = matrix.m13;
		float m3 = matrix.m23;
		return new Vector3(m, m2, m3);
	}

	public static Vector3 Scale(this Matrix4x4 m)
	{
		Vector3 result = new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
		if (Vector3.Cross(m.GetColumn(0), m.GetColumn(1)).normalized != (Vector3)m.GetColumn(2).normalized)
		{
			result.x *= -1f;
		}
		return result;
	}

	public static void SetLocalRelativeToParentMatrixWithParityAxis(this in Matrix4x4 matrix, ParityOptions parity = ParityOptions.XFlip)
	{
	}

	public static void MultiplyInPlaceWith(this ref Vector3 a, in Vector3 b)
	{
		a.x *= b.x;
		a.y *= b.y;
		a.z *= b.z;
	}

	public static void DecomposeWithXFlip(this in Matrix4x4 matrix, out Vector3 transformation, out Quaternion rotation, out Vector3 scale)
	{
		Matrix4x4 matrix2 = matrix;
		transformation = matrix2.Position();
		int index = 2;
		Vector3 forward = matrix2.GetColumnNoCopy(in index);
		int index2 = 1;
		rotation = Quaternion.LookRotation(forward, matrix2.GetColumnNoCopy(in index2));
		scale = matrix.lossyScale;
	}

	public static void SetLocalMatrixRelativeToParentWithXParity(this Transform transform, in Matrix4x4 matrix4X4)
	{
		matrix4X4.DecomposeWithXFlip(out var transformation, out var rotation, out var scale);
		transform.localPosition = transformation;
		transform.localRotation = rotation;
		transform.localScale = scale;
	}

	public static Matrix4x4 Matrix4x4Scale(in Vector3 vector)
	{
		Matrix4x4 result = default(Matrix4x4);
		result.m00 = vector.x;
		result.m01 = 0f;
		result.m02 = 0f;
		result.m03 = 0f;
		result.m10 = 0f;
		result.m11 = vector.y;
		result.m12 = 0f;
		result.m13 = 0f;
		result.m20 = 0f;
		result.m21 = 0f;
		result.m22 = vector.z;
		result.m23 = 0f;
		result.m30 = 0f;
		result.m31 = 0f;
		result.m32 = 0f;
		result.m33 = 1f;
		return result;
	}

	public static Vector4 GetColumnNoCopy(this in Matrix4x4 matrix, in int index)
	{
		return index switch
		{
			0 => new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30), 
			1 => new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31), 
			2 => new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32), 
			3 => new Vector4(matrix.m03, matrix.m13, matrix.m23, matrix.m33), 
			_ => throw new IndexOutOfRangeException("Invalid column index!"), 
		};
	}

	public static Quaternion RotationWithScaleContext(this in Matrix4x4 m, in Vector3 scale)
	{
		Matrix4x4 matrix = m * Matrix4x4Scale(in scale);
		int index = 2;
		Vector3 forward = matrix.GetColumnNoCopy(in index);
		int index2 = 1;
		return Quaternion.LookRotation(forward, matrix.GetColumnNoCopy(in index2));
	}

	public static Quaternion Rotation(this in Matrix4x4 m)
	{
		int index = 2;
		Vector3 forward = m.GetColumnNoCopy(in index);
		int index2 = 1;
		return Quaternion.LookRotation(forward, m.GetColumnNoCopy(in index2));
	}

	public static Vector3 x0z(this Vector3 v)
	{
		return new Vector3(v.x, 0f, v.z);
	}

	public static Matrix4x4 LocalMatrixRelativeToParentNoScale(this Transform transform)
	{
		return Matrix4x4.TRS(transform.localPosition, transform.localRotation, Vector3.one);
	}

	public static Matrix4x4 LocalMatrixRelativeToParentWithScale(this Transform transform)
	{
		if (transform.parent == null)
		{
			return transform.localToWorldMatrix;
		}
		return transform.parent.worldToLocalMatrix * transform.localToWorldMatrix;
	}

	public static void SetLocalMatrixRelativeToParent(this Transform transform, Matrix4x4 matrix)
	{
		transform.localPosition = matrix.Position();
		transform.localRotation = matrix.Rotation();
		transform.localScale = matrix.Scale();
	}

	public static void SetLocalMatrixRelativeToParentNoScale(this Transform transform, Matrix4x4 matrix)
	{
		transform.localPosition = matrix.Position();
		transform.localRotation = matrix.Rotation();
	}

	public static void SetLocalToWorldMatrixNoScale(this Transform transform, Matrix4x4 matrix)
	{
		transform.position = matrix.Position();
		transform.rotation = matrix.Rotation();
	}

	public static Matrix4x4 localToWorldNoScale(this Transform transform)
	{
		return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
	}

	public static void SetLocalToWorldMatrixWithScale(this Transform transform, Matrix4x4 matrix)
	{
		transform.position = matrix.Position();
		transform.rotation = matrix.rotation;
		transform.SetLossyScale(matrix.lossyScale);
	}

	public static Matrix4x4 Matrix4X4LerpNoScale(Matrix4x4 a, Matrix4x4 b, float t)
	{
		return Matrix4x4.TRS(Vector3.Lerp(a.Position(), b.Position(), t), Quaternion.Slerp(a.rotation, b.rotation, t), b.lossyScale);
	}

	public static Matrix4x4 LerpTo(this Matrix4x4 a, Matrix4x4 b, float t)
	{
		return Matrix4X4LerpNoScale(a, b, t);
	}

	public static bool IsNaN(this in Vector3 vector)
	{
		if (!float.IsNaN(vector.x) && !float.IsNaN(vector.y))
		{
			return float.IsNaN(vector.z);
		}
		return true;
	}

	public static bool IsNan(this in Quaternion quat)
	{
		if (!float.IsNaN(quat.x) && !float.IsNaN(quat.y) && !float.IsNaN(quat.z))
		{
			return float.IsNaN(quat.w);
		}
		return true;
	}

	public static bool IsInfinity(this in Vector3 vector)
	{
		if (!float.IsInfinity(vector.x) && !float.IsInfinity(vector.y))
		{
			return float.IsInfinity(vector.z);
		}
		return true;
	}

	public static bool IsInfinity(this in Quaternion quat)
	{
		if (!float.IsInfinity(quat.x) && !float.IsInfinity(quat.y) && !float.IsInfinity(quat.z))
		{
			return float.IsInfinity(quat.w);
		}
		return true;
	}

	public static bool IsValid(this in Vector3 vector)
	{
		if (!vector.IsNaN())
		{
			return !vector.IsInfinity();
		}
		return false;
	}

	public static bool IsValid(this in Quaternion quat)
	{
		if (!quat.IsNan())
		{
			return !quat.IsInfinity();
		}
		return false;
	}

	public static Matrix4x4 Matrix4X4LerpHandleNegativeScale(Matrix4x4 a, Matrix4x4 b, float t)
	{
		return Matrix4x4.TRS(Vector3.Lerp(a.Position(), b.Position(), t), Quaternion.Slerp(a.Rotation(), b.Rotation(), t), b.lossyScale);
	}

	public static Matrix4x4 LerpTo_HandleNegativeScale(this Matrix4x4 a, Matrix4x4 b, float t)
	{
		return Matrix4X4LerpHandleNegativeScale(a, b, t);
	}

	public static Vector3 LerpToUnclamped(this in Vector3 a, in Vector3 b, float t)
	{
		return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
	}

	public static Vector2 xx(this float v)
	{
		return new Vector2(v, v);
	}

	public static Vector2 xx(this Vector2 v)
	{
		return new Vector2(v.x, v.x);
	}

	public static Vector2 xy(this Vector2 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static Vector2 yy(this Vector2 v)
	{
		return new Vector2(v.y, v.y);
	}

	public static Vector2 xx(this Vector3 v)
	{
		return new Vector2(v.x, v.x);
	}

	public static Vector2 xy(this Vector3 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static Vector2 xz(this Vector3 v)
	{
		return new Vector2(v.x, v.z);
	}

	public static Vector2 yy(this Vector3 v)
	{
		return new Vector2(v.y, v.y);
	}

	public static Vector2 yz(this Vector3 v)
	{
		return new Vector2(v.y, v.z);
	}

	public static Vector2 zz(this Vector3 v)
	{
		return new Vector2(v.z, v.z);
	}

	public static Vector2 xx(this Vector4 v)
	{
		return new Vector2(v.x, v.x);
	}

	public static Vector2 xy(this Vector4 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static Vector2 xz(this Vector4 v)
	{
		return new Vector2(v.x, v.z);
	}

	public static Vector2 xw(this Vector4 v)
	{
		return new Vector2(v.x, v.w);
	}

	public static Vector2 yy(this Vector4 v)
	{
		return new Vector2(v.y, v.y);
	}

	public static Vector2 yz(this Vector4 v)
	{
		return new Vector2(v.y, v.z);
	}

	public static Vector2 yw(this Vector4 v)
	{
		return new Vector2(v.y, v.w);
	}

	public static Vector2 zz(this Vector4 v)
	{
		return new Vector2(v.z, v.z);
	}

	public static Vector2 zw(this Vector4 v)
	{
		return new Vector2(v.z, v.w);
	}

	public static Vector2 ww(this Vector4 v)
	{
		return new Vector2(v.w, v.w);
	}

	public static Vector3 xxx(this float v)
	{
		return new Vector3(v, v, v);
	}

	public static Vector3 xxx(this Vector2 v)
	{
		return new Vector3(v.x, v.x, v.x);
	}

	public static Vector3 xxy(this Vector2 v)
	{
		return new Vector3(v.x, v.x, v.y);
	}

	public static Vector3 xyy(this Vector2 v)
	{
		return new Vector3(v.x, v.y, v.y);
	}

	public static Vector3 yyy(this Vector2 v)
	{
		return new Vector3(v.y, v.y, v.y);
	}

	public static Vector3 xxx(this Vector3 v)
	{
		return new Vector3(v.x, v.x, v.x);
	}

	public static Vector3 xxy(this Vector3 v)
	{
		return new Vector3(v.x, v.x, v.y);
	}

	public static Vector3 xxz(this Vector3 v)
	{
		return new Vector3(v.x, v.x, v.z);
	}

	public static Vector3 xyy(this Vector3 v)
	{
		return new Vector3(v.x, v.y, v.y);
	}

	public static Vector3 xyz(this Vector3 v)
	{
		return new Vector3(v.x, v.y, v.z);
	}

	public static Vector3 xzz(this Vector3 v)
	{
		return new Vector3(v.x, v.z, v.z);
	}

	public static Vector3 yyy(this Vector3 v)
	{
		return new Vector3(v.y, v.y, v.y);
	}

	public static Vector3 yyz(this Vector3 v)
	{
		return new Vector3(v.y, v.y, v.z);
	}

	public static Vector3 yzz(this Vector3 v)
	{
		return new Vector3(v.y, v.z, v.z);
	}

	public static Vector3 zzz(this Vector3 v)
	{
		return new Vector3(v.z, v.z, v.z);
	}

	public static Vector3 xxx(this Vector4 v)
	{
		return new Vector3(v.x, v.x, v.x);
	}

	public static Vector3 xxy(this Vector4 v)
	{
		return new Vector3(v.x, v.x, v.y);
	}

	public static Vector3 xxz(this Vector4 v)
	{
		return new Vector3(v.x, v.x, v.z);
	}

	public static Vector3 xxw(this Vector4 v)
	{
		return new Vector3(v.x, v.x, v.w);
	}

	public static Vector3 xyy(this Vector4 v)
	{
		return new Vector3(v.x, v.y, v.y);
	}

	public static Vector3 xyz(this Vector4 v)
	{
		return new Vector3(v.x, v.y, v.z);
	}

	public static Vector3 xyw(this Vector4 v)
	{
		return new Vector3(v.x, v.y, v.w);
	}

	public static Vector3 xzz(this Vector4 v)
	{
		return new Vector3(v.x, v.z, v.z);
	}

	public static Vector3 xzw(this Vector4 v)
	{
		return new Vector3(v.x, v.z, v.w);
	}

	public static Vector3 xww(this Vector4 v)
	{
		return new Vector3(v.x, v.w, v.w);
	}

	public static Vector3 yyy(this Vector4 v)
	{
		return new Vector3(v.y, v.y, v.y);
	}

	public static Vector3 yyz(this Vector4 v)
	{
		return new Vector3(v.y, v.y, v.z);
	}

	public static Vector3 yyw(this Vector4 v)
	{
		return new Vector3(v.y, v.y, v.w);
	}

	public static Vector3 yzz(this Vector4 v)
	{
		return new Vector3(v.y, v.z, v.z);
	}

	public static Vector3 yzw(this Vector4 v)
	{
		return new Vector3(v.y, v.z, v.w);
	}

	public static Vector3 yww(this Vector4 v)
	{
		return new Vector3(v.y, v.w, v.w);
	}

	public static Vector3 zzz(this Vector4 v)
	{
		return new Vector3(v.z, v.z, v.z);
	}

	public static Vector3 zzw(this Vector4 v)
	{
		return new Vector3(v.z, v.z, v.w);
	}

	public static Vector3 zww(this Vector4 v)
	{
		return new Vector3(v.z, v.w, v.w);
	}

	public static Vector3 www(this Vector4 v)
	{
		return new Vector3(v.w, v.w, v.w);
	}

	public static Vector4 xxxx(this float v)
	{
		return new Vector4(v, v, v, v);
	}

	public static Vector4 xxxx(this Vector2 v)
	{
		return new Vector4(v.x, v.x, v.x, v.x);
	}

	public static Vector4 xxxy(this Vector2 v)
	{
		return new Vector4(v.x, v.x, v.x, v.y);
	}

	public static Vector4 xxyy(this Vector2 v)
	{
		return new Vector4(v.x, v.x, v.y, v.y);
	}

	public static Vector4 xyyy(this Vector2 v)
	{
		return new Vector4(v.x, v.y, v.y, v.y);
	}

	public static Vector4 yyyy(this Vector2 v)
	{
		return new Vector4(v.y, v.y, v.y, v.y);
	}

	public static Vector4 xxxx(this Vector3 v)
	{
		return new Vector4(v.x, v.x, v.x, v.x);
	}

	public static Vector4 xxxy(this Vector3 v)
	{
		return new Vector4(v.x, v.x, v.x, v.y);
	}

	public static Vector4 xxxz(this Vector3 v)
	{
		return new Vector4(v.x, v.x, v.x, v.z);
	}

	public static Vector4 xxyy(this Vector3 v)
	{
		return new Vector4(v.x, v.x, v.y, v.y);
	}

	public static Vector4 xxyz(this Vector3 v)
	{
		return new Vector4(v.x, v.x, v.y, v.z);
	}

	public static Vector4 xxzz(this Vector3 v)
	{
		return new Vector4(v.x, v.x, v.z, v.z);
	}

	public static Vector4 xyyy(this Vector3 v)
	{
		return new Vector4(v.x, v.y, v.y, v.y);
	}

	public static Vector4 xyyz(this Vector3 v)
	{
		return new Vector4(v.x, v.y, v.y, v.z);
	}

	public static Vector4 xyzz(this Vector3 v)
	{
		return new Vector4(v.x, v.y, v.z, v.z);
	}

	public static Vector4 xzzz(this Vector3 v)
	{
		return new Vector4(v.x, v.z, v.z, v.z);
	}

	public static Vector4 yyyy(this Vector3 v)
	{
		return new Vector4(v.y, v.y, v.y, v.y);
	}

	public static Vector4 yyyz(this Vector3 v)
	{
		return new Vector4(v.y, v.y, v.y, v.z);
	}

	public static Vector4 yyzz(this Vector3 v)
	{
		return new Vector4(v.y, v.y, v.z, v.z);
	}

	public static Vector4 yzzz(this Vector3 v)
	{
		return new Vector4(v.y, v.z, v.z, v.z);
	}

	public static Vector4 zzzz(this Vector3 v)
	{
		return new Vector4(v.z, v.z, v.z, v.z);
	}

	public static Vector4 xxxx(this Vector4 v)
	{
		return new Vector4(v.x, v.x, v.x, v.x);
	}

	public static Vector4 xxxy(this Vector4 v)
	{
		return new Vector4(v.x, v.x, v.x, v.y);
	}

	public static Vector4 xxxz(this Vector4 v)
	{
		return new Vector4(v.x, v.x, v.x, v.z);
	}

	public static Vector4 xxxw(this Vector4 v)
	{
		return new Vector4(v.x, v.x, v.x, v.w);
	}

	public static Vector4 xxyy(this Vector4 v)
	{
		return new Vector4(v.x, v.x, v.y, v.y);
	}

	public static Vector4 xxyz(this Vector4 v)
	{
		return new Vector4(v.x, v.x, v.y, v.z);
	}

	public static Vector4 xxyw(this Vector4 v)
	{
		return new Vector4(v.x, v.x, v.y, v.w);
	}

	public static Vector4 xxzz(this Vector4 v)
	{
		return new Vector4(v.x, v.x, v.z, v.z);
	}

	public static Vector4 xxzw(this Vector4 v)
	{
		return new Vector4(v.x, v.x, v.z, v.w);
	}

	public static Vector4 xxww(this Vector4 v)
	{
		return new Vector4(v.x, v.x, v.w, v.w);
	}

	public static Vector4 xyyy(this Vector4 v)
	{
		return new Vector4(v.x, v.y, v.y, v.y);
	}

	public static Vector4 xyyz(this Vector4 v)
	{
		return new Vector4(v.x, v.y, v.y, v.z);
	}

	public static Vector4 xyyw(this Vector4 v)
	{
		return new Vector4(v.x, v.y, v.y, v.w);
	}

	public static Vector4 xyzz(this Vector4 v)
	{
		return new Vector4(v.x, v.y, v.z, v.z);
	}

	public static Vector4 xyzw(this Vector4 v)
	{
		return new Vector4(v.x, v.y, v.z, v.w);
	}

	public static Vector4 xyww(this Vector4 v)
	{
		return new Vector4(v.x, v.y, v.w, v.w);
	}

	public static Vector4 xzzz(this Vector4 v)
	{
		return new Vector4(v.x, v.z, v.z, v.z);
	}

	public static Vector4 xzzw(this Vector4 v)
	{
		return new Vector4(v.x, v.z, v.z, v.w);
	}

	public static Vector4 xzww(this Vector4 v)
	{
		return new Vector4(v.x, v.z, v.w, v.w);
	}

	public static Vector4 xwww(this Vector4 v)
	{
		return new Vector4(v.x, v.w, v.w, v.w);
	}

	public static Vector4 yyyy(this Vector4 v)
	{
		return new Vector4(v.y, v.y, v.y, v.y);
	}

	public static Vector4 yyyz(this Vector4 v)
	{
		return new Vector4(v.y, v.y, v.y, v.z);
	}

	public static Vector4 yyyw(this Vector4 v)
	{
		return new Vector4(v.y, v.y, v.y, v.w);
	}

	public static Vector4 yyzz(this Vector4 v)
	{
		return new Vector4(v.y, v.y, v.z, v.z);
	}

	public static Vector4 yyzw(this Vector4 v)
	{
		return new Vector4(v.y, v.y, v.z, v.w);
	}

	public static Vector4 yyww(this Vector4 v)
	{
		return new Vector4(v.y, v.y, v.w, v.w);
	}

	public static Vector4 yzzz(this Vector4 v)
	{
		return new Vector4(v.y, v.z, v.z, v.z);
	}

	public static Vector4 yzzw(this Vector4 v)
	{
		return new Vector4(v.y, v.z, v.z, v.w);
	}

	public static Vector4 yzww(this Vector4 v)
	{
		return new Vector4(v.y, v.z, v.w, v.w);
	}

	public static Vector4 ywww(this Vector4 v)
	{
		return new Vector4(v.y, v.w, v.w, v.w);
	}

	public static Vector4 zzzz(this Vector4 v)
	{
		return new Vector4(v.z, v.z, v.z, v.z);
	}

	public static Vector4 zzzw(this Vector4 v)
	{
		return new Vector4(v.z, v.z, v.z, v.w);
	}

	public static Vector4 zzww(this Vector4 v)
	{
		return new Vector4(v.z, v.z, v.w, v.w);
	}

	public static Vector4 zwww(this Vector4 v)
	{
		return new Vector4(v.z, v.w, v.w, v.w);
	}

	public static Vector4 wwww(this Vector4 v)
	{
		return new Vector4(v.w, v.w, v.w, v.w);
	}

	public static Vector4 WithX(this Vector4 v, float x)
	{
		return new Vector4(x, v.y, v.z, v.w);
	}

	public static Vector4 WithY(this Vector4 v, float y)
	{
		return new Vector4(v.x, y, v.z, v.w);
	}

	public static Vector4 WithZ(this Vector4 v, float z)
	{
		return new Vector4(v.x, v.y, z, v.w);
	}

	public static Vector4 WithW(this Vector4 v, float w)
	{
		return new Vector4(v.x, v.y, v.z, w);
	}

	public static Vector3 WithX(this Vector3 v, float x)
	{
		return new Vector3(x, v.y, v.z);
	}

	public static Vector3 WithY(this Vector3 v, float y)
	{
		return new Vector3(v.x, y, v.z);
	}

	public static Vector3 WithZ(this Vector3 v, float z)
	{
		return new Vector3(v.x, v.y, z);
	}

	public static Vector4 WithW(this Vector3 v, float w)
	{
		return new Vector4(v.x, v.y, v.z, w);
	}

	public static Vector2 WithX(this Vector2 v, float x)
	{
		return new Vector2(x, v.y);
	}

	public static Vector2 WithY(this Vector2 v, float y)
	{
		return new Vector2(v.x, y);
	}

	public static Vector3 WithZ(this Vector2 v, float z)
	{
		return new Vector3(v.x, v.y, z);
	}
}
