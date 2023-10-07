using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200010F RID: 271
public class HoldableObject : MonoBehaviourPunCallbacks
{
	// Token: 0x060006A9 RID: 1705 RVA: 0x00029D27 File Offset: 0x00027F27
	public virtual void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x00029D29 File Offset: 0x00027F29
	public virtual void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x00029D2B File Offset: 0x00027F2B
	public virtual void DropItemCleanup()
	{
	}
}
