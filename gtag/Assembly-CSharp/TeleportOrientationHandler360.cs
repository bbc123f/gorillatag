using System;

// Token: 0x0200008D RID: 141
public class TeleportOrientationHandler360 : TeleportOrientationHandler
{
	// Token: 0x06000312 RID: 786 RVA: 0x00012CDB File Offset: 0x00010EDB
	protected override void InitializeTeleportDestination()
	{
	}

	// Token: 0x06000313 RID: 787 RVA: 0x00012CE0 File Offset: 0x00010EE0
	protected override void UpdateTeleportDestination()
	{
		base.LocomotionTeleport.OnUpdateTeleportDestination(this.AimData.TargetValid, this.AimData.Destination, null, null);
	}
}
