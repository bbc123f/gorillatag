using System;
using UnityEngine;

// Token: 0x02000212 RID: 530
public class DelayedDestroyPooledObj : MonoBehaviour
{
	// Token: 0x06000D48 RID: 3400 RVA: 0x0004DF1A File Offset: 0x0004C11A
	protected void OnEnable()
	{
		if (ObjectPools.instance == null || !ObjectPools.instance.initialized)
		{
			return;
		}
		this.timeToDie = Time.time + this.destroyDelay;
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x0004DF48 File Offset: 0x0004C148
	protected void LateUpdate()
	{
		if (Time.time > this.timeToDie)
		{
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x04001076 RID: 4214
	[Tooltip("Return to the object pool after this many seconds.")]
	public float destroyDelay;

	// Token: 0x04001077 RID: 4215
	private float timeToDie = -1f;
}
