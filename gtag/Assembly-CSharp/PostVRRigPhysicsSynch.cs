using System;
using UnityEngine;

// Token: 0x020001E4 RID: 484
public class PostVRRigPhysicsSynch : MonoBehaviour
{
	// Token: 0x06000C82 RID: 3202 RVA: 0x0004B88C File Offset: 0x00049A8C
	private void LateUpdate()
	{
		Physics.SyncTransforms();
	}
}
