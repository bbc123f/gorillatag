using System;
using UnityEngine;

// Token: 0x02000231 RID: 561
public class SpawnManager : MonoBehaviour
{
	// Token: 0x06000DE4 RID: 3556 RVA: 0x00050C34 File Offset: 0x0004EE34
	public Transform[] ChildrenXfs()
	{
		return base.transform.GetComponentsInChildren<Transform>();
	}
}
