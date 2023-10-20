using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020000F5 RID: 245
[Serializable]
public class VRMapIndex : VRMap
{
	// Token: 0x060005B5 RID: 1461 RVA: 0x0002368C File Offset: 0x0002188C
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		this.triggerValue = ControllerInputPoller.TriggerFloat(this.vrTargetNode);
		this.triggerTouch = ControllerInputPoller.TriggerTouch(this.vrTargetNode);
		this.calcT = 0.1f * this.triggerTouch;
		this.calcT += 0.9f * this.triggerValue;
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x000236FC File Offset: 0x000218FC
	public override void LerpFinger(float lerpValue, bool isOther)
	{
		if (isOther)
		{
			this.currentAngle1 = Mathf.Lerp(this.currentAngle1, this.calcT, lerpValue);
			this.currentAngle2 = Mathf.Lerp(this.currentAngle2, this.calcT, lerpValue);
			this.currentAngle3 = Mathf.Lerp(this.currentAngle3, this.calcT, lerpValue);
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
			}
			this.myTempInt = (int)(this.currentAngle3 * 10.1f);
			if (this.myTempInt != this.lastAngle3)
			{
				this.lastAngle3 = this.myTempInt;
				this.fingerBone3.localRotation = this.angle3Table[this.lastAngle3];
				return;
			}
		}
		else
		{
			this.fingerBone1.localRotation = Quaternion.Lerp(this.fingerBone1.localRotation, Quaternion.Lerp(Quaternion.Euler(this.startingAngle1), Quaternion.Euler(this.closedAngle1), this.calcT), lerpValue);
			this.fingerBone2.localRotation = Quaternion.Lerp(this.fingerBone2.localRotation, Quaternion.Lerp(Quaternion.Euler(this.startingAngle2), Quaternion.Euler(this.closedAngle2), this.calcT), lerpValue);
			this.fingerBone3.localRotation = Quaternion.Lerp(this.fingerBone3.localRotation, Quaternion.Lerp(Quaternion.Euler(this.startingAngle3), Quaternion.Euler(this.closedAngle3), this.calcT), lerpValue);
		}
	}

	// Token: 0x040006AC RID: 1708
	public InputFeatureUsage inputAxis;

	// Token: 0x040006AD RID: 1709
	public float triggerTouch;

	// Token: 0x040006AE RID: 1710
	public float triggerValue;

	// Token: 0x040006AF RID: 1711
	public Transform fingerBone1;

	// Token: 0x040006B0 RID: 1712
	public Transform fingerBone2;

	// Token: 0x040006B1 RID: 1713
	public Transform fingerBone3;

	// Token: 0x040006B2 RID: 1714
	public float closedAngles;

	// Token: 0x040006B3 RID: 1715
	public Vector3 closedAngle1;

	// Token: 0x040006B4 RID: 1716
	public Vector3 closedAngle2;

	// Token: 0x040006B5 RID: 1717
	public Vector3 closedAngle3;

	// Token: 0x040006B6 RID: 1718
	public Vector3 startingAngle1;

	// Token: 0x040006B7 RID: 1719
	public Vector3 startingAngle2;

	// Token: 0x040006B8 RID: 1720
	public Vector3 startingAngle3;

	// Token: 0x040006B9 RID: 1721
	private int lastAngle1;

	// Token: 0x040006BA RID: 1722
	private int lastAngle2;

	// Token: 0x040006BB RID: 1723
	private int lastAngle3;

	// Token: 0x040006BC RID: 1724
	private InputDevice myInputDevice;

	// Token: 0x040006BD RID: 1725
	public Quaternion[] angle1Table;

	// Token: 0x040006BE RID: 1726
	public Quaternion[] angle2Table;

	// Token: 0x040006BF RID: 1727
	public Quaternion[] angle3Table;

	// Token: 0x040006C0 RID: 1728
	private float currentAngle1;

	// Token: 0x040006C1 RID: 1729
	private float currentAngle2;

	// Token: 0x040006C2 RID: 1730
	private float currentAngle3;

	// Token: 0x040006C3 RID: 1731
	private int myTempInt;
}
