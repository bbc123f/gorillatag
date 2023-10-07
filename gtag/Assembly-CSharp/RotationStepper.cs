using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000019 RID: 25
public class RotationStepper : MonoBehaviour
{
	// Token: 0x06000055 RID: 85 RVA: 0x000045BD File Offset: 0x000027BD
	public void OnEnable()
	{
		this.m_phase = 0f;
		Random.InitState(0);
	}

	// Token: 0x06000056 RID: 86 RVA: 0x000045D0 File Offset: 0x000027D0
	public void Update()
	{
		this.m_phase += this.Frequency * Time.deltaTime;
		RotationStepper.ModeEnum mode = this.Mode;
		if (mode == RotationStepper.ModeEnum.Fixed)
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, (Mathf.Repeat(this.m_phase, 2f) < 1f) ? -25f : 25f);
			return;
		}
		if (mode != RotationStepper.ModeEnum.Random)
		{
			return;
		}
		while (this.m_phase >= 1f)
		{
			Random.InitState(Time.frameCount);
			base.transform.rotation = Random.rotationUniform;
			this.m_phase -= 1f;
		}
	}

	// Token: 0x0400007F RID: 127
	public RotationStepper.ModeEnum Mode;

	// Token: 0x04000080 RID: 128
	[ConditionalField("Mode", RotationStepper.ModeEnum.Fixed, null, null, null, null, null)]
	public float Angle = 25f;

	// Token: 0x04000081 RID: 129
	public float Frequency;

	// Token: 0x04000082 RID: 130
	private float m_phase;

	// Token: 0x02000385 RID: 901
	public enum ModeEnum
	{
		// Token: 0x04001AEC RID: 6892
		Fixed,
		// Token: 0x04001AED RID: 6893
		Random
	}
}
