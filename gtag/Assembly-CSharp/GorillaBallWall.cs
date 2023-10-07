using System;
using UnityEngine;

// Token: 0x02000166 RID: 358
public class GorillaBallWall : MonoBehaviour
{
	// Token: 0x060008E0 RID: 2272 RVA: 0x00035F4F File Offset: 0x0003414F
	private void Awake()
	{
		if (GorillaBallWall.instance == null)
		{
			GorillaBallWall.instance = this;
			return;
		}
		if (GorillaBallWall.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x00035F83 File Offset: 0x00034183
	private void Update()
	{
	}

	// Token: 0x04000AEC RID: 2796
	public static volatile GorillaBallWall instance;
}
