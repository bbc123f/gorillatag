using System;
using UnityEngine;

// Token: 0x0200023F RID: 575
[Serializable]
public struct ShaderGroup
{
	// Token: 0x06000E1E RID: 3614 RVA: 0x00051C69 File Offset: 0x0004FE69
	public ShaderGroup(Material material, Shader original, Shader gameplay, Shader baking)
	{
		this.material = material;
		this.originalShader = original;
		this.gameplayShader = gameplay;
		this.bakingShader = baking;
	}

	// Token: 0x04001151 RID: 4433
	public Material material;

	// Token: 0x04001152 RID: 4434
	public Shader originalShader;

	// Token: 0x04001153 RID: 4435
	public Shader gameplayShader;

	// Token: 0x04001154 RID: 4436
	public Shader bakingShader;
}
