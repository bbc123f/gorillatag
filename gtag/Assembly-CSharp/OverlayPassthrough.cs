using System;
using UnityEngine;

// Token: 0x020000B3 RID: 179
public class OverlayPassthrough : MonoBehaviour
{
	// Token: 0x060003F7 RID: 1015 RVA: 0x0001A730 File Offset: 0x00018930
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
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x0001A780 File Offset: 0x00018980
	private void Update()
	{
		if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
		{
			this.passthroughLayer.hidden = !this.passthroughLayer.hidden;
		}
		float x = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch).x;
		this.passthroughLayer.textureOpacity = x * 0.5f + 0.5f;
	}

	// Token: 0x040004A6 RID: 1190
	private OVRPassthroughLayer passthroughLayer;
}
