using System;
using System.Collections;
using UnityEngine;

public abstract class TeleportOrientationHandler : TeleportSupport
{
	public enum OrientationModes
	{
		HeadRelative,
		ForwardFacing
	}

	private readonly Action _updateOrientationAction;

	private readonly Action<LocomotionTeleport.AimData> _updateAimDataAction;

	protected LocomotionTeleport.AimData AimData;

	protected TeleportOrientationHandler()
	{
		_updateOrientationAction = delegate
		{
			StartCoroutine(UpdateOrientationCoroutine());
		};
		_updateAimDataAction = UpdateAimData;
	}

	private void UpdateAimData(LocomotionTeleport.AimData aimData)
	{
		AimData = aimData;
	}

	protected override void AddEventHandlers()
	{
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateAim += _updateOrientationAction;
		base.LocomotionTeleport.UpdateAimData += _updateAimDataAction;
	}

	protected override void RemoveEventHandlers()
	{
		base.RemoveEventHandlers();
		base.LocomotionTeleport.EnterStateAim -= _updateOrientationAction;
		base.LocomotionTeleport.UpdateAimData -= _updateAimDataAction;
	}

	private IEnumerator UpdateOrientationCoroutine()
	{
		InitializeTeleportDestination();
		while (base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.Aim || base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.PreTeleport)
		{
			if (AimData != null)
			{
				UpdateTeleportDestination();
			}
			yield return null;
		}
	}

	protected abstract void InitializeTeleportDestination();

	protected abstract void UpdateTeleportDestination();

	protected Quaternion GetLandingOrientation(OrientationModes mode, Quaternion rotation)
	{
		if (mode != 0)
		{
			return rotation * Quaternion.Euler(0f, 0f - base.LocomotionTeleport.LocomotionController.CameraRig.trackingSpace.localEulerAngles.y, 0f);
		}
		return rotation;
	}
}
