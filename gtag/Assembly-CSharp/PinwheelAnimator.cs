using System;
using UnityEngine;

// Token: 0x02000034 RID: 52
public class PinwheelAnimator : MonoBehaviour
{
	// Token: 0x0600012F RID: 303 RVA: 0x0000A9AA File Offset: 0x00008BAA
	protected void OnEnable()
	{
		this.oldPos = this.spinnerTransform.position;
		this.spinSpeed = 0f;
	}

	// Token: 0x06000130 RID: 304 RVA: 0x0000A9C8 File Offset: 0x00008BC8
	protected void LateUpdate()
	{
		Vector3 position = this.spinnerTransform.position;
		Vector3 forward = base.transform.forward;
		Vector3 vector = position - this.oldPos;
		float b = Mathf.Clamp(vector.magnitude / Time.deltaTime * Vector3.Dot(vector.normalized, forward) * this.spinSpeedMultiplier, -this.maxSpinSpeed, this.maxSpinSpeed);
		this.spinSpeed = Mathf.Lerp(this.spinSpeed, b, Time.deltaTime * this.damping);
		this.spinnerTransform.Rotate(Vector3.forward, this.spinSpeed * 360f * Time.deltaTime);
		this.oldPos = position;
	}

	// Token: 0x0400019F RID: 415
	public Transform spinnerTransform;

	// Token: 0x040001A0 RID: 416
	[Tooltip("In revolutions per second.")]
	public float maxSpinSpeed = 4f;

	// Token: 0x040001A1 RID: 417
	public float spinSpeedMultiplier = 5f;

	// Token: 0x040001A2 RID: 418
	public float damping = 0.5f;

	// Token: 0x040001A3 RID: 419
	private Vector3 oldPos;

	// Token: 0x040001A4 RID: 420
	private float spinSpeed;
}
