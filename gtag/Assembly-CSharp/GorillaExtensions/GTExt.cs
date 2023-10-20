using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaExtensions
{
	// Token: 0x020002FA RID: 762
	public static class GTExt
	{
		// Token: 0x060014A5 RID: 5285 RVA: 0x000747A8 File Offset: 0x000729A8
		public static T GetComponentInHierarchy<T>(this Scene scene, bool includeInactive = true) where T : Component
		{
			foreach (GameObject gameObject in scene.GetRootGameObjects())
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
			return default(T);
		}

		// Token: 0x060014A6 RID: 5286 RVA: 0x00074828 File Offset: 0x00072A28
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

		// Token: 0x060014A7 RID: 5287 RVA: 0x00074864 File Offset: 0x00072A64
		public static List<Object> GetComponentsInHierarchy(this Scene scene, Type type, bool includeInactive = true, int capacity = 64)
		{
			List<Object> list = new List<Object>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				Component[] componentsInChildren = rootGameObjects[i].GetComponentsInChildren(type, includeInactive);
				list.AddRange(componentsInChildren);
			}
			return list;
		}

		// Token: 0x060014A8 RID: 5288 RVA: 0x000748A1 File Offset: 0x00072AA1
		public static List<GameObject> GetGameObjectsInHierarchy(this Scene scene, bool includeInactive = true, int capacity = 64)
		{
			return scene.GetComponentsInHierarchy(includeInactive, capacity);
		}

		// Token: 0x060014A9 RID: 5289 RVA: 0x000748AC File Offset: 0x00072AAC
		public static List<T> GetComponentsInChildrenUntil<T, TStop1, TStop2, TStop3>(this Component root, bool includeInactive = false, bool stopAtRoot = true) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			GTExt.<>c__DisplayClass4_0<T, TStop1, TStop2, TStop3> CS$<>8__locals1;
			CS$<>8__locals1.includeInactive = includeInactive;
			List<T> list = new List<T>();
			if (stopAtRoot && (root.GetComponent<TStop1>() != null || root.GetComponent<TStop2>() != null || root.GetComponent<TStop3>() != null))
			{
				return list;
			}
			T component = root.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
			GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|4_0<T, TStop1, TStop2, TStop3>(root.transform, ref list, ref CS$<>8__locals1);
			return list;
		}

		// Token: 0x060014AA RID: 5290 RVA: 0x00074934 File Offset: 0x00072B34
		public static T GetComponentWithRegex<T>(this Component root, string regexString) where T : Component
		{
			T[] componentsInChildren = root.GetComponentsInChildren<T>();
			Regex regex = new Regex(regexString);
			foreach (T t in componentsInChildren)
			{
				if (regex.IsMatch(t.name))
				{
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x060014AB RID: 5291 RVA: 0x00074984 File Offset: 0x00072B84
		private static List<T> GetComponentsWithRegex_Internal<T>(IEnumerable<T> allComponents, string regexString, bool includeInactive, int capacity = 64) where T : Component
		{
			List<T> result = new List<T>(capacity);
			Regex regex = new Regex(regexString);
			GTExt.GetComponentsWithRegex_Internal<T>(allComponents, regex, ref result);
			return result;
		}

		// Token: 0x060014AC RID: 5292 RVA: 0x000749AC File Offset: 0x00072BAC
		private static void GetComponentsWithRegex_Internal<T>(IEnumerable<T> allComponents, Regex regex, ref List<T> foundComponents) where T : Component
		{
			foreach (T t in allComponents)
			{
				string name = t.name;
				if (regex.IsMatch(name))
				{
					foundComponents.Add(t);
				}
			}
		}

		// Token: 0x060014AD RID: 5293 RVA: 0x00074A0C File Offset: 0x00072C0C
		public static List<T> GetComponentsWithRegex<T>(this Scene scene, string regexString, bool includeInactive, int capacity) where T : Component
		{
			return GTExt.GetComponentsWithRegex_Internal<T>(scene.GetComponentsInHierarchy(includeInactive, capacity), regexString, includeInactive, capacity);
		}

		// Token: 0x060014AE RID: 5294 RVA: 0x00074A1E File Offset: 0x00072C1E
		public static List<T> GetComponentsWithRegex<T>(this Component root, string regexString, bool includeInactive, int capacity) where T : Component
		{
			return GTExt.GetComponentsWithRegex_Internal<T>(root.GetComponentsInChildren<T>(includeInactive), regexString, includeInactive, capacity);
		}

		// Token: 0x060014AF RID: 5295 RVA: 0x00074A30 File Offset: 0x00072C30
		public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string regexString, bool includeInactive = true, int capacity = 64)
		{
			List<Transform> componentsWithRegex = scene.GetComponentsWithRegex(regexString, includeInactive, capacity);
			List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
			foreach (Transform transform in componentsWithRegex)
			{
				list.Add(transform.gameObject);
			}
			return list;
		}

		// Token: 0x060014B0 RID: 5296 RVA: 0x00074A98 File Offset: 0x00072C98
		public static void GetComponentsWithRegex_Internal<T>(this List<T> allComponents, Regex[] regexes, int maxCount, ref List<T> foundComponents) where T : Component
		{
			if (maxCount == 0)
			{
				return;
			}
			int num = 0;
			foreach (T t in allComponents)
			{
				for (int i = 0; i < regexes.Length; i++)
				{
					if (regexes[i].IsMatch(t.name))
					{
						foundComponents.Add(t);
						num++;
						if (maxCount > 0 && num >= maxCount)
						{
							return;
						}
					}
				}
			}
		}

		// Token: 0x060014B1 RID: 5297 RVA: 0x00074B28 File Offset: 0x00072D28
		public static List<T> GetComponentsWithRegex<T>(this Scene scene, string[] regexStrings, bool includeInactive = true, int maxCount = -1, int capacity = 64) where T : Component
		{
			List<T> componentsInHierarchy = scene.GetComponentsInHierarchy(includeInactive, capacity);
			List<T> result = new List<T>(componentsInHierarchy.Count);
			Regex[] array = new Regex[regexStrings.Length];
			for (int i = 0; i < regexStrings.Length; i++)
			{
				array[i] = new Regex(regexStrings[i]);
			}
			componentsInHierarchy.GetComponentsWithRegex_Internal(array, maxCount, ref result);
			return result;
		}

		// Token: 0x060014B2 RID: 5298 RVA: 0x00074B78 File Offset: 0x00072D78
		public static List<T> GetComponentsWithRegex<T>(this Scene scene, string[] regexStrings, string[] excludeRegexStrings, bool includeInactive = true, int maxCount = -1) where T : Component
		{
			List<T> componentsInHierarchy = scene.GetComponentsInHierarchy(includeInactive, 64);
			List<T> list = new List<T>(componentsInHierarchy.Count);
			if (maxCount == 0)
			{
				return list;
			}
			int num = 0;
			foreach (T t in componentsInHierarchy)
			{
				bool flag = false;
				foreach (string pattern in regexStrings)
				{
					if (!flag && Regex.IsMatch(t.name, pattern))
					{
						foreach (string pattern2 in excludeRegexStrings)
						{
							if (!flag)
							{
								flag = Regex.IsMatch(t.name, pattern2);
							}
						}
						if (!flag)
						{
							list.Add(t);
							num++;
							if (maxCount > 0 && num >= maxCount)
							{
								return list;
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x060014B3 RID: 5299 RVA: 0x00074C7C File Offset: 0x00072E7C
		public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string[] regexStrings, bool includeInactive = true, int maxCount = -1)
		{
			List<Transform> componentsWithRegex = scene.GetComponentsWithRegex(regexStrings, includeInactive, maxCount, 64);
			List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
			foreach (Transform transform in componentsWithRegex)
			{
				list.Add(transform.gameObject);
			}
			return list;
		}

		// Token: 0x060014B4 RID: 5300 RVA: 0x00074CE8 File Offset: 0x00072EE8
		public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string[] regexStrings, string[] excludeRegexStrings, bool includeInactive = true, int maxCount = -1)
		{
			List<Transform> componentsWithRegex = scene.GetComponentsWithRegex(regexStrings, excludeRegexStrings, includeInactive, maxCount);
			List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
			foreach (Transform transform in componentsWithRegex)
			{
				list.Add(transform.gameObject);
			}
			return list;
		}

		// Token: 0x060014B5 RID: 5301 RVA: 0x00074D54 File Offset: 0x00072F54
		public static List<T> GetComponentsByName<T>(this Transform xform, string name, bool includeInactive = true) where T : Component
		{
			T[] componentsInChildren = xform.GetComponentsInChildren<T>(includeInactive);
			List<T> list = new List<T>(componentsInChildren.Length);
			foreach (T t in componentsInChildren)
			{
				if (t.name == name)
				{
					list.Add(t);
				}
			}
			return list;
		}

		// Token: 0x060014B6 RID: 5302 RVA: 0x00074DA4 File Offset: 0x00072FA4
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

		// Token: 0x060014B7 RID: 5303 RVA: 0x00074DF0 File Offset: 0x00072FF0
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

		// Token: 0x060014B8 RID: 5304 RVA: 0x00074E44 File Offset: 0x00073044
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

		// Token: 0x060014B9 RID: 5305 RVA: 0x00074E9B File Offset: 0x0007309B
		public static string GetPath(this GameObject gameObject)
		{
			return gameObject.transform.GetPath();
		}

		// Token: 0x060014BA RID: 5306 RVA: 0x00074EA8 File Offset: 0x000730A8
		public static string GetPath(this GameObject gameObject, int limit)
		{
			return gameObject.transform.GetPath(limit);
		}

		// Token: 0x060014BB RID: 5307 RVA: 0x00074EB8 File Offset: 0x000730B8
		public static void GetPathWithSiblingIndexes(this Transform transform, StringBuilder strBuilder)
		{
			int length = strBuilder.Length;
			while (transform != null)
			{
				strBuilder.Insert(length, transform.name);
				strBuilder.Insert(length, "|");
				strBuilder.Insert(length, transform.GetSiblingIndex().ToString("0000"));
				strBuilder.Insert(length, "/");
				transform = transform.parent;
			}
		}

		// Token: 0x060014BC RID: 5308 RVA: 0x00074F22 File Offset: 0x00073122
		public static string GetPathWithSiblingIndexes(this Transform transform)
		{
			GTExt.tempStringBuilder.Clear();
			transform.GetPathWithSiblingIndexes(GTExt.tempStringBuilder);
			return GTExt.tempStringBuilder.ToString();
		}

		// Token: 0x060014BD RID: 5309 RVA: 0x00074F44 File Offset: 0x00073144
		public static void GetPathWithSiblingIndexes(this GameObject gameObject, StringBuilder stringBuilder)
		{
			gameObject.transform.GetPathWithSiblingIndexes(stringBuilder);
		}

		// Token: 0x060014BE RID: 5310 RVA: 0x00074F52 File Offset: 0x00073152
		public static string GetPathWithSiblingIndexes(this GameObject gameObject)
		{
			return gameObject.transform.GetPathWithSiblingIndexes();
		}

		// Token: 0x060014BF RID: 5311 RVA: 0x00074F60 File Offset: 0x00073160
		public static List<GameObject> GetGameObjectsInHierarchy(this Scene scene, string name, bool includeInactive = true)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject gameObject in scene.GetRootGameObjects())
			{
				if (gameObject.name.Contains(name))
				{
					list.Add(gameObject);
				}
				foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(includeInactive))
				{
					if (transform.name.Contains(name))
					{
						list.Add(transform.gameObject);
					}
				}
			}
			return list;
		}

		// Token: 0x060014C0 RID: 5312 RVA: 0x00074FE2 File Offset: 0x000731E2
		public static string GetComponentPath(this Component component, int maxDepth = 2147483647)
		{
			GTExt.tempStringBuilder.Clear();
			component.GetComponentPath(GTExt.tempStringBuilder, maxDepth);
			return GTExt.tempStringBuilder.ToString();
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x00075005 File Offset: 0x00073205
		public static string GetComponentPath<T>(this T component, int maxDepth = 2147483647) where T : Component
		{
			GTExt.tempStringBuilder.Clear();
			component.GetComponentPath(GTExt.tempStringBuilder, maxDepth);
			return GTExt.tempStringBuilder.ToString();
		}

		// Token: 0x060014C2 RID: 5314 RVA: 0x00075028 File Offset: 0x00073228
		public static void GetComponentPath<T>(this T component, StringBuilder strBuilder, int maxDepth = 2147483647) where T : Component
		{
			Transform transform = component.transform;
			int length = strBuilder.Length;
			if (maxDepth > 0)
			{
				strBuilder.Append("/");
			}
			strBuilder.Append("->/");
			Type typeFromHandle = typeof(T);
			strBuilder.Append(typeFromHandle.Name);
			Component[] components = transform.GetComponents(typeFromHandle);
			if (components.Length > 1)
			{
				strBuilder.Append("#");
				strBuilder.Append(Array.IndexOf<Component>(components, component));
			}
			if (maxDepth <= 0)
			{
				return;
			}
			int num = 0;
			while (transform != null)
			{
				strBuilder.Insert(length, transform.name);
				num++;
				if (maxDepth <= num)
				{
					break;
				}
				strBuilder.Insert(length, "/");
				transform = transform.parent;
			}
		}

		// Token: 0x060014C3 RID: 5315 RVA: 0x000750EC File Offset: 0x000732EC
		public static void GetComponentPathWithSiblingIndexes<T>(this T component, StringBuilder strBuilder) where T : Component
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
				strBuilder.Append(Array.IndexOf<Component>(components, component));
			}
			while (transform != null)
			{
				strBuilder.Insert(length, transform.name);
				strBuilder.Insert(length, "|");
				strBuilder.Insert(length, transform.GetSiblingIndex().ToString("0000"));
				strBuilder.Insert(length, "/");
				transform = transform.parent;
			}
		}

		// Token: 0x060014C4 RID: 5316 RVA: 0x000751B3 File Offset: 0x000733B3
		public static string GetComponentPathWithSiblingIndexes<T>(this T component) where T : Component
		{
			GTExt.tempStringBuilder.Clear();
			component.GetComponentPathWithSiblingIndexes(GTExt.tempStringBuilder);
			return GTExt.tempStringBuilder.ToString();
		}

		// Token: 0x060014C5 RID: 5317 RVA: 0x000751D8 File Offset: 0x000733D8
		public static T GetComponentByPath<T>(this GameObject root, string path) where T : Component
		{
			string[] array = path.Split(new string[]
			{
				"/->/"
			}, StringSplitOptions.None);
			if (array.Length < 2)
			{
				return default(T);
			}
			string[] array2 = array[0].Split(new string[]
			{
				"/"
			}, StringSplitOptions.RemoveEmptyEntries);
			Transform transform = root.transform;
			for (int i = 1; i < array2.Length; i++)
			{
				string n = array2[i];
				transform = transform.Find(n);
				if (transform == null)
				{
					return default(T);
				}
			}
			Type type = Type.GetType(array[1].Split('#', StringSplitOptions.None)[0]);
			if (type == null)
			{
				return default(T);
			}
			Component component = transform.GetComponent(type);
			if (component == null)
			{
				return default(T);
			}
			return component as T;
		}

		// Token: 0x060014C6 RID: 5318 RVA: 0x000752B4 File Offset: 0x000734B4
		public static int GetDepth(this Transform xform)
		{
			int num = 0;
			Transform parent = xform.parent;
			while (parent != null)
			{
				num++;
				parent = parent.parent;
			}
			return num;
		}

		// Token: 0x060014C7 RID: 5319 RVA: 0x000752E1 File Offset: 0x000734E1
		public static T GetOrAddComponent<T>(this GameObject gameObject, ref T component) where T : Component
		{
			if (component == null)
			{
				component = gameObject.GetOrAddComponent<T>();
			}
			return component;
		}

		// Token: 0x060014C8 RID: 5320 RVA: 0x00075308 File Offset: 0x00073508
		public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
		{
			T result;
			if (!gameObject.TryGetComponent<T>(out result))
			{
				result = gameObject.AddComponent<T>();
			}
			return result;
		}

		// Token: 0x060014C9 RID: 5321 RVA: 0x00075328 File Offset: 0x00073528
		public static void SetLossyScale(this Transform transform, Vector3 scale)
		{
			scale = transform.InverseTransformVector(scale);
			Debug.Log(scale);
			Vector3 lossyScale = transform.lossyScale;
			Debug.Log(scale);
			transform.localScale = new Vector3(scale.x / lossyScale.x, scale.y / lossyScale.y, scale.z / lossyScale.z);
		}

		// Token: 0x060014CA RID: 5322 RVA: 0x00075390 File Offset: 0x00073590
		public static void ForEachBackwards<T>(this List<T> list, Action<T> action)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				T obj = list[i];
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

		// Token: 0x060014CB RID: 5323 RVA: 0x000753D8 File Offset: 0x000735D8
		public static void SafeForEachBackwards<T>(this List<T> list, Action<T> action)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				T obj = list[i];
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

		// Token: 0x060014CC RID: 5324 RVA: 0x00075420 File Offset: 0x00073620
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
			return num == num5 && num2 == num6 && num3 == num7 && num4 == num8;
		}

		// Token: 0x060014CD RID: 5325 RVA: 0x000754B4 File Offset: 0x000736B4
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

		// Token: 0x060014CE RID: 5326 RVA: 0x00075558 File Offset: 0x00073758
		public static Vector3 Position(this Matrix4x4 matrix)
		{
			float m = matrix.m03;
			float m2 = matrix.m13;
			float m3 = matrix.m23;
			return new Vector3(m, m2, m3);
		}

		// Token: 0x060014CF RID: 5327 RVA: 0x00075580 File Offset: 0x00073780
		public static Vector3 Scale(this Matrix4x4 m)
		{
			Vector3 result = new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
			if (Vector3.Cross(m.GetColumn(0), m.GetColumn(1)).normalized != m.GetColumn(2).normalized)
			{
				result.x *= -1f;
			}
			return result;
		}

		// Token: 0x060014D0 RID: 5328 RVA: 0x00075618 File Offset: 0x00073818
		public static void SetLocalRelativeToParentMatrixWithParityAxis(this Matrix4x4 matrix, GTExt.ParityOptions parity = GTExt.ParityOptions.XFlip)
		{
		}

		// Token: 0x060014D1 RID: 5329 RVA: 0x0007561A File Offset: 0x0007381A
		public static void MultiplyInPlaceWith(this Vector3 a, in Vector3 b)
		{
			a.x *= b.x;
			a.y *= b.y;
			a.z *= b.z;
		}

		// Token: 0x060014D2 RID: 5330 RVA: 0x0007564C File Offset: 0x0007384C
		public static void DecomposeWithXFlip(this Matrix4x4 matrix, out Vector3 transformation, out Quaternion rotation, out Vector3 scale)
		{
			Matrix4x4 matrix2 = matrix;
			transformation = matrix2.Position();
			int num = 2;
			Vector3 forward = matrix2.GetColumnNoCopy(num);
			int num2 = 1;
			rotation = Quaternion.LookRotation(forward, matrix2.GetColumnNoCopy(num2));
			Matrix4x4 matrix4x = matrix;
			scale = matrix4x.lossyScale;
		}

		// Token: 0x060014D3 RID: 5331 RVA: 0x000756AC File Offset: 0x000738AC
		public static void SetLocalMatrixRelativeToParentWithXParity(this Transform transform, in Matrix4x4 matrix4X4)
		{
			Vector3 localPosition;
			Quaternion localRotation;
			Vector3 localScale;
			matrix4X4.DecomposeWithXFlip(out localPosition, out localRotation, out localScale);
			transform.localPosition = localPosition;
			transform.localRotation = localRotation;
			transform.localScale = localScale;
		}

		// Token: 0x060014D4 RID: 5332 RVA: 0x000756DC File Offset: 0x000738DC
		public static Matrix4x4 Matrix4x4Scale(in Vector3 vector)
		{
			Matrix4x4 result;
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

		// Token: 0x060014D5 RID: 5333 RVA: 0x000757B0 File Offset: 0x000739B0
		public static Vector4 GetColumnNoCopy(this Matrix4x4 matrix, in int index)
		{
			switch (index)
			{
			case 0:
				return new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30);
			case 1:
				return new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31);
			case 2:
				return new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32);
			case 3:
				return new Vector4(matrix.m03, matrix.m13, matrix.m23, matrix.m33);
			default:
				throw new IndexOutOfRangeException("Invalid column index!");
			}
		}

		// Token: 0x060014D6 RID: 5334 RVA: 0x0007585C File Offset: 0x00073A5C
		public static Quaternion RotationWithScaleContext(this Matrix4x4 m, in Vector3 scale)
		{
			Matrix4x4 matrix4x = m * GTExt.Matrix4x4Scale(scale);
			int num = 2;
			Vector3 forward = matrix4x.GetColumnNoCopy(num);
			int num2 = 1;
			return Quaternion.LookRotation(forward, matrix4x.GetColumnNoCopy(num2));
		}

		// Token: 0x060014D7 RID: 5335 RVA: 0x000758A0 File Offset: 0x00073AA0
		public static Quaternion Rotation(this Matrix4x4 m)
		{
			int num = 2;
			Vector3 forward = m.GetColumnNoCopy(num);
			int num2 = 1;
			return Quaternion.LookRotation(forward, m.GetColumnNoCopy(num2));
		}

		// Token: 0x060014D8 RID: 5336 RVA: 0x000758D0 File Offset: 0x00073AD0
		public static Vector3 x0y(this Vector2 v)
		{
			return new Vector3(v.x, 0f, v.y);
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x000758E8 File Offset: 0x00073AE8
		public static Vector3 x0y(this Vector3 v)
		{
			return new Vector3(v.x, 0f, v.y);
		}

		// Token: 0x060014DA RID: 5338 RVA: 0x00075900 File Offset: 0x00073B00
		public static Vector3 xy0(this Vector2 v)
		{
			return new Vector3(v.x, v.y, 0f);
		}

		// Token: 0x060014DB RID: 5339 RVA: 0x00075918 File Offset: 0x00073B18
		public static Vector3 xy0(this Vector3 v)
		{
			return new Vector3(v.x, v.y, 0f);
		}

		// Token: 0x060014DC RID: 5340 RVA: 0x00075930 File Offset: 0x00073B30
		public static Vector3 xz0(this Vector3 v)
		{
			return new Vector3(v.x, v.z, 0f);
		}

		// Token: 0x060014DD RID: 5341 RVA: 0x00075948 File Offset: 0x00073B48
		public static Vector3 x0z(this Vector3 v)
		{
			return new Vector3(v.x, 0f, v.z);
		}

		// Token: 0x060014DE RID: 5342 RVA: 0x00075960 File Offset: 0x00073B60
		public static Matrix4x4 LocalMatrixRelativeToParentNoScale(this Transform transform)
		{
			return Matrix4x4.TRS(transform.localPosition, transform.localRotation, Vector3.one);
		}

		// Token: 0x060014DF RID: 5343 RVA: 0x00075978 File Offset: 0x00073B78
		public static Matrix4x4 LocalMatrixRelativeToParentWithScale(this Transform transform)
		{
			if (transform.parent == null)
			{
				return transform.localToWorldMatrix;
			}
			return transform.parent.worldToLocalMatrix * transform.localToWorldMatrix;
		}

		// Token: 0x060014E0 RID: 5344 RVA: 0x000759A5 File Offset: 0x00073BA5
		public static void SetLocalMatrixRelativeToParent(this Transform transform, Matrix4x4 matrix)
		{
			transform.localPosition = matrix.Position();
			transform.localRotation = matrix.Rotation();
			transform.localScale = matrix.Scale();
		}

		// Token: 0x060014E1 RID: 5345 RVA: 0x000759CC File Offset: 0x00073BCC
		public static void SetLocalMatrixRelativeToParentNoScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.localPosition = matrix.Position();
			transform.localRotation = matrix.Rotation();
		}

		// Token: 0x060014E2 RID: 5346 RVA: 0x000759E7 File Offset: 0x00073BE7
		public static void SetLocalToWorldMatrixNoScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.position = matrix.Position();
			transform.rotation = matrix.Rotation();
		}

		// Token: 0x060014E3 RID: 5347 RVA: 0x00075A02 File Offset: 0x00073C02
		public static Matrix4x4 localToWorldNoScale(this Transform transform)
		{
			return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		}

		// Token: 0x060014E4 RID: 5348 RVA: 0x00075A1A File Offset: 0x00073C1A
		public static void SetLocalToWorldMatrixWithScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.position = matrix.Position();
			transform.rotation = matrix.rotation;
			transform.SetLossyScale(matrix.lossyScale);
		}

		// Token: 0x060014E5 RID: 5349 RVA: 0x00075A42 File Offset: 0x00073C42
		public static Matrix4x4 Matrix4X4LerpNoScale(Matrix4x4 a, Matrix4x4 b, float t)
		{
			return Matrix4x4.TRS(Vector3.Lerp(a.Position(), b.Position(), t), Quaternion.Slerp(a.rotation, b.rotation, t), b.lossyScale);
		}

		// Token: 0x060014E6 RID: 5350 RVA: 0x00075A76 File Offset: 0x00073C76
		public static Matrix4x4 LerpTo(this Matrix4x4 a, Matrix4x4 b, float t)
		{
			return GTExt.Matrix4X4LerpNoScale(a, b, t);
		}

		// Token: 0x060014E7 RID: 5351 RVA: 0x00075A80 File Offset: 0x00073C80
		public static bool IsNaN(this Vector3 vector)
		{
			return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);
		}

		// Token: 0x060014E8 RID: 5352 RVA: 0x00075AA9 File Offset: 0x00073CA9
		public static bool IsNan(this Quaternion quat)
		{
			return float.IsNaN(quat.x) || float.IsNaN(quat.y) || float.IsNaN(quat.z) || float.IsNaN(quat.w);
		}

		// Token: 0x060014E9 RID: 5353 RVA: 0x00075ADF File Offset: 0x00073CDF
		public static bool IsInfinity(this Vector3 vector)
		{
			return float.IsInfinity(vector.x) || float.IsInfinity(vector.y) || float.IsInfinity(vector.z);
		}

		// Token: 0x060014EA RID: 5354 RVA: 0x00075B08 File Offset: 0x00073D08
		public static bool IsInfinity(this Quaternion quat)
		{
			return float.IsInfinity(quat.x) || float.IsInfinity(quat.y) || float.IsInfinity(quat.z) || float.IsInfinity(quat.w);
		}

		// Token: 0x060014EB RID: 5355 RVA: 0x00075B3E File Offset: 0x00073D3E
		public static bool IsValid(this Vector3 vector)
		{
			return !vector.IsNaN() && !vector.IsInfinity();
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x00075B54 File Offset: 0x00073D54
		public static bool IsValid(this Quaternion quat)
		{
			return !quat.IsNan() && !quat.IsInfinity() && (quat.x != 0f || quat.y != 0f || quat.z != 0f || quat.w != 0f);
		}

		// Token: 0x060014ED RID: 5357 RVA: 0x00075BAC File Offset: 0x00073DAC
		public static Matrix4x4 Matrix4X4LerpHandleNegativeScale(Matrix4x4 a, Matrix4x4 b, float t)
		{
			return Matrix4x4.TRS(Vector3.Lerp(a.Position(), b.Position(), t), Quaternion.Slerp(a.Rotation(), b.Rotation(), t), b.lossyScale);
		}

		// Token: 0x060014EE RID: 5358 RVA: 0x00075BE0 File Offset: 0x00073DE0
		public static Matrix4x4 LerpTo_HandleNegativeScale(this Matrix4x4 a, Matrix4x4 b, float t)
		{
			return GTExt.Matrix4X4LerpHandleNegativeScale(a, b, t);
		}

		// Token: 0x060014EF RID: 5359 RVA: 0x00075BEC File Offset: 0x00073DEC
		public static Vector3 LerpToUnclamped(this Vector3 a, in Vector3 b, float t)
		{
			return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
		}

		// Token: 0x060014F0 RID: 5360 RVA: 0x00075C40 File Offset: 0x00073E40
		public static int GetRandomIndex<T>(this IReadOnlyList<T> self)
		{
			return Random.Range(0, self.Count);
		}

		// Token: 0x060014F1 RID: 5361 RVA: 0x00075C4E File Offset: 0x00073E4E
		public static T GetRandomItem<T>(this IReadOnlyList<T> self)
		{
			return self[self.GetRandomIndex<T>()];
		}

		// Token: 0x060014F2 RID: 5362 RVA: 0x00075C5C File Offset: 0x00073E5C
		public static Vector2 xx(this float v)
		{
			return new Vector2(v, v);
		}

		// Token: 0x060014F3 RID: 5363 RVA: 0x00075C65 File Offset: 0x00073E65
		public static Vector2 xx(this Vector2 v)
		{
			return new Vector2(v.x, v.x);
		}

		// Token: 0x060014F4 RID: 5364 RVA: 0x00075C78 File Offset: 0x00073E78
		public static Vector2 xy(this Vector2 v)
		{
			return new Vector2(v.x, v.y);
		}

		// Token: 0x060014F5 RID: 5365 RVA: 0x00075C8B File Offset: 0x00073E8B
		public static Vector2 yy(this Vector2 v)
		{
			return new Vector2(v.y, v.y);
		}

		// Token: 0x060014F6 RID: 5366 RVA: 0x00075C9E File Offset: 0x00073E9E
		public static Vector2 xx(this Vector3 v)
		{
			return new Vector2(v.x, v.x);
		}

		// Token: 0x060014F7 RID: 5367 RVA: 0x00075CB1 File Offset: 0x00073EB1
		public static Vector2 xy(this Vector3 v)
		{
			return new Vector2(v.x, v.y);
		}

		// Token: 0x060014F8 RID: 5368 RVA: 0x00075CC4 File Offset: 0x00073EC4
		public static Vector2 xz(this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		// Token: 0x060014F9 RID: 5369 RVA: 0x00075CD7 File Offset: 0x00073ED7
		public static Vector2 yy(this Vector3 v)
		{
			return new Vector2(v.y, v.y);
		}

		// Token: 0x060014FA RID: 5370 RVA: 0x00075CEA File Offset: 0x00073EEA
		public static Vector2 yz(this Vector3 v)
		{
			return new Vector2(v.y, v.z);
		}

		// Token: 0x060014FB RID: 5371 RVA: 0x00075CFD File Offset: 0x00073EFD
		public static Vector2 zz(this Vector3 v)
		{
			return new Vector2(v.z, v.z);
		}

		// Token: 0x060014FC RID: 5372 RVA: 0x00075D10 File Offset: 0x00073F10
		public static Vector2 xx(this Vector4 v)
		{
			return new Vector2(v.x, v.x);
		}

		// Token: 0x060014FD RID: 5373 RVA: 0x00075D23 File Offset: 0x00073F23
		public static Vector2 xy(this Vector4 v)
		{
			return new Vector2(v.x, v.y);
		}

		// Token: 0x060014FE RID: 5374 RVA: 0x00075D36 File Offset: 0x00073F36
		public static Vector2 xz(this Vector4 v)
		{
			return new Vector2(v.x, v.z);
		}

		// Token: 0x060014FF RID: 5375 RVA: 0x00075D49 File Offset: 0x00073F49
		public static Vector2 xw(this Vector4 v)
		{
			return new Vector2(v.x, v.w);
		}

		// Token: 0x06001500 RID: 5376 RVA: 0x00075D5C File Offset: 0x00073F5C
		public static Vector2 yy(this Vector4 v)
		{
			return new Vector2(v.y, v.y);
		}

		// Token: 0x06001501 RID: 5377 RVA: 0x00075D6F File Offset: 0x00073F6F
		public static Vector2 yz(this Vector4 v)
		{
			return new Vector2(v.y, v.z);
		}

		// Token: 0x06001502 RID: 5378 RVA: 0x00075D82 File Offset: 0x00073F82
		public static Vector2 yw(this Vector4 v)
		{
			return new Vector2(v.y, v.w);
		}

		// Token: 0x06001503 RID: 5379 RVA: 0x00075D95 File Offset: 0x00073F95
		public static Vector2 zz(this Vector4 v)
		{
			return new Vector2(v.z, v.z);
		}

		// Token: 0x06001504 RID: 5380 RVA: 0x00075DA8 File Offset: 0x00073FA8
		public static Vector2 zw(this Vector4 v)
		{
			return new Vector2(v.z, v.w);
		}

		// Token: 0x06001505 RID: 5381 RVA: 0x00075DBB File Offset: 0x00073FBB
		public static Vector2 ww(this Vector4 v)
		{
			return new Vector2(v.w, v.w);
		}

		// Token: 0x06001506 RID: 5382 RVA: 0x00075DCE File Offset: 0x00073FCE
		public static Vector3 xxx(this float v)
		{
			return new Vector3(v, v, v);
		}

		// Token: 0x06001507 RID: 5383 RVA: 0x00075DD8 File Offset: 0x00073FD8
		public static Vector3 xxx(this Vector2 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		// Token: 0x06001508 RID: 5384 RVA: 0x00075DF1 File Offset: 0x00073FF1
		public static Vector3 xxy(this Vector2 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		// Token: 0x06001509 RID: 5385 RVA: 0x00075E0A File Offset: 0x0007400A
		public static Vector3 xyy(this Vector2 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		// Token: 0x0600150A RID: 5386 RVA: 0x00075E23 File Offset: 0x00074023
		public static Vector3 yyy(this Vector2 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		// Token: 0x0600150B RID: 5387 RVA: 0x00075E3C File Offset: 0x0007403C
		public static Vector3 xxx(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		// Token: 0x0600150C RID: 5388 RVA: 0x00075E55 File Offset: 0x00074055
		public static Vector3 xxy(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		// Token: 0x0600150D RID: 5389 RVA: 0x00075E6E File Offset: 0x0007406E
		public static Vector3 xxz(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.z);
		}

		// Token: 0x0600150E RID: 5390 RVA: 0x00075E87 File Offset: 0x00074087
		public static Vector3 xyy(this Vector3 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		// Token: 0x0600150F RID: 5391 RVA: 0x00075EA0 File Offset: 0x000740A0
		public static Vector3 xyz(this Vector3 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		// Token: 0x06001510 RID: 5392 RVA: 0x00075EB9 File Offset: 0x000740B9
		public static Vector3 xzz(this Vector3 v)
		{
			return new Vector3(v.x, v.z, v.z);
		}

		// Token: 0x06001511 RID: 5393 RVA: 0x00075ED2 File Offset: 0x000740D2
		public static Vector3 yyy(this Vector3 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		// Token: 0x06001512 RID: 5394 RVA: 0x00075EEB File Offset: 0x000740EB
		public static Vector3 yyz(this Vector3 v)
		{
			return new Vector3(v.y, v.y, v.z);
		}

		// Token: 0x06001513 RID: 5395 RVA: 0x00075F04 File Offset: 0x00074104
		public static Vector3 yzz(this Vector3 v)
		{
			return new Vector3(v.y, v.z, v.z);
		}

		// Token: 0x06001514 RID: 5396 RVA: 0x00075F1D File Offset: 0x0007411D
		public static Vector3 zzz(this Vector3 v)
		{
			return new Vector3(v.z, v.z, v.z);
		}

		// Token: 0x06001515 RID: 5397 RVA: 0x00075F36 File Offset: 0x00074136
		public static Vector3 xxx(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		// Token: 0x06001516 RID: 5398 RVA: 0x00075F4F File Offset: 0x0007414F
		public static Vector3 xxy(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		// Token: 0x06001517 RID: 5399 RVA: 0x00075F68 File Offset: 0x00074168
		public static Vector3 xxz(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.z);
		}

		// Token: 0x06001518 RID: 5400 RVA: 0x00075F81 File Offset: 0x00074181
		public static Vector3 xxw(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.w);
		}

		// Token: 0x06001519 RID: 5401 RVA: 0x00075F9A File Offset: 0x0007419A
		public static Vector3 xyy(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		// Token: 0x0600151A RID: 5402 RVA: 0x00075FB3 File Offset: 0x000741B3
		public static Vector3 xyz(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		// Token: 0x0600151B RID: 5403 RVA: 0x00075FCC File Offset: 0x000741CC
		public static Vector3 xyw(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.w);
		}

		// Token: 0x0600151C RID: 5404 RVA: 0x00075FE5 File Offset: 0x000741E5
		public static Vector3 xzz(this Vector4 v)
		{
			return new Vector3(v.x, v.z, v.z);
		}

		// Token: 0x0600151D RID: 5405 RVA: 0x00075FFE File Offset: 0x000741FE
		public static Vector3 xzw(this Vector4 v)
		{
			return new Vector3(v.x, v.z, v.w);
		}

		// Token: 0x0600151E RID: 5406 RVA: 0x00076017 File Offset: 0x00074217
		public static Vector3 xww(this Vector4 v)
		{
			return new Vector3(v.x, v.w, v.w);
		}

		// Token: 0x0600151F RID: 5407 RVA: 0x00076030 File Offset: 0x00074230
		public static Vector3 yyy(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		// Token: 0x06001520 RID: 5408 RVA: 0x00076049 File Offset: 0x00074249
		public static Vector3 yyz(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.z);
		}

		// Token: 0x06001521 RID: 5409 RVA: 0x00076062 File Offset: 0x00074262
		public static Vector3 yyw(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.w);
		}

		// Token: 0x06001522 RID: 5410 RVA: 0x0007607B File Offset: 0x0007427B
		public static Vector3 yzz(this Vector4 v)
		{
			return new Vector3(v.y, v.z, v.z);
		}

		// Token: 0x06001523 RID: 5411 RVA: 0x00076094 File Offset: 0x00074294
		public static Vector3 yzw(this Vector4 v)
		{
			return new Vector3(v.y, v.z, v.w);
		}

		// Token: 0x06001524 RID: 5412 RVA: 0x000760AD File Offset: 0x000742AD
		public static Vector3 yww(this Vector4 v)
		{
			return new Vector3(v.y, v.w, v.w);
		}

		// Token: 0x06001525 RID: 5413 RVA: 0x000760C6 File Offset: 0x000742C6
		public static Vector3 zzz(this Vector4 v)
		{
			return new Vector3(v.z, v.z, v.z);
		}

		// Token: 0x06001526 RID: 5414 RVA: 0x000760DF File Offset: 0x000742DF
		public static Vector3 zzw(this Vector4 v)
		{
			return new Vector3(v.z, v.z, v.w);
		}

		// Token: 0x06001527 RID: 5415 RVA: 0x000760F8 File Offset: 0x000742F8
		public static Vector3 zww(this Vector4 v)
		{
			return new Vector3(v.z, v.w, v.w);
		}

		// Token: 0x06001528 RID: 5416 RVA: 0x00076111 File Offset: 0x00074311
		public static Vector3 www(this Vector4 v)
		{
			return new Vector3(v.w, v.w, v.w);
		}

		// Token: 0x06001529 RID: 5417 RVA: 0x0007612A File Offset: 0x0007432A
		public static Vector4 xxxx(this float v)
		{
			return new Vector4(v, v, v, v);
		}

		// Token: 0x0600152A RID: 5418 RVA: 0x00076135 File Offset: 0x00074335
		public static Vector4 xxxx(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		// Token: 0x0600152B RID: 5419 RVA: 0x00076154 File Offset: 0x00074354
		public static Vector4 xxxy(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		// Token: 0x0600152C RID: 5420 RVA: 0x00076173 File Offset: 0x00074373
		public static Vector4 xxyy(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		// Token: 0x0600152D RID: 5421 RVA: 0x00076192 File Offset: 0x00074392
		public static Vector4 xyyy(this Vector2 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		// Token: 0x0600152E RID: 5422 RVA: 0x000761B1 File Offset: 0x000743B1
		public static Vector4 yyyy(this Vector2 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		// Token: 0x0600152F RID: 5423 RVA: 0x000761D0 File Offset: 0x000743D0
		public static Vector4 xxxx(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		// Token: 0x06001530 RID: 5424 RVA: 0x000761EF File Offset: 0x000743EF
		public static Vector4 xxxy(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		// Token: 0x06001531 RID: 5425 RVA: 0x0007620E File Offset: 0x0007440E
		public static Vector4 xxxz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.z);
		}

		// Token: 0x06001532 RID: 5426 RVA: 0x0007622D File Offset: 0x0007442D
		public static Vector4 xxyy(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		// Token: 0x06001533 RID: 5427 RVA: 0x0007624C File Offset: 0x0007444C
		public static Vector4 xxyz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.y, v.z);
		}

		// Token: 0x06001534 RID: 5428 RVA: 0x0007626B File Offset: 0x0007446B
		public static Vector4 xxzz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.z, v.z);
		}

		// Token: 0x06001535 RID: 5429 RVA: 0x0007628A File Offset: 0x0007448A
		public static Vector4 xyyy(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		// Token: 0x06001536 RID: 5430 RVA: 0x000762A9 File Offset: 0x000744A9
		public static Vector4 xyyz(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.y, v.z);
		}

		// Token: 0x06001537 RID: 5431 RVA: 0x000762C8 File Offset: 0x000744C8
		public static Vector4 xyzz(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.z, v.z);
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x000762E7 File Offset: 0x000744E7
		public static Vector4 xzzz(this Vector3 v)
		{
			return new Vector4(v.x, v.z, v.z, v.z);
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x00076306 File Offset: 0x00074506
		public static Vector4 yyyy(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		// Token: 0x0600153A RID: 5434 RVA: 0x00076325 File Offset: 0x00074525
		public static Vector4 yyyz(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.y, v.z);
		}

		// Token: 0x0600153B RID: 5435 RVA: 0x00076344 File Offset: 0x00074544
		public static Vector4 yyzz(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.z, v.z);
		}

		// Token: 0x0600153C RID: 5436 RVA: 0x00076363 File Offset: 0x00074563
		public static Vector4 yzzz(this Vector3 v)
		{
			return new Vector4(v.y, v.z, v.z, v.z);
		}

		// Token: 0x0600153D RID: 5437 RVA: 0x00076382 File Offset: 0x00074582
		public static Vector4 zzzz(this Vector3 v)
		{
			return new Vector4(v.z, v.z, v.z, v.z);
		}

		// Token: 0x0600153E RID: 5438 RVA: 0x000763A1 File Offset: 0x000745A1
		public static Vector4 xxxx(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		// Token: 0x0600153F RID: 5439 RVA: 0x000763C0 File Offset: 0x000745C0
		public static Vector4 xxxy(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		// Token: 0x06001540 RID: 5440 RVA: 0x000763DF File Offset: 0x000745DF
		public static Vector4 xxxz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.z);
		}

		// Token: 0x06001541 RID: 5441 RVA: 0x000763FE File Offset: 0x000745FE
		public static Vector4 xxxw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.w);
		}

		// Token: 0x06001542 RID: 5442 RVA: 0x0007641D File Offset: 0x0007461D
		public static Vector4 xxyy(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		// Token: 0x06001543 RID: 5443 RVA: 0x0007643C File Offset: 0x0007463C
		public static Vector4 xxyz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.z);
		}

		// Token: 0x06001544 RID: 5444 RVA: 0x0007645B File Offset: 0x0007465B
		public static Vector4 xxyw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.w);
		}

		// Token: 0x06001545 RID: 5445 RVA: 0x0007647A File Offset: 0x0007467A
		public static Vector4 xxzz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.z, v.z);
		}

		// Token: 0x06001546 RID: 5446 RVA: 0x00076499 File Offset: 0x00074699
		public static Vector4 xxzw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.z, v.w);
		}

		// Token: 0x06001547 RID: 5447 RVA: 0x000764B8 File Offset: 0x000746B8
		public static Vector4 xxww(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.w, v.w);
		}

		// Token: 0x06001548 RID: 5448 RVA: 0x000764D7 File Offset: 0x000746D7
		public static Vector4 xyyy(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		// Token: 0x06001549 RID: 5449 RVA: 0x000764F6 File Offset: 0x000746F6
		public static Vector4 xyyz(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.z);
		}

		// Token: 0x0600154A RID: 5450 RVA: 0x00076515 File Offset: 0x00074715
		public static Vector4 xyyw(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.w);
		}

		// Token: 0x0600154B RID: 5451 RVA: 0x00076534 File Offset: 0x00074734
		public static Vector4 xyzz(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.z, v.z);
		}

		// Token: 0x0600154C RID: 5452 RVA: 0x00076553 File Offset: 0x00074753
		public static Vector4 xyzw(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.z, v.w);
		}

		// Token: 0x0600154D RID: 5453 RVA: 0x00076572 File Offset: 0x00074772
		public static Vector4 xyww(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.w, v.w);
		}

		// Token: 0x0600154E RID: 5454 RVA: 0x00076591 File Offset: 0x00074791
		public static Vector4 xzzz(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.z, v.z);
		}

		// Token: 0x0600154F RID: 5455 RVA: 0x000765B0 File Offset: 0x000747B0
		public static Vector4 xzzw(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.z, v.w);
		}

		// Token: 0x06001550 RID: 5456 RVA: 0x000765CF File Offset: 0x000747CF
		public static Vector4 xzww(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.w, v.w);
		}

		// Token: 0x06001551 RID: 5457 RVA: 0x000765EE File Offset: 0x000747EE
		public static Vector4 xwww(this Vector4 v)
		{
			return new Vector4(v.x, v.w, v.w, v.w);
		}

		// Token: 0x06001552 RID: 5458 RVA: 0x0007660D File Offset: 0x0007480D
		public static Vector4 yyyy(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		// Token: 0x06001553 RID: 5459 RVA: 0x0007662C File Offset: 0x0007482C
		public static Vector4 yyyz(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.z);
		}

		// Token: 0x06001554 RID: 5460 RVA: 0x0007664B File Offset: 0x0007484B
		public static Vector4 yyyw(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.w);
		}

		// Token: 0x06001555 RID: 5461 RVA: 0x0007666A File Offset: 0x0007486A
		public static Vector4 yyzz(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.z, v.z);
		}

		// Token: 0x06001556 RID: 5462 RVA: 0x00076689 File Offset: 0x00074889
		public static Vector4 yyzw(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.z, v.w);
		}

		// Token: 0x06001557 RID: 5463 RVA: 0x000766A8 File Offset: 0x000748A8
		public static Vector4 yyww(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.w, v.w);
		}

		// Token: 0x06001558 RID: 5464 RVA: 0x000766C7 File Offset: 0x000748C7
		public static Vector4 yzzz(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.z, v.z);
		}

		// Token: 0x06001559 RID: 5465 RVA: 0x000766E6 File Offset: 0x000748E6
		public static Vector4 yzzw(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.z, v.w);
		}

		// Token: 0x0600155A RID: 5466 RVA: 0x00076705 File Offset: 0x00074905
		public static Vector4 yzww(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.w, v.w);
		}

		// Token: 0x0600155B RID: 5467 RVA: 0x00076724 File Offset: 0x00074924
		public static Vector4 ywww(this Vector4 v)
		{
			return new Vector4(v.y, v.w, v.w, v.w);
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x00076743 File Offset: 0x00074943
		public static Vector4 zzzz(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.z, v.z);
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x00076762 File Offset: 0x00074962
		public static Vector4 zzzw(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.z, v.w);
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x00076781 File Offset: 0x00074981
		public static Vector4 zzww(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.w, v.w);
		}

		// Token: 0x0600155F RID: 5471 RVA: 0x000767A0 File Offset: 0x000749A0
		public static Vector4 zwww(this Vector4 v)
		{
			return new Vector4(v.z, v.w, v.w, v.w);
		}

		// Token: 0x06001560 RID: 5472 RVA: 0x000767BF File Offset: 0x000749BF
		public static Vector4 wwww(this Vector4 v)
		{
			return new Vector4(v.w, v.w, v.w, v.w);
		}

		// Token: 0x06001561 RID: 5473 RVA: 0x000767DE File Offset: 0x000749DE
		public static Vector4 WithX(this Vector4 v, float x)
		{
			return new Vector4(x, v.y, v.z, v.w);
		}

		// Token: 0x06001562 RID: 5474 RVA: 0x000767F8 File Offset: 0x000749F8
		public static Vector4 WithY(this Vector4 v, float y)
		{
			return new Vector4(v.x, y, v.z, v.w);
		}

		// Token: 0x06001563 RID: 5475 RVA: 0x00076812 File Offset: 0x00074A12
		public static Vector4 WithZ(this Vector4 v, float z)
		{
			return new Vector4(v.x, v.y, z, v.w);
		}

		// Token: 0x06001564 RID: 5476 RVA: 0x0007682C File Offset: 0x00074A2C
		public static Vector4 WithW(this Vector4 v, float w)
		{
			return new Vector4(v.x, v.y, v.z, w);
		}

		// Token: 0x06001565 RID: 5477 RVA: 0x00076846 File Offset: 0x00074A46
		public static Vector3 WithX(this Vector3 v, float x)
		{
			return new Vector3(x, v.y, v.z);
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x0007685A File Offset: 0x00074A5A
		public static Vector3 WithY(this Vector3 v, float y)
		{
			return new Vector3(v.x, y, v.z);
		}

		// Token: 0x06001567 RID: 5479 RVA: 0x0007686E File Offset: 0x00074A6E
		public static Vector3 WithZ(this Vector3 v, float z)
		{
			return new Vector3(v.x, v.y, z);
		}

		// Token: 0x06001568 RID: 5480 RVA: 0x00076882 File Offset: 0x00074A82
		public static Vector4 WithW(this Vector3 v, float w)
		{
			return new Vector4(v.x, v.y, v.z, w);
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x0007689C File Offset: 0x00074A9C
		public static Vector2 WithX(this Vector2 v, float x)
		{
			return new Vector2(x, v.y);
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x000768AA File Offset: 0x00074AAA
		public static Vector2 WithY(this Vector2 v, float y)
		{
			return new Vector2(v.x, y);
		}

		// Token: 0x0600156B RID: 5483 RVA: 0x000768B8 File Offset: 0x00074AB8
		public static Vector3 WithZ(this Vector2 v, float z)
		{
			return new Vector3(v.x, v.y, z);
		}

		// Token: 0x0600156C RID: 5484 RVA: 0x000768CC File Offset: 0x00074ACC
		public static Vector3 GetClosestPoint(this Ray ray, Vector3 target)
		{
			float d = Vector3.Dot(target - ray.origin, ray.direction);
			return ray.origin + ray.direction * d;
		}

		// Token: 0x0600156D RID: 5485 RVA: 0x0007690C File Offset: 0x00074B0C
		public static float GetClosestDistSqr(this Ray ray, Vector3 target)
		{
			return (ray.GetClosestPoint(target) - target).sqrMagnitude;
		}

		// Token: 0x0600156E RID: 5486 RVA: 0x00076930 File Offset: 0x00074B30
		public static float GetClosestDistance(this Ray ray, Vector3 target)
		{
			return (ray.GetClosestPoint(target) - target).magnitude;
		}

		// Token: 0x0600156F RID: 5487 RVA: 0x00076954 File Offset: 0x00074B54
		public static Vector3 ProjectToPlane(this Ray ray, Vector3 planeOrigin, Vector3 planeNormalMustBeLength1)
		{
			Vector3 rhs = planeOrigin - ray.origin;
			float d = Vector3.Dot(planeNormalMustBeLength1, rhs);
			float d2 = Vector3.Dot(planeNormalMustBeLength1, ray.direction);
			return ray.origin + ray.direction * d / d2;
		}

		// Token: 0x06001570 RID: 5488 RVA: 0x000769A4 File Offset: 0x00074BA4
		public static Vector3 ProjectToLine(this Ray ray, Vector3 lineStart, Vector3 lineEnd)
		{
			Vector3 normalized = (lineEnd - lineStart).normalized;
			Vector3 normalized2 = Vector3.Cross(Vector3.Cross(ray.direction, normalized), normalized).normalized;
			return ray.ProjectToPlane(lineStart, normalized2);
		}

		// Token: 0x06001571 RID: 5489 RVA: 0x000769E5 File Offset: 0x00074BE5
		public static bool IsNull(this MonoBehaviour mono)
		{
			return mono == null || !mono;
		}

		// Token: 0x06001572 RID: 5490 RVA: 0x000769F5 File Offset: 0x00074BF5
		public static bool IsNotNull(this MonoBehaviour mono)
		{
			return !mono.IsNull();
		}

		// Token: 0x06001574 RID: 5492 RVA: 0x00076A14 File Offset: 0x00074C14
		[CompilerGenerated]
		internal static bool <GetComponentsInChildrenUntil>g__GetRecursive|4_0<T, TStop1, TStop2, TStop3>(Transform currentTransform, ref List<T> components, ref GTExt.<>c__DisplayClass4_0<T, TStop1, TStop2, TStop3> A_2) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if (A_2.includeInactive || transform.gameObject.activeSelf)
				{
					if (transform.GetComponent<TStop1>() != null || transform.GetComponent<TStop2>() != null || transform.GetComponent<TStop3>() != null)
					{
						return true;
					}
					T component = transform.GetComponent<T>();
					if (component != null)
					{
						components.Add(component);
					}
					if (GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|4_0<T, TStop1, TStop2, TStop3>(transform, ref components, ref A_2))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x04001781 RID: 6017
		private static StringBuilder tempStringBuilder = new StringBuilder(1024);

		// Token: 0x020004F9 RID: 1273
		public enum ParityOptions
		{
			// Token: 0x040020C6 RID: 8390
			XFlip,
			// Token: 0x040020C7 RID: 8391
			YFlip,
			// Token: 0x040020C8 RID: 8392
			ZFlip,
			// Token: 0x040020C9 RID: 8393
			AllFlip
		}
	}
}
