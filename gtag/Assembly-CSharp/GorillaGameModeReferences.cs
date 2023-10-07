using System;
using GorillaLocomotion.Swimming;
using GorillaTag;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class GorillaGameModeReferences : MonoBehaviour
{
	// Token: 0x1700006A RID: 106
	// (get) Token: 0x0600095C RID: 2396 RVA: 0x000389B7 File Offset: 0x00036BB7
	public static GorillaGameModeReferences Instance
	{
		get
		{
			return GorillaGameModeReferences.instance;
		}
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x000389BE File Offset: 0x00036BBE
	protected void Awake()
	{
		if (GorillaGameModeReferences.instance != null && GorillaGameModeReferences.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		GorillaGameModeReferences.instance = this;
	}

	// Token: 0x04000B6D RID: 2925
	private static GorillaGameModeReferences instance;

	// Token: 0x04000B6E RID: 2926
	[Header("InfectionLavaController")]
	public Transform lavaMeshTransform;

	// Token: 0x04000B6F RID: 2927
	public Transform lavaSurfacePlaneTransform;

	// Token: 0x04000B70 RID: 2928
	public WaterVolume lavaVolume;

	// Token: 0x04000B71 RID: 2929
	public MeshRenderer lavaActivationRenderer;

	// Token: 0x04000B72 RID: 2930
	public Transform lavaActivationStartPos;

	// Token: 0x04000B73 RID: 2931
	public Transform lavaActivationEndPos;

	// Token: 0x04000B74 RID: 2932
	public SlingshotProjectileHitNotifier lavaActivationProjectileHitNotifier;

	// Token: 0x04000B75 RID: 2933
	public VolcanoEffects[] volcanoEffects;
}
