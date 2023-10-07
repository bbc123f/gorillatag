using System;
using UnityEngine;

// Token: 0x020001A6 RID: 422
public class TransformFollow : MonoBehaviour
{
	// Token: 0x06000ADA RID: 2778 RVA: 0x00043326 File Offset: 0x00041526
	private void Awake()
	{
		this.prevPos = base.transform.position;
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x0004333C File Offset: 0x0004153C
	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		base.transform.rotation = this.transformToFollow.rotation;
		base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
	}

	// Token: 0x04000DAB RID: 3499
	public Transform transformToFollow;

	// Token: 0x04000DAC RID: 3500
	public Vector3 offset;

	// Token: 0x04000DAD RID: 3501
	public Vector3 prevPos;
}
