using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag;
using UnityEngine;

// Token: 0x0200011B RID: 283
[Serializable]
public class SlotTransformOverride
{
	// Token: 0x06000735 RID: 1845 RVA: 0x0002DA50 File Offset: 0x0002BC50
	public void Initialize(Transform anchor)
	{
		this.overrideTransformMatrix = this.overrideTransform.LocalMatrixRelativeToParentWithScale();
		if (!this.useAdvancedGrab)
		{
			return;
		}
		this.AdvOriginLocalToParentAnchorLocal = anchor.worldToLocalMatrix * this.advancedGrabPointOrigin.localToWorldMatrix;
		this.AdvAnchorLocalToAdvOriginLocal = this.advancedGrabPointOrigin.worldToLocalMatrix * this.advancedGrabPointAnchor.localToWorldMatrix;
		foreach (SubGrabPoint subGrabPoint in this.multiPoints)
		{
			if (subGrabPoint == null)
			{
				break;
			}
			subGrabPoint.InitializePoints(anchor, this.advancedGrabPointAnchor, this.advancedGrabPointOrigin);
		}
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x0002DB0C File Offset: 0x0002BD0C
	public void AddLineButton()
	{
		this.multiPoints.Add(new SubLineGrabPoint());
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x0002DB20 File Offset: 0x0002BD20
	public void AddSubGrabPoint(TransferrableObjectGripPosition togp)
	{
		SubGrabPoint item = togp.CreateSubGrabPoint(this);
		this.multiPoints.Add(item);
	}

	// Token: 0x040008D0 RID: 2256
	public Transform overrideTransform;

	// Token: 0x040008D1 RID: 2257
	public TransferrableObject.PositionState positionState;

	// Token: 0x040008D2 RID: 2258
	public bool useAdvancedGrab;

	// Token: 0x040008D3 RID: 2259
	[DebugReadout]
	public Matrix4x4 overrideTransformMatrix = Matrix4x4.identity;

	// Token: 0x040008D4 RID: 2260
	public Transform advancedGrabPointAnchor;

	// Token: 0x040008D5 RID: 2261
	public Transform advancedGrabPointOrigin;

	// Token: 0x040008D6 RID: 2262
	[SerializeReference]
	public List<SubGrabPoint> multiPoints = new List<SubGrabPoint>();

	// Token: 0x040008D7 RID: 2263
	public Matrix4x4 AdvOriginLocalToParentAnchorLocal;

	// Token: 0x040008D8 RID: 2264
	public Matrix4x4 AdvAnchorLocalToAdvOriginLocal;
}
