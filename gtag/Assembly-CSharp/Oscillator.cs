using System;
using UnityEngine;

// Token: 0x02000018 RID: 24
public class Oscillator : MonoBehaviour
{
	// Token: 0x06000050 RID: 80 RVA: 0x000043E9 File Offset: 0x000025E9
	public void Init(Vector3 center, Vector3 radius, Vector3 frequency, Vector3 startPhase)
	{
		this.Center = center;
		this.Radius = radius;
		this.Frequency = frequency;
		this.Phase = startPhase;
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00004408 File Offset: 0x00002608
	private float SampleWave(float phase)
	{
		switch (this.WaveType)
		{
		case Oscillator.WaveTypeEnum.Sine:
			return Mathf.Sin(phase);
		case Oscillator.WaveTypeEnum.Square:
			phase = Mathf.Repeat(phase, 6.2831855f);
			if (phase >= 3.1415927f)
			{
				return -1f;
			}
			return 1f;
		case Oscillator.WaveTypeEnum.Triangle:
			phase = Mathf.Repeat(phase, 6.2831855f);
			if (phase < 1.5707964f)
			{
				return phase / 1.5707964f;
			}
			if (phase < 3.1415927f)
			{
				return 1f - (phase - 1.5707964f) / 1.5707964f;
			}
			if (phase < 4.712389f)
			{
				return (3.1415927f - phase) / 1.5707964f;
			}
			return (phase - 4.712389f) / 1.5707964f - 1f;
		default:
			return 0f;
		}
	}

	// Token: 0x06000052 RID: 82 RVA: 0x000044C3 File Offset: 0x000026C3
	public void OnEnable()
	{
		this.m_initCenter = base.transform.position;
	}

	// Token: 0x06000053 RID: 83 RVA: 0x000044D8 File Offset: 0x000026D8
	public void Update()
	{
		this.Phase += this.Frequency * 2f * 3.1415927f * Time.deltaTime;
		Vector3 position = this.UseCenter ? this.Center : this.m_initCenter;
		position.x += this.Radius.x * this.SampleWave(this.Phase.x);
		position.y += this.Radius.y * this.SampleWave(this.Phase.y);
		position.z += this.Radius.z * this.SampleWave(this.Phase.z);
		base.transform.position = position;
	}

	// Token: 0x04000078 RID: 120
	public Oscillator.WaveTypeEnum WaveType;

	// Token: 0x04000079 RID: 121
	private Vector3 m_initCenter;

	// Token: 0x0400007A RID: 122
	public bool UseCenter;

	// Token: 0x0400007B RID: 123
	public Vector3 Center;

	// Token: 0x0400007C RID: 124
	public Vector3 Radius;

	// Token: 0x0400007D RID: 125
	public Vector3 Frequency;

	// Token: 0x0400007E RID: 126
	public Vector3 Phase;

	// Token: 0x02000386 RID: 902
	public enum WaveTypeEnum
	{
		// Token: 0x04001AF5 RID: 6901
		Sine,
		// Token: 0x04001AF6 RID: 6902
		Square,
		// Token: 0x04001AF7 RID: 6903
		Triangle
	}
}
