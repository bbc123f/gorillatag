using System;
using UnityEngine;

// Token: 0x02000140 RID: 320
public class GorillaPlaySpaceForces : MonoBehaviour
{
	// Token: 0x0600082B RID: 2091 RVA: 0x00033038 File Offset: 0x00031238
	private void Start()
	{
		this.playspaceRigidbody = base.GetComponent<Rigidbody>();
		this.leftHandRigidbody = this.leftHand.GetComponent<Rigidbody>();
		this.leftHandCollider = this.leftHand.GetComponent<Collider>();
		this.rightHandRigidbody = this.rightHand.GetComponent<Rigidbody>();
		this.rightHandCollider = this.rightHand.GetComponent<Collider>();
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x00033095 File Offset: 0x00031295
	private void FixedUpdate()
	{
		if (Time.time >= 0.1f)
		{
			this.bodyCollider.transform.position = this.headsetTransform.position + this.bodyColliderOffset;
		}
	}

	// Token: 0x04000A05 RID: 2565
	public GameObject rightHand;

	// Token: 0x04000A06 RID: 2566
	public GameObject leftHand;

	// Token: 0x04000A07 RID: 2567
	public Collider bodyCollider;

	// Token: 0x04000A08 RID: 2568
	private Collider leftHandCollider;

	// Token: 0x04000A09 RID: 2569
	private Collider rightHandCollider;

	// Token: 0x04000A0A RID: 2570
	public Transform rightHandTransform;

	// Token: 0x04000A0B RID: 2571
	public Transform leftHandTransform;

	// Token: 0x04000A0C RID: 2572
	private Rigidbody leftHandRigidbody;

	// Token: 0x04000A0D RID: 2573
	private Rigidbody rightHandRigidbody;

	// Token: 0x04000A0E RID: 2574
	public Vector3 bodyColliderOffset;

	// Token: 0x04000A0F RID: 2575
	public float forceConstant;

	// Token: 0x04000A10 RID: 2576
	private Vector3 lastLeftHandPosition;

	// Token: 0x04000A11 RID: 2577
	private Vector3 lastRightHandPosition;

	// Token: 0x04000A12 RID: 2578
	private Rigidbody playspaceRigidbody;

	// Token: 0x04000A13 RID: 2579
	public Transform headsetTransform;
}
