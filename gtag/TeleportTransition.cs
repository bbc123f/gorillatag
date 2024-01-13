public abstract class TeleportTransition : TeleportSupport
{
	protected override void AddEventHandlers()
	{
		base.LocomotionTeleport.EnterStateTeleporting += LocomotionTeleportOnEnterStateTeleporting;
		base.AddEventHandlers();
	}

	protected override void RemoveEventHandlers()
	{
		base.LocomotionTeleport.EnterStateTeleporting -= LocomotionTeleportOnEnterStateTeleporting;
		base.RemoveEventHandlers();
	}

	protected abstract void LocomotionTeleportOnEnterStateTeleporting();
}
