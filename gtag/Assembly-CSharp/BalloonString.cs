using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D9 RID: 217
public class BalloonString : MonoBehaviour
{
	// Token: 0x060004EC RID: 1260 RVA: 0x0001F568 File Offset: 0x0001D768
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.vertices = new List<Vector3>(this.numSegments + 1);
		if (this.startPositionXf != null && this.endPositionXf != null)
		{
			this.vertices.Add(this.startPositionXf.position);
			int num = this.vertices.Count - 2;
			for (int i = 0; i < num; i++)
			{
				float t = (float)((i + 1) / (this.vertices.Count - 1));
				Vector3 item = Vector3.Lerp(this.startPositionXf.position, this.endPositionXf.position, t);
				this.vertices.Add(item);
			}
			this.vertices.Add(this.endPositionXf.position);
		}
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x0001F638 File Offset: 0x0001D838
	private void UpdateDynamics()
	{
		this.vertices[0] = this.startPositionXf.position;
		this.vertices[this.vertices.Count - 1] = this.endPositionXf.position;
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x0001F674 File Offset: 0x0001D874
	private void UpdateRenderPositions()
	{
		this.lineRenderer.SetPosition(0, this.startPositionXf.transform.position);
		this.lineRenderer.SetPosition(1, this.endPositionXf.transform.position);
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x0001F6AE File Offset: 0x0001D8AE
	private void LateUpdate()
	{
		if (this.startPositionXf != null && this.endPositionXf != null)
		{
			this.UpdateDynamics();
			this.UpdateRenderPositions();
		}
	}

	// Token: 0x040005B0 RID: 1456
	public Transform startPositionXf;

	// Token: 0x040005B1 RID: 1457
	public Transform endPositionXf;

	// Token: 0x040005B2 RID: 1458
	private List<Vector3> vertices;

	// Token: 0x040005B3 RID: 1459
	public int numSegments = 1;

	// Token: 0x040005B4 RID: 1460
	private bool endPositionFixed;

	// Token: 0x040005B5 RID: 1461
	private LineRenderer lineRenderer;
}
