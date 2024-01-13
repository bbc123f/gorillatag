public class TeleportOrientationHandler360 : TeleportOrientationHandler
{
	protected override void InitializeTeleportDestination()
	{
	}

	protected override void UpdateTeleportDestination()
	{
		base.LocomotionTeleport.OnUpdateTeleportDestination(AimData.TargetValid, AimData.Destination, null, null);
	}
}
