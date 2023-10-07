using System;
using UnityEngine;

// Token: 0x0200018A RID: 394
public class GorillaUIParent : MonoBehaviour
{
	// Token: 0x06000A23 RID: 2595 RVA: 0x0003ED32 File Offset: 0x0003CF32
	private void Awake()
	{
		if (GorillaUIParent.instance == null)
		{
			GorillaUIParent.instance = this;
			return;
		}
		if (GorillaUIParent.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04000C8E RID: 3214
	public static volatile GorillaUIParent instance;
}
