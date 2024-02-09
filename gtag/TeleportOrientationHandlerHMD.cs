using System;
using UnityEngine;

public class TeleportOrientationHandlerHMD : TeleportOrientationHandler
{
	protected override void InitializeTeleportDestination()
	{
		this._initialRotation = Quaternion.identity;
	}

	protected override void UpdateTeleportDestination()
	{
		if (this.AimData.Destination != null && (this.UpdateOrientationDuringAim || base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.PreTeleport))
		{
			Transform centerEyeAnchor = base.LocomotionTeleport.LocomotionController.CameraRig.centerEyeAnchor;
			Vector3 valueOrDefault = this.AimData.Destination.GetValueOrDefault();
			Plane plane = new Plane(Vector3.up, valueOrDefault);
			float num;
			if (plane.Raycast(new Ray(centerEyeAnchor.position, centerEyeAnchor.forward), out num))
			{
				Vector3 vector = centerEyeAnchor.position + centerEyeAnchor.forward * num - valueOrDefault;
				vector.y = 0f;
				float magnitude = vector.magnitude;
				if (magnitude > this.AimDistanceThreshold)
				{
					vector.Normalize();
					Quaternion quaternion = Quaternion.LookRotation(new Vector3(vector.x, 0f, vector.z), Vector3.up);
					this._initialRotation = quaternion;
					if (this.AimDistanceMaxRange > 0f && magnitude > this.AimDistanceMaxRange)
					{
						this.AimData.TargetValid = false;
					}
					base.LocomotionTeleport.OnUpdateTeleportDestination(this.AimData.TargetValid, this.AimData.Destination, new Quaternion?(quaternion), new Quaternion?(base.GetLandingOrientation(this.OrientationMode, quaternion)));
					return;
				}
			}
		}
		base.LocomotionTeleport.OnUpdateTeleportDestination(this.AimData.TargetValid, this.AimData.Destination, new Quaternion?(this._initialRotation), new Quaternion?(base.GetLandingOrientation(this.OrientationMode, this._initialRotation)));
	}

	[Tooltip("HeadRelative=Character will orient to match the arrow. ForwardFacing=When user orients to match the arrow, they will be facing the sensors.")]
	public TeleportOrientationHandler.OrientationModes OrientationMode;

	[Tooltip("Should the destination orientation be updated during the aim state in addition to the PreTeleport state?")]
	public bool UpdateOrientationDuringAim;

	[Tooltip("How far from the destination must the HMD be pointing before using it for orientation")]
	public float AimDistanceThreshold;

	[Tooltip("How far from the destination must the HMD be pointing before rejecting the teleport")]
	public float AimDistanceMaxRange;

	private Quaternion _initialRotation;
}
