using System;

// Token: 0x02000098 RID: 152
public class TeleportTransitionInstant : TeleportTransition
{
	// Token: 0x06000340 RID: 832 RVA: 0x00013830 File Offset: 0x00011A30
	protected override void LocomotionTeleportOnEnterStateTeleporting()
	{
		base.LocomotionTeleport.DoTeleport();
	}
}
