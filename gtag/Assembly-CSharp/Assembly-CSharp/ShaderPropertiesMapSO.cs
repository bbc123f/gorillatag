using System;
using System.Collections.Generic;
using UnityEngine;

public class ShaderPropertiesMapSO : ScriptableObject
{
	public static bool hasInstance { get; private set; }

	public static ShaderPropertiesMapSO GetInstance()
	{
		if (ShaderPropertiesMapSO.hasInstance)
		{
			return ShaderPropertiesMapSO._instance;
		}
		ShaderPropertiesMapSO._instance = Resources.Load<ShaderPropertiesMapSO>("ShaderPropertiesMapSO");
		if (ShaderPropertiesMapSO._instance == null && ShaderPropertiesMapSO._instance == null)
		{
			Debug.LogError("ShaderPropertiesMapSO: Could not load \"ShaderPropertiesMapSO.asset\"");
		}
		ShaderPropertiesMapSO.hasInstance = (ShaderPropertiesMapSO._instance != null);
		return ShaderPropertiesMapSO._instance;
	}

	public Dictionary<string, string[]> shaderToTex2DProps = new Dictionary<string, string[]>(600);

	private const string kResourcesAssetName = "ShaderPropertiesMapSO";

	private static ShaderPropertiesMapSO _instance;
}
