using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ThrowableBugReliableState : MonoBehaviour, IRequestableOwnershipGuardCallbacks, IPunObservable
{
	public Vector3 travelingDirection = Vector3.zero;

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(travelingDirection);
		}
		else
		{
			travelingDirection = (Vector3)stream.ReceiveNext();
		}
	}

	public void OnOwnershipTransferred(Player toPlayer, Player fromPlayer)
	{
		throw new NotImplementedException();
	}

	public bool OnOwnershipRequest(Player fromPlayer)
	{
		throw new NotImplementedException();
	}

	public void OnMyOwnerLeft()
	{
		throw new NotImplementedException();
	}

	public bool OnMasterClientAssistedTakeoverRequest(Player fromPlayer, Player toPlayer)
	{
		throw new NotImplementedException();
	}

	public void OnMyCreatorLeft()
	{
		throw new NotImplementedException();
	}
}
