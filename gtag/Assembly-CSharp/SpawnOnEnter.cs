using System;
using UnityEngine;

// Token: 0x0200002E RID: 46
public class SpawnOnEnter : MonoBehaviour
{
	// Token: 0x06000104 RID: 260 RVA: 0x000099AA File Offset: 0x00007BAA
	public void OnTriggerEnter(Collider other)
	{
		if (Time.time > this.lastSpawnTime + this.cooldown)
		{
			this.lastSpawnTime = Time.time;
			ObjectPools.instance.Instantiate(this.prefab, other.transform.position);
		}
	}

	// Token: 0x04000166 RID: 358
	public GameObject prefab;

	// Token: 0x04000167 RID: 359
	public float cooldown = 0.1f;

	// Token: 0x04000168 RID: 360
	private float lastSpawnTime;
}
