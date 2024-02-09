using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

internal class RequestableOwnershipGaurdHandler : IPunOwnershipCallbacks, IInRoomCallbacks
{
	static RequestableOwnershipGaurdHandler()
	{
		PhotonNetwork.AddCallbackTarget(RequestableOwnershipGaurdHandler.callbackInstance);
	}

	internal static void RegisterView(PhotonView view, RequestableOwnershipGuard guard)
	{
		if (view == null || RequestableOwnershipGaurdHandler.gaurdedViews.Contains(view))
		{
			return;
		}
		RequestableOwnershipGaurdHandler.gaurdedViews.Add(view);
		RequestableOwnershipGaurdHandler.guardingLookup.Add(view, guard);
	}

	internal static void RemoveView(PhotonView view)
	{
		if (view == null)
		{
			return;
		}
		RequestableOwnershipGaurdHandler.gaurdedViews.Remove(view);
		RequestableOwnershipGaurdHandler.guardingLookup.Remove(view);
	}

	internal static void RegisterViews(PhotonView[] views, RequestableOwnershipGuard guard)
	{
		for (int i = 0; i < views.Length; i++)
		{
			RequestableOwnershipGaurdHandler.RegisterView(views[i], guard);
		}
	}

	public static void RemoveViews(PhotonView[] views, RequestableOwnershipGuard guard)
	{
		for (int i = 0; i < views.Length; i++)
		{
			RequestableOwnershipGaurdHandler.RemoveView(views[i]);
		}
	}

	void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
		if (!RequestableOwnershipGaurdHandler.gaurdedViews.Contains(targetView))
		{
			return;
		}
		RequestableOwnershipGuard requestableOwnershipGuard;
		if (!RequestableOwnershipGaurdHandler.guardingLookup.TryGetValue(targetView, out requestableOwnershipGuard))
		{
			return;
		}
		if (!object.Equals(previousOwner, requestableOwnershipGuard.currentOwner))
		{
			Debug.LogError("Ownership transferred but the previous owner didn't initiate the request, Switching back");
			targetView.OwnerActorNr = requestableOwnershipGuard.currentOwner.ActorNumber;
			targetView.ControllerActorNr = requestableOwnershipGuard.currentOwner.ActorNumber;
		}
	}

	void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	void IPunOwnershipCallbacks.OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	public void OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	public void OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		foreach (PhotonView photonView in RequestableOwnershipGaurdHandler.gaurdedViews)
		{
			RequestableOwnershipGuard requestableOwnershipGuard;
			if (!RequestableOwnershipGaurdHandler.guardingLookup.TryGetValue(photonView, out requestableOwnershipGuard))
			{
				break;
			}
			if (photonView.Owner == null)
			{
				Debug.LogError("OnMasterClientSwitched changed the photon view without our permission. view Owner is null");
				break;
			}
			if (requestableOwnershipGuard.currentOwner == null)
			{
				Debug.LogError("OnMasterClientSwitched changed the photon view without our permission. current Owner is null");
				break;
			}
			if (!object.Equals(photonView.Owner, requestableOwnershipGuard.currentOwner))
			{
				photonView.OwnerActorNr = requestableOwnershipGuard.currentOwner.ActorNumber;
				photonView.ControllerActorNr = requestableOwnershipGuard.currentOwner.ActorNumber;
			}
		}
	}

	private static HashSet<PhotonView> gaurdedViews = new HashSet<PhotonView>();

	private static readonly RequestableOwnershipGaurdHandler callbackInstance = new RequestableOwnershipGaurdHandler();

	private static Dictionary<PhotonView, RequestableOwnershipGuard> guardingLookup = new Dictionary<PhotonView, RequestableOwnershipGuard>();
}
