using System;
using GorillaLocomotion.Swimming;
using UnityEngine;

// Token: 0x02000077 RID: 119
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class WaterRippleEffect : MonoBehaviour
{
	// Token: 0x06000256 RID: 598 RVA: 0x0000F935 File Offset: 0x0000DB35
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.renderer = base.GetComponent<SpriteRenderer>();
		this.ripplePlaybackSpeedHash = Animator.StringToHash(this.ripplePlaybackSpeedName);
	}

	// Token: 0x06000257 RID: 599 RVA: 0x0000F960 File Offset: 0x0000DB60
	public void Destroy()
	{
		this.waterVolume = null;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x06000258 RID: 600 RVA: 0x0000F97C File Offset: 0x0000DB7C
	public void PlayEffect(WaterVolume volume = null)
	{
		this.waterVolume = volume;
		this.rippleStartTime = Time.time;
		this.animator.SetFloat(this.ripplePlaybackSpeedHash, this.ripplePlaybackSpeed);
		if (this.waterVolume != null && this.waterVolume.Parameters != null)
		{
			this.renderer.color = this.waterVolume.Parameters.rippleSpriteColor;
		}
		Color color = this.renderer.color;
		color.a = 1f;
		this.renderer.color = color;
	}

	// Token: 0x06000259 RID: 601 RVA: 0x0000FA14 File Offset: 0x0000DC14
	private void Update()
	{
		if (this.waterVolume != null && !this.waterVolume.isStationary && this.waterVolume.surfacePlane != null)
		{
			Vector3 b = Vector3.Dot(base.transform.position - this.waterVolume.surfacePlane.position, this.waterVolume.surfacePlane.up) * this.waterVolume.surfacePlane.up;
			base.transform.position = base.transform.position - b;
		}
		float num = Mathf.Clamp01((Time.time - this.rippleStartTime - this.fadeOutDelay) / this.fadeOutTime);
		Color color = this.renderer.color;
		color.a = 1f - num;
		this.renderer.color = color;
		if (num >= 1f - Mathf.Epsilon)
		{
			this.Destroy();
			return;
		}
	}

	// Token: 0x04000310 RID: 784
	[SerializeField]
	private float ripplePlaybackSpeed = 1f;

	// Token: 0x04000311 RID: 785
	[SerializeField]
	private float fadeOutDelay = 0.5f;

	// Token: 0x04000312 RID: 786
	[SerializeField]
	private float fadeOutTime = 1f;

	// Token: 0x04000313 RID: 787
	private string ripplePlaybackSpeedName = "RipplePlaybackSpeed";

	// Token: 0x04000314 RID: 788
	private int ripplePlaybackSpeedHash;

	// Token: 0x04000315 RID: 789
	private float rippleStartTime = -1f;

	// Token: 0x04000316 RID: 790
	private Animator animator;

	// Token: 0x04000317 RID: 791
	private SpriteRenderer renderer;

	// Token: 0x04000318 RID: 792
	private WaterVolume waterVolume;
}
