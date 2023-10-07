using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaExtensions
{
	// Token: 0x020002F8 RID: 760
	public static class GTExt
	{
		// Token: 0x0600149E RID: 5278 RVA: 0x000742DC File Offset: 0x000724DC
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

		// Token: 0x0600149F RID: 5279 RVA: 0x0007435C File Offset: 0x0007255C
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

		// Token: 0x060014A0 RID: 5280 RVA: 0x00074398 File Offset: 0x00072598
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

		// Token: 0x060014A1 RID: 5281 RVA: 0x000743D5 File Offset: 0x000725D5
		public static List<GameObject> GetGameObjectsInHierarchy(this Scene scene, bool includeInactive = true, int capacity = 64)
		{
			return scene.GetComponentsInHierarchy(includeInactive, capacity);
		}

		// Token: 0x060014A2 RID: 5282 RVA: 0x000743E0 File Offset: 0x000725E0
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

		// Token: 0x060014A3 RID: 5283 RVA: 0x00074468 File Offset: 0x00072668
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

		// Token: 0x060014A4 RID: 5284 RVA: 0x000744B8 File Offset: 0x000726B8
		private static List<T> GetComponentsWithRegex_Internal<T>(IEnumerable<T> allComponents, string regexString, bool includeInactive, int capacity = 64) where T : Component
		{
			List<T> result = new List<T>(capacity);
			Regex regex = new Regex(regexString);
			GTExt.GetComponentsWithRegex_Internal<T>(allComponents, regex, ref result);
			return result;
		}

		// Token: 0x060014A5 RID: 5285 RVA: 0x000744E0 File Offset: 0x000726E0
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

		// Token: 0x060014A6 RID: 5286 RVA: 0x00074540 File Offset: 0x00072740
		public static List<T> GetComponentsWithRegex<T>(this Scene scene, string regexString, bool includeInactive, int capacity) where T : Component
		{
			return GTExt.GetComponentsWithRegex_Internal<T>(scene.GetComponentsInHierarchy(includeInactive, capacity), regexString, includeInactive, capacity);
		}

		// Token: 0x060014A7 RID: 5287 RVA: 0x00074552 File Offset: 0x00072752
		public static List<T> GetComponentsWithRegex<T>(this Component root, string regexString, bool includeInactive, int capacity) where T : Component
		{
			return GTExt.GetComponentsWithRegex_Internal<T>(root.GetComponentsInChildren<T>(includeInactive), regexString, includeInactive, capacity);
		}

		// Token: 0x060014A8 RID: 5288 RVA: 0x00074564 File Offset: 0x00072764
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

		// Token: 0x060014A9 RID: 5289 RVA: 0x000745CC File Offset: 0x000727CC
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

		// Token: 0x060014AA RID: 5290 RVA: 0x0007465C File Offset: 0x0007285C
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

		// Token: 0x060014AB RID: 5291 RVA: 0x000746AC File Offset: 0x000728AC
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

		// Token: 0x060014AC RID: 5292 RVA: 0x000747B0 File Offset: 0x000729B0
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

		// Token: 0x060014AD RID: 5293 RVA: 0x0007481C File Offset: 0x00072A1C
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

		// Token: 0x060014AE RID: 5294 RVA: 0x00074888 File Offset: 0x00072A88
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

		// Token: 0x060014AF RID: 5295 RVA: 0x000748D8 File Offset: 0x00072AD8
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

		// Token: 0x060014B0 RID: 5296 RVA: 0x00074924 File Offset: 0x00072B24
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

		// Token: 0x060014B1 RID: 5297 RVA: 0x00074978 File Offset: 0x00072B78
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

		// Token: 0x060014B2 RID: 5298 RVA: 0x000749CF File Offset: 0x00072BCF
		public static string GetPath(this GameObject gameObject)
		{
			return gameObject.transform.GetPath();
		}

		// Token: 0x060014B3 RID: 5299 RVA: 0x000749DC File Offset: 0x00072BDC
		public static string GetPath(this GameObject gameObject, int limit)
		{
			return gameObject.transform.GetPath(limit);
		}

		// Token: 0x060014B4 RID: 5300 RVA: 0x000749EC File Offset: 0x00072BEC
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

		// Token: 0x060014B5 RID: 5301 RVA: 0x00074A56 File Offset: 0x00072C56
		public static string GetPathWithSiblingIndexes(this Transform transform)
		{
			GTExt.tempStringBuilder.Clear();
			transform.GetPathWithSiblingIndexes(GTExt.tempStringBuilder);
			return GTExt.tempStringBuilder.ToString();
		}

		// Token: 0x060014B6 RID: 5302 RVA: 0x00074A78 File Offset: 0x00072C78
		public static void GetPathWithSiblingIndexes(this GameObject gameObject, StringBuilder stringBuilder)
		{
			gameObject.transform.GetPathWithSiblingIndexes(stringBuilder);
		}

		// Token: 0x060014B7 RID: 5303 RVA: 0x00074A86 File Offset: 0x00072C86
		public static string GetPathWithSiblingIndexes(this GameObject gameObject)
		{
			return gameObject.transform.GetPathWithSiblingIndexes();
		}

		// Token: 0x060014B8 RID: 5304 RVA: 0x00074A94 File Offset: 0x00072C94
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

		// Token: 0x060014B9 RID: 5305 RVA: 0x00074B16 File Offset: 0x00072D16
		public static string GetComponentPath(this Component component, int maxDepth = 2147483647)
		{
			GTExt.tempStringBuilder.Clear();
			component.GetComponentPath(GTExt.tempStringBuilder, maxDepth);
			return GTExt.tempStringBuilder.ToString();
		}

		// Token: 0x060014BA RID: 5306 RVA: 0x00074B39 File Offset: 0x00072D39
		public static string GetComponentPath<T>(this T component, int maxDepth = 2147483647) where T : Component
		{
			GTExt.tempStringBuilder.Clear();
			component.GetComponentPath(GTExt.tempStringBuilder, maxDepth);
			return GTExt.tempStringBuilder.ToString();
		}

		// Token: 0x060014BB RID: 5307 RVA: 0x00074B5C File Offset: 0x00072D5C
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

		// Token: 0x060014BC RID: 5308 RVA: 0x00074C20 File Offset: 0x00072E20
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

		// Token: 0x060014BD RID: 5309 RVA: 0x00074CE7 File Offset: 0x00072EE7
		public static string GetComponentPathWithSiblingIndexes<T>(this T component) where T : Component
		{
			GTExt.tempStringBuilder.Clear();
			component.GetComponentPathWithSiblingIndexes(GTExt.tempStringBuilder);
			return GTExt.tempStringBuilder.ToString();
		}

		// Token: 0x060014BE RID: 5310 RVA: 0x00074D0C File Offset: 0x00072F0C
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

		// Token: 0x060014BF RID: 5311 RVA: 0x00074DE8 File Offset: 0x00072FE8
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

		// Token: 0x060014C0 RID: 5312 RVA: 0x00074E15 File Offset: 0x00073015
		public static T GetOrAddComponent<T>(this GameObject gameObject, ref T component) where T : Component
		{
			if (component == null)
			{
				component = gameObject.GetOrAddComponent<T>();
			}
			return component;
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x00074E3C File Offset: 0x0007303C
		public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
		{
			T result;
			if (!gameObject.TryGetComponent<T>(out result))
			{
				result = gameObject.AddComponent<T>();
			}
			return result;
		}

		// Token: 0x060014C2 RID: 5314 RVA: 0x00074E5C File Offset: 0x0007305C
		public static void SetLossyScale(this Transform transform, Vector3 scale)
		{
			scale = transform.InverseTransformVector(scale);
			Debug.Log(scale);
			Vector3 lossyScale = transform.lossyScale;
			Debug.Log(scale);
			transform.localScale = new Vector3(scale.x / lossyScale.x, scale.y / lossyScale.y, scale.z / lossyScale.z);
		}

		// Token: 0x060014C3 RID: 5315 RVA: 0x00074EC4 File Offset: 0x000730C4
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

		// Token: 0x060014C4 RID: 5316 RVA: 0x00074F0C File Offset: 0x0007310C
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

		// Token: 0x060014C5 RID: 5317 RVA: 0x00074F54 File Offset: 0x00073154
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

		// Token: 0x060014C6 RID: 5318 RVA: 0x00074FE8 File Offset: 0x000731E8
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

		// Token: 0x060014C7 RID: 5319 RVA: 0x0007508C File Offset: 0x0007328C
		public static Vector3 Position(this Matrix4x4 matrix)
		{
			float m = matrix.m03;
			float m2 = matrix.m13;
			float m3 = matrix.m23;
			return new Vector3(m, m2, m3);
		}

		// Token: 0x060014C8 RID: 5320 RVA: 0x000750B4 File Offset: 0x000732B4
		public static Vector3 Scale(this Matrix4x4 m)
		{
			Vector3 result = new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
			if (Vector3.Cross(m.GetColumn(0), m.GetColumn(1)).normalized != m.GetColumn(2).normalized)
			{
				result.x *= -1f;
			}
			return result;
		}

		// Token: 0x060014C9 RID: 5321 RVA: 0x0007514C File Offset: 0x0007334C
		public static void SetLocalRelativeToParentMatrixWithParityAxis(this Matrix4x4 matrix, GTExt.ParityOptions parity = GTExt.ParityOptions.XFlip)
		{
		}

		// Token: 0x060014CA RID: 5322 RVA: 0x0007514E File Offset: 0x0007334E
		public static void MultiplyInPlaceWith(this Vector3 a, in Vector3 b)
		{
			a.x *= b.x;
			a.y *= b.y;
			a.z *= b.z;
		}

		// Token: 0x060014CB RID: 5323 RVA: 0x00075180 File Offset: 0x00073380
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

		// Token: 0x060014CC RID: 5324 RVA: 0x000751E0 File Offset: 0x000733E0
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

		// Token: 0x060014CD RID: 5325 RVA: 0x00075210 File Offset: 0x00073410
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

		// Token: 0x060014CE RID: 5326 RVA: 0x000752E4 File Offset: 0x000734E4
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

		// Token: 0x060014CF RID: 5327 RVA: 0x00075390 File Offset: 0x00073590
		public static Quaternion RotationWithScaleContext(this Matrix4x4 m, in Vector3 scale)
		{
			Matrix4x4 matrix4x = m * GTExt.Matrix4x4Scale(scale);
			int num = 2;
			Vector3 forward = matrix4x.GetColumnNoCopy(num);
			int num2 = 1;
			return Quaternion.LookRotation(forward, matrix4x.GetColumnNoCopy(num2));
		}

		// Token: 0x060014D0 RID: 5328 RVA: 0x000753D4 File Offset: 0x000735D4
		public static Quaternion Rotation(this Matrix4x4 m)
		{
			int num = 2;
			Vector3 forward = m.GetColumnNoCopy(num);
			int num2 = 1;
			return Quaternion.LookRotation(forward, m.GetColumnNoCopy(num2));
		}

		// Token: 0x060014D1 RID: 5329 RVA: 0x00075404 File Offset: 0x00073604
		public static Vector3 x0y(this Vector2 v)
		{
			return new Vector3(v.x, 0f, v.y);
		}

		// Token: 0x060014D2 RID: 5330 RVA: 0x0007541C File Offset: 0x0007361C
		public static Vector3 x0y(this Vector3 v)
		{
			return new Vector3(v.x, 0f, v.y);
		}

		// Token: 0x060014D3 RID: 5331 RVA: 0x00075434 File Offset: 0x00073634
		public static Vector3 xy0(this Vector2 v)
		{
			return new Vector3(v.x, v.y, 0f);
		}

		// Token: 0x060014D4 RID: 5332 RVA: 0x0007544C File Offset: 0x0007364C
		public static Vector3 xy0(this Vector3 v)
		{
			return new Vector3(v.x, v.y, 0f);
		}

		// Token: 0x060014D5 RID: 5333 RVA: 0x00075464 File Offset: 0x00073664
		public static Vector3 xz0(this Vector3 v)
		{
			return new Vector3(v.x, v.z, 0f);
		}

		// Token: 0x060014D6 RID: 5334 RVA: 0x0007547C File Offset: 0x0007367C
		public static Vector3 x0z(this Vector3 v)
		{
			return new Vector3(v.x, 0f, v.z);
		}

		// Token: 0x060014D7 RID: 5335 RVA: 0x00075494 File Offset: 0x00073694
		public static Matrix4x4 LocalMatrixRelativeToParentNoScale(this Transform transform)
		{
			return Matrix4x4.TRS(transform.localPosition, transform.localRotation, Vector3.one);
		}

		// Token: 0x060014D8 RID: 5336 RVA: 0x000754AC File Offset: 0x000736AC
		public static Matrix4x4 LocalMatrixRelativeToParentWithScale(this Transform transform)
		{
			if (transform.parent == null)
			{
				return transform.localToWorldMatrix;
			}
			return transform.parent.worldToLocalMatrix * transform.localToWorldMatrix;
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x000754D9 File Offset: 0x000736D9
		public static void SetLocalMatrixRelativeToParent(this Transform transform, Matrix4x4 matrix)
		{
			transform.localPosition = matrix.Position();
			transform.localRotation = matrix.Rotation();
			transform.localScale = matrix.Scale();
		}

		// Token: 0x060014DA RID: 5338 RVA: 0x00075500 File Offset: 0x00073700
		public static void SetLocalMatrixRelativeToParentNoScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.localPosition = matrix.Position();
			transform.localRotation = matrix.Rotation();
		}

		// Token: 0x060014DB RID: 5339 RVA: 0x0007551B File Offset: 0x0007371B
		public static void SetLocalToWorldMatrixNoScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.position = matrix.Position();
			transform.rotation = matrix.Rotation();
		}

		// Token: 0x060014DC RID: 5340 RVA: 0x00075536 File Offset: 0x00073736
		public static Matrix4x4 localToWorldNoScale(this Transform transform)
		{
			return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		}

		// Token: 0x060014DD RID: 5341 RVA: 0x0007554E File Offset: 0x0007374E
		public static void SetLocalToWorldMatrixWithScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.position = matrix.Position();
			transform.rotation = matrix.rotation;
			transform.SetLossyScale(matrix.lossyScale);
		}

		// Token: 0x060014DE RID: 5342 RVA: 0x00075576 File Offset: 0x00073776
		public static Matrix4x4 Matrix4X4LerpNoScale(Matrix4x4 a, Matrix4x4 b, float t)
		{
			return Matrix4x4.TRS(Vector3.Lerp(a.Position(), b.Position(), t), Quaternion.Slerp(a.rotation, b.rotation, t), b.lossyScale);
		}

		// Token: 0x060014DF RID: 5343 RVA: 0x000755AA File Offset: 0x000737AA
		public static Matrix4x4 LerpTo(this Matrix4x4 a, Matrix4x4 b, float t)
		{
			return GTExt.Matrix4X4LerpNoScale(a, b, t);
		}

		// Token: 0x060014E0 RID: 5344 RVA: 0x000755B4 File Offset: 0x000737B4
		public static bool IsNaN(this Vector3 vector)
		{
			return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);
		}

		// Token: 0x060014E1 RID: 5345 RVA: 0x000755DD File Offset: 0x000737DD
		public static bool IsNan(this Quaternion quat)
		{
			return float.IsNaN(quat.x) || float.IsNaN(quat.y) || float.IsNaN(quat.z) || float.IsNaN(quat.w);
		}

		// Token: 0x060014E2 RID: 5346 RVA: 0x00075613 File Offset: 0x00073813
		public static bool IsInfinity(this Vector3 vector)
		{
			return float.IsInfinity(vector.x) || float.IsInfinity(vector.y) || float.IsInfinity(vector.z);
		}

		// Token: 0x060014E3 RID: 5347 RVA: 0x0007563C File Offset: 0x0007383C
		public static bool IsInfinity(this Quaternion quat)
		{
			return float.IsInfinity(quat.x) || float.IsInfinity(quat.y) || float.IsInfinity(quat.z) || float.IsInfinity(quat.w);
		}

		// Token: 0x060014E4 RID: 5348 RVA: 0x00075672 File Offset: 0x00073872
		public static bool IsValid(this Vector3 vector)
		{
			return !vector.IsNaN() && !vector.IsInfinity();
		}

		// Token: 0x060014E5 RID: 5349 RVA: 0x00075688 File Offset: 0x00073888
		public static bool IsValid(this Quaternion quat)
		{
			return !quat.IsNan() && !quat.IsInfinity() && (quat.x != 0f || quat.y != 0f || quat.z != 0f || quat.w != 0f);
		}

		// Token: 0x060014E6 RID: 5350 RVA: 0x000756E0 File Offset: 0x000738E0
		public static Matrix4x4 Matrix4X4LerpHandleNegativeScale(Matrix4x4 a, Matrix4x4 b, float t)
		{
			return Matrix4x4.TRS(Vector3.Lerp(a.Position(), b.Position(), t), Quaternion.Slerp(a.Rotation(), b.Rotation(), t), b.lossyScale);
		}

		// Token: 0x060014E7 RID: 5351 RVA: 0x00075714 File Offset: 0x00073914
		public static Matrix4x4 LerpTo_HandleNegativeScale(this Matrix4x4 a, Matrix4x4 b, float t)
		{
			return GTExt.Matrix4X4LerpHandleNegativeScale(a, b, t);
		}

		// Token: 0x060014E8 RID: 5352 RVA: 0x00075720 File Offset: 0x00073920
		public static Vector3 LerpToUnclamped(this Vector3 a, in Vector3 b, float t)
		{
			return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
		}

		// Token: 0x060014E9 RID: 5353 RVA: 0x00075774 File Offset: 0x00073974
		public static int GetRandomIndex<T>(this IReadOnlyList<T> self)
		{
			return Random.Range(0, self.Count);
		}

		// Token: 0x060014EA RID: 5354 RVA: 0x00075782 File Offset: 0x00073982
		public static T GetRandomItem<T>(this IReadOnlyList<T> self)
		{
			return self[self.GetRandomIndex<T>()];
		}

		// Token: 0x060014EB RID: 5355 RVA: 0x00075790 File Offset: 0x00073990
		public static Vector2 xx(this float v)
		{
			return new Vector2(v, v);
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x00075799 File Offset: 0x00073999
		public static Vector2 xx(this Vector2 v)
		{
			return new Vector2(v.x, v.x);
		}

		// Token: 0x060014ED RID: 5357 RVA: 0x000757AC File Offset: 0x000739AC
		public static Vector2 xy(this Vector2 v)
		{
			return new Vector2(v.x, v.y);
		}

		// Token: 0x060014EE RID: 5358 RVA: 0x000757BF File Offset: 0x000739BF
		public static Vector2 yy(this Vector2 v)
		{
			return new Vector2(v.y, v.y);
		}

		// Token: 0x060014EF RID: 5359 RVA: 0x000757D2 File Offset: 0x000739D2
		public static Vector2 xx(this Vector3 v)
		{
			return new Vector2(v.x, v.x);
		}

		// Token: 0x060014F0 RID: 5360 RVA: 0x000757E5 File Offset: 0x000739E5
		public static Vector2 xy(this Vector3 v)
		{
			return new Vector2(v.x, v.y);
		}

		// Token: 0x060014F1 RID: 5361 RVA: 0x000757F8 File Offset: 0x000739F8
		public static Vector2 xz(this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		// Token: 0x060014F2 RID: 5362 RVA: 0x0007580B File Offset: 0x00073A0B
		public static Vector2 yy(this Vector3 v)
		{
			return new Vector2(v.y, v.y);
		}

		// Token: 0x060014F3 RID: 5363 RVA: 0x0007581E File Offset: 0x00073A1E
		public static Vector2 yz(this Vector3 v)
		{
			return new Vector2(v.y, v.z);
		}

		// Token: 0x060014F4 RID: 5364 RVA: 0x00075831 File Offset: 0x00073A31
		public static Vector2 zz(this Vector3 v)
		{
			return new Vector2(v.z, v.z);
		}

		// Token: 0x060014F5 RID: 5365 RVA: 0x00075844 File Offset: 0x00073A44
		public static Vector2 xx(this Vector4 v)
		{
			return new Vector2(v.x, v.x);
		}

		// Token: 0x060014F6 RID: 5366 RVA: 0x00075857 File Offset: 0x00073A57
		public static Vector2 xy(this Vector4 v)
		{
			return new Vector2(v.x, v.y);
		}

		// Token: 0x060014F7 RID: 5367 RVA: 0x0007586A File Offset: 0x00073A6A
		public static Vector2 xz(this Vector4 v)
		{
			return new Vector2(v.x, v.z);
		}

		// Token: 0x060014F8 RID: 5368 RVA: 0x0007587D File Offset: 0x00073A7D
		public static Vector2 xw(this Vector4 v)
		{
			return new Vector2(v.x, v.w);
		}

		// Token: 0x060014F9 RID: 5369 RVA: 0x00075890 File Offset: 0x00073A90
		public static Vector2 yy(this Vector4 v)
		{
			return new Vector2(v.y, v.y);
		}

		// Token: 0x060014FA RID: 5370 RVA: 0x000758A3 File Offset: 0x00073AA3
		public static Vector2 yz(this Vector4 v)
		{
			return new Vector2(v.y, v.z);
		}

		// Token: 0x060014FB RID: 5371 RVA: 0x000758B6 File Offset: 0x00073AB6
		public static Vector2 yw(this Vector4 v)
		{
			return new Vector2(v.y, v.w);
		}

		// Token: 0x060014FC RID: 5372 RVA: 0x000758C9 File Offset: 0x00073AC9
		public static Vector2 zz(this Vector4 v)
		{
			return new Vector2(v.z, v.z);
		}

		// Token: 0x060014FD RID: 5373 RVA: 0x000758DC File Offset: 0x00073ADC
		public static Vector2 zw(this Vector4 v)
		{
			return new Vector2(v.z, v.w);
		}

		// Token: 0x060014FE RID: 5374 RVA: 0x000758EF File Offset: 0x00073AEF
		public static Vector2 ww(this Vector4 v)
		{
			return new Vector2(v.w, v.w);
		}

		// Token: 0x060014FF RID: 5375 RVA: 0x00075902 File Offset: 0x00073B02
		public static Vector3 xxx(this float v)
		{
			return new Vector3(v, v, v);
		}

		// Token: 0x06001500 RID: 5376 RVA: 0x0007590C File Offset: 0x00073B0C
		public static Vector3 xxx(this Vector2 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		// Token: 0x06001501 RID: 5377 RVA: 0x00075925 File Offset: 0x00073B25
		public static Vector3 xxy(this Vector2 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		// Token: 0x06001502 RID: 5378 RVA: 0x0007593E File Offset: 0x00073B3E
		public static Vector3 xyy(this Vector2 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		// Token: 0x06001503 RID: 5379 RVA: 0x00075957 File Offset: 0x00073B57
		public static Vector3 yyy(this Vector2 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		// Token: 0x06001504 RID: 5380 RVA: 0x00075970 File Offset: 0x00073B70
		public static Vector3 xxx(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		// Token: 0x06001505 RID: 5381 RVA: 0x00075989 File Offset: 0x00073B89
		public static Vector3 xxy(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		// Token: 0x06001506 RID: 5382 RVA: 0x000759A2 File Offset: 0x00073BA2
		public static Vector3 xxz(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.z);
		}

		// Token: 0x06001507 RID: 5383 RVA: 0x000759BB File Offset: 0x00073BBB
		public static Vector3 xyy(this Vector3 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		// Token: 0x06001508 RID: 5384 RVA: 0x000759D4 File Offset: 0x00073BD4
		public static Vector3 xyz(this Vector3 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		// Token: 0x06001509 RID: 5385 RVA: 0x000759ED File Offset: 0x00073BED
		public static Vector3 xzz(this Vector3 v)
		{
			return new Vector3(v.x, v.z, v.z);
		}

		// Token: 0x0600150A RID: 5386 RVA: 0x00075A06 File Offset: 0x00073C06
		public static Vector3 yyy(this Vector3 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		// Token: 0x0600150B RID: 5387 RVA: 0x00075A1F File Offset: 0x00073C1F
		public static Vector3 yyz(this Vector3 v)
		{
			return new Vector3(v.y, v.y, v.z);
		}

		// Token: 0x0600150C RID: 5388 RVA: 0x00075A38 File Offset: 0x00073C38
		public static Vector3 yzz(this Vector3 v)
		{
			return new Vector3(v.y, v.z, v.z);
		}

		// Token: 0x0600150D RID: 5389 RVA: 0x00075A51 File Offset: 0x00073C51
		public static Vector3 zzz(this Vector3 v)
		{
			return new Vector3(v.z, v.z, v.z);
		}

		// Token: 0x0600150E RID: 5390 RVA: 0x00075A6A File Offset: 0x00073C6A
		public static Vector3 xxx(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		// Token: 0x0600150F RID: 5391 RVA: 0x00075A83 File Offset: 0x00073C83
		public static Vector3 xxy(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		// Token: 0x06001510 RID: 5392 RVA: 0x00075A9C File Offset: 0x00073C9C
		public static Vector3 xxz(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.z);
		}

		// Token: 0x06001511 RID: 5393 RVA: 0x00075AB5 File Offset: 0x00073CB5
		public static Vector3 xxw(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.w);
		}

		// Token: 0x06001512 RID: 5394 RVA: 0x00075ACE File Offset: 0x00073CCE
		public static Vector3 xyy(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		// Token: 0x06001513 RID: 5395 RVA: 0x00075AE7 File Offset: 0x00073CE7
		public static Vector3 xyz(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		// Token: 0x06001514 RID: 5396 RVA: 0x00075B00 File Offset: 0x00073D00
		public static Vector3 xyw(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.w);
		}

		// Token: 0x06001515 RID: 5397 RVA: 0x00075B19 File Offset: 0x00073D19
		public static Vector3 xzz(this Vector4 v)
		{
			return new Vector3(v.x, v.z, v.z);
		}

		// Token: 0x06001516 RID: 5398 RVA: 0x00075B32 File Offset: 0x00073D32
		public static Vector3 xzw(this Vector4 v)
		{
			return new Vector3(v.x, v.z, v.w);
		}

		// Token: 0x06001517 RID: 5399 RVA: 0x00075B4B File Offset: 0x00073D4B
		public static Vector3 xww(this Vector4 v)
		{
			return new Vector3(v.x, v.w, v.w);
		}

		// Token: 0x06001518 RID: 5400 RVA: 0x00075B64 File Offset: 0x00073D64
		public static Vector3 yyy(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		// Token: 0x06001519 RID: 5401 RVA: 0x00075B7D File Offset: 0x00073D7D
		public static Vector3 yyz(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.z);
		}

		// Token: 0x0600151A RID: 5402 RVA: 0x00075B96 File Offset: 0x00073D96
		public static Vector3 yyw(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.w);
		}

		// Token: 0x0600151B RID: 5403 RVA: 0x00075BAF File Offset: 0x00073DAF
		public static Vector3 yzz(this Vector4 v)
		{
			return new Vector3(v.y, v.z, v.z);
		}

		// Token: 0x0600151C RID: 5404 RVA: 0x00075BC8 File Offset: 0x00073DC8
		public static Vector3 yzw(this Vector4 v)
		{
			return new Vector3(v.y, v.z, v.w);
		}

		// Token: 0x0600151D RID: 5405 RVA: 0x00075BE1 File Offset: 0x00073DE1
		public static Vector3 yww(this Vector4 v)
		{
			return new Vector3(v.y, v.w, v.w);
		}

		// Token: 0x0600151E RID: 5406 RVA: 0x00075BFA File Offset: 0x00073DFA
		public static Vector3 zzz(this Vector4 v)
		{
			return new Vector3(v.z, v.z, v.z);
		}

		// Token: 0x0600151F RID: 5407 RVA: 0x00075C13 File Offset: 0x00073E13
		public static Vector3 zzw(this Vector4 v)
		{
			return new Vector3(v.z, v.z, v.w);
		}

		// Token: 0x06001520 RID: 5408 RVA: 0x00075C2C File Offset: 0x00073E2C
		public static Vector3 zww(this Vector4 v)
		{
			return new Vector3(v.z, v.w, v.w);
		}

		// Token: 0x06001521 RID: 5409 RVA: 0x00075C45 File Offset: 0x00073E45
		public static Vector3 www(this Vector4 v)
		{
			return new Vector3(v.w, v.w, v.w);
		}

		// Token: 0x06001522 RID: 5410 RVA: 0x00075C5E File Offset: 0x00073E5E
		public static Vector4 xxxx(this float v)
		{
			return new Vector4(v, v, v, v);
		}

		// Token: 0x06001523 RID: 5411 RVA: 0x00075C69 File Offset: 0x00073E69
		public static Vector4 xxxx(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		// Token: 0x06001524 RID: 5412 RVA: 0x00075C88 File Offset: 0x00073E88
		public static Vector4 xxxy(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		// Token: 0x06001525 RID: 5413 RVA: 0x00075CA7 File Offset: 0x00073EA7
		public static Vector4 xxyy(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		// Token: 0x06001526 RID: 5414 RVA: 0x00075CC6 File Offset: 0x00073EC6
		public static Vector4 xyyy(this Vector2 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		// Token: 0x06001527 RID: 5415 RVA: 0x00075CE5 File Offset: 0x00073EE5
		public static Vector4 yyyy(this Vector2 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		// Token: 0x06001528 RID: 5416 RVA: 0x00075D04 File Offset: 0x00073F04
		public static Vector4 xxxx(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		// Token: 0x06001529 RID: 5417 RVA: 0x00075D23 File Offset: 0x00073F23
		public static Vector4 xxxy(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		// Token: 0x0600152A RID: 5418 RVA: 0x00075D42 File Offset: 0x00073F42
		public static Vector4 xxxz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.z);
		}

		// Token: 0x0600152B RID: 5419 RVA: 0x00075D61 File Offset: 0x00073F61
		public static Vector4 xxyy(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		// Token: 0x0600152C RID: 5420 RVA: 0x00075D80 File Offset: 0x00073F80
		public static Vector4 xxyz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.y, v.z);
		}

		// Token: 0x0600152D RID: 5421 RVA: 0x00075D9F File Offset: 0x00073F9F
		public static Vector4 xxzz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.z, v.z);
		}

		// Token: 0x0600152E RID: 5422 RVA: 0x00075DBE File Offset: 0x00073FBE
		public static Vector4 xyyy(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		// Token: 0x0600152F RID: 5423 RVA: 0x00075DDD File Offset: 0x00073FDD
		public static Vector4 xyyz(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.y, v.z);
		}

		// Token: 0x06001530 RID: 5424 RVA: 0x00075DFC File Offset: 0x00073FFC
		public static Vector4 xyzz(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.z, v.z);
		}

		// Token: 0x06001531 RID: 5425 RVA: 0x00075E1B File Offset: 0x0007401B
		public static Vector4 xzzz(this Vector3 v)
		{
			return new Vector4(v.x, v.z, v.z, v.z);
		}

		// Token: 0x06001532 RID: 5426 RVA: 0x00075E3A File Offset: 0x0007403A
		public static Vector4 yyyy(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		// Token: 0x06001533 RID: 5427 RVA: 0x00075E59 File Offset: 0x00074059
		public static Vector4 yyyz(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.y, v.z);
		}

		// Token: 0x06001534 RID: 5428 RVA: 0x00075E78 File Offset: 0x00074078
		public static Vector4 yyzz(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.z, v.z);
		}

		// Token: 0x06001535 RID: 5429 RVA: 0x00075E97 File Offset: 0x00074097
		public static Vector4 yzzz(this Vector3 v)
		{
			return new Vector4(v.y, v.z, v.z, v.z);
		}

		// Token: 0x06001536 RID: 5430 RVA: 0x00075EB6 File Offset: 0x000740B6
		public static Vector4 zzzz(this Vector3 v)
		{
			return new Vector4(v.z, v.z, v.z, v.z);
		}

		// Token: 0x06001537 RID: 5431 RVA: 0x00075ED5 File Offset: 0x000740D5
		public static Vector4 xxxx(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x00075EF4 File Offset: 0x000740F4
		public static Vector4 xxxy(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x00075F13 File Offset: 0x00074113
		public static Vector4 xxxz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.z);
		}

		// Token: 0x0600153A RID: 5434 RVA: 0x00075F32 File Offset: 0x00074132
		public static Vector4 xxxw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.w);
		}

		// Token: 0x0600153B RID: 5435 RVA: 0x00075F51 File Offset: 0x00074151
		public static Vector4 xxyy(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		// Token: 0x0600153C RID: 5436 RVA: 0x00075F70 File Offset: 0x00074170
		public static Vector4 xxyz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.z);
		}

		// Token: 0x0600153D RID: 5437 RVA: 0x00075F8F File Offset: 0x0007418F
		public static Vector4 xxyw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.w);
		}

		// Token: 0x0600153E RID: 5438 RVA: 0x00075FAE File Offset: 0x000741AE
		public static Vector4 xxzz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.z, v.z);
		}

		// Token: 0x0600153F RID: 5439 RVA: 0x00075FCD File Offset: 0x000741CD
		public static Vector4 xxzw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.z, v.w);
		}

		// Token: 0x06001540 RID: 5440 RVA: 0x00075FEC File Offset: 0x000741EC
		public static Vector4 xxww(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.w, v.w);
		}

		// Token: 0x06001541 RID: 5441 RVA: 0x0007600B File Offset: 0x0007420B
		public static Vector4 xyyy(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		// Token: 0x06001542 RID: 5442 RVA: 0x0007602A File Offset: 0x0007422A
		public static Vector4 xyyz(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.z);
		}

		// Token: 0x06001543 RID: 5443 RVA: 0x00076049 File Offset: 0x00074249
		public static Vector4 xyyw(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.w);
		}

		// Token: 0x06001544 RID: 5444 RVA: 0x00076068 File Offset: 0x00074268
		public static Vector4 xyzz(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.z, v.z);
		}

		// Token: 0x06001545 RID: 5445 RVA: 0x00076087 File Offset: 0x00074287
		public static Vector4 xyzw(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.z, v.w);
		}

		// Token: 0x06001546 RID: 5446 RVA: 0x000760A6 File Offset: 0x000742A6
		public static Vector4 xyww(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.w, v.w);
		}

		// Token: 0x06001547 RID: 5447 RVA: 0x000760C5 File Offset: 0x000742C5
		public static Vector4 xzzz(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.z, v.z);
		}

		// Token: 0x06001548 RID: 5448 RVA: 0x000760E4 File Offset: 0x000742E4
		public static Vector4 xzzw(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.z, v.w);
		}

		// Token: 0x06001549 RID: 5449 RVA: 0x00076103 File Offset: 0x00074303
		public static Vector4 xzww(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.w, v.w);
		}

		// Token: 0x0600154A RID: 5450 RVA: 0x00076122 File Offset: 0x00074322
		public static Vector4 xwww(this Vector4 v)
		{
			return new Vector4(v.x, v.w, v.w, v.w);
		}

		// Token: 0x0600154B RID: 5451 RVA: 0x00076141 File Offset: 0x00074341
		public static Vector4 yyyy(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		// Token: 0x0600154C RID: 5452 RVA: 0x00076160 File Offset: 0x00074360
		public static Vector4 yyyz(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.z);
		}

		// Token: 0x0600154D RID: 5453 RVA: 0x0007617F File Offset: 0x0007437F
		public static Vector4 yyyw(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.w);
		}

		// Token: 0x0600154E RID: 5454 RVA: 0x0007619E File Offset: 0x0007439E
		public static Vector4 yyzz(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.z, v.z);
		}

		// Token: 0x0600154F RID: 5455 RVA: 0x000761BD File Offset: 0x000743BD
		public static Vector4 yyzw(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.z, v.w);
		}

		// Token: 0x06001550 RID: 5456 RVA: 0x000761DC File Offset: 0x000743DC
		public static Vector4 yyww(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.w, v.w);
		}

		// Token: 0x06001551 RID: 5457 RVA: 0x000761FB File Offset: 0x000743FB
		public static Vector4 yzzz(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.z, v.z);
		}

		// Token: 0x06001552 RID: 5458 RVA: 0x0007621A File Offset: 0x0007441A
		public static Vector4 yzzw(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.z, v.w);
		}

		// Token: 0x06001553 RID: 5459 RVA: 0x00076239 File Offset: 0x00074439
		public static Vector4 yzww(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.w, v.w);
		}

		// Token: 0x06001554 RID: 5460 RVA: 0x00076258 File Offset: 0x00074458
		public static Vector4 ywww(this Vector4 v)
		{
			return new Vector4(v.y, v.w, v.w, v.w);
		}

		// Token: 0x06001555 RID: 5461 RVA: 0x00076277 File Offset: 0x00074477
		public static Vector4 zzzz(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.z, v.z);
		}

		// Token: 0x06001556 RID: 5462 RVA: 0x00076296 File Offset: 0x00074496
		public static Vector4 zzzw(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.z, v.w);
		}

		// Token: 0x06001557 RID: 5463 RVA: 0x000762B5 File Offset: 0x000744B5
		public static Vector4 zzww(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.w, v.w);
		}

		// Token: 0x06001558 RID: 5464 RVA: 0x000762D4 File Offset: 0x000744D4
		public static Vector4 zwww(this Vector4 v)
		{
			return new Vector4(v.z, v.w, v.w, v.w);
		}

		// Token: 0x06001559 RID: 5465 RVA: 0x000762F3 File Offset: 0x000744F3
		public static Vector4 wwww(this Vector4 v)
		{
			return new Vector4(v.w, v.w, v.w, v.w);
		}

		// Token: 0x0600155A RID: 5466 RVA: 0x00076312 File Offset: 0x00074512
		public static Vector4 WithX(this Vector4 v, float x)
		{
			return new Vector4(x, v.y, v.z, v.w);
		}

		// Token: 0x0600155B RID: 5467 RVA: 0x0007632C File Offset: 0x0007452C
		public static Vector4 WithY(this Vector4 v, float y)
		{
			return new Vector4(v.x, y, v.z, v.w);
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x00076346 File Offset: 0x00074546
		public static Vector4 WithZ(this Vector4 v, float z)
		{
			return new Vector4(v.x, v.y, z, v.w);
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x00076360 File Offset: 0x00074560
		public static Vector4 WithW(this Vector4 v, float w)
		{
			return new Vector4(v.x, v.y, v.z, w);
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x0007637A File Offset: 0x0007457A
		public static Vector3 WithX(this Vector3 v, float x)
		{
			return new Vector3(x, v.y, v.z);
		}

		// Token: 0x0600155F RID: 5471 RVA: 0x0007638E File Offset: 0x0007458E
		public static Vector3 WithY(this Vector3 v, float y)
		{
			return new Vector3(v.x, y, v.z);
		}

		// Token: 0x06001560 RID: 5472 RVA: 0x000763A2 File Offset: 0x000745A2
		public static Vector3 WithZ(this Vector3 v, float z)
		{
			return new Vector3(v.x, v.y, z);
		}

		// Token: 0x06001561 RID: 5473 RVA: 0x000763B6 File Offset: 0x000745B6
		public static Vector4 WithW(this Vector3 v, float w)
		{
			return new Vector4(v.x, v.y, v.z, w);
		}

		// Token: 0x06001562 RID: 5474 RVA: 0x000763D0 File Offset: 0x000745D0
		public static Vector2 WithX(this Vector2 v, float x)
		{
			return new Vector2(x, v.y);
		}

		// Token: 0x06001563 RID: 5475 RVA: 0x000763DE File Offset: 0x000745DE
		public static Vector2 WithY(this Vector2 v, float y)
		{
			return new Vector2(v.x, y);
		}

		// Token: 0x06001564 RID: 5476 RVA: 0x000763EC File Offset: 0x000745EC
		public static Vector3 WithZ(this Vector2 v, float z)
		{
			return new Vector3(v.x, v.y, z);
		}

		// Token: 0x06001565 RID: 5477 RVA: 0x00076400 File Offset: 0x00074600
		public static Vector3 GetClosestPoint(this Ray ray, Vector3 target)
		{
			float d = Vector3.Dot(target - ray.origin, ray.direction);
			return ray.origin + ray.direction * d;
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x00076440 File Offset: 0x00074640
		public static float GetClosestDistSqr(this Ray ray, Vector3 target)
		{
			return (ray.GetClosestPoint(target) - target).sqrMagnitude;
		}

		// Token: 0x06001567 RID: 5479 RVA: 0x00076464 File Offset: 0x00074664
		public static float GetClosestDistance(this Ray ray, Vector3 target)
		{
			return (ray.GetClosestPoint(target) - target).magnitude;
		}

		// Token: 0x06001568 RID: 5480 RVA: 0x00076488 File Offset: 0x00074688
		public static Vector3 ProjectToPlane(this Ray ray, Vector3 planeOrigin, Vector3 planeNormalMustBeLength1)
		{
			Vector3 rhs = planeOrigin - ray.origin;
			float d = Vector3.Dot(planeNormalMustBeLength1, rhs);
			float d2 = Vector3.Dot(planeNormalMustBeLength1, ray.direction);
			return ray.origin + ray.direction * d / d2;
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x000764D8 File Offset: 0x000746D8
		public static Vector3 ProjectToLine(this Ray ray, Vector3 lineStart, Vector3 lineEnd)
		{
			Vector3 normalized = (lineEnd - lineStart).normalized;
			Vector3 normalized2 = Vector3.Cross(Vector3.Cross(ray.direction, normalized), normalized).normalized;
			return ray.ProjectToPlane(lineStart, normalized2);
		}

		// Token: 0x0600156B RID: 5483 RVA: 0x0007652C File Offset: 0x0007472C
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

		// Token: 0x04001774 RID: 6004
		private static StringBuilder tempStringBuilder = new StringBuilder(1024);

		// Token: 0x020004F7 RID: 1271
		public enum ParityOptions
		{
			// Token: 0x040020B9 RID: 8377
			XFlip,
			// Token: 0x040020BA RID: 8378
			YFlip,
			// Token: 0x040020BB RID: 8379
			ZFlip,
			// Token: 0x040020BC RID: 8380
			AllFlip
		}
	}
}
