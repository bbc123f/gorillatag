using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020000F7 RID: 247
[Serializable]
public class VRMapThumb : VRMap
{
	// Token: 0x060005BB RID: 1467 RVA: 0x00023B2C File Offset: 0x00021D2C
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		if (this.vrTargetNode == XRNode.LeftHand)
		{
			this.primaryButtonPress = ControllerInputPoller.instance.leftControllerPrimaryButton;
			this.primaryButtonTouch = ControllerInputPoller.instance.leftControllerPrimaryButtonTouch;
			this.secondaryButtonPress = ControllerInputPoller.instance.leftControllerSecondaryButton;
			this.secondaryButtonTouch = ControllerInputPoller.instance.leftControllerSecondaryButtonTouch;
		}
		else
		{
			this.primaryButtonPress = ControllerInputPoller.instance.rightControllerPrimaryButton;
			this.primaryButtonTouch = ControllerInputPoller.instance.rightControllerPrimaryButtonTouch;
			this.secondaryButtonPress = ControllerInputPoller.instance.rightControllerSecondaryButton;
			this.secondaryButtonTouch = ControllerInputPoller.instance.rightControllerSecondaryButtonTouch;
		}
		if (this.primaryButtonPress || this.secondaryButtonPress)
		{
			this.calcT = 1f;
		}
		else if (this.primaryButtonTouch || this.secondaryButtonTouch)
		{
			this.calcT = 0.1f;
		}
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x00023C20 File Offset: 0x00021E20
	public override void LerpFinger(float lerpValue, bool isOther)
	{
		if (isOther)
		{
			this.currentAngle1 = Mathf.Lerp(this.currentAngle1, this.calcT, lerpValue);
			this.currentAngle2 = Mathf.Lerp(this.currentAngle2, this.calcT, lerpValue);
			this.myTempInt = (int)(this.currentAngle1 * 10.1f);
			if (this.myTempInt != this.lastAngle1)
			{
				this.lastAngle1 = this.myTempInt;
				this.fingerBone1.localRotation = this.angle1Table[this.lastAngle1];
			}
			this.myTempInt = (int)(this.currentAngle2 * 10.1f);
			if (this.myTempInt != this.lastAngle2)
			{
				this.lastAngle2 = this.myTempInt;
				this.fingerBone2.localRotation = this.angle2Table[this.lastAngle2];
				return;
			}
		}
		else
		{
			this.fingerBone1.localRotation = Quaternion.Lerp(this.fingerBone1.localRotation, Quaternion.Lerp(Quaternion.Euler(this.startingAngle1), Quaternion.Euler(this.closedAngle1), this.calcT), lerpValue);
			this.fingerBone2.localRotation = Quaternion.Lerp(this.fingerBone2.localRotation, Quaternion.Lerp(Quaternion.Euler(this.startingAngle2), Quaternion.Euler(this.closedAngle2), this.calcT), lerpValue);
		}
	}

	// Token: 0x040006DB RID: 1755
	public InputFeatureUsage inputAxis;

	// Token: 0x040006DC RID: 1756
	public bool primaryButtonTouch;

	// Token: 0x040006DD RID: 1757
	public bool primaryButtonPress;

	// Token: 0x040006DE RID: 1758
	public bool secondaryButtonTouch;

	// Token: 0x040006DF RID: 1759
	public bool secondaryButtonPress;

	// Token: 0x040006E0 RID: 1760
	public Transform fingerBone1;

	// Token: 0x040006E1 RID: 1761
	public Transform fingerBone2;

	// Token: 0x040006E2 RID: 1762
	public Vector3 closedAngle1;

	// Token: 0x040006E3 RID: 1763
	public Vector3 closedAngle2;

	// Token: 0x040006E4 RID: 1764
	public Vector3 startingAngle1;

	// Token: 0x040006E5 RID: 1765
	public Vector3 startingAngle2;

	// Token: 0x040006E6 RID: 1766
	public Quaternion[] angle1Table;

	// Token: 0x040006E7 RID: 1767
	public Quaternion[] angle2Table;

	// Token: 0x040006E8 RID: 1768
	private float currentAngle1;

	// Token: 0x040006E9 RID: 1769
	private float currentAngle2;

	// Token: 0x040006EA RID: 1770
	private int lastAngle1;

	// Token: 0x040006EB RID: 1771
	private int lastAngle2;

	// Token: 0x040006EC RID: 1772
	private InputDevice tempDevice;

	// Token: 0x040006ED RID: 1773
	private int myTempInt;
}
