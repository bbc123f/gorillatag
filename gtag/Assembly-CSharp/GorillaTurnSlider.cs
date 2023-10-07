using System;
using UnityEngine;

// Token: 0x020001B8 RID: 440
public class GorillaTurnSlider : MonoBehaviour
{
	// Token: 0x06000B38 RID: 2872 RVA: 0x0004528D File Offset: 0x0004348D
	private void Awake()
	{
		this.startingLocation = base.transform.position;
		this.SetPosition(this.gorillaTurn.currentSpeed);
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x000452B1 File Offset: 0x000434B1
	private void FixedUpdate()
	{
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x000452B4 File Offset: 0x000434B4
	public void SetPosition(float speed)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		float x = (speed - this.minValue) * (num2 - num) / (this.maxValue - this.minValue) + num;
		base.transform.position = new Vector3(x, this.startingLocation.y, this.startingLocation.z);
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x00045338 File Offset: 0x00043538
	public float InterpolateValue(float value)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		return (value - num) / (num2 - num) * (this.maxValue - this.minValue) + this.minValue;
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x00045394 File Offset: 0x00043594
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

	// Token: 0x04000E9F RID: 3743
	public float zRange;

	// Token: 0x04000EA0 RID: 3744
	public float maxValue;

	// Token: 0x04000EA1 RID: 3745
	public float minValue;

	// Token: 0x04000EA2 RID: 3746
	public GorillaTurning gorillaTurn;

	// Token: 0x04000EA3 RID: 3747
	private float startingZ;

	// Token: 0x04000EA4 RID: 3748
	public Vector3 startingLocation;
}
