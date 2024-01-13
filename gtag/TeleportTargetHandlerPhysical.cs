using UnityEngine;

public class TeleportTargetHandlerPhysical : TeleportTargetHandler
{
	protected override bool ConsiderTeleport(Vector3 start, ref Vector3 end)
	{
		if (base.LocomotionTeleport.AimCollisionTest(start, end, AimCollisionLayerMask, out AimData.TargetHitInfo))
		{
			Vector3 normalized = (end - start).normalized;
			end = start + normalized * AimData.TargetHitInfo.distance;
			return true;
		}
		return false;
	}
}
