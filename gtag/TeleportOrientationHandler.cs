using System;
using System.Collections;
using UnityEngine;

public abstract class TeleportOrientationHandler : TeleportSupport
{
	protected TeleportOrientationHandler()
	{
		this._updateOrientationAction = delegate
		{
			base.StartCoroutine(this.UpdateOrientationCoroutine());
		};
		this._updateAimDataAction = new Action<LocomotionTeleport.AimData>(this.UpdateAimData);
	}

	private void UpdateAimData(LocomotionTeleport.AimData aimData)
	{
		this.AimData = aimData;
	}

	protected override void AddEventHandlers()
	{
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateAim += this._updateOrientationAction;
		base.LocomotionTeleport.UpdateAimData += this._updateAimDataAction;
	}

	protected override void RemoveEventHandlers()
	{
		base.RemoveEventHandlers();
		base.LocomotionTeleport.EnterStateAim -= this._updateOrientationAction;
		base.LocomotionTeleport.UpdateAimData -= this._updateAimDataAction;
	}

	private IEnumerator UpdateOrientationCoroutine()
	{
		this.InitializeTeleportDestination();
		while (base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.Aim || base.LocomotionTeleport.CurrentState == LocomotionTeleport.States.PreTeleport)
		{
			if (this.AimData != null)
			{
				this.UpdateTeleportDestination();
			}
			yield return null;
		}
		yield break;
	}

	protected abstract void InitializeTeleportDestination();

	protected abstract void UpdateTeleportDestination();

	protected Quaternion GetLandingOrientation(TeleportOrientationHandler.OrientationModes mode, Quaternion rotation)
	{
		if (mode != TeleportOrientationHandler.OrientationModes.HeadRelative)
		{
			return rotation * Quaternion.Euler(0f, -base.LocomotionTeleport.LocomotionController.CameraRig.trackingSpace.localEulerAngles.y, 0f);
		}
		return rotation;
	}

	private readonly Action _updateOrientationAction;

	private readonly Action<LocomotionTeleport.AimData> _updateAimDataAction;

	protected LocomotionTeleport.AimData AimData;

	public enum OrientationModes
	{
		HeadRelative,
		ForwardFacing
	}
}
