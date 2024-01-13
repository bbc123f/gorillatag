using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class TeleportTargetHandlerNavMesh : TeleportTargetHandler
{
	public int NavMeshAreaMask = -1;

	private NavMeshPath _path;

	private void Awake()
	{
		_path = new NavMeshPath();
	}

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

	public override Vector3? ConsiderDestination(Vector3 location)
	{
		Vector3? result = base.ConsiderDestination(location);
		if (result.HasValue)
		{
			Vector3 characterPosition = base.LocomotionTeleport.GetCharacterPosition();
			Vector3 valueOrDefault = result.GetValueOrDefault();
			NavMesh.CalculatePath(characterPosition, valueOrDefault, NavMeshAreaMask, _path);
			if (_path.status == NavMeshPathStatus.PathComplete)
			{
				return result;
			}
		}
		return null;
	}

	[Conditional("SHOW_PATH_RESULT")]
	private void OnDrawGizmos()
	{
	}
}
