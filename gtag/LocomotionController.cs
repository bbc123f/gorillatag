using System;
using UnityEngine;

public class LocomotionController : MonoBehaviour
{
	private void Start()
	{
		if (this.CameraRig == null)
		{
			this.CameraRig = Object.FindObjectOfType<OVRCameraRig>();
		}
	}

	public OVRCameraRig CameraRig;

	public CapsuleCollider CharacterController;

	public SimpleCapsuleWithStickMovement PlayerController;
}
