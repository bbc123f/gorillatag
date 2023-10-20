using System;
using UnityEngine;

// Token: 0x020001E5 RID: 485
public class PostVRRigPhysicsSynch : MonoBehaviour
{
	// Token: 0x06000C88 RID: 3208 RVA: 0x0004BAF4 File Offset: 0x00049CF4
	private void LateUpdate()
	{
		Physics.SyncTransforms();
	}
}
