using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200006A RID: 106
public class ShaderPropertiesMapSO : ScriptableObject
{
	// Token: 0x17000016 RID: 22
	// (get) Token: 0x060001FA RID: 506 RVA: 0x0000DF20 File Offset: 0x0000C120
	// (set) Token: 0x060001FB RID: 507 RVA: 0x0000DF27 File Offset: 0x0000C127
	public static bool hasInstance { get; private set; }

	// Token: 0x060001FC RID: 508 RVA: 0x0000DF30 File Offset: 0x0000C130
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

	// Token: 0x040002C1 RID: 705
	public Dictionary<string, string[]> shaderToTex2DProps = new Dictionary<string, string[]>(600);

	// Token: 0x040002C2 RID: 706
	private const string kResourcesAssetName = "ShaderPropertiesMapSO";

	// Token: 0x040002C3 RID: 707
	private static ShaderPropertiesMapSO _instance;
}
