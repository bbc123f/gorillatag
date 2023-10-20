using System;
using UnityEngine;

// Token: 0x0200003D RID: 61
public class VacuumHoldable : TransferrableObject
{
	// Token: 0x06000154 RID: 340 RVA: 0x0000B56B File Offset: 0x0000976B
	protected override void Awake()
	{
		base.Awake();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06000155 RID: 341 RVA: 0x0000B57A File Offset: 0x0000977A
	public override void OnEnable()
	{
		base.OnEnable();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.hasAudioSource = (this.audioSource != null && this.audioSource.clip != null);
	}

	// Token: 0x06000156 RID: 342 RVA: 0x0000B5B4 File Offset: 0x000097B4
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

	// Token: 0x06000157 RID: 343 RVA: 0x0000B608 File Offset: 0x00009808
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

	// Token: 0x06000158 RID: 344 RVA: 0x0000B654 File Offset: 0x00009854
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06000159 RID: 345 RVA: 0x0000B664 File Offset: 0x00009864
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

	// Token: 0x0600015A RID: 346 RVA: 0x0000B758 File Offset: 0x00009958
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

	// Token: 0x0600015B RID: 347 RVA: 0x0000B7A4 File Offset: 0x000099A4
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

	// Token: 0x02000394 RID: 916
	private enum VacuumState
	{
		// Token: 0x04001B25 RID: 6949
		None = 1,
		// Token: 0x04001B26 RID: 6950
		Active
	}
}
