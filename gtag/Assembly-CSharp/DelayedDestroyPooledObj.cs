using System;
using UnityEngine;

// Token: 0x02000211 RID: 529
public class DelayedDestroyPooledObj : MonoBehaviour
{
	// Token: 0x06000D42 RID: 3394 RVA: 0x0004DCBA File Offset: 0x0004BEBA
	protected void OnEnable()
	{
		if (ObjectPools.instance == null || !ObjectPools.instance.initialized)
		{
			return;
		}
		this.timeToDie = Time.time + this.destroyDelay;
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x0004DCE8 File Offset: 0x0004BEE8
	protected void LateUpdate()
	{
		if (Time.time > this.timeToDie)
		{
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x04001071 RID: 4209
	[Tooltip("Return to the object pool after this many seconds.")]
	public float destroyDelay;

	// Token: 0x04001072 RID: 4210
	private float timeToDie = -1f;
}
