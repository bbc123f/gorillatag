using System;
using System.Collections;
using System.Collections.Generic;
using GorillaExtensions;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200006D RID: 109
[RequireComponent(typeof(PhotonView))]
public class RequestableOwnershipGuard : MonoBehaviourPunCallbacks, ISelfValidator
{
	// Token: 0x0600020C RID: 524 RVA: 0x0000E1B4 File Offset: 0x0000C3B4
	private void SetViewToRequest()
	{
		base.GetComponent<PhotonView>().OwnershipTransfer = OwnershipOption.Request;
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x0600020D RID: 525 RVA: 0x0000E1C2 File Offset: 0x0000C3C2
	private new PhotonView photonView
	{
		get
		{
			if (this.photonViews == null)
			{
				return null;
			}
			if (this.photonViews.Length == 0)
			{
				return null;
			}
			return this.photonViews[0];
		}
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x0600020E RID: 526 RVA: 0x0000E1E1 File Offset: 0x0000C3E1
	[DevInspectorShow]
	public bool isTrulyMine
	{
		get
		{
			return object.Equals(this.actualOwner, PhotonNetwork.LocalPlayer);
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x0600020F RID: 527 RVA: 0x0000E1F3 File Offset: 0x0000C3F3
	public bool isMine
	{
		get
		{
			return object.Equals(this.currentOwner, PhotonNetwork.LocalPlayer);
		}
	}

	// Token: 0x06000210 RID: 528 RVA: 0x0000E205 File Offset: 0x0000C405
	private void BindPhotonViews()
	{
		this.photonViews = base.GetComponents<PhotonView>();
	}

	// Token: 0x06000211 RID: 529 RVA: 0x0000E213 File Offset: 0x0000C413
	public override void OnDisable()
	{
		base.OnDisable();
		RequestableOwnershipGaurdHandler.RemoveViews(this.photonViews, this);
		this.currentMasterClient = null;
		this.currentOwner = null;
		this.actualOwner = null;
		this.creator = PhotonNetwork.LocalPlayer;
		this.currentState = NetworkingState.IsOwner;
	}

	// Token: 0x06000212 RID: 530 RVA: 0x0000E250 File Offset: 0x0000C450
	public override void OnEnable()
	{
		base.OnEnable();
		if (this.autoRegister)
		{
			this.BindPhotonViews();
		}
		if (this.photonViews == null)
		{
			return;
		}
		RequestableOwnershipGaurdHandler.RegisterViews(this.photonViews, this);
		if (!PhotonNetwork.InRoom)
		{
			GorillaTagger.OnPlayerSpawned(delegate
			{
				this.SetOwnership(PhotonNetwork.LocalPlayer, false, false);
			});
			return;
		}
		this.currentMasterClient = PhotonNetwork.MasterClient;
		if (this.photonView.CreatorActorNr != PhotonNetwork.MasterClient.ActorNumber)
		{
			this.SetOwnership(PhotonNetwork.CurrentRoom.GetPlayer(this.photonView.CreatorActorNr, false), false, false);
			return;
		}
		if (this.PlayerHasAuthority(PhotonNetwork.LocalPlayer))
		{
			this.SetOwnership(PhotonNetwork.LocalPlayer, false, false);
			this.currentState = NetworkingState.IsOwner;
			return;
		}
		this.currentState = NetworkingState.IsBlindClient;
		this.SetOwnership(null, false, false);
		this.RequestTheCurrentOwnerFromAuthority();
	}

	// Token: 0x06000213 RID: 531 RVA: 0x0000E318 File Offset: 0x0000C518
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06000214 RID: 532 RVA: 0x0000E31F File Offset: 0x0000C51F
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		if (PhotonNetwork.InRoom && this.PlayerHasAuthority(PhotonNetwork.LocalPlayer))
		{
			this.photonView.RPC("SetOwnershipFromMasterClient", newPlayer, new object[]
			{
				this.currentOwner
			});
		}
	}

	// Token: 0x06000215 RID: 533 RVA: 0x0000E358 File Offset: 0x0000C558
	public override void OnPreLeavingRoom()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsBlindClient:
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			break;
		case NetworkingState.IsClient:
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
			{
				callback.OnMyOwnerLeft();
			});
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		this.SetOwnership(PhotonNetwork.LocalPlayer, false, false);
	}

	// Token: 0x06000216 RID: 534 RVA: 0x0000E3D7 File Offset: 0x0000C5D7
	public override void OnJoinedRoom()
	{
		this.currentMasterClient = PhotonNetwork.MasterClient;
		if (this.PlayerHasAuthority(PhotonNetwork.LocalPlayer))
		{
			this.SetOwnership(PhotonNetwork.LocalPlayer, false, false);
			this.currentState = NetworkingState.IsOwner;
			return;
		}
		this.currentState = NetworkingState.IsBlindClient;
		this.SetOwnership(null, false, false);
	}

	// Token: 0x06000217 RID: 535 RVA: 0x0000E418 File Offset: 0x0000C618
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			break;
		case NetworkingState.IsBlindClient:
			if (this.PlayerHasAuthority(PhotonNetwork.LocalPlayer))
			{
				this.SetOwnership(PhotonNetwork.LocalPlayer, false, false);
				return;
			}
			this.RequestTheCurrentOwnerFromAuthority();
			return;
		case NetworkingState.IsClient:
			if (this.creator != null && object.Equals(this.creator, otherPlayer))
			{
				this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
				{
					callback.OnMyCreatorLeft();
				});
			}
			if (object.Equals(this.actualOwner, otherPlayer))
			{
				this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
				{
					callback.OnMyOwnerLeft();
				});
				if (this.fallbackOwner != null)
				{
					this.SetOwnership(this.fallbackOwner, false, false);
					return;
				}
				this.SetOwnership(this.currentMasterClient, false, false);
				return;
			}
			break;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			if (this.creator != null && object.Equals(this.creator, otherPlayer))
			{
				this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
				{
					callback.OnMyCreatorLeft();
				});
			}
			if (this.currentState == NetworkingState.ForcefullyTakingOver && object.Equals(this.currentOwner, otherPlayer))
			{
				this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
				{
					callback.OnMyOwnerLeft();
				});
			}
			if (object.Equals(this.actualOwner, otherPlayer))
			{
				if (this.fallbackOwner != null)
				{
					this.SetOwnership(this.fallbackOwner, false, false);
					if (object.Equals(this.fallbackOwner, PhotonNetwork.LocalPlayer))
					{
						Action action = this.ownershipRequestAccepted;
						if (action == null)
						{
							return;
						}
						action();
						return;
					}
					else
					{
						Action action2 = this.ownershipDenied;
						if (action2 == null)
						{
							return;
						}
						action2();
						return;
					}
				}
				else if (object.Equals(this.currentMasterClient, PhotonNetwork.LocalPlayer))
				{
					Action action3 = this.ownershipRequestAccepted;
					if (action3 == null)
					{
						return;
					}
					action3();
					return;
				}
				else
				{
					Action action4 = this.ownershipDenied;
					if (action4 == null)
					{
						return;
					}
					action4();
					return;
				}
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06000218 RID: 536 RVA: 0x0000E634 File Offset: 0x0000C834
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsClient:
			if (this.actualOwner == null && this.currentMasterClient == null)
			{
				this.SetOwnership(newMasterClient, false, false);
			}
			break;
		case NetworkingState.IsBlindClient:
			if (object.Equals(newMasterClient, PhotonNetwork.LocalPlayer))
			{
				this.SetOwnership(PhotonNetwork.LocalPlayer, false, false);
			}
			else
			{
				this.RequestTheCurrentOwnerFromAuthority();
			}
			break;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		this.currentMasterClient = newMasterClient;
	}

	// Token: 0x06000219 RID: 537 RVA: 0x0000E6B8 File Offset: 0x0000C8B8
	[PunRPC]
	public void RequestCurrentOwnerFromAuthorityRPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestCurrentOwnerFromAuthorityRPC");
		if (!this.PlayerHasAuthority(PhotonNetwork.LocalPlayer))
		{
			return;
		}
		this.photonView.RPC("SetOwnershipFromMasterClient", info.Sender, new object[]
		{
			this.actualOwner
		});
	}

	// Token: 0x0600021A RID: 538 RVA: 0x0000E6F8 File Offset: 0x0000C8F8
	[PunRPC]
	public void TransferOwnershipFromToRPC([CanBeNull] Player player, string nonce, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "TransferOwnershipFromToRPC");
		if (!this.PlayerHasAuthority(PhotonNetwork.LocalPlayer) && this.photonView.OwnerActorNr != info.Sender.ActorNumber)
		{
			Player player2 = this.currentOwner;
			int? num = (player2 != null) ? new int?(player2.ActorNumber) : null;
			int actorNumber = info.Sender.ActorNumber;
			if (!(num.GetValueOrDefault() == actorNumber & num != null))
			{
				Player player3 = this.actualOwner;
				num = ((player3 != null) ? new int?(player3.ActorNumber) : null);
				actorNumber = info.Sender.ActorNumber;
				if (!(num.GetValueOrDefault() == actorNumber & num != null))
				{
					return;
				}
			}
		}
		if (this.currentOwner == null)
		{
			this.RequestTheCurrentOwnerFromAuthority();
			return;
		}
		if (this.currentOwner.ActorNumber != this.photonView.OwnerActorNr)
		{
			return;
		}
		if (this.actualOwner.ActorNumber == player.ActorNumber)
		{
			return;
		}
		switch (this.currentState)
		{
		case NetworkingState.IsClient:
			this.SetOwnership(player, false, false);
			return;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			if (this.ownershipRequestNonce == nonce)
			{
				this.ownershipRequestNonce = "";
				this.SetOwnership(PhotonNetwork.LocalPlayer, false, false);
				return;
			}
			this.actualOwner = player;
			return;
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			this.RequestTheCurrentOwnerFromAuthority();
			return;
		}
		throw new ArgumentOutOfRangeException();
	}

	// Token: 0x0600021B RID: 539 RVA: 0x0000E868 File Offset: 0x0000CA68
	[PunRPC]
	public void SetOwnershipFromMasterClient([CanBeNull] Player player, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SetOwnershipFromMasterClient");
		if (player == null)
		{
			return;
		}
		if (!this.PlayerHasAuthority(info.Sender))
		{
			GorillaNot.instance.SendReport("Sent an SetOwnershipFromMasterClient when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		NetworkingState networkingState;
		if (this.currentOwner == null)
		{
			networkingState = this.currentState;
			if (networkingState != NetworkingState.IsBlindClient)
			{
				int num = networkingState - NetworkingState.RequestingOwnershipWaitingForSight;
			}
		}
		networkingState = this.currentState;
		if (networkingState - NetworkingState.ForcefullyTakingOver <= 3 && object.Equals(player, PhotonNetwork.LocalPlayer))
		{
			Action action = this.ownershipRequestAccepted;
			if (action != null)
			{
				action();
			}
			this.SetOwnership(player, false, false);
			return;
		}
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsBlindClient:
		case NetworkingState.IsClient:
			this.SetOwnership(player, false, false);
			return;
		case NetworkingState.ForcefullyTakingOver:
			this.actualOwner = player;
			this.currentState = NetworkingState.ForcefullyTakingOver;
			return;
		case NetworkingState.RequestingOwnership:
			this.SetOwnership(PhotonNetwork.LocalPlayer, false, false);
			this.currentState = NetworkingState.RequestingOwnership;
			return;
		case NetworkingState.RequestingOwnershipWaitingForSight:
			this.SetOwnership(PhotonNetwork.LocalPlayer, false, false);
			this.currentState = NetworkingState.RequestingOwnership;
			this.ownershipRequestNonce = Guid.NewGuid().ToString();
			this.photonView.RPC("OwnershipRequested", this.actualOwner, new object[]
			{
				this.ownershipRequestNonce
			});
			return;
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			this.actualOwner = player;
			this.currentState = NetworkingState.ForcefullyTakingOver;
			this.ownershipRequestNonce = Guid.NewGuid().ToString();
			this.photonView.RPC("OwnershipRequested", this.actualOwner, new object[]
			{
				this.ownershipRequestNonce
			});
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0600021C RID: 540 RVA: 0x0000EA0C File Offset: 0x0000CC0C
	[PunRPC]
	public void OnMasterClientAssistedTakeoverRequest(string nonce, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "OnMasterClientAssistedTakeoverRequest");
		bool flag = true;
		using (List<IRequestableOwnershipGuardCallbacks>.Enumerator enumerator = this.callbacksList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.OnMasterClientAssistedTakeoverRequest(this.actualOwner, info.Sender))
				{
					flag = false;
				}
			}
		}
		if (!flag)
		{
			this.photonView.RPC("OwnershipRequestDenied", info.Sender, new object[]
			{
				nonce
			});
			return;
		}
		this.TransferOwnership(info.Sender, nonce);
	}

	// Token: 0x0600021D RID: 541 RVA: 0x0000EAAC File Offset: 0x0000CCAC
	[PunRPC]
	public void OwnershipRequested(string nonce, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "OwnershipRequested");
		bool flag = true;
		using (List<IRequestableOwnershipGuardCallbacks>.Enumerator enumerator = this.callbacksList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.OnOwnershipRequest(info.Sender))
				{
					flag = false;
				}
			}
		}
		if (!flag)
		{
			this.photonView.RPC("OwnershipRequestDenied", info.Sender, new object[]
			{
				nonce
			});
			return;
		}
		this.TransferOwnership(info.Sender, nonce);
	}

	// Token: 0x0600021E RID: 542 RVA: 0x0000EB44 File Offset: 0x0000CD44
	private void TransferOwnershipWithID(int id)
	{
		this.TransferOwnership(PhotonNetwork.CurrentRoom.GetPlayer(id, false), "");
	}

	// Token: 0x0600021F RID: 543 RVA: 0x0000EB60 File Offset: 0x0000CD60
	public void TransferOwnership(Player player, string Nonce = "")
	{
		if (this.photonView.IsMine)
		{
			this.SetOwnership(player, false, false);
			this.photonView.RPC("TransferOwnershipFromToRPC", RpcTarget.Others, new object[]
			{
				player,
				Nonce
			});
			return;
		}
		if (this.PlayerHasAuthority(PhotonNetwork.LocalPlayer))
		{
			this.SetOwnership(player, false, false);
			this.photonView.RPC("SetOwnershipFromMasterClient", RpcTarget.Others, new object[]
			{
				player
			});
			return;
		}
		Debug.LogError("Tried to transfer ownership when im not the owner or a master client");
	}

	// Token: 0x06000220 RID: 544 RVA: 0x0000EBDF File Offset: 0x0000CDDF
	public void RequestTheCurrentOwnerFromAuthority()
	{
		this.photonView.RPC("RequestCurrentOwnerFromAuthorityRPC", this.GetAuthoritativePlayer(), Array.Empty<object>());
	}

	// Token: 0x06000221 RID: 545 RVA: 0x0000EBFC File Offset: 0x0000CDFC
	protected void SetCurrentOwner(Player player)
	{
		if (player == null)
		{
			this.currentOwner = null;
		}
		else
		{
			this.currentOwner = player;
		}
		foreach (PhotonView photonView in this.photonViews)
		{
			if (player == null)
			{
				photonView.OwnerActorNr = -1;
				photonView.ControllerActorNr = -1;
			}
			else
			{
				photonView.OwnerActorNr = player.ActorNumber;
				photonView.ControllerActorNr = player.ActorNumber;
			}
		}
	}

	// Token: 0x06000222 RID: 546 RVA: 0x0000EC60 File Offset: 0x0000CE60
	protected internal void SetOwnership(Player player, bool isLocalOnly = false, bool dontPropigate = false)
	{
		if (!object.Equals(player, this.currentOwner) && !dontPropigate)
		{
			this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks actualOwner)
			{
				actualOwner.OnOwnershipTransferred(player, this.currentOwner);
			});
		}
		this.SetCurrentOwner(player);
		if (isLocalOnly)
		{
			return;
		}
		this.actualOwner = player;
		if (player == null)
		{
			return;
		}
		if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.currentState = NetworkingState.IsOwner;
			return;
		}
		this.currentState = NetworkingState.IsClient;
	}

	// Token: 0x06000223 RID: 547 RVA: 0x0000ECF9 File Offset: 0x0000CEF9
	public Player GetAuthoritativePlayer()
	{
		if (this.giveCreatorAbsoluteAuthority)
		{
			return this.creator;
		}
		return PhotonNetwork.MasterClient;
	}

	// Token: 0x06000224 RID: 548 RVA: 0x0000ED0F File Offset: 0x0000CF0F
	public bool PlayerHasAuthority(Player player)
	{
		return object.Equals(this.GetAuthoritativePlayer(), player);
	}

	// Token: 0x06000225 RID: 549 RVA: 0x0000ED20 File Offset: 0x0000CF20
	[PunRPC]
	public void OwnershipRequestDenied(string nonce, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "OwnershipRequestDenied");
		int actorNumber = info.Sender.ActorNumber;
		Player player = this.actualOwner;
		int? num = (player != null) ? new int?(player.ActorNumber) : null;
		if (!(actorNumber == num.GetValueOrDefault() & num != null) && !this.PlayerHasAuthority(info.Sender))
		{
			return;
		}
		if (this.attemptMasterAssistedTakeoverOnDeny)
		{
			this.ownershipRequestNonce = Guid.NewGuid().ToString();
			this.photonView.RPC("OnMasterClientAssistedTakeoverRequest", RpcTarget.MasterClient, new object[]
			{
				this.ownershipRequestNonce
			});
			this.attemptMasterAssistedTakeoverOnDeny = false;
			return;
		}
		Action action = this.ownershipDenied;
		if (action != null)
		{
			action();
		}
		this.ownershipDenied = null;
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsBlindClient:
		case NetworkingState.IsClient:
			return;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			this.currentState = NetworkingState.IsClient;
			this.SetOwnership(this.actualOwner, false, false);
			return;
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			this.photonView.RPC("OwnershipRequested", this.actualOwner, new object[]
			{
				this.ownershipRequestNonce
			});
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06000226 RID: 550 RVA: 0x0000EE53 File Offset: 0x0000D053
	public IEnumerator RequestTimeout()
	{
		Debug.Log(string.Format("Timeout request started...  {0} ", this.currentState));
		yield return new WaitForSecondsRealtime(2f);
		Debug.Log(string.Format("Timeout request ended! {0} ", this.currentState));
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsBlindClient:
		case NetworkingState.IsClient:
			break;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			this.currentState = NetworkingState.IsClient;
			this.SetOwnership(this.actualOwner, false, false);
			break;
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			this.photonView.RPC("OwnershipRequested", this.actualOwner, new object[]
			{
				this.ownershipRequestNonce
			});
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		yield break;
	}

	// Token: 0x06000227 RID: 551 RVA: 0x0000EE64 File Offset: 0x0000D064
	public void RequestOwnership(Action onRequestSuccess, Action onRequestFailed)
	{
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
			return;
		case NetworkingState.IsBlindClient:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			this.currentState = NetworkingState.RequestingOwnershipWaitingForSight;
			base.StartCoroutine("RequestTimeout");
			return;
		case NetworkingState.IsClient:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			this.ownershipRequestNonce = Guid.NewGuid().ToString();
			this.currentState = NetworkingState.RequestingOwnership;
			this.photonView.RPC("OwnershipRequested", this.actualOwner, new object[]
			{
				this.ownershipRequestNonce
			});
			base.StartCoroutine("RequestTimeout");
			return;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			base.StartCoroutine("RequestTimeout");
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06000228 RID: 552 RVA: 0x0000EF60 File Offset: 0x0000D160
	public void RequestOwnershipImmediately(Action onRequestFailed)
	{
		Debug.Log("WorldShareable RequestOwnershipImmediately");
		if (this.PlayerHasAuthority(PhotonNetwork.LocalPlayer))
		{
			this.RequestOwnershipImmediatelyWithGuaranteedAuthority();
			return;
		}
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		{
			bool inRoom = PhotonNetwork.InRoom;
			return;
		}
		case NetworkingState.IsBlindClient:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			this.currentState = NetworkingState.ForcefullyTakingOverWaitingForSight;
			this.SetOwnership(PhotonNetwork.LocalPlayer, true, false);
			this.RequestTheCurrentOwnerFromAuthority();
			return;
		case NetworkingState.IsClient:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			this.ownershipRequestNonce = Guid.NewGuid().ToString();
			this.currentState = NetworkingState.ForcefullyTakingOver;
			this.SetOwnership(PhotonNetwork.LocalPlayer, true, false);
			this.photonView.RPC("OwnershipRequested", this.actualOwner, new object[]
			{
				this.ownershipRequestNonce
			});
			base.StartCoroutine("RequestTimeout");
			return;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			this.currentState = NetworkingState.ForcefullyTakingOver;
			base.StartCoroutine("RequestTimeout");
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06000229 RID: 553 RVA: 0x0000F090 File Offset: 0x0000D290
	public void RequestOwnershipImmediatelyWithGuaranteedAuthority()
	{
		Debug.Log("WorldShareable RequestOwnershipImmediatelyWithGuaranteedAuthority");
		if (!this.PlayerHasAuthority(PhotonNetwork.LocalPlayer))
		{
			Debug.LogError("Tried to request ownership immediately with guaranteed authority without acutely having authority ");
		}
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
			return;
		case NetworkingState.IsBlindClient:
			this.currentState = NetworkingState.ForcefullyTakingOverWaitingForSight;
			this.SetOwnership(PhotonNetwork.LocalPlayer, true, false);
			this.RequestTheCurrentOwnerFromAuthority();
			return;
		case NetworkingState.IsClient:
			this.currentState = NetworkingState.ForcefullyTakingOver;
			this.SetOwnership(PhotonNetwork.LocalPlayer, true, false);
			this.photonView.RPC("SetOwnershipFromMasterClient", RpcTarget.All, new object[]
			{
				PhotonNetwork.LocalPlayer
			});
			base.StartCoroutine("RequestTimeout");
			return;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			this.currentState = NetworkingState.ForcefullyTakingOver;
			base.StartCoroutine("RequestTimeout");
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0600022A RID: 554 RVA: 0x0000F156 File Offset: 0x0000D356
	public void Validate(SelfValidationResult result)
	{
	}

	// Token: 0x0600022B RID: 555 RVA: 0x0000F158 File Offset: 0x0000D358
	public void AddCallbackTarget(IRequestableOwnershipGuardCallbacks callbackObject)
	{
		if (!this.callbacksList.Contains(callbackObject))
		{
			this.callbacksList.Add(callbackObject);
			if (this.currentOwner != null)
			{
				callbackObject.OnOwnershipTransferred(this.currentOwner, null);
			}
		}
	}

	// Token: 0x0600022C RID: 556 RVA: 0x0000F189 File Offset: 0x0000D389
	public void RemoveCallbackTarget(IRequestableOwnershipGuardCallbacks callbackObject)
	{
		if (this.callbacksList.Contains(callbackObject))
		{
			this.callbacksList.Remove(callbackObject);
			if (this.currentOwner != null)
			{
				callbackObject.OnOwnershipTransferred(null, this.currentOwner);
			}
		}
	}

	// Token: 0x0600022D RID: 557 RVA: 0x0000F1BB File Offset: 0x0000D3BB
	public void SetCreator(Player player)
	{
		this.creator = player;
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x0600022E RID: 558 RVA: 0x0000F1C4 File Offset: 0x0000D3C4
	private NetworkingState EdCurrentState
	{
		get
		{
			return this.currentState;
		}
	}

	// Token: 0x040002D0 RID: 720
	[DevInspectorShow]
	[DevInspectorColor("#ff5")]
	public NetworkingState currentState;

	// Token: 0x040002D1 RID: 721
	[FormerlySerializedAs("photonView")]
	[SerializeField]
	private PhotonView[] photonViews;

	// Token: 0x040002D2 RID: 722
	[DevInspectorHide]
	[SerializeField]
	private bool autoRegister = true;

	// Token: 0x040002D3 RID: 723
	[DevInspectorShow]
	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	public Player currentOwner;

	// Token: 0x040002D4 RID: 724
	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	private Player currentMasterClient;

	// Token: 0x040002D5 RID: 725
	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	private Player fallbackOwner;

	// Token: 0x040002D6 RID: 726
	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	public Player creator;

	// Token: 0x040002D7 RID: 727
	public bool giveCreatorAbsoluteAuthority;

	// Token: 0x040002D8 RID: 728
	public bool attemptMasterAssistedTakeoverOnDeny;

	// Token: 0x040002D9 RID: 729
	private Action ownershipDenied;

	// Token: 0x040002DA RID: 730
	private Action ownershipRequestAccepted;

	// Token: 0x040002DB RID: 731
	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	[DevInspectorShow]
	public Player actualOwner;

	// Token: 0x040002DC RID: 732
	public string ownershipRequestNonce;

	// Token: 0x040002DD RID: 733
	public List<IRequestableOwnershipGuardCallbacks> callbacksList = new List<IRequestableOwnershipGuardCallbacks>();
}
