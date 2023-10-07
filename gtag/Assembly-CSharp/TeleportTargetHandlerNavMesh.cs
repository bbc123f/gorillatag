﻿using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000093 RID: 147
public class TeleportTargetHandlerNavMesh : TeleportTargetHandler
{
	// Token: 0x06000330 RID: 816 RVA: 0x000134FE File Offset: 0x000116FE
	private void Awake()
	{
		this._path = new NavMeshPath();
	}

	// Token: 0x06000331 RID: 817 RVA: 0x0001350C File Offset: 0x0001170C
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

	// Token: 0x06000332 RID: 818 RVA: 0x00013578 File Offset: 0x00011778
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

	// Token: 0x06000333 RID: 819 RVA: 0x000135D4 File Offset: 0x000117D4
	[Conditional("SHOW_PATH_RESULT")]
	private void OnDrawGizmos()
	{
	}

	// Token: 0x040003E0 RID: 992
	public int NavMeshAreaMask = -1;

	// Token: 0x040003E1 RID: 993
	private NavMeshPath _path;
}
