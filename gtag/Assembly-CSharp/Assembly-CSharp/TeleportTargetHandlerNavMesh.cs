using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class TeleportTargetHandlerNavMesh : TeleportTargetHandler
{
	private void Awake()
	{
		this._path = new NavMeshPath();
	}

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

	public override Vector3? ConsiderDestination(Vector3 location)
	{
		Vector3? result = base.ConsiderDestination(location);
		if (result != null)
		{
			Vector3 characterPosition = base.LocomotionTeleport.GetCharacterPosition();
			Vector3 valueOrDefault = result.GetValueOrDefault();
			NavMesh.CalculatePath(characterPosition, valueOrDefault, this.NavMeshAreaMask, this._path);
			if (this._path.status == NavMeshPathStatus.PathComplete)
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

	public int NavMeshAreaMask = -1;

	private NavMeshPath _path;
}
