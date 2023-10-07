using System;
using BoingKit;
using UnityEngine;

// Token: 0x0200000B RID: 11
public class UFOCamera : MonoBehaviour
{
	// Token: 0x06000023 RID: 35 RVA: 0x00002BB8 File Offset: 0x00000DB8
	private void Start()
	{
		if (this.Target == null)
		{
			return;
		}
		this.m_targetOffset = base.transform.position - this.Target.position;
		this.m_spring.Reset(base.transform.position);
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00002C0C File Offset: 0x00000E0C
	private void FixedUpdate()
	{
		if (this.Target == null)
		{
			return;
		}
		Vector3 targetValue = this.Target.position + this.m_targetOffset;
		base.transform.position = this.m_spring.TrackExponential(targetValue, 0.02f, Time.fixedDeltaTime);
	}

	// Token: 0x04000024 RID: 36
	public Transform Target;

	// Token: 0x04000025 RID: 37
	private Vector3 m_targetOffset;

	// Token: 0x04000026 RID: 38
	private Vector3Spring m_spring;
}
