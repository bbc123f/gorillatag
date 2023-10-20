using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000188 RID: 392
public class GorillaTurning : GorillaTriggerBox
{
	// Token: 0x06000A21 RID: 2593 RVA: 0x0003EE3C File Offset: 0x0003D03C
	private void Awake()
	{
	}

	// Token: 0x04000C84 RID: 3204
	public Material redMaterial;

	// Token: 0x04000C85 RID: 3205
	public Material blueMaterial;

	// Token: 0x04000C86 RID: 3206
	public Material greenMaterial;

	// Token: 0x04000C87 RID: 3207
	public Material transparentBlueMaterial;

	// Token: 0x04000C88 RID: 3208
	public Material transparentRedMaterial;

	// Token: 0x04000C89 RID: 3209
	public Material transparentGreenMaterial;

	// Token: 0x04000C8A RID: 3210
	public MeshRenderer smoothTurnBox;

	// Token: 0x04000C8B RID: 3211
	public MeshRenderer snapTurnBox;

	// Token: 0x04000C8C RID: 3212
	public MeshRenderer noTurnBox;

	// Token: 0x04000C8D RID: 3213
	public GorillaSnapTurn snapTurn;

	// Token: 0x04000C8E RID: 3214
	public string currentChoice;

	// Token: 0x04000C8F RID: 3215
	public float currentSpeed;
}
