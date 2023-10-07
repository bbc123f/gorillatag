using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020000F4 RID: 244
[Serializable]
public class VRMap
{
	// Token: 0x060005AF RID: 1455 RVA: 0x000236D4 File Offset: 0x000218D4
	public void MapOther(float lerpValue)
	{
		this.rigTarget.localPosition = Vector3.Lerp(this.rigTarget.localPosition, this.syncPos, lerpValue);
		this.rigTarget.localRotation = Quaternion.Lerp(this.rigTarget.localRotation, this.syncRotation, lerpValue);
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x00023728 File Offset: 0x00021928
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

	// Token: 0x060005B1 RID: 1457 RVA: 0x00023879 File Offset: 0x00021A79
	public virtual void MapOtherFinger(float handSync, float lerpValue)
	{
		this.calcT = handSync;
		this.LerpFinger(lerpValue, true);
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x0002388A File Offset: 0x00021A8A
	public virtual void MapMyFinger(float lerpValue)
	{
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x0002388C File Offset: 0x00021A8C
	public virtual void LerpFinger(float lerpValue, bool isOther)
	{
	}

	// Token: 0x0400069F RID: 1695
	public XRNode vrTargetNode;

	// Token: 0x040006A0 RID: 1696
	public Transform overrideTarget;

	// Token: 0x040006A1 RID: 1697
	public Transform rigTarget;

	// Token: 0x040006A2 RID: 1698
	public Vector3 trackingPositionOffset;

	// Token: 0x040006A3 RID: 1699
	public Vector3 trackingRotationOffset;

	// Token: 0x040006A4 RID: 1700
	public Transform headTransform;

	// Token: 0x040006A5 RID: 1701
	public Vector3 syncPos;

	// Token: 0x040006A6 RID: 1702
	public Quaternion syncRotation;

	// Token: 0x040006A7 RID: 1703
	public float calcT;

	// Token: 0x040006A8 RID: 1704
	private InputDevice myInputDevice;

	// Token: 0x040006A9 RID: 1705
	private Vector3 tempPosition;

	// Token: 0x040006AA RID: 1706
	private Quaternion tempRotation;

	// Token: 0x040006AB RID: 1707
	public int tempInt;
}
