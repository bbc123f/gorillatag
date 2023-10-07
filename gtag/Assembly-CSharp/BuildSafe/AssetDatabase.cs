using System;
using System.Diagnostics;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x020002BB RID: 699
	public static class AssetDatabase
	{
		// Token: 0x060012F7 RID: 4855 RVA: 0x0006E5C4 File Offset: 0x0006C7C4
		public static T LoadAssetAtPath<T>(string assetPath) where T : Object
		{
			return default(T);
		}

		// Token: 0x060012F8 RID: 4856 RVA: 0x0006E5DA File Offset: 0x0006C7DA
		public static T[] LoadAssetsOfType<T>() where T : Object
		{
			return Array.Empty<T>();
		}

		// Token: 0x060012F9 RID: 4857 RVA: 0x0006E5E1 File Offset: 0x0006C7E1
		public static string[] FindAssetsOfType<T>() where T : Object
		{
			return Array.Empty<string>();
		}

		// Token: 0x060012FA RID: 4858 RVA: 0x0006E5E8 File Offset: 0x0006C7E8
		[Conditional("UNITY_EDITOR")]
		public static void SaveToDisk(params Object[] assetsToSave)
		{
			AssetDatabase.SaveAssetsToDisk(assetsToSave, true);
		}

		// Token: 0x060012FB RID: 4859 RVA: 0x0006E5F1 File Offset: 0x0006C7F1
		public static void SaveAssetsToDisk(Object[] assetsToSave, bool saveProject = true)
		{
		}
	}
}
