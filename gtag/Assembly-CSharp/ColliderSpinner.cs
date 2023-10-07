using System;
using BoingKit;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class ColliderSpinner : MonoBehaviour
{
	// Token: 0x06000020 RID: 32 RVA: 0x00002B0C File Offset: 0x00000D0C
	private void Start()
	{
		this.m_targetOffset = ((this.Target != null) ? (base.transform.position - this.Target.position) : Vector3.zero);
		this.m_spring.Reset(base.transform.position);
	}

	// Token: 0x06000021 RID: 33 RVA: 0x00002B68 File Offset: 0x00000D68
	private void FixedUpdate()
	{
		Vector3 targetValue = this.Target.position + this.m_targetOffset;
		base.transform.position = this.m_spring.TrackExponential(targetValue, 0.02f, Time.fixedDeltaTime);
	}

	// Token: 0x04000021 RID: 33
	public Transform Target;

	// Token: 0x04000022 RID: 34
	private Vector3 m_targetOffset;

	// Token: 0x04000023 RID: 35
	private Vector3Spring m_spring;
}
