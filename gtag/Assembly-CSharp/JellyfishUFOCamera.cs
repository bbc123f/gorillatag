using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class JellyfishUFOCamera : MonoBehaviour
{
	// Token: 0x06000014 RID: 20 RVA: 0x00002456 File Offset: 0x00000656
	private void Start()
	{
		if (this.Target == null)
		{
			return;
		}
		this.m_spring.Reset(this.Target.transform.position);
	}

	// Token: 0x06000015 RID: 21 RVA: 0x00002484 File Offset: 0x00000684
	private void FixedUpdate()
	{
		if (this.Target == null)
		{
			return;
		}
		this.m_spring.TrackExponential(this.Target.transform.position, 0.5f, Time.fixedDeltaTime);
		Vector3 normalized = (this.m_spring.Value - base.transform.position).normalized;
		base.transform.rotation = Quaternion.LookRotation(normalized);
	}

	// Token: 0x04000009 RID: 9
	public Transform Target;

	// Token: 0x0400000A RID: 10
	private Vector3Spring m_spring;
}
