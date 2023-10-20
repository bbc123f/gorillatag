using System;
using System.Diagnostics;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x020002BD RID: 701
	public static class AssetDatabase
	{
		// Token: 0x060012FE RID: 4862 RVA: 0x0006EA90 File Offset: 0x0006CC90
		public static T LoadAssetAtPath<T>(string assetPath) where T : Object
		{
			return default(T);
		}

		// Token: 0x060012FF RID: 4863 RVA: 0x0006EAA6 File Offset: 0x0006CCA6
		public static T[] LoadAssetsOfType<T>() where T : Object
		{
			return Array.Empty<T>();
		}

		// Token: 0x06001300 RID: 4864 RVA: 0x0006EAAD File Offset: 0x0006CCAD
		public static string[] FindAssetsOfType<T>() where T : Object
		{
			return Array.Empty<string>();
		}

		// Token: 0x06001301 RID: 4865 RVA: 0x0006EAB4 File Offset: 0x0006CCB4
		[Conditional("UNITY_EDITOR")]
		public static void SaveToDisk(params Object[] assetsToSave)
		{
			AssetDatabase.SaveAssetsToDisk(assetsToSave, true);
		}

		// Token: 0x06001302 RID: 4866 RVA: 0x0006EABD File Offset: 0x0006CCBD
		public static void SaveAssetsToDisk(Object[] assetsToSave, bool saveProject = true)
		{
		}
	}
}
