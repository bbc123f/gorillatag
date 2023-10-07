using System;
using UnityEngine;

// Token: 0x0200013C RID: 316
public class GorillaHandHistory : MonoBehaviour
{
	// Token: 0x0600081D RID: 2077 RVA: 0x00033078 File Offset: 0x00031278
	private void Start()
	{
		this.direction = default(Vector3);
		this.lastPosition = default(Vector3);
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x00033092 File Offset: 0x00031292
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
