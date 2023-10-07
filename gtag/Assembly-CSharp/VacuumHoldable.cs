using System;
using UnityEngine;

// Token: 0x0200003D RID: 61
public class VacuumHoldable : TransferrableObject
{
	// Token: 0x06000154 RID: 340 RVA: 0x0000B523 File Offset: 0x00009723
	protected override void Awake()
	{
		base.Awake();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06000155 RID: 341 RVA: 0x0000B532 File Offset: 0x00009732
	public override void OnEnable()
	{
		base.OnEnable();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.hasAudioSource = (this.audioSource != null && this.audioSource.clip != null);
	}

	// Token: 0x06000156 RID: 342 RVA: 0x0000B56C File Offset: 0x0000976C
	public override void OnDisable()
	{
		base.OnDisable();
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.particleFX.isPlaying)
		{
			this.particleFX.Stop();
		}
		if (this.hasAudioSource && this.audioSource.isPlaying)
		{
			this.audioSource.Stop();
		}
	}

	// Token: 0x06000157 RID: 343 RVA: 0x0000B5C0 File Offset: 0x000097C0
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.particleFX.isPlaying)
		{
			this.particleFX.Stop();
		}
		if (this.hasAudioSource && this.audioSource.isPlaying)
		{
			this.audioSource.Stop();
		}
	}

	// Token: 0x06000158 RID: 344 RVA: 0x0000B60C File Offset: 0x0000980C
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06000159 RID: 345 RVA: 0x0000B61C File Offset: 0x0000981C
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (!this.IsMyItem() && this.myOnlineRig != null && this.myOnlineRig.muted)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			if (this.particleFX.isPlaying)
			{
				this.particleFX.Stop();
			}
			if (this.hasAudioSource && this.audioSource.isPlaying)
			{
				this.audioSource.Stop();
				return;
			}
		}
		else
		{
			if (!this.particleFX.isEmitting)
			{
				this.particleFX.Play();
			}
			if (this.hasAudioSource && !this.audioSource.isPlaying)
			{
				this.audioSource.Play();
			}
			if (this.IsMyItem() && Time.time > this.activationStartTime + this.activationVibrationStartDuration)
			{
				GorillaTagger.Instance.StartVibration(this.currentState == TransferrableObject.PositionState.InLeftHand, this.activationVibrationLoopStrength, Time.deltaTime);
			}
		}
	}

	// Token: 0x0600015A RID: 346 RVA: 0x0000B710 File Offset: 0x00009910
	public override void OnActivate()
	{
		base.OnActivate();
		this.itemState = TransferrableObject.ItemStates.State1;
		if (this.IsMyItem())
		{
			this.activationStartTime = Time.time;
			GorillaTagger.Instance.StartVibration(this.currentState == TransferrableObject.PositionState.InLeftHand, this.activationVibrationStartStrength, this.activationVibrationStartDuration);
		}
	}

	// Token: 0x0600015B RID: 347 RVA: 0x0000B75C File Offset: 0x0000995C
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x040001E5 RID: 485
	[Tooltip("Emission rate will be increase when the trigger button is pressed.")]
	public ParticleSystem particleFX;

	// Token: 0x040001E6 RID: 486
	[Tooltip("Sound will loop and fade in/out volume when trigger pressed.")]
	public AudioSource audioSource;

	// Token: 0x040001E7 RID: 487
	private float activationVibrationStartStrength = 0.8f;

	// Token: 0x040001E8 RID: 488
	private float activationVibrationStartDuration = 0.05f;

	// Token: 0x040001E9 RID: 489
	private float activationVibrationLoopStrength = 0.005f;

	// Token: 0x040001EA RID: 490
	private float activationStartTime;

	// Token: 0x040001EB RID: 491
	private bool hasAudioSource;

	// Token: 0x02000392 RID: 914
	private enum VacuumState
	{
		// Token: 0x04001B18 RID: 6936
		None = 1,
		// Token: 0x04001B19 RID: 6937
		Active
	}
}
