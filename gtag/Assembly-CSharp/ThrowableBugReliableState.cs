using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000237 RID: 567
public class ThrowableBugReliableState : MonoBehaviour, IRequestableOwnershipGuardCallbacks, IPunObservable
{
	// Token: 0x06000E08 RID: 3592 RVA: 0x00051A9D File Offset: 0x0004FC9D
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.travelingDirection);
			return;
		}
		this.travelingDirection = (Vector3)stream.ReceiveNext();
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x00051ACA File Offset: 0x0004FCCA
	public void OnOwnershipTransferred(Player toPlayer, Player fromPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000E0A RID: 3594 RVA: 0x00051AD1 File Offset: 0x0004FCD1
	public bool OnOwnershipRequest(Player fromPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x00051AD8 File Offset: 0x0004FCD8
	public void OnMyOwnerLeft()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000E0C RID: 3596 RVA: 0x00051ADF File Offset: 0x0004FCDF
	public bool OnMasterClientAssistedTakeoverRequest(Player fromPlayer, Player toPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x00051AE6 File Offset: 0x0004FCE6
	public void OnMyCreatorLeft()
	{
		throw new NotImplementedException();
	}

	// Token: 0x0400111E RID: 4382
	public Vector3 travelingDirection = Vector3.zero;
}
