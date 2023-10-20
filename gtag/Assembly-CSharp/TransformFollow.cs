using System;
using UnityEngine;

// Token: 0x020001A7 RID: 423
public class TransformFollow : MonoBehaviour
{
	// Token: 0x06000ADF RID: 2783 RVA: 0x0004345E File Offset: 0x0004165E
	private void Awake()
	{
		this.prevPos = base.transform.position;
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x00043474 File Offset: 0x00041674
	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		base.transform.rotation = this.transformToFollow.rotation;
		base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
	}

	// Token: 0x04000DAF RID: 3503
	public Transform transformToFollow;

	// Token: 0x04000DB0 RID: 3504
	public Vector3 offset;

	// Token: 0x04000DB1 RID: 3505
	public Vector3 prevPos;
}
