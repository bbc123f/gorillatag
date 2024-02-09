using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandedInputSelector : MonoBehaviour
{
	private void Start()
	{
		this.m_CameraRig = Object.FindObjectOfType<OVRCameraRig>();
		this.m_InputModule = Object.FindObjectOfType<OVRInputModule>();
	}

	private void Update()
	{
		if (OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
		{
			this.SetActiveController(OVRInput.Controller.LTouch);
			return;
		}
		this.SetActiveController(OVRInput.Controller.RTouch);
	}

	private void SetActiveController(OVRInput.Controller c)
	{
		Transform transform;
		if (c == OVRInput.Controller.LTouch)
		{
			transform = this.m_CameraRig.leftHandAnchor;
		}
		else
		{
			transform = this.m_CameraRig.rightHandAnchor;
		}
		this.m_InputModule.rayTransform = transform;
	}

	private OVRCameraRig m_CameraRig;

	private OVRInputModule m_InputModule;
}
