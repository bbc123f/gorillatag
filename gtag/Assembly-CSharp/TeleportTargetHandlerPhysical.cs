using System;
using UnityEngine;

// Token: 0x02000095 RID: 149
public class TeleportTargetHandlerPhysical : TeleportTargetHandler
{
	// Token: 0x06000337 RID: 823 RVA: 0x000136D8 File Offset: 0x000118D8
	protected override bool ConsiderTeleport(Vector3 start, ref Vector3 end)
	{
		if (base.LocomotionTeleport.AimCollisionTest(start, end, this.AimCollisionLayerMask, out this.AimData.TargetHitInfo))
		{
			Vector3 normalized = (end - start).normalized;
			end = start + normalized * this.AimData.TargetHitInfo.distance;
			return true;
		}
		return false;
	}
}
