using System;
using JetBrains.Annotations;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020000EF RID: 239
public class WorldTargetItem
{
	// Token: 0x0600057C RID: 1404 RVA: 0x00022A9F File Offset: 0x00020C9F
	public bool IsValid()
	{
		return this.itemIdx != -1 && this.owner != null;
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x00022AB8 File Offset: 0x00020CB8
	[CanBeNull]
	public static WorldTargetItem GenerateTargetFromPlayerAndID(Player owner, int itemIdx)
	{
		VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(owner);
		if (vrrig == null)
		{
			Debug.LogError("Tried to setup a sharable object but the target rig is null...");
			return null;
		}
		Transform component = vrrig.myBodyDockPositions.TransferrableItem(itemIdx).gameObject.GetComponent<Transform>();
		return new WorldTargetItem(owner, itemIdx, component);
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x00022B00 File Offset: 0x00020D00
	public static WorldTargetItem GenerateTargetFromWorldSharableItem(Player owner, int itemIdx, Transform transform)
	{
		return new WorldTargetItem(owner, itemIdx, transform);
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x00022B0A File Offset: 0x00020D0A
	private WorldTargetItem(Player owner, int itemIdx, Transform transform)
	{
		this.owner = owner;
		this.itemIdx = itemIdx;
		this.targetObject = transform;
		this.transferrableObject = transform.GetComponent<TransferrableObject>();
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x00022B33 File Offset: 0x00020D33
	public override string ToString()
	{
		return string.Format("Id: {0} ({1})", this.itemIdx, this.owner);
	}

	// Token: 0x0400067B RID: 1659
	public readonly Player owner;

	// Token: 0x0400067C RID: 1660
	public readonly int itemIdx;

	// Token: 0x0400067D RID: 1661
	public readonly Transform targetObject;

	// Token: 0x0400067E RID: 1662
	public readonly TransferrableObject transferrableObject;
}
