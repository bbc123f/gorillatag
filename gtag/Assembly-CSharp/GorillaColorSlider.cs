using System;
using UnityEngine;

// Token: 0x020001AD RID: 429
public class GorillaColorSlider : MonoBehaviour
{
	// Token: 0x06000B02 RID: 2818 RVA: 0x000441AD File Offset: 0x000423AD
	private void Start()
	{
		if (!this.setRandomly)
		{
			this.startingLocation = base.transform.position;
		}
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x000441C8 File Offset: 0x000423C8
	public void SetPosition(float speed)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		float x = (speed - this.minValue) * (num2 - num) / (this.maxValue - this.minValue) + num;
		base.transform.position = new Vector3(x, this.startingLocation.y, this.startingLocation.z);
		this.valueImReporting = this.InterpolateValue(base.transform.position.x);
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x00044268 File Offset: 0x00042468
	public float InterpolateValue(float value)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		return (value - num) / (num2 - num) * (this.maxValue - this.minValue) + this.minValue;
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x000442C4 File Offset: 0x000424C4
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

	// Token: 0x04000E25 RID: 3621
	public bool setRandomly;

	// Token: 0x04000E26 RID: 3622
	public float zRange;

	// Token: 0x04000E27 RID: 3623
	public float maxValue;

	// Token: 0x04000E28 RID: 3624
	public float minValue;

	// Token: 0x04000E29 RID: 3625
	public Vector3 startingLocation;

	// Token: 0x04000E2A RID: 3626
	public int valueIndex;

	// Token: 0x04000E2B RID: 3627
	public float valueImReporting;

	// Token: 0x04000E2C RID: 3628
	public GorillaTriggerBox gorilla;

	// Token: 0x04000E2D RID: 3629
	private float startingZ;
}
