using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200010F RID: 271
public class HoldableObject : MonoBehaviourPunCallbacks
{
	// Token: 0x060006AA RID: 1706 RVA: 0x00029B67 File Offset: 0x00027D67
	public virtual void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x00029B69 File Offset: 0x00027D69
	public virtual void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x00029B6B File Offset: 0x00027D6B
	public virtual void DropItemCleanup()
	{
	}
}
