using System;
using UnityEngine;
using UnityEngine.XR;

[Serializable]
public class VRMap
{
	public void MapOther(float lerpValue)
	{
		this.rigTarget.localPosition = Vector3.Lerp(this.rigTarget.localPosition, this.syncPos, lerpValue);
		this.rigTarget.localRotation = Quaternion.Lerp(this.rigTarget.localRotation, this.syncRotation, lerpValue);
	}

	public void MapMine(float ratio, Transform playerOffsetTransform)
	{
		if (this.overrideTarget != null)
		{
			this.rigTarget.rotation = this.overrideTarget.rotation * Quaternion.Euler(this.trackingRotationOffset);
			this.rigTarget.position = this.overrideTarget.position + this.rigTarget.rotation * this.trackingPositionOffset * ratio;
			return;
		}
		if (ConnectedControllerHandler.Instance.GetValidForXRNode(this.vrTargetNode))
		{
			this.myInputDevice = InputDevices.GetDeviceAtXRNode(this.vrTargetNode);
			if (this.myInputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.tempRotation) && this.myInputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.tempPosition))
			{
				this.rigTarget.rotation = this.tempRotation * Quaternion.Euler(this.trackingRotationOffset);
				this.rigTarget.position = this.tempPosition + this.rigTarget.rotation * this.trackingPositionOffset * ratio + playerOffsetTransform.position;
				this.rigTarget.RotateAround(playerOffsetTransform.position, Vector3.up, playerOffsetTransform.eulerAngles.y);
			}
		}
	}

	public virtual void MapOtherFinger(float handSync, float lerpValue)
	{
		this.calcT = handSync;
		this.LerpFinger(lerpValue, true);
	}

	public virtual void MapMyFinger(float lerpValue)
	{
	}

	public virtual void LerpFinger(float lerpValue, bool isOther)
	{
	}

	public XRNode vrTargetNode;

	public Transform overrideTarget;

	public Transform rigTarget;

	public Vector3 trackingPositionOffset;

	public Vector3 trackingRotationOffset;

	public Transform headTransform;

	public Vector3 syncPos;

	public Quaternion syncRotation;

	public float calcT;

	private InputDevice myInputDevice;

	private Vector3 tempPosition;

	private Quaternion tempRotation;

	public int tempInt;
}
