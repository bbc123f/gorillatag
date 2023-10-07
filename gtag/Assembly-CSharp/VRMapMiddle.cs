using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020000F6 RID: 246
[Serializable]
public class VRMapMiddle : VRMap
{
	// Token: 0x060005B8 RID: 1464 RVA: 0x00023B01 File Offset: 0x00021D01
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		this.gripValue = ControllerInputPoller.GripFloat(this.vrTargetNode);
		this.calcT = 1f * this.gripValue;
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x00023B3C File Offset: 0x00021D3C
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

	// Token: 0x040006C4 RID: 1732
	public InputFeatureUsage inputAxis;

	// Token: 0x040006C5 RID: 1733
	public float gripValue;

	// Token: 0x040006C6 RID: 1734
	public Transform fingerBone1;

	// Token: 0x040006C7 RID: 1735
	public Transform fingerBone2;

	// Token: 0x040006C8 RID: 1736
	public Transform fingerBone3;

	// Token: 0x040006C9 RID: 1737
	public float closedAngles;

	// Token: 0x040006CA RID: 1738
	public Vector3 closedAngle1;

	// Token: 0x040006CB RID: 1739
	public Vector3 closedAngle2;

	// Token: 0x040006CC RID: 1740
	public Vector3 closedAngle3;

	// Token: 0x040006CD RID: 1741
	public Vector3 startingAngle1;

	// Token: 0x040006CE RID: 1742
	public Vector3 startingAngle2;

	// Token: 0x040006CF RID: 1743
	public Vector3 startingAngle3;

	// Token: 0x040006D0 RID: 1744
	public Quaternion[] angle1Table;

	// Token: 0x040006D1 RID: 1745
	public Quaternion[] angle2Table;

	// Token: 0x040006D2 RID: 1746
	public Quaternion[] angle3Table;

	// Token: 0x040006D3 RID: 1747
	private int lastAngle1;

	// Token: 0x040006D4 RID: 1748
	private int lastAngle2;

	// Token: 0x040006D5 RID: 1749
	private int lastAngle3;

	// Token: 0x040006D6 RID: 1750
	private float currentAngle1;

	// Token: 0x040006D7 RID: 1751
	private float currentAngle2;

	// Token: 0x040006D8 RID: 1752
	private float currentAngle3;

	// Token: 0x040006D9 RID: 1753
	private InputDevice tempDevice;

	// Token: 0x040006DA RID: 1754
	private int myTempInt;
}
