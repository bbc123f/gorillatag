using System;
using UnityEngine;

// Token: 0x020001A0 RID: 416
public class XfToXfLine : MonoBehaviour
{
	// Token: 0x06000ABF RID: 2751 RVA: 0x000423D0 File Offset: 0x000405D0
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x000423DE File Offset: 0x000405DE
	private void Update()
	{
		this.lineRenderer.SetPosition(0, this.pt0.transform.position);
		this.lineRenderer.SetPosition(1, this.pt1.transform.position);
	}

	// Token: 0x04000D87 RID: 3463
	public Transform pt0;

	// Token: 0x04000D88 RID: 3464
	public Transform pt1;

	// Token: 0x04000D89 RID: 3465
	private LineRenderer lineRenderer;
}
