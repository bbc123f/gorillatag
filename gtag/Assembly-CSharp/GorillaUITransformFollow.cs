using System;
using UnityEngine;

// Token: 0x0200018C RID: 396
public class GorillaUITransformFollow : MonoBehaviour
{
	// Token: 0x06000A2A RID: 2602 RVA: 0x0003EE9E File Offset: 0x0003D09E
	private void Start()
	{
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x0003EEA0 File Offset: 0x0003D0A0
	private void LateUpdate()
	{
		if (this.doesMove)
		{
			base.transform.rotation = this.transformToFollow.rotation;
			base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
		}
	}

	// Token: 0x04000C93 RID: 3219
	public Transform transformToFollow;

	// Token: 0x04000C94 RID: 3220
	public Vector3 offset;

	// Token: 0x04000C95 RID: 3221
	public bool doesMove;
}
