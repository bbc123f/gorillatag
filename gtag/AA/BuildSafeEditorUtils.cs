using System;
using UnityEngine;

namespace AA;

public static class BuildSafeEditorUtils
{
	public static T[] LoadAssetsOfType<T>() where T : UnityEngine.Object
	{
		return Array.Empty<T>();
	}

	public static string[] FindAssetsOfType<T>() where T : UnityEngine.Object
	{
		return Array.Empty<string>();
	}

	public static void SaveToDisk(params UnityEngine.Object[] assetsToSave)
	{
		if (assetsToSave != null && assetsToSave.Length != 0)
		{
			SaveAssetsToDisk(assetsToSave);
		}
	}

	public static void SaveAssetsToDisk(UnityEngine.Object[] assetsToSave)
	{
	}
}
