using System;
using UnityEngine;

// Token: 0x0200019F RID: 415
public class XfToXfLine : MonoBehaviour
{
	// Token: 0x06000ABA RID: 2746 RVA: 0x00042298 File Offset: 0x00040498
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x000422A6 File Offset: 0x000404A6
	private void Update()
	{
		this.lineRenderer.SetPosition(0, this.pt0.transform.position);
		this.lineRenderer.SetPosition(1, this.pt1.transform.position);
	}

	// Token: 0x04000D83 RID: 3459
	public Transform pt0;

	// Token: 0x04000D84 RID: 3460
	public Transform pt1;

	// Token: 0x04000D85 RID: 3461
	private LineRenderer lineRenderer;
}
