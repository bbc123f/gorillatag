using System;
using UnityEngine;

// Token: 0x02000148 RID: 328
public class GorillaWalkingGrab : MonoBehaviour
{
	// Token: 0x0600083B RID: 2107 RVA: 0x000331B7 File Offset: 0x000313B7
	private void Start()
	{
		this.thisRigidbody = base.gameObject.GetComponent<Rigidbody>();
		this.positionHistory = new Vector3[this.historySteps];
		this.historyIndex = 0;
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x000331E4 File Offset: 0x000313E4
	private void FixedUpdate()
	{
		this.historyIndex++;
		if (this.historyIndex >= this.historySteps)
		{
			this.historyIndex = 0;
		}
		this.positionHistory[this.historyIndex] = this.handToStickTo.transform.position;
		this.thisRigidbody.MovePosition(this.handToStickTo.transform.position);
		base.transform.rotation = this.handToStickTo.transform.rotation;
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x0003326B File Offset: 0x0003146B
	private bool MakeJump()
	{
		return false;
	}

	// Token: 0x0600083E RID: 2110 RVA: 0x00033270 File Offset: 0x00031470
	private void OnCollisionStay(Collision collision)
	{
		if (!this.MakeJump())
		{
			Vector3 b = Vector3.ProjectOnPlane(this.positionHistory[(this.historyIndex != 0) ? (this.historyIndex - 1) : (this.historySteps - 1)] - this.handToStickTo.transform.position, collision.GetContact(0).normal);
			Vector3 b2 = this.thisRigidbody.transform.position - this.handToStickTo.transform.position;
			this.playspaceRigidbody.MovePosition(this.playspaceRigidbody.transform.position + b - b2);
		}
	}

	// Token: 0x04000A1F RID: 2591
	public GameObject handToStickTo;

	// Token: 0x04000A20 RID: 2592
	public float ratioToUse;

	// Token: 0x04000A21 RID: 2593
	public float forceMultiplier;

	// Token: 0x04000A22 RID: 2594
	public int historySteps;

	// Token: 0x04000A23 RID: 2595
	public Rigidbody playspaceRigidbody;

	// Token: 0x04000A24 RID: 2596
	private Rigidbody thisRigidbody;

	// Token: 0x04000A25 RID: 2597
	private Vector3 lastPosition;

	// Token: 0x04000A26 RID: 2598
	private Vector3 maybeLastPositionIDK;

	// Token: 0x04000A27 RID: 2599
	private Vector3[] positionHistory;

	// Token: 0x04000A28 RID: 2600
	private int historyIndex;
}
