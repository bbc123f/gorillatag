using System;
using UnityEngine;

// Token: 0x0200008F RID: 143
public class TeleportOrientationHandlerThumbstick : TeleportOrientationHandler
{
	// Token: 0x06000318 RID: 792 RVA: 0x00013108 File Offset: 0x00011308
	protected override void InitializeTeleportDestination()
	{
		this._initialRotation = base.LocomotionTeleport.GetHeadRotationY();
		this._currentRotation = this._initialRotation;
		this._lastValidDirection = default(Vector2);
	}

	// Token: 0x06000319 RID: 793 RVA: 0x00013134 File Offset: 0x00011334
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
			Quaternion rhs = this._initialRotation * Quaternion.LookRotation(new Vector3(vector3.x, 0f, vector3.y), Vector3.up);
			this._currentRotation = rotation * rhs;
		}
		else
		{
			this._currentRotation = rotation * base.LocomotionTeleport.GetHeadRotationY();
		}
		base.LocomotionTeleport.OnUpdateTeleportDestination(this.AimData.TargetValid, this.AimData.Destination, new Quaternion?(this._currentRotation), new Quaternion?(base.GetLandingOrientation(this.OrientationMode, this._currentRotation)));
	}

	// Token: 0x040003CE RID: 974
	[Tooltip("HeadRelative=Character will orient to match the arrow. ForwardFacing=When user orients to match the arrow, they will be facing the sensors.")]
	public TeleportOrientationHandler.OrientationModes OrientationMode;

	// Token: 0x040003CF RID: 975
	[Tooltip("Which thumbstick is to be used for adjusting the teleport orientation. Supports LTouch, RTouch, or Touch for either.")]
	public OVRInput.Controller Thumbstick;

	// Token: 0x040003D0 RID: 976
	[Tooltip("The orientation will only change if the thumbstick magnitude is above this value. This will usually be larger than the TeleportInputHandlerTouch.ThumbstickTeleportThreshold.")]
	public float RotateStickThreshold = 0.8f;

	// Token: 0x040003D1 RID: 977
	private Quaternion _initialRotation;

	// Token: 0x040003D2 RID: 978
	private Quaternion _currentRotation;

	// Token: 0x040003D3 RID: 979
	private Vector2 _lastValidDirection;
}
