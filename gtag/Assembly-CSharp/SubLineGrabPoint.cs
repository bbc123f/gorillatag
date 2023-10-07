using System;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000119 RID: 281
[Serializable]
public class SubLineGrabPoint : SubGrabPoint
{
	// Token: 0x06000729 RID: 1833 RVA: 0x0002D72C File Offset: 0x0002B92C
	public override Matrix4x4 GetTransformation_GripPointLocalToAdvOriginLocal(AdvancedItemState.PreData advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		float distAlongLine = advancedItemState.distAlongLine;
		Vector3 pos = Vector3.Lerp(this.startPointRelativeTransformToGrabPointOrigin.Position(), this.endPointRelativeTransformToGrabPointOrigin.Position(), distAlongLine);
		Quaternion q = Quaternion.Slerp(this.startPointRelativeTransformToGrabPointOrigin.rotation, this.endPointRelativeTransformToGrabPointOrigin.rotation, distAlongLine);
		return Matrix4x4.TRS(pos, q, Vector3.one);
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x0002D784 File Offset: 0x0002B984
	public override void InitializePoints(Transform anchor, Transform grabPointAnchor, Transform advancedGrabPointOrigin)
	{
		base.InitializePoints(anchor, grabPointAnchor, advancedGrabPointOrigin);
		if (this.startPoint == null || this.endPoint == null)
		{
			return;
		}
		this.startPointRelativeToGrabPointOrigin = advancedGrabPointOrigin.InverseTransformPoint(this.startPoint.position);
		this.endPointRelativeToGrabPointOrigin = advancedGrabPointOrigin.InverseTransformPoint(this.endPoint.position);
		this.endPointRelativeTransformToGrabPointOrigin = advancedGrabPointOrigin.worldToLocalMatrix * this.endPoint.localToWorldMatrix;
		this.startPointRelativeTransformToGrabPointOrigin = advancedGrabPointOrigin.worldToLocalMatrix * this.startPoint.localToWorldMatrix;
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x0002D81D File Offset: 0x0002BA1D
	public override AdvancedItemState.PreData GetPreData(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		return new AdvancedItemState.PreData
		{
			distAlongLine = SubLineGrabPoint.<GetPreData>g__FindNearestFractionOnLine|8_0(objectTransform.TransformPoint(this.startPointRelativeToGrabPointOrigin), objectTransform.TransformPoint(this.endPointRelativeToGrabPointOrigin), handTransform.position),
			pointType = AdvancedItemState.PointType.DistanceBased
		};
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x0002D854 File Offset: 0x0002BA54
	public override float EvaluateScore(Transform objectTransform, Transform handTransform, Transform targetDock)
	{
		float t = SubLineGrabPoint.<EvaluateScore>g__FindNearestFractionOnLine|9_0(objectTransform.TransformPoint(this.startPointRelativeToGrabPointOrigin), objectTransform.TransformPoint(this.endPointRelativeToGrabPointOrigin), handTransform.position);
		Vector3 a = Vector3.Lerp(this.startPointRelativeTransformToGrabPointOrigin.Position(), this.endPointRelativeTransformToGrabPointOrigin.Position(), t);
		Vector3 b = objectTransform.InverseTransformPoint(handTransform.position);
		return Vector3.SqrMagnitude(a - b);
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x0002D8C4 File Offset: 0x0002BAC4
	[CompilerGenerated]
	internal static float <GetPreData>g__FindNearestFractionOnLine|8_0(Vector3 origin, Vector3 end, Vector3 point)
	{
		Vector3 vector = end - origin;
		float magnitude = vector.magnitude;
		vector /= magnitude;
		return Mathf.Clamp01(Vector3.Dot(point - origin, vector) / magnitude);
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x0002D900 File Offset: 0x0002BB00
	[CompilerGenerated]
	internal static float <EvaluateScore>g__FindNearestFractionOnLine|9_0(Vector3 origin, Vector3 end, Vector3 point)
	{
		Vector3 vector = end - origin;
		float magnitude = vector.magnitude;
		vector /= magnitude;
		return Mathf.Clamp01(Vector3.Dot(point - origin, vector) / magnitude);
	}

	// Token: 0x040008C7 RID: 2247
	public Transform startPoint;

	// Token: 0x040008C8 RID: 2248
	public Transform endPoint;

	// Token: 0x040008C9 RID: 2249
	public Vector3 startPointRelativeToGrabPointOrigin;

	// Token: 0x040008CA RID: 2250
	public Vector3 endPointRelativeToGrabPointOrigin;

	// Token: 0x040008CB RID: 2251
	public Matrix4x4 startPointRelativeTransformToGrabPointOrigin;

	// Token: 0x040008CC RID: 2252
	public Matrix4x4 endPointRelativeTransformToGrabPointOrigin;
}
