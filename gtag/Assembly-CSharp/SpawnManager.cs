using System;
using UnityEngine;

// Token: 0x02000230 RID: 560
public class SpawnManager : MonoBehaviour
{
	// Token: 0x06000DDD RID: 3549 RVA: 0x00050857 File Offset: 0x0004EA57
	public Transform[] ChildrenXfs()
	{
		return base.transform.GetComponentsInChildren<Transform>();
	}
}
