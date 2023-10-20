using System;
using GorillaLocomotion.Swimming;
using GorillaTag;
using UnityEngine;

// Token: 0x02000174 RID: 372
public class GorillaGameModeReferences : MonoBehaviour
{
	// Token: 0x1700006C RID: 108
	// (get) Token: 0x06000960 RID: 2400 RVA: 0x0003896F File Offset: 0x00036B6F
	public static GorillaGameModeReferences Instance
	{
		get
		{
			return GorillaGameModeReferences.instance;
		}
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x00038976 File Offset: 0x00036B76
	protected void Awake()
	{
		if (GorillaGameModeReferences.instance != null && GorillaGameModeReferences.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		GorillaGameModeReferences.instance = this;
	}

	// Token: 0x04000B71 RID: 2929
	private static GorillaGameModeReferences instance;

	// Token: 0x04000B72 RID: 2930
	[Header("InfectionLavaController")]
	public Transform lavaMeshTransform;

	// Token: 0x04000B73 RID: 2931
	public Transform lavaSurfacePlaneTransform;

	// Token: 0x04000B74 RID: 2932
	public WaterVolume lavaVolume;

	// Token: 0x04000B75 RID: 2933
	public MeshRenderer lavaActivationRenderer;

	// Token: 0x04000B76 RID: 2934
	public Transform lavaActivationStartPos;

	// Token: 0x04000B77 RID: 2935
	public Transform lavaActivationEndPos;

	// Token: 0x04000B78 RID: 2936
	public SlingshotProjectileHitNotifier lavaActivationProjectileHitNotifier;

	// Token: 0x04000B79 RID: 2937
	public VolcanoEffects[] volcanoEffects;
}
