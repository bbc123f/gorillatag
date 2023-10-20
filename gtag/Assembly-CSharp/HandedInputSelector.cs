using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200007E RID: 126
public class HandedInputSelector : MonoBehaviour
{
	// Token: 0x06000284 RID: 644 RVA: 0x00010CF6 File Offset: 0x0000EEF6
	private void Start()
	{
		this.m_CameraRig = Object.FindObjectOfType<OVRCameraRig>();
		this.m_InputModule = Object.FindObjectOfType<OVRInputModule>();
	}

	// Token: 0x06000285 RID: 645 RVA: 0x00010D0E File Offset: 0x0000EF0E
	private void Update()
	{
		if (OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
		{
			this.SetActiveController(OVRInput.Controller.LTouch);
			return;
		}
		this.SetActiveController(OVRInput.Controller.RTouch);
	}

	// Token: 0x06000286 RID: 646 RVA: 0x00010D28 File Offset: 0x0000EF28
	private void SetActiveController(OVRInput.Controller c)
	{
		Transform rayTransform;
		if (c == OVRInput.Controller.LTouch)
		{
			rayTransform = this.m_CameraRig.leftHandAnchor;
		}
		else
		{
			rayTransform = this.m_CameraRig.rightHandAnchor;
		}
		this.m_InputModule.rayTransform = rayTransform;
	}

	// Token: 0x0400035D RID: 861
	private OVRCameraRig m_CameraRig;

	// Token: 0x0400035E RID: 862
	private OVRInputModule m_InputModule;
}
