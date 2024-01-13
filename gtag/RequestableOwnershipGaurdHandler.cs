using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

internal class RequestableOwnershipGaurdHandler : IPunOwnershipCallbacks, IInRoomCallbacks
{
	private static HashSet<PhotonView> gaurdedViews;

	private static readonly RequestableOwnershipGaurdHandler callbackInstance;

	private static Dictionary<PhotonView, RequestableOwnershipGuard> guardingLookup;

	static RequestableOwnershipGaurdHandler()
	{
		gaurdedViews = new HashSet<PhotonView>();
		callbackInstance = new RequestableOwnershipGaurdHandler();
		guardingLookup = new Dictionary<PhotonView, RequestableOwnershipGuard>();
		PhotonNetwork.AddCallbackTarget(callbackInstance);
	}

	internal static void RegisterView(PhotonView view, RequestableOwnershipGuard guard)
	{
		if (!(view == null) && !gaurdedViews.Contains(view))
		{
			gaurdedViews.Add(view);
			guardingLookup.Add(view, guard);
		}
	}

	internal static void RemoveView(PhotonView view)
	{
		if (!(view == null))
		{
			gaurdedViews.Remove(view);
			guardingLookup.Remove(view);
		}
	}

	internal static void RegisterViews(PhotonView[] views, RequestableOwnershipGuard guard)
	{
		for (int i = 0; i < views.Length; i++)
		{
			RegisterView(views[i], guard);
		}
	}

	public static void RemoveViews(PhotonView[] views, RequestableOwnershipGuard guard)
	{
		for (int i = 0; i < views.Length; i++)
		{
			RemoveView(views[i]);
		}
	}

	void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
		if (gaurdedViews.Contains(targetView) && guardingLookup.TryGetValue(targetView, out var value) && !object.Equals(previousOwner, value.currentOwner))
		{
			Debug.LogError("Ownership transferred but the previous owner didn't initiate the request, Switching back");
			targetView.OwnerActorNr = value.currentOwner.ActorNumber;
			targetView.ControllerActorNr = value.currentOwner.ActorNumber;
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
		foreach (PhotonView gaurdedView in gaurdedViews)
		{
			if (!guardingLookup.TryGetValue(gaurdedView, out var value))
			{
				break;
			}
			if (gaurdedView.Owner == null)
			{
				Debug.LogError("OnMasterClientSwitched changed the photon view without our permission. view Owner is null");
				break;
			}
			if (value.currentOwner == null)
			{
				Debug.LogError("OnMasterClientSwitched changed the photon view without our permission. current Owner is null");
				break;
			}
			if (!object.Equals(gaurdedView.Owner, value.currentOwner))
			{
				gaurdedView.OwnerActorNr = value.currentOwner.ActorNumber;
				gaurdedView.ControllerActorNr = value.currentOwner.ActorNumber;
			}
		}
	}
}
