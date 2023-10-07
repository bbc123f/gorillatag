using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000011 RID: 17
public class ScaleSpring : MonoBehaviour
{
	// Token: 0x06000035 RID: 53 RVA: 0x00003294 File Offset: 0x00001494
	public void Tick()
	{
		this.m_targetScale = ((this.m_targetScale == ScaleSpring.kSmallScale) ? ScaleSpring.kLargeScale : ScaleSpring.kSmallScale);
		this.m_lastTickTime = Time.time;
		base.GetComponent<BoingEffector>().MoveDistance = ScaleSpring.kMoveDistance * ((this.m_targetScale == ScaleSpring.kSmallScale) ? -1f : 1f);
	}

	// Token: 0x06000036 RID: 54 RVA: 0x000032F5 File Offset: 0x000014F5
	public void Start()
	{
		this.Tick();
		this.m_spring.Reset(this.m_targetScale * Vector3.one);
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00003318 File Offset: 0x00001518
	public void FixedUpdate()
	{
		if (Time.time - this.m_lastTickTime > ScaleSpring.kInterval)
		{
			this.Tick();
		}
		this.m_spring.TrackHalfLife(this.m_targetScale * Vector3.one, 6f, 0.05f, Time.fixedDeltaTime);
		base.transform.localScale = this.m_spring.Value;
		base.GetComponent<BoingEffector>().MoveDistance *= Mathf.Min(0.99f, 35f * Time.fixedDeltaTime);
	}

	// Token: 0x04000039 RID: 57
	private static readonly float kInterval = 2f;

	// Token: 0x0400003A RID: 58
	private static readonly float kSmallScale = 0.6f;

	// Token: 0x0400003B RID: 59
	private static readonly float kLargeScale = 2f;

	// Token: 0x0400003C RID: 60
	private static readonly float kMoveDistance = 30f;

	// Token: 0x0400003D RID: 61
	private Vector3Spring m_spring;

	// Token: 0x0400003E RID: 62
	private float m_targetScale;

	// Token: 0x0400003F RID: 63
	private float m_lastTickTime;
}
