using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class SubGrabPoint
{
	[FormerlySerializedAs("transform")]
	public Transform gripPoint;

	public LimitAxis limitAxis;

	public bool allowReverseGrip;

	private Vector3 pointRelativeToGrabPointOrigin;

	public Quaternion rotationRelativeToObjectAnchor;

	public Matrix4x4 gripPointRelativeToGrabOrigin;

	public virtual Matrix4x4 GetTransformationMatrixOfGrabPointRelativeToGripOrigin(AdvancedItemState.PreData advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		return gripPointRelativeToGrabOrigin;
	}

	public virtual Quaternion GetRotationRelativeToObjectAnchor(AdvancedItemState advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		return rotationRelativeToObjectAnchor;
	}

	public virtual Vector3 GetGrabPositionRelativeToGrabPointOrigin(AdvancedItemState advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		return pointRelativeToGrabPointOrigin;
	}

	public virtual void InitializePoints(Transform anchor, Transform grabPointAnchor, Transform advancedGrabPointOrigin)
	{
		Debug.Log(anchor);
		Debug.Log(gripPoint);
		if (!(gripPoint == null))
		{
			pointRelativeToGrabPointOrigin = advancedGrabPointOrigin.InverseTransformPoint(gripPoint.position);
			rotationRelativeToObjectAnchor = Quaternion.Inverse(anchor.rotation) * gripPoint.rotation;
			gripPointRelativeToGrabOrigin = advancedGrabPointOrigin.worldToLocalMatrix * gripPoint.localToWorldMatrix;
		}
	}

	public Vector3 GetPositionOnObject(Transform transferableObject, SlotTransformOverride slotTransformOverride)
	{
		return transferableObject.TransformPoint(pointRelativeToGrabPointOrigin);
	}

	public virtual Matrix4x4 GetTransformFromPositionState(AdvancedItemState advancedItemState, SlotTransformOverride slotTransformOverride, Transform targetDockXf)
	{
		Quaternion deltaRotation = advancedItemState.deltaRotation;
		Matrix4x4 matrix4x = Matrix4x4.TRS(Vector3.zero, deltaRotation, Vector3.one);
		Matrix4x4 matrix4x2 = GetTransformationMatrixOfGrabPointRelativeToGripOrigin(advancedItemState.preData, slotTransformOverride) * matrix4x.inverse;
		Matrix4x4 matrix4x3 = slotTransformOverride.GrabPointAnchorRelativeToGripOrigin * matrix4x2.inverse;
		return slotTransformOverride.grabPointRelativeToObjectAnchor * matrix4x3;
	}

	public AdvancedItemState GetAdvancedItemStateFromHand(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		AdvancedItemState.PreData preData = GetPreData(objectTransform, handTransform, targetDock, slotTransformOverride);
		Matrix4x4 matrix4x = targetDock.localToWorldMatrix * slotTransformOverride.grabPointRelativeToObjectAnchor * slotTransformOverride.GrabPointAnchorRelativeToGripOrigin;
		Matrix4x4 matrix4x2 = objectTransform.localToWorldMatrix * GetTransformationMatrixOfGrabPointRelativeToGripOrigin(preData, slotTransformOverride);
		Quaternion quaternion = (matrix4x.inverse * matrix4x2).rotation;
		Vector3 vector = quaternion * Vector3.up;
		Vector3 vector2 = quaternion * Vector3.right;
		Vector3 rhs = quaternion * Vector3.forward;
		bool reverseGrip = false;
		Vector2 angleVectorWhereUpIsStandard = Vector2.up;
		float angle = 0f;
		switch (limitAxis)
		{
		case LimitAxis.NoMovement:
			quaternion = Quaternion.identity;
			break;
		case LimitAxis.YAxis:
			if (allowReverseGrip)
			{
				if (Vector3.Dot(vector, Vector3.up) < 0f)
				{
					Debug.Log("Using Reverse Grip");
					reverseGrip = true;
					vector = Vector3.down;
				}
				else
				{
					vector = Vector3.up;
				}
			}
			else
			{
				vector = Vector3.up;
			}
			vector2 = Vector3.Cross(vector, rhs);
			rhs = Vector3.Cross(vector2, vector);
			angleVectorWhereUpIsStandard = new Vector2(rhs.z, rhs.x);
			quaternion = Quaternion.LookRotation(rhs, vector);
			break;
		case LimitAxis.XAxis:
			vector2 = Vector3.right;
			rhs = Vector3.Cross(vector2, vector);
			vector = Vector3.Cross(rhs, vector2);
			break;
		case LimitAxis.ZAxis:
			rhs = Vector3.forward;
			vector2 = Vector3.Cross(vector, rhs);
			vector = Vector3.Cross(rhs, vector2);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return new AdvancedItemState
		{
			preData = preData,
			limitAxis = limitAxis,
			angle = angle,
			reverseGrip = reverseGrip,
			angleVectorWhereUpIsStandard = angleVectorWhereUpIsStandard,
			deltaRotation = quaternion
		};
	}

	public virtual AdvancedItemState.PreData GetPreData(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		return new AdvancedItemState.PreData
		{
			pointType = AdvancedItemState.PointType.Standard
		};
	}

	public virtual float EvaluateScore(Transform objectTransform, Transform handTransform)
	{
		Vector3 vector = objectTransform.InverseTransformPoint(handTransform.position);
		return Vector3.SqrMagnitude(pointRelativeToGrabPointOrigin - vector);
	}
}
