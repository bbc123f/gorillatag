using System;
using UnityEngine;

// Token: 0x0200003B RID: 59
public class FeatherDusterHoldable : MonoBehaviour
{
	// Token: 0x0600014D RID: 333 RVA: 0x0000B20F File Offset: 0x0000940F
	protected void Awake()
	{
		this.timeSinceLastSound = this.soundCooldown;
		this.emissionModule = this.particleFx.emission;
		this.initialRateOverTime = this.emissionModule.rateOverTimeMultiplier;
	}

	// Token: 0x0600014E RID: 334 RVA: 0x0000B23F File Offset: 0x0000943F
	protected void OnEnable()
	{
		this.lastWorldPos = base.transform.position;
		this.emissionModule.rateOverTimeMultiplier = 0f;
	}

	// Token: 0x0600014F RID: 335 RVA: 0x0000B264 File Offset: 0x00009464
	protected void Update()
	{
		this.timeSinceLastSound += Time.deltaTime;
		Transform transform = base.transform;
		Vector3 position = transform.position;
		float num = (position - this.lastWorldPos).magnitude / Time.deltaTime;
		this.emissionModule.rateOverTimeMultiplier = 0f;
		if (num >= this.collideMinSpeed && Physics.OverlapSphereNonAlloc(position, this.overlapSphereRadius * transform.localScale.x, this.colliderResult, this.collisionLayer) > 0)
		{
			this.emissionModule.rateOverTimeMultiplier = this.initialRateOverTime;
			if (this.timeSinceLastSound >= this.soundCooldown)
			{
				this.soundBankPlayer.Play(null, null);
				this.timeSinceLastSound = 0f;
			}
		}
		this.lastWorldPos = position;
	}

	// Token: 0x040001D1 RID: 465
	public LayerMask collisionLayer;

	// Token: 0x040001D2 RID: 466
	public float overlapSphereRadius = 0.08f;

	// Token: 0x040001D3 RID: 467
	[Tooltip("Collision is not tested until this speed requirement is met.")]
	private float collideMinSpeed = 1f;

	// Token: 0x040001D4 RID: 468
	public ParticleSystem particleFx;

	// Token: 0x040001D5 RID: 469
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x040001D6 RID: 470
	private float soundCooldown = 0.8f;

	// Token: 0x040001D7 RID: 471
	private ParticleSystem.EmissionModule emissionModule;

	// Token: 0x040001D8 RID: 472
	private float initialRateOverTime;

	// Token: 0x040001D9 RID: 473
	private float timeSinceLastSound;

	// Token: 0x040001DA RID: 474
	private Vector3 lastWorldPos;

	// Token: 0x040001DB RID: 475
	private Collider[] colliderResult = new Collider[1];
}
