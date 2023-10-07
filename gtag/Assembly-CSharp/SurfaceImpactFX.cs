using System;
using UnityEngine;

// Token: 0x02000235 RID: 565
public class SurfaceImpactFX : MonoBehaviour
{
	// Token: 0x06000DFC RID: 3580 RVA: 0x00051128 File Offset: 0x0004F328
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

	// Token: 0x06000DFD RID: 3581 RVA: 0x00051181 File Offset: 0x0004F381
	public void SetScale(float scale)
	{
		this.fxMainModule.gravityModifierMultiplier = this.startingGravityModifier * scale;
		base.transform.localScale = this.startingScale * scale;
	}

	// Token: 0x040010EF RID: 4335
	public ParticleSystem particleFX;

	// Token: 0x040010F0 RID: 4336
	public float startingGravityModifier;

	// Token: 0x040010F1 RID: 4337
	public Vector3 startingScale = Vector3.one;

	// Token: 0x040010F2 RID: 4338
	private ParticleSystem.MainModule fxMainModule;
}
