using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag;
using UnityEngine;

[Serializable]
public class SlotTransformOverride
{
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

	public void AddLineButton()
	{
		this.multiPoints.Add(new SubLineGrabPoint());
	}

	public void AddSubGrabPoint(TransferrableObjectGripPosition togp)
	{
		SubGrabPoint item = togp.CreateSubGrabPoint(this);
		this.multiPoints.Add(item);
	}

	public Transform overrideTransform;

	public TransferrableObject.PositionState positionState;

	public bool useAdvancedGrab;

	[DebugReadout]
	public Matrix4x4 overrideTransformMatrix = Matrix4x4.identity;

	public Transform advancedGrabPointAnchor;

	public Transform advancedGrabPointOrigin;

	[SerializeReference]
	public List<SubGrabPoint> multiPoints = new List<SubGrabPoint>();

	public Matrix4x4 AdvOriginLocalToParentAnchorLocal;

	public Matrix4x4 AdvAnchorLocalToAdvOriginLocal;
}
