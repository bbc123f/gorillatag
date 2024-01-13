using UnityEngine;

public class LocomotionController : MonoBehaviour
{
	public OVRCameraRig CameraRig;

	public CapsuleCollider CharacterController;

	public SimpleCapsuleWithStickMovement PlayerController;

	private void Start()
	{
		if (CameraRig == null)
		{
			CameraRig = Object.FindObjectOfType<OVRCameraRig>();
		}
	}
}
