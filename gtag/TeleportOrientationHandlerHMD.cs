using UnityEngine;

public class TeleportOrientationHandlerHMD : TeleportOrientationHandler
{
	[Tooltip("HeadRelative=Character will orient to match the arrow. ForwardFacing=When user orients to match the arrow, they will be facing the sensors.")]
	public OrientationModes OrientationMode;

	[Tooltip("Should the destination orientation be updated during the aim state in addition to the PreTeleport state?")]
	public bool UpdateOrientationDuringAim;

	[Tooltip("How far from the destination must the HMD be pointing before using it for orientation")]
	public float AimDistanceThreshold;

	[Tooltip("How far from the destination must the HMD be pointing before rejecting the teleport")]
	public float AimDistanceMaxRange;

	private Quaternion _initialRotation;

	protected override void InitializeTeleportDestination()
	{
		_initialRotation = Quaternion.identity;
	}

	protected override void UpdateTeleportDestination()
	{
		if (AimData.Destination.HasValue && (UpdateOrientationDuringAim || base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.PreTeleport))
		{
			Transform centerEyeAnchor = base.LocomotionTeleport.LocomotionController.CameraRig.centerEyeAnchor;
			Vector3 valueOrDefault = AimData.Destination.GetValueOrDefault();
			if (new Plane(Vector3.up, valueOrDefault).Raycast(new Ray(centerEyeAnchor.position, centerEyeAnchor.forward), out var enter))
			{
				Vector3 vector = centerEyeAnchor.position + centerEyeAnchor.forward * enter - valueOrDefault;
				vector.y = 0f;
				float magnitude = vector.magnitude;
				if (magnitude > AimDistanceThreshold)
				{
					vector.Normalize();
					Quaternion quaternion = (_initialRotation = Quaternion.LookRotation(new Vector3(vector.x, 0f, vector.z), Vector3.up));
					if (AimDistanceMaxRange > 0f && magnitude > AimDistanceMaxRange)
					{
						AimData.TargetValid = false;
					}
					base.LocomotionTeleport.OnUpdateTeleportDestination(AimData.TargetValid, AimData.Destination, quaternion, GetLandingOrientation(OrientationMode, quaternion));
					return;
				}
			}
		}
		base.LocomotionTeleport.OnUpdateTeleportDestination(AimData.TargetValid, AimData.Destination, _initialRotation, GetLandingOrientation(OrientationMode, _initialRotation));
	}
}
