using UnityEngine;

public class TeleportTargetHandlerNode : TeleportTargetHandler
{
	[Tooltip("When checking line of sight to the destination, add this value to the vertical offset for targeting collision checks.")]
	public float LOSOffset = 1f;

	[Tooltip("Teleport logic will only work with TeleportPoint components that exist in the layers specified by this mask.")]
	public LayerMask TeleportLayerMask;

	protected override bool ConsiderTeleport(Vector3 start, ref Vector3 end)
	{
		if (!base.LocomotionTeleport.AimCollisionTest(start, end, (int)AimCollisionLayerMask | (int)TeleportLayerMask, out AimData.TargetHitInfo))
		{
			return false;
		}
		TeleportPoint component = AimData.TargetHitInfo.collider.gameObject.GetComponent<TeleportPoint>();
		if (component == null)
		{
			return false;
		}
		Vector3 position = component.destTransform.position;
		Vector3 end2 = new Vector3(position.x, position.y + LOSOffset, position.z);
		if (base.LocomotionTeleport.AimCollisionTest(start, end2, (int)AimCollisionLayerMask & ~(int)TeleportLayerMask, out AimData.TargetHitInfo))
		{
			return false;
		}
		end = position;
		return true;
	}
}
