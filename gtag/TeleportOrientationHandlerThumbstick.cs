using UnityEngine;

public class TeleportOrientationHandlerThumbstick : TeleportOrientationHandler
{
	[Tooltip("HeadRelative=Character will orient to match the arrow. ForwardFacing=When user orients to match the arrow, they will be facing the sensors.")]
	public OrientationModes OrientationMode;

	[Tooltip("Which thumbstick is to be used for adjusting the teleport orientation. Supports LTouch, RTouch, or Touch for either.")]
	public OVRInput.Controller Thumbstick;

	[Tooltip("The orientation will only change if the thumbstick magnitude is above this value. This will usually be larger than the TeleportInputHandlerTouch.ThumbstickTeleportThreshold.")]
	public float RotateStickThreshold = 0.8f;

	private Quaternion _initialRotation;

	private Quaternion _currentRotation;

	private Vector2 _lastValidDirection;

	protected override void InitializeTeleportDestination()
	{
		_initialRotation = base.LocomotionTeleport.GetHeadRotationY();
		_currentRotation = _initialRotation;
		_lastValidDirection = default(Vector2);
	}

	protected override void UpdateTeleportDestination()
	{
		float num;
		Vector2 lastValidDirection;
		if (Thumbstick == OVRInput.Controller.Touch)
		{
			Vector2 vector = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
			Vector2 vector2 = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);
			float magnitude = vector.magnitude;
			float magnitude2 = vector2.magnitude;
			if (magnitude > magnitude2)
			{
				num = magnitude;
				lastValidDirection = vector;
			}
			else
			{
				num = magnitude2;
				lastValidDirection = vector2;
			}
		}
		else
		{
			lastValidDirection = ((Thumbstick != OVRInput.Controller.LTouch) ? OVRInput.Get(OVRInput.RawAxis2D.RThumbstick) : OVRInput.Get(OVRInput.RawAxis2D.LThumbstick));
			num = lastValidDirection.magnitude;
		}
		if (!AimData.TargetValid)
		{
			_lastValidDirection = default(Vector2);
		}
		if (num < RotateStickThreshold)
		{
			lastValidDirection = _lastValidDirection;
			num = lastValidDirection.magnitude;
			if (num < RotateStickThreshold)
			{
				_initialRotation = base.LocomotionTeleport.GetHeadRotationY();
				lastValidDirection.x = 0f;
				lastValidDirection.y = 1f;
			}
		}
		else
		{
			_lastValidDirection = lastValidDirection;
		}
		Quaternion rotation = base.LocomotionTeleport.LocomotionController.CameraRig.trackingSpace.rotation;
		if (num > RotateStickThreshold)
		{
			lastValidDirection /= num;
			Quaternion quaternion = _initialRotation * Quaternion.LookRotation(new Vector3(lastValidDirection.x, 0f, lastValidDirection.y), Vector3.up);
			_currentRotation = rotation * quaternion;
		}
		else
		{
			_currentRotation = rotation * base.LocomotionTeleport.GetHeadRotationY();
		}
		base.LocomotionTeleport.OnUpdateTeleportDestination(AimData.TargetValid, AimData.Destination, _currentRotation, GetLandingOrientation(OrientationMode, _currentRotation));
	}
}
