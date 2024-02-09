using System;
using UnityEngine;

public class TeleportOrientationHandlerThumbstick : TeleportOrientationHandler
{
	protected override void InitializeTeleportDestination()
	{
		this._initialRotation = base.LocomotionTeleport.GetHeadRotationY();
		this._currentRotation = this._initialRotation;
		this._lastValidDirection = default(Vector2);
	}

	protected override void UpdateTeleportDestination()
	{
		float num;
		Vector2 vector3;
		if (this.Thumbstick == OVRInput.Controller.Touch)
		{
			Vector2 vector = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, OVRInput.Controller.Active);
			Vector2 vector2 = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.Active);
			float magnitude = vector.magnitude;
			float magnitude2 = vector2.magnitude;
			if (magnitude > magnitude2)
			{
				num = magnitude;
				vector3 = vector;
			}
			else
			{
				num = magnitude2;
				vector3 = vector2;
			}
		}
		else
		{
			if (this.Thumbstick == OVRInput.Controller.LTouch)
			{
				vector3 = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, OVRInput.Controller.Active);
			}
			else
			{
				vector3 = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.Active);
			}
			num = vector3.magnitude;
		}
		if (!this.AimData.TargetValid)
		{
			this._lastValidDirection = default(Vector2);
		}
		if (num < this.RotateStickThreshold)
		{
			vector3 = this._lastValidDirection;
			num = vector3.magnitude;
			if (num < this.RotateStickThreshold)
			{
				this._initialRotation = base.LocomotionTeleport.GetHeadRotationY();
				vector3.x = 0f;
				vector3.y = 1f;
			}
		}
		else
		{
			this._lastValidDirection = vector3;
		}
		Quaternion rotation = base.LocomotionTeleport.LocomotionController.CameraRig.trackingSpace.rotation;
		if (num > this.RotateStickThreshold)
		{
			vector3 /= num;
			Quaternion quaternion = this._initialRotation * Quaternion.LookRotation(new Vector3(vector3.x, 0f, vector3.y), Vector3.up);
			this._currentRotation = rotation * quaternion;
		}
		else
		{
			this._currentRotation = rotation * base.LocomotionTeleport.GetHeadRotationY();
		}
		base.LocomotionTeleport.OnUpdateTeleportDestination(this.AimData.TargetValid, this.AimData.Destination, new Quaternion?(this._currentRotation), new Quaternion?(base.GetLandingOrientation(this.OrientationMode, this._currentRotation)));
	}

	[Tooltip("HeadRelative=Character will orient to match the arrow. ForwardFacing=When user orients to match the arrow, they will be facing the sensors.")]
	public TeleportOrientationHandler.OrientationModes OrientationMode;

	[Tooltip("Which thumbstick is to be used for adjusting the teleport orientation. Supports LTouch, RTouch, or Touch for either.")]
	public OVRInput.Controller Thumbstick;

	[Tooltip("The orientation will only change if the thumbstick magnitude is above this value. This will usually be larger than the TeleportInputHandlerTouch.ThumbstickTeleportThreshold.")]
	public float RotateStickThreshold = 0.8f;

	private Quaternion _initialRotation;

	private Quaternion _currentRotation;

	private Vector2 _lastValidDirection;
}
