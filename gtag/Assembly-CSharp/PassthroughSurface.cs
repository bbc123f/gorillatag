using System;
using UnityEngine;

// Token: 0x020000B8 RID: 184
public class PassthroughSurface : MonoBehaviour
{
	// Token: 0x06000415 RID: 1045 RVA: 0x0001B116 File Offset: 0x00019316
	private void Start()
	{
		Object.Destroy(this.projectionObject.GetComponent<MeshRenderer>());
		this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, true);
	}

	// Token: 0x040004C3 RID: 1219
	public OVRPassthroughLayer passthroughLayer;

	// Token: 0x040004C4 RID: 1220
	public MeshFilter projectionObject;
}
