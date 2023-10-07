using System;
using UnityEngine;

// Token: 0x020001AC RID: 428
public class GorillaColorSlider : MonoBehaviour
{
	// Token: 0x06000AFD RID: 2813 RVA: 0x00044075 File Offset: 0x00042275
	private void Start()
	{
		if (!this.setRandomly)
		{
			this.startingLocation = base.transform.position;
		}
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x00044090 File Offset: 0x00042290
	public void SetPosition(float speed)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		float x = (speed - this.minValue) * (num2 - num) / (this.maxValue - this.minValue) + num;
		base.transform.position = new Vector3(x, this.startingLocation.y, this.startingLocation.z);
		this.valueImReporting = this.InterpolateValue(base.transform.position.x);
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x00044130 File Offset: 0x00042330
	public float InterpolateValue(float value)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		return (value - num) / (num2 - num) * (this.maxValue - this.minValue) + this.minValue;
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x0004418C File Offset: 0x0004238C
	public void OnSliderRelease()
	{
		if (this.zRange != 0f && (base.transform.position - this.startingLocation).magnitude > this.zRange / 2f)
		{
			if (base.transform.position.x > this.startingLocation.x)
			{
				base.transform.position = new Vector3(this.startingLocation.x + this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
			}
			else
			{
				base.transform.position = new Vector3(this.startingLocation.x - this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
			}
		}
		this.valueImReporting = this.InterpolateValue(base.transform.position.x);
	}

	// Token: 0x04000E21 RID: 3617
	public bool setRandomly;

	// Token: 0x04000E22 RID: 3618
	public float zRange;

	// Token: 0x04000E23 RID: 3619
	public float maxValue;

	// Token: 0x04000E24 RID: 3620
	public float minValue;

	// Token: 0x04000E25 RID: 3621
	public Vector3 startingLocation;

	// Token: 0x04000E26 RID: 3622
	public int valueIndex;

	// Token: 0x04000E27 RID: 3623
	public float valueImReporting;

	// Token: 0x04000E28 RID: 3624
	public GorillaTriggerBox gorilla;

	// Token: 0x04000E29 RID: 3625
	private float startingZ;
}
