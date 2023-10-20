using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000151 RID: 337
[Serializable]
internal class HandTransformFollowOffest
{
	// Token: 0x06000858 RID: 2136 RVA: 0x00033CB4 File Offset: 0x00031EB4
	internal void UpdatePositionRotation()
	{
		if (this.followTransform == null || this.targetTransforms == null)
		{
			return;
		}
		this.position = this.followTransform.position + this.followTransform.rotation * this.positionOffset * Player.Instance.scale;
		this.rotation = this.followTransform.rotation * this.rotationOffset;
		foreach (Transform transform in this.targetTransforms)
		{
			transform.position = this.position;
			transform.rotation = this.rotation;
		}
	}

	// Token: 0x04000A64 RID: 2660
	internal Transform followTransform;

	// Token: 0x04000A65 RID: 2661
	[SerializeField]
	private Transform[] targetTransforms;

	// Token: 0x04000A66 RID: 2662
	[SerializeField]
	internal Vector3 positionOffset;

	// Token: 0x04000A67 RID: 2663
	[SerializeField]
	internal Quaternion rotationOffset;

	// Token: 0x04000A68 RID: 2664
	private Vector3 position;

	// Token: 0x04000A69 RID: 2665
	private Quaternion rotation;
}
