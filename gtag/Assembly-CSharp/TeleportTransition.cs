using System;

// Token: 0x02000096 RID: 150
public abstract class TeleportTransition : TeleportSupport
{
	// Token: 0x06000339 RID: 825 RVA: 0x00013528 File Offset: 0x00011728
	protected override void AddEventHandlers()
	{
		base.LocomotionTeleport.EnterStateTeleporting += this.LocomotionTeleportOnEnterStateTeleporting;
		base.AddEventHandlers();
	}

	// Token: 0x0600033A RID: 826 RVA: 0x00013548 File Offset: 0x00011748
	protected override void RemoveEventHandlers()
	{
		base.LocomotionTeleport.EnterStateTeleporting -= this.LocomotionTeleportOnEnterStateTeleporting;
		base.RemoveEventHandlers();
	}

	// Token: 0x0600033B RID: 827
	protected abstract void LocomotionTeleportOnEnterStateTeleporting();
}
