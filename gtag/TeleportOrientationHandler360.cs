using System;

public class TeleportOrientationHandler360 : TeleportOrientationHandler
{
	protected override void InitializeTeleportDestination()
	{
	}

	protected override void UpdateTeleportDestination()
	{
		base.LocomotionTeleport.OnUpdateTeleportDestination(this.AimData.TargetValid, this.AimData.Destination, null, null);
	}
}
