using System;
using UnityEngine;

// Token: 0x0200023F RID: 575
public class MaterialMapping : ScriptableObject
{
	// Token: 0x06000E22 RID: 3618 RVA: 0x00052025 File Offset: 0x00050225
	public void CleanUpData()
	{
	}

	// Token: 0x04001151 RID: 4433
	private static string path = "Assets/UberShaderConversion/MaterialMap.asset";

	// Token: 0x04001152 RID: 4434
	public static string materialDirectory = "Assets/UberShaderConversion/Materials/";

	// Token: 0x04001153 RID: 4435
	private static MaterialMapping instance;

	// Token: 0x04001154 RID: 4436
	public ShaderGroup[] map;

	// Token: 0x04001155 RID: 4437
	public Material mirrorMat;

	// Token: 0x04001156 RID: 4438
	public RenderTexture mirrorTexture;
}
