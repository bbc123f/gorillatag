using System;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class GorillaUIParent : MonoBehaviour
{
	// Token: 0x06000A28 RID: 2600 RVA: 0x0003EE62 File Offset: 0x0003D062
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

	// Token: 0x04000C92 RID: 3218
	public static volatile GorillaUIParent instance;
}
