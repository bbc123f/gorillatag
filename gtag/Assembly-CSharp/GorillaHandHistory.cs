using System;
using UnityEngine;

// Token: 0x0200013C RID: 316
public class GorillaHandHistory : MonoBehaviour
{
	// Token: 0x0600081E RID: 2078 RVA: 0x00032EB8 File Offset: 0x000310B8
	private void Start()
	{
		this.direction = default(Vector3);
		this.lastPosition = default(Vector3);
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x00032ED2 File Offset: 0x000310D2
	private void FixedUpdate()
	{
		this.direction = this.lastPosition - base.transform.position;
		this.lastLastPosition = this.lastPosition;
		this.lastPosition = base.transform.position;
	}

	// Token: 0x040009D9 RID: 2521
	public Vector3 direction;

	// Token: 0x040009DA RID: 2522
	private Vector3 lastPosition;

	// Token: 0x040009DB RID: 2523
	private Vector3 lastLastPosition;
}
