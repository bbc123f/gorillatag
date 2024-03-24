using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ShaderPropertiesMapSO : ScriptableObject
{
	public static bool hasInstance
	{
		[CompilerGenerated]
		get
		{
			return ShaderPropertiesMapSO.<hasInstance>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			ShaderPropertiesMapSO.<hasInstance>k__BackingField = value;
		}
	}

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
		ShaderPropertiesMapSO.hasInstance = ShaderPropertiesMapSO._instance != null;
		return ShaderPropertiesMapSO._instance;
	}

	public ShaderPropertiesMapSO()
	{
	}

	public Dictionary<string, string[]> shaderToTex2DProps = new Dictionary<string, string[]>(600);

	private const string kResourcesAssetName = "ShaderPropertiesMapSO";

	private static ShaderPropertiesMapSO _instance;

	[CompilerGenerated]
	private static bool <hasInstance>k__BackingField;
}
