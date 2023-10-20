using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000238 RID: 568
public class ThrowableBugReliableState : MonoBehaviour, IRequestableOwnershipGuardCallbacks, IPunObservable
{
	// Token: 0x06000E0F RID: 3599 RVA: 0x00051E79 File Offset: 0x00050079
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.travelingDirection);
			return;
		}
		this.travelingDirection = (Vector3)stream.ReceiveNext();
	}

	// Token: 0x06000E10 RID: 3600 RVA: 0x00051EA6 File Offset: 0x000500A6
	public void OnOwnershipTransferred(Player toPlayer, Player fromPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000E11 RID: 3601 RVA: 0x00051EAD File Offset: 0x000500AD
	public bool OnOwnershipRequest(Player fromPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x00051EB4 File Offset: 0x000500B4
	public void OnMyOwnerLeft()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x00051EBB File Offset: 0x000500BB
	public bool OnMasterClientAssistedTakeoverRequest(Player fromPlayer, Player toPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x00051EC2 File Offset: 0x000500C2
	public void OnMyCreatorLeft()
	{
		throw new NotImplementedException();
	}

	// Token: 0x04001124 RID: 4388
	public Vector3 travelingDirection = Vector3.zero;
}
