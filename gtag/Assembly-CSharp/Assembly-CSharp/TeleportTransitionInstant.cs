using System;

public class TeleportTransitionInstant : TeleportTransition
{
	protected override void LocomotionTeleportOnEnterStateTeleporting()
	{
		base.LocomotionTeleport.DoTeleport();
	}
}
