using System;
using UnityEngine;

public class SimpleCapsuleWithStickMovement : MonoBehaviour
{
	public bool EnableLinearMovement = true;

	public bool EnableRotation = true;

	public bool HMDRotatesPlayer = true;

	public bool RotationEitherThumbstick;

	public float RotationAngle = 45f;

	public float Speed;

	public OVRCameraRig CameraRig;

	private bool ReadyToSnapTurn;

	private Rigidbody _rigidbody;

	public event Action CameraUpdated;

	public event Action PreCharacterMove;

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		if (CameraRig == null)
		{
			CameraRig = GetComponentInChildren<OVRCameraRig>();
		}
	}

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		if (this.CameraUpdated != null)
		{
			this.CameraUpdated();
		}
		if (this.PreCharacterMove != null)
		{
			this.PreCharacterMove();
		}
		if (HMDRotatesPlayer)
		{
			RotatePlayerToHMD();
		}
		if (EnableLinearMovement)
		{
			StickMovement();
		}
		if (EnableRotation)
		{
			SnapTurn();
		}
	}

	private void RotatePlayerToHMD()
	{
		Transform trackingSpace = CameraRig.trackingSpace;
		Transform centerEyeAnchor = CameraRig.centerEyeAnchor;
		Vector3 position = trackingSpace.position;
		Quaternion rotation = trackingSpace.rotation;
		base.transform.rotation = Quaternion.Euler(0f, centerEyeAnchor.rotation.eulerAngles.y, 0f);
		trackingSpace.position = position;
		trackingSpace.rotation = rotation;
	}

	private void StickMovement()
	{
		Vector3 eulerAngles = CameraRig.centerEyeAnchor.rotation.eulerAngles;
		eulerAngles.z = (eulerAngles.x = 0f);
		Quaternion quaternion = Quaternion.Euler(eulerAngles);
		Vector3 zero = Vector3.zero;
		Vector2 vector = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
		zero += quaternion * (vector.x * Vector3.right);
		zero += quaternion * (vector.y * Vector3.forward);
		_rigidbody.MovePosition(_rigidbody.position + zero * Speed * Time.fixedDeltaTime);
	}

	private void SnapTurn()
	{
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) || (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft)))
		{
			if (ReadyToSnapTurn)
			{
				eulerAngles.y -= RotationAngle;
				ReadyToSnapTurn = false;
			}
		}
		else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) || (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)))
		{
			if (ReadyToSnapTurn)
			{
				eulerAngles.y += RotationAngle;
				ReadyToSnapTurn = false;
			}
		}
		else
		{
			ReadyToSnapTurn = true;
		}
		base.transform.rotation = Quaternion.Euler(eulerAngles);
	}
}
