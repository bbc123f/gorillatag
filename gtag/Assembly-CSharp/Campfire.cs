using System;
using UnityEngine;

// Token: 0x0200014E RID: 334
public class Campfire : MonoBehaviour
{
	// Token: 0x0600084E RID: 2126 RVA: 0x00033934 File Offset: 0x00031B34
	private void Start()
	{
		this.lastAngleBottom = 0f;
		this.lastAngleMiddle = 0f;
		this.lastAngleTop = 0f;
		this.perlinBottom = (float)Random.Range(0, 100);
		this.perlinMiddle = (float)Random.Range(200, 300);
		this.perlinTop = (float)Random.Range(400, 500);
		this.startingRotationBottom = this.baseFire.localEulerAngles.x;
		this.startingRotationMiddle = this.middleFire.localEulerAngles.x;
		this.startingRotationTop = this.topFire.localEulerAngles.x;
		this.tempVec = new Vector3(0f, 0f, 0f);
		this.mergedBottom = false;
		this.mergedMiddle = false;
		this.mergedTop = false;
		this.wasActive = false;
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x00033A18 File Offset: 0x00031C18
	private void LateUpdate()
	{
		if (BetterDayNightManager.instance == null)
		{
			return;
		}
		if ((this.isActive[BetterDayNightManager.instance.currentTimeIndex] && BetterDayNightManager.instance.CurrentWeather() != BetterDayNightManager.WeatherType.Raining) || this.overrideDayNight == 1)
		{
			if (!this.wasActive)
			{
				this.wasActive = true;
				this.mergedBottom = false;
				this.mergedMiddle = false;
				this.mergedTop = false;
				Color.RGBToHSV(this.mat.color, out this.h, out this.s, out this.v);
				this.mat.color = Color.HSVToRGB(this.h, this.s, 1f);
			}
			this.Flap(ref this.perlinBottom, this.perlinStepBottom, ref this.lastAngleBottom, ref this.baseFire, this.bottomRange, this.baseMultiplier, ref this.mergedBottom);
			this.Flap(ref this.perlinMiddle, this.perlinStepMiddle, ref this.lastAngleMiddle, ref this.middleFire, this.middleRange, this.middleMultiplier, ref this.mergedMiddle);
			this.Flap(ref this.perlinTop, this.perlinStepTop, ref this.lastAngleTop, ref this.topFire, this.topRange, this.topMultiplier, ref this.mergedTop);
			return;
		}
		if (this.wasActive)
		{
			this.wasActive = false;
			this.mergedBottom = false;
			this.mergedMiddle = false;
			this.mergedTop = false;
			Color.RGBToHSV(this.mat.color, out this.h, out this.s, out this.v);
			this.mat.color = Color.HSVToRGB(this.h, this.s, 0.25f);
		}
		this.ReturnToOff(ref this.baseFire, this.startingRotationBottom, ref this.mergedBottom);
		this.ReturnToOff(ref this.middleFire, this.startingRotationMiddle, ref this.mergedMiddle);
		this.ReturnToOff(ref this.topFire, this.startingRotationTop, ref this.mergedTop);
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x00033C0C File Offset: 0x00031E0C
	private void Flap(ref float perlinValue, float perlinStep, ref float lastAngle, ref Transform flameTransform, float range, float multiplier, ref bool isMerged)
	{
		perlinValue += perlinStep;
		lastAngle += Time.deltaTime * Mathf.PerlinNoise(perlinValue, 0f);
		this.tempVec.x = range * Mathf.Sin(lastAngle * multiplier);
		if (Mathf.Abs(this.tempVec.x - flameTransform.localEulerAngles.x) > 180f)
		{
			if (this.tempVec.x > flameTransform.localEulerAngles.x)
			{
				this.tempVec.x = this.tempVec.x - 360f;
			}
			else
			{
				this.tempVec.x = this.tempVec.x + 360f;
			}
		}
		if (isMerged)
		{
			flameTransform.localEulerAngles = this.tempVec;
			return;
		}
		if (Mathf.Abs(flameTransform.localEulerAngles.x - this.tempVec.x) < 1f)
		{
			isMerged = true;
			flameTransform.localEulerAngles = this.tempVec;
			return;
		}
		this.tempVec.x = (this.tempVec.x - flameTransform.localEulerAngles.x) * this.slerp + flameTransform.localEulerAngles.x;
		flameTransform.localEulerAngles = this.tempVec;
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x00033D4C File Offset: 0x00031F4C
	private void ReturnToOff(ref Transform startTransform, float targetAngle, ref bool isMerged)
	{
		this.tempVec.x = targetAngle;
		if (Mathf.Abs(this.tempVec.x - startTransform.localEulerAngles.x) > 180f)
		{
			if (this.tempVec.x > startTransform.localEulerAngles.x)
			{
				this.tempVec.x = this.tempVec.x - 360f;
			}
			else
			{
				this.tempVec.x = this.tempVec.x + 360f;
			}
		}
		if (!isMerged)
		{
			if (Mathf.Abs(startTransform.localEulerAngles.x - targetAngle) < 1f)
			{
				isMerged = true;
				return;
			}
			this.tempVec.x = (this.tempVec.x - startTransform.localEulerAngles.x) * this.slerp + startTransform.localEulerAngles.x;
			startTransform.localEulerAngles = this.tempVec;
		}
	}

	// Token: 0x04000A3E RID: 2622
	public Transform baseFire;

	// Token: 0x04000A3F RID: 2623
	public Transform middleFire;

	// Token: 0x04000A40 RID: 2624
	public Transform topFire;

	// Token: 0x04000A41 RID: 2625
	public float baseMultiplier;

	// Token: 0x04000A42 RID: 2626
	public float middleMultiplier;

	// Token: 0x04000A43 RID: 2627
	public float topMultiplier;

	// Token: 0x04000A44 RID: 2628
	public float bottomRange;

	// Token: 0x04000A45 RID: 2629
	public float middleRange;

	// Token: 0x04000A46 RID: 2630
	public float topRange;

	// Token: 0x04000A47 RID: 2631
	private float lastAngleBottom;

	// Token: 0x04000A48 RID: 2632
	private float lastAngleMiddle;

	// Token: 0x04000A49 RID: 2633
	private float lastAngleTop;

	// Token: 0x04000A4A RID: 2634
	public float perlinStepBottom;

	// Token: 0x04000A4B RID: 2635
	public float perlinStepMiddle;

	// Token: 0x04000A4C RID: 2636
	public float perlinStepTop;

	// Token: 0x04000A4D RID: 2637
	private float perlinBottom;

	// Token: 0x04000A4E RID: 2638
	private float perlinMiddle;

	// Token: 0x04000A4F RID: 2639
	private float perlinTop;

	// Token: 0x04000A50 RID: 2640
	public float startingRotationBottom;

	// Token: 0x04000A51 RID: 2641
	public float startingRotationMiddle;

	// Token: 0x04000A52 RID: 2642
	public float startingRotationTop;

	// Token: 0x04000A53 RID: 2643
	public float slerp = 0.01f;

	// Token: 0x04000A54 RID: 2644
	private bool mergedBottom;

	// Token: 0x04000A55 RID: 2645
	private bool mergedMiddle;

	// Token: 0x04000A56 RID: 2646
	private bool mergedTop;

	// Token: 0x04000A57 RID: 2647
	public string lastTimeOfDay;

	// Token: 0x04000A58 RID: 2648
	public Material mat;

	// Token: 0x04000A59 RID: 2649
	private float h;

	// Token: 0x04000A5A RID: 2650
	private float s;

	// Token: 0x04000A5B RID: 2651
	private float v;

	// Token: 0x04000A5C RID: 2652
	public int overrideDayNight;

	// Token: 0x04000A5D RID: 2653
	private Vector3 tempVec;

	// Token: 0x04000A5E RID: 2654
	public bool[] isActive;

	// Token: 0x04000A5F RID: 2655
	public bool wasActive;
}
