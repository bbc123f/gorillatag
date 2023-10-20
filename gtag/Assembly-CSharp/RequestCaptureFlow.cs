using System;
using UnityEngine;

// Token: 0x020000C1 RID: 193
public class RequestCaptureFlow : MonoBehaviour
{
	// Token: 0x06000436 RID: 1078 RVA: 0x0001B7F5 File Offset: 0x000199F5
	private void Start()
	{
		this._sceneManager = Object.FindObjectOfType<OVRSceneManager>();
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x0001B802 File Offset: 0x00019A02
	private void Update()
	{
		if (OVRInput.GetUp(this.RequestCaptureBtn, OVRInput.Controller.Active))
		{
			this._sceneManager.RequestSceneCapture();
		}
	}

	// Token: 0x040004E4 RID: 1252
	public OVRInput.Button RequestCaptureBtn = OVRInput.Button.Two;

	// Token: 0x040004E5 RID: 1253
	private OVRSceneManager _sceneManager;
}
