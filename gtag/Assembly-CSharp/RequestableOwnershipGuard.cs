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

[RequireComponent(typeof(PhotonView))]
public class RequestableOwnershipGuard : MonoBehaviourPunCallbacks, ISelfValidator
{
	private void SetViewToRequest()
	{
		base.GetComponent<PhotonView>().OwnershipTransfer = OwnershipOption.Request;
	}

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

	[DevInspectorShow]
	public bool isTrulyMine
	{
		get
		{
			return object.Equals(this.actualOwner, PhotonNetwork.LocalPlayer);
		}
	}

	public bool isMine
	{
		get
		{
			return object.Equals(this.currentOwner, PhotonNetwork.LocalPlayer);
		}
	}

	private void BindPhotonViews()
	{
		this.photonViews = base.GetComponents<PhotonView>();
	}

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

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		throw new NotImplementedException();
	}

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

	[PunRPC]
	public void OwnershipRequested(string nonce, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "OwnershipRequested");
		if (info.Sender == PhotonNetwork.LocalPlayer)
		{
			return;
		}
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

	private void TransferOwnershipWithID(int id)
	{
		this.TransferOwnership(PhotonNetwork.CurrentRoom.GetPlayer(id, false), "");
	}

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

	public void RequestTheCurrentOwnerFromAuthority()
	{
		this.photonView.RPC("RequestCurrentOwnerFromAuthorityRPC", this.GetAuthoritativePlayer(), Array.Empty<object>());
	}

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

	public Player GetAuthoritativePlayer()
	{
		if (this.giveCreatorAbsoluteAuthority)
		{
			return this.creator;
		}
		return PhotonNetwork.MasterClient;
	}

	public bool PlayerHasAuthority(Player player)
	{
		return object.Equals(this.GetAuthoritativePlayer(), player);
	}

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

	public void Validate(SelfValidationResult result)
	{
	}

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

	public void SetCreator(Player player)
	{
		this.creator = player;
	}

	private NetworkingState EdCurrentState
	{
		get
		{
			return this.currentState;
		}
	}

	[DevInspectorShow]
	[DevInspectorColor("#ff5")]
	public NetworkingState currentState;

	[FormerlySerializedAs("photonView")]
	[SerializeField]
	private PhotonView[] photonViews;

	[DevInspectorHide]
	[SerializeField]
	private bool autoRegister = true;

	[DevInspectorShow]
	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	public Player currentOwner;

	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	private Player currentMasterClient;

	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	private Player fallbackOwner;

	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	public Player creator;

	public bool giveCreatorAbsoluteAuthority;

	public bool attemptMasterAssistedTakeoverOnDeny;

	private Action ownershipDenied;

	private Action ownershipRequestAccepted;

	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	[DevInspectorShow]
	public Player actualOwner;

	public string ownershipRequestNonce;

	public List<IRequestableOwnershipGuardCallbacks> callbacksList = new List<IRequestableOwnershipGuardCallbacks>();
}
