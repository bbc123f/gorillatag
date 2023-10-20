using System;
using UnityEngine;

// Token: 0x020001B9 RID: 441
public class GorillaTurnSlider : MonoBehaviour
{
	// Token: 0x06000B3E RID: 2878 RVA: 0x000454F5 File Offset: 0x000436F5
	private void Awake()
	{
		this.startingLocation = base.transform.position;
		this.SetPosition(this.gorillaTurn.currentSpeed);
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x00045519 File Offset: 0x00043719
	private void FixedUpdate()
	{
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x0004551C File Offset: 0x0004371C
	public void SetPosition(float speed)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		float x = (speed - this.minValue) * (num2 - num) / (this.maxValue - this.minValue) + num;
		base.transform.position = new Vector3(x, this.startingLocation.y, this.startingLocation.z);
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x000455A0 File Offset: 0x000437A0
	public float InterpolateValue(float value)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		return (value - num) / (num2 - num) * (this.maxValue - this.minValue) + this.minValue;
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x000455FC File Offset: 0x000437FC
	public void OnSliderRelease()
	{
		if (this.zRange != 0f && (base.transform.position - this.startingLocation).magnitude > this.zRange / 2f)
		{
			if (base.transform.position.x > this.startingLocation.x)
			{
				base.transform.position = new Vector3(this.startingLocation.x + this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
				return;
			}
			base.transform.position = new Vector3(this.startingLocation.x - this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
		}
	}

	// Token: 0x04000EA3 RID: 3747
	public float zRange;

	// Token: 0x04000EA4 RID: 3748
	public float maxValue;

	// Token: 0x04000EA5 RID: 3749
	public float minValue;

	// Token: 0x04000EA6 RID: 3750
	public GorillaTurning gorillaTurn;

	// Token: 0x04000EA7 RID: 3751
	private float startingZ;

	// Token: 0x04000EA8 RID: 3752
	public Vector3 startingLocation;
}
