using System;
using UnityEngine;

// Token: 0x02000108 RID: 264
public class ChestObjectHysteresis : MonoBehaviour
{
	// Token: 0x0600067E RID: 1662 RVA: 0x000289C7 File Offset: 0x00026BC7
	private void Start()
	{
		this.lastAngleQuat = base.transform.rotation;
		this.currentAngleQuat = base.transform.rotation;
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x000289EC File Offset: 0x00026BEC
	private void LateUpdate()
	{
		this.currentAngleQuat = this.angleFollower.rotation;
		this.angleBetween = Quaternion.Angle(this.currentAngleQuat, this.lastAngleQuat);
		if (this.angleBetween > this.angleHysteresis)
		{
			base.transform.rotation = Quaternion.Slerp(this.currentAngleQuat, this.lastAngleQuat, this.angleHysteresis / this.angleBetween);
			this.lastAngleQuat = base.transform.rotation;
		}
		base.transform.rotation = this.lastAngleQuat;
	}

	// Token: 0x040007C5 RID: 1989
	public float angleHysteresis;

	// Token: 0x040007C6 RID: 1990
	public float angleBetween;

	// Token: 0x040007C7 RID: 1991
	public Transform angleFollower;

	// Token: 0x040007C8 RID: 1992
	private Quaternion lastAngleQuat;

	// Token: 0x040007C9 RID: 1993
	private Quaternion currentAngleQuat;
}
