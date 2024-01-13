using UnityEngine;
using UnityEngine.EventSystems;

public class HandedInputSelector : MonoBehaviour
{
	private OVRCameraRig m_CameraRig;

	private OVRInputModule m_InputModule;

	private void Start()
	{
		m_CameraRig = Object.FindObjectOfType<OVRCameraRig>();
		m_InputModule = Object.FindObjectOfType<OVRInputModule>();
	}

	private void Update()
	{
		if (OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
		{
			SetActiveController(OVRInput.Controller.LTouch);
		}
		else
		{
			SetActiveController(OVRInput.Controller.RTouch);
		}
	}

	private void SetActiveController(OVRInput.Controller c)
	{
		Transform rayTransform = ((c != OVRInput.Controller.LTouch) ? m_CameraRig.rightHandAnchor : m_CameraRig.leftHandAnchor);
		m_InputModule.rayTransform = rayTransform;
	}
}
