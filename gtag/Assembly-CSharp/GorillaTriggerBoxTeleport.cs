using System;
using UnityEngine;

// Token: 0x02000147 RID: 327
public class GorillaTriggerBoxTeleport : GorillaTriggerBox
{
	// Token: 0x06000839 RID: 2105 RVA: 0x00033173 File Offset: 0x00031373
	public override void OnBoxTriggered()
	{
		this.cameraOffest.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
		this.cameraOffest.transform.position = this.teleportLocation;
	}

	// Token: 0x04000A1D RID: 2589
	public Vector3 teleportLocation;

	// Token: 0x04000A1E RID: 2590
	public GameObject cameraOffest;
}
