using System;

public abstract class TeleportTransition : TeleportSupport
{
	protected override void AddEventHandlers()
	{
		base.LocomotionTeleport.EnterStateTeleporting += this.LocomotionTeleportOnEnterStateTeleporting;
		base.AddEventHandlers();
	}

	protected override void RemoveEventHandlers()
	{
		base.LocomotionTeleport.EnterStateTeleporting -= this.LocomotionTeleportOnEnterStateTeleporting;
		base.RemoveEventHandlers();
	}

	protected abstract void LocomotionTeleportOnEnterStateTeleporting();
}
