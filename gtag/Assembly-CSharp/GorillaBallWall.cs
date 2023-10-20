using System;
using UnityEngine;

// Token: 0x02000167 RID: 359
public class GorillaBallWall : MonoBehaviour
{
	// Token: 0x060008E4 RID: 2276 RVA: 0x00035E50 File Offset: 0x00034050
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

	// Token: 0x060008E5 RID: 2277 RVA: 0x00035E84 File Offset: 0x00034084
	private void Update()
	{
	}

	// Token: 0x04000AF0 RID: 2800
	public static volatile GorillaBallWall instance;
}
