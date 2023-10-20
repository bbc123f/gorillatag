using System;
using UnityEngine;

// Token: 0x020000BB RID: 187
public class SPPquad : MonoBehaviour
{
	// Token: 0x0600041C RID: 1052 RVA: 0x0001B0E8 File Offset: 0x000192E8
	private void Start()
	{
		this.passthroughLayer = base.GetComponent<OVRPassthroughLayer>();
		this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, false);
		if (base.GetComponent<GrabObject>())
		{
			GrabObject component = base.GetComponent<GrabObject>();
			component.GrabbedObjectDelegate = (GrabObject.GrabbedObject)Delegate.Combine(component.GrabbedObjectDelegate, new GrabObject.GrabbedObject(this.Grab));
			GrabObject component2 = base.GetComponent<GrabObject>();
			component2.ReleasedObjectDelegate = (GrabObject.ReleasedObject)Delegate.Combine(component2.ReleasedObjectDelegate, new GrabObject.ReleasedObject(this.Release));
		}
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x0001B173 File Offset: 0x00019373
	public void Grab(OVRInput.Controller grabHand)
	{
		this.passthroughLayer.RemoveSurfaceGeometry(this.projectionObject.gameObject);
		this.controllerHand = grabHand;
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x0001B192 File Offset: 0x00019392
	public void Release()
	{
		this.controllerHand = OVRInput.Controller.None;
		this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, false);
	}

	// Token: 0x040004C9 RID: 1225
	private OVRPassthroughLayer passthroughLayer;

	// Token: 0x040004CA RID: 1226
	public MeshFilter projectionObject;

	// Token: 0x040004CB RID: 1227
	private OVRInput.Controller controllerHand;
}
