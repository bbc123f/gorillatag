using System;
using UnityEngine;

// Token: 0x0200023E RID: 574
public class MaterialMapping : ScriptableObject
{
	// Token: 0x06000E1B RID: 3611 RVA: 0x00051C49 File Offset: 0x0004FE49
	public void CleanUpData()
	{
	}

	// Token: 0x0400114B RID: 4427
	private static string path = "Assets/UberShaderConversion/MaterialMap.asset";

	// Token: 0x0400114C RID: 4428
	public static string materialDirectory = "Assets/UberShaderConversion/Materials/";

	// Token: 0x0400114D RID: 4429
	private static MaterialMapping instance;

	// Token: 0x0400114E RID: 4430
	public ShaderGroup[] map;

	// Token: 0x0400114F RID: 4431
	public Material mirrorMat;

	// Token: 0x04001150 RID: 4432
	public RenderTexture mirrorTexture;
}
