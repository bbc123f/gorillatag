using System;
using UnityEngine;

public class SelectivePassthroughExperience : MonoBehaviour
{
	private void Update()
	{
		Camera.main.depthTextureMode = DepthTextureMode.Depth;
		bool flag = OVRInput.GetActiveController() == OVRInput.Controller.LTouch || OVRInput.GetActiveController() == OVRInput.Controller.RTouch || OVRInput.GetActiveController() == OVRInput.Controller.Touch;
		this.leftMaskObject.SetActive(flag);
		this.rightMaskObject.SetActive(flag);
		if (flag)
		{
			Vector3 vector = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch) + OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) * Vector3.forward * 0.1f;
			Vector3 vector2 = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch) + OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward * 0.1f;
			this.leftMaskObject.transform.position = vector;
			this.rightMaskObject.transform.position = vector2;
			return;
		}
		if (OVRInput.GetActiveController() != OVRInput.Controller.LHand && OVRInput.GetActiveController() != OVRInput.Controller.RHand)
		{
			OVRInput.GetActiveController();
		}
	}

	public SelectivePassthroughExperience()
	{
	}

	public GameObject leftMaskObject;

	public GameObject rightMaskObject;
}
