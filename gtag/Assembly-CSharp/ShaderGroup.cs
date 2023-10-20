using System;
using UnityEngine;

// Token: 0x02000240 RID: 576
[Serializable]
public struct ShaderGroup
{
	// Token: 0x06000E25 RID: 3621 RVA: 0x00052045 File Offset: 0x00050245
	public ShaderGroup(Material material, Shader original, Shader gameplay, Shader baking)
	{
		this.material = material;
		this.originalShader = original;
		this.gameplayShader = gameplay;
		this.bakingShader = baking;
	}

	// Token: 0x04001157 RID: 4439
	public Material material;

	// Token: 0x04001158 RID: 4440
	public Shader originalShader;

	// Token: 0x04001159 RID: 4441
	public Shader gameplayShader;

	// Token: 0x0400115A RID: 4442
	public Shader bakingShader;
}
