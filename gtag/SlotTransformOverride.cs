using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag;
using UnityEngine;

[Serializable]
public class SlotTransformOverride
{
	public Transform overrideTransform;

	public TransferrableObject.PositionState positionState;

	public bool useAdvancedGrab;

	[DebugReadout]
	public Matrix4x4 overrideTransformMatrix = Matrix4x4.identity;

	public Transform advancedGrabPointAnchor;

	public Transform advancedGrabPointOrigin;

	[SerializeReference]
	public List<SubGrabPoint> multiPoints = new List<SubGrabPoint>();

	public Matrix4x4 grabPointRelativeToObjectAnchor;

	public Matrix4x4 GrabPointAnchorRelativeToGripOrigin;

	public void Initialize(Transform anchor)
	{
		overrideTransformMatrix = overrideTransform.LocalMatrixRelativeToParentWithScale();
		if (!useAdvancedGrab)
		{
			return;
		}
		grabPointRelativeToObjectAnchor = anchor.worldToLocalMatrix * advancedGrabPointOrigin.localToWorldMatrix;
		GrabPointAnchorRelativeToGripOrigin = advancedGrabPointOrigin.worldToLocalMatrix * advancedGrabPointAnchor.localToWorldMatrix;
		foreach (SubGrabPoint multiPoint in multiPoints)
		{
			if (multiPoint == null)
			{
				break;
			}
			multiPoint.InitializePoints(anchor, advancedGrabPointAnchor, advancedGrabPointOrigin);
		}
	}

	public void AddLineButton()
	{
		multiPoints.Add(new SubLineGrabPoint());
	}
}
