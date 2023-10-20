using System;
using UnityEngine;

// Token: 0x02000236 RID: 566
public class SurfaceImpactFX : MonoBehaviour
{
	// Token: 0x06000E03 RID: 3587 RVA: 0x00051504 File Offset: 0x0004F704
	public void Awake()
	{
		if (this.particleFX == null)
		{
			this.particleFX = base.GetComponent<ParticleSystem>();
		}
		if (this.particleFX == null)
		{
			Debug.LogError("SurfaceImpactFX: No ParticleSystem found! Disabling component.", this);
			base.enabled = false;
			return;
		}
		this.fxMainModule = this.particleFX.main;
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x0005155D File Offset: 0x0004F75D
	public void SetScale(float scale)
	{
		this.fxMainModule.gravityModifierMultiplier = this.startingGravityModifier * scale;
		base.transform.localScale = this.startingScale * scale;
	}

	// Token: 0x040010F5 RID: 4341
	public ParticleSystem particleFX;

	// Token: 0x040010F6 RID: 4342
	public float startingGravityModifier;

	// Token: 0x040010F7 RID: 4343
	public Vector3 startingScale = Vector3.one;

	// Token: 0x040010F8 RID: 4344
	private ParticleSystem.MainModule fxMainModule;
}
