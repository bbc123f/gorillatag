using System;
using UnityEngine;

// Token: 0x020000B6 RID: 182
public class PassthroughProjectionSurface : MonoBehaviour
{
	// Token: 0x06000404 RID: 1028 RVA: 0x0001ABBC File Offset: 0x00018DBC
	private void Start()
	{
		GameObject gameObject = GameObject.Find("OVRCameraRig");
		if (gameObject == null)
		{
			Debug.LogError("Scene does not contain an OVRCameraRig");
			return;
		}
		this.passthroughLayer = gameObject.GetComponent<OVRPassthroughLayer>();
		if (this.passthroughLayer == null)
		{
			Debug.LogError("OVRCameraRig does not contain an OVRPassthroughLayer component");
		}
		this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, true);
		this.quadOutline = this.projectionObject.GetComponent<MeshRenderer>();
		this.quadOutline.enabled = false;
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x0001AC40 File Offset: 0x00018E40
	private void Update()
	{
		if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.Active))
		{
			this.passthroughLayer.RemoveSurfaceGeometry(this.projectionObject.gameObject);
			this.quadOutline.enabled = true;
		}
		if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.Active))
		{
			OVRInput.Controller controllerType = OVRInput.Controller.RTouch;
			base.transform.position = OVRInput.GetLocalControllerPosition(controllerType);
			base.transform.rotation = OVRInput.GetLocalControllerRotation(controllerType);
		}
		if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.Active))
		{
			this.passthroughLayer.AddSurfaceGeometry(this.projectionObject.gameObject, false);
			this.quadOutline.enabled = false;
		}
	}

	// Token: 0x040004B2 RID: 1202
	private OVRPassthroughLayer passthroughLayer;

	// Token: 0x040004B3 RID: 1203
	public MeshFilter projectionObject;

	// Token: 0x040004B4 RID: 1204
	private MeshRenderer quadOutline;
}
