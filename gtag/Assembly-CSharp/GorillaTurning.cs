using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000187 RID: 391
public class GorillaTurning : GorillaTriggerBox
{
	// Token: 0x06000A1C RID: 2588 RVA: 0x0003ED0C File Offset: 0x0003CF0C
	private void Awake()
	{
	}

	// Token: 0x04000C80 RID: 3200
	public Material redMaterial;

	// Token: 0x04000C81 RID: 3201
	public Material blueMaterial;

	// Token: 0x04000C82 RID: 3202
	public Material greenMaterial;

	// Token: 0x04000C83 RID: 3203
	public Material transparentBlueMaterial;

	// Token: 0x04000C84 RID: 3204
	public Material transparentRedMaterial;

	// Token: 0x04000C85 RID: 3205
	public Material transparentGreenMaterial;

	// Token: 0x04000C86 RID: 3206
	public MeshRenderer smoothTurnBox;

	// Token: 0x04000C87 RID: 3207
	public MeshRenderer snapTurnBox;

	// Token: 0x04000C88 RID: 3208
	public MeshRenderer noTurnBox;

	// Token: 0x04000C89 RID: 3209
	public GorillaSnapTurn snapTurn;

	// Token: 0x04000C8A RID: 3210
	public string currentChoice;

	// Token: 0x04000C8B RID: 3211
	public float currentSpeed;
}
