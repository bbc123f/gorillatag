using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200006B RID: 107
internal class RequestableOwnershipGaurdHandler : IPunOwnershipCallbacks, IInRoomCallbacks
{
	// Token: 0x060001FE RID: 510 RVA: 0x0000DFAA File Offset: 0x0000C1AA
	static RequestableOwnershipGaurdHandler()
	{
		PhotonNetwork.AddCallbackTarget(RequestableOwnershipGaurdHandler.callbackInstance);
	}

	// Token: 0x060001FF RID: 511 RVA: 0x0000DFD4 File Offset: 0x0000C1D4
	internal static void RegisterView(PhotonView view, RequestableOwnershipGuard guard)
	{
		if (view == null || RequestableOwnershipGaurdHandler.gaurdedViews.Contains(view))
		{
			return;
		}
		RequestableOwnershipGaurdHandler.gaurdedViews.Add(view);
		RequestableOwnershipGaurdHandler.guardingLookup.Add(view, guard);
	}

	// Token: 0x06000200 RID: 512 RVA: 0x0000E005 File Offset: 0x0000C205
	internal static void RemoveView(PhotonView view)
	{
		if (view == null)
		{
			return;
		}
		RequestableOwnershipGaurdHandler.gaurdedViews.Remove(view);
		RequestableOwnershipGaurdHandler.guardingLookup.Remove(view);
	}

	// Token: 0x06000201 RID: 513 RVA: 0x0000E02C File Offset: 0x0000C22C
	internal static void RegisterViews(PhotonView[] views, RequestableOwnershipGuard guard)
	{
		for (int i = 0; i < views.Length; i++)
		{
			RequestableOwnershipGaurdHandler.RegisterView(views[i], guard);
		}
	}

	// Token: 0x06000202 RID: 514 RVA: 0x0000E054 File Offset: 0x0000C254
	public static void RemoveViews(PhotonView[] views, RequestableOwnershipGuard guard)
	{
		for (int i = 0; i < views.Length; i++)
		{
			RequestableOwnershipGaurdHandler.RemoveView(views[i]);
		}
	}

	// Token: 0x06000203 RID: 515 RVA: 0x0000E07C File Offset: 0x0000C27C
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

	// Token: 0x06000204 RID: 516 RVA: 0x0000E0E1 File Offset: 0x0000C2E1
	void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x06000205 RID: 517 RVA: 0x0000E0E3 File Offset: 0x0000C2E3
	void IPunOwnershipCallbacks.OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x06000206 RID: 518 RVA: 0x0000E0E5 File Offset: 0x0000C2E5
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06000207 RID: 519 RVA: 0x0000E0E7 File Offset: 0x0000C2E7
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x06000208 RID: 520 RVA: 0x0000E0E9 File Offset: 0x0000C2E9
	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06000209 RID: 521 RVA: 0x0000E0EB File Offset: 0x0000C2EB
	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x0600020A RID: 522 RVA: 0x0000E0F0 File Offset: 0x0000C2F0
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

	// Token: 0x040002C5 RID: 709
	private static HashSet<PhotonView> gaurdedViews = new HashSet<PhotonView>();

	// Token: 0x040002C6 RID: 710
	private static readonly RequestableOwnershipGaurdHandler callbackInstance = new RequestableOwnershipGaurdHandler();

	// Token: 0x040002C7 RID: 711
	private static Dictionary<PhotonView, RequestableOwnershipGuard> guardingLookup = new Dictionary<PhotonView, RequestableOwnershipGuard>();
}
