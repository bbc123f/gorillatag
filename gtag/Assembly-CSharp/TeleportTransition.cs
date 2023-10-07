using System;

// Token: 0x02000096 RID: 150
public abstract class TeleportTransition : TeleportSupport
{
	// Token: 0x06000339 RID: 825 RVA: 0x0001374C File Offset: 0x0001194C
	protected override void AddEventHandlers()
	{
		base.LocomotionTeleport.EnterStateTeleporting += this.LocomotionTeleportOnEnterStateTeleporting;
		base.AddEventHandlers();
	}

	// Token: 0x0600033A RID: 826 RVA: 0x0001376C File Offset: 0x0001196C
	protected override void RemoveEventHandlers()
	{
		base.LocomotionTeleport.EnterStateTeleporting -= this.LocomotionTeleportOnEnterStateTeleporting;
		base.RemoveEventHandlers();
	}

	// Token: 0x0600033B RID: 827
	protected abstract void LocomotionTeleportOnEnterStateTeleporting();
}
