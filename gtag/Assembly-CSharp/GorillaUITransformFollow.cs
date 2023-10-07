using System;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class GorillaUITransformFollow : MonoBehaviour
{
	// Token: 0x06000A25 RID: 2597 RVA: 0x0003ED6E File Offset: 0x0003CF6E
	private void Start()
	{
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x0003ED70 File Offset: 0x0003CF70
	private void LateUpdate()
	{
		if (this.doesMove)
		{
			base.transform.rotation = this.transformToFollow.rotation;
			base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
		}
	}

	// Token: 0x04000C8F RID: 3215
	public Transform transformToFollow;

	// Token: 0x04000C90 RID: 3216
	public Vector3 offset;

	// Token: 0x04000C91 RID: 3217
	public bool doesMove;
}
