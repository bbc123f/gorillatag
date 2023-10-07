using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200008C RID: 140
public abstract class TeleportOrientationHandler : TeleportSupport
{
	// Token: 0x06000309 RID: 777 RVA: 0x00012E1C File Offset: 0x0001101C
	protected TeleportOrientationHandler()
	{
		this._updateOrientationAction = delegate()
		{
			base.StartCoroutine(this.UpdateOrientationCoroutine());
		};
		this._updateAimDataAction = new Action<LocomotionTeleport.AimData>(this.UpdateAimData);
	}

	// Token: 0x0600030A RID: 778 RVA: 0x00012E48 File Offset: 0x00011048
	private void UpdateAimData(LocomotionTeleport.AimData aimData)
	{
		this.AimData = aimData;
	}

	// Token: 0x0600030B RID: 779 RVA: 0x00012E51 File Offset: 0x00011051
	protected override void AddEventHandlers()
	{
		base.AddEventHandlers();
		base.LocomotionTeleport.EnterStateAim += this._updateOrientationAction;
		base.LocomotionTeleport.UpdateAimData += this._updateAimDataAction;
	}

	// Token: 0x0600030C RID: 780 RVA: 0x00012E7B File Offset: 0x0001107B
	protected override void RemoveEventHandlers()
	{
		base.RemoveEventHandlers();
		base.LocomotionTeleport.EnterStateAim -= this._updateOrientationAction;
		base.LocomotionTeleport.UpdateAimData -= this._updateAimDataAction;
	}

	// Token: 0x0600030D RID: 781 RVA: 0x00012EA5 File Offset: 0x000110A5
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

	// Token: 0x0600030E RID: 782
	protected abstract void InitializeTeleportDestination();

	// Token: 0x0600030F RID: 783
	protected abstract void UpdateTeleportDestination();

	// Token: 0x06000310 RID: 784 RVA: 0x00012EB4 File Offset: 0x000110B4
	protected Quaternion GetLandingOrientation(TeleportOrientationHandler.OrientationModes mode, Quaternion rotation)
	{
		if (mode != TeleportOrientationHandler.OrientationModes.HeadRelative)
		{
			return rotation * Quaternion.Euler(0f, -base.LocomotionTeleport.LocomotionController.CameraRig.trackingSpace.localEulerAngles.y, 0f);
		}
		return rotation;
	}

	// Token: 0x040003C6 RID: 966
	private readonly Action _updateOrientationAction;

	// Token: 0x040003C7 RID: 967
	private readonly Action<LocomotionTeleport.AimData> _updateAimDataAction;

	// Token: 0x040003C8 RID: 968
	protected LocomotionTeleport.AimData AimData;

	// Token: 0x020003BD RID: 957
	public enum OrientationModes
	{
		// Token: 0x04001BB2 RID: 7090
		HeadRelative,
		// Token: 0x04001BB3 RID: 7091
		ForwardFacing
	}
}
