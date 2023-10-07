using System;
using UnityEngine;

// Token: 0x02000032 RID: 50
public class CloudUmbrellaCloud : MonoBehaviour
{
	// Token: 0x06000125 RID: 293 RVA: 0x0000A545 File Offset: 0x00008745
	protected void Awake()
	{
		this.umbrellaXform = this.umbrella.transform;
		this.cloudScaleXform = this.cloudRenderer.transform;
	}

	// Token: 0x06000126 RID: 294 RVA: 0x0000A56C File Offset: 0x0000876C
	protected void LateUpdate()
	{
		float time = Vector3.Dot(this.umbrellaXform.up, Vector3.up);
		float num = Mathf.Clamp01(this.scaleCurve.Evaluate(time));
		this.rendererOn = ((num > 0.09f && num < 0.1f) ? this.rendererOn : (num > 0.1f));
		this.cloudRenderer.enabled = this.rendererOn;
		this.cloudScaleXform.localScale = new Vector3(num, num, num);
		this.cloudRotateXform.up = Vector3.up;
	}

	// Token: 0x04000189 RID: 393
	public UmbrellaItem umbrella;

	// Token: 0x0400018A RID: 394
	public Transform cloudRotateXform;

	// Token: 0x0400018B RID: 395
	public Renderer cloudRenderer;

	// Token: 0x0400018C RID: 396
	public AnimationCurve scaleCurve;

	// Token: 0x0400018D RID: 397
	private const float kHideAtScale = 0.1f;

	// Token: 0x0400018E RID: 398
	private const float kHideAtScaleTolerance = 0.01f;

	// Token: 0x0400018F RID: 399
	private bool rendererOn;

	// Token: 0x04000190 RID: 400
	private Transform umbrellaXform;

	// Token: 0x04000191 RID: 401
	private Transform cloudScaleXform;
}
