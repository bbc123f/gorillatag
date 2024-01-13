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

	private new PhotonView photonView
	{
		get
		{
			if (photonViews == null)
			{
				return null;
			}
			if (photonViews.Length == 0)
			{
				return null;
			}
			return photonViews[0];
		}
	}

	[DevInspectorShow]
	public bool isTrulyMine => object.Equals(actualOwner, PhotonNetwork.LocalPlayer);

	public bool isMine => object.Equals(currentOwner, PhotonNetwork.LocalPlayer);

	private NetworkingState EdCurrentState => currentState;

	private void SetViewToRequest()
	{
		GetComponent<PhotonView>().OwnershipTransfer = OwnershipOption.Request;
	}

	private void BindPhotonViews()
	{
		photonViews = GetComponents<PhotonView>();
	}

	public override void OnDisable()
	{
		base.OnDisable();
		RequestableOwnershipGaurdHandler.RemoveViews(photonViews, this);
		currentMasterClient = null;
		currentOwner = null;
		actualOwner = null;
		creator = PhotonNetwork.LocalPlayer;
		currentState = NetworkingState.IsOwner;
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (autoRegister)
		{
			BindPhotonViews();
		}
		if (photonViews == null)
		{
			return;
		}
		RequestableOwnershipGaurdHandler.RegisterViews(photonViews, this);
		if (PhotonNetwork.InRoom)
		{
			currentMasterClient = PhotonNetwork.MasterClient;
			if (photonView.CreatorActorNr != PhotonNetwork.MasterClient.ActorNumber)
			{
				SetOwnership(PhotonNetwork.CurrentRoom.GetPlayer(photonView.CreatorActorNr));
			}
			else if (PlayerHasAuthority(PhotonNetwork.LocalPlayer))
			{
				SetOwnership(PhotonNetwork.LocalPlayer);
				currentState = NetworkingState.IsOwner;
			}
			else
			{
				currentState = NetworkingState.IsBlindClient;
				SetOwnership(null);
				RequestTheCurrentOwnerFromAuthority();
			}
		}
		else
		{
			GorillaTagger.OnPlayerSpawned(delegate
			{
				SetOwnership(PhotonNetwork.LocalPlayer);
			});
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		throw new NotImplementedException();
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Debug.Log($"Seeing if i can send Ownership to new player {PhotonNetwork.InRoom} {PhotonNetwork.LocalPlayer.IsMasterClient}");
		if (PhotonNetwork.InRoom && PlayerHasAuthority(PhotonNetwork.LocalPlayer))
		{
			Debug.Log("Sending Ownership to new player");
			photonView.RPC("SetOwnershipFromMasterClient", newPlayer, currentOwner);
		}
	}

	public override void OnPreLeavingRoom()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		Debug.Log("Pre leaving room", this);
		switch (currentState)
		{
		case NetworkingState.IsClient:
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
			{
				callback.OnMyOwnerLeft();
			});
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case NetworkingState.IsOwner:
		case NetworkingState.IsBlindClient:
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			break;
		}
		SetOwnership(PhotonNetwork.LocalPlayer);
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("Joined room, Setting self to blind client");
		currentMasterClient = PhotonNetwork.MasterClient;
		if (PlayerHasAuthority(PhotonNetwork.LocalPlayer))
		{
			SetOwnership(PhotonNetwork.LocalPlayer);
			currentState = NetworkingState.IsOwner;
		}
		else
		{
			currentState = NetworkingState.IsBlindClient;
			SetOwnership(null);
		}
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		switch (currentState)
		{
		case NetworkingState.IsClient:
			if (creator != null && object.Equals(creator, otherPlayer))
			{
				callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
				{
					callback.OnMyCreatorLeft();
				});
			}
			if (object.Equals(actualOwner, otherPlayer))
			{
				callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
				{
					callback.OnMyOwnerLeft();
				});
				if (fallbackOwner != null)
				{
					SetOwnership(fallbackOwner);
				}
				else
				{
					SetOwnership(currentMasterClient);
				}
			}
			break;
		case NetworkingState.IsBlindClient:
			if (PlayerHasAuthority(PhotonNetwork.LocalPlayer))
			{
				SetOwnership(PhotonNetwork.LocalPlayer);
			}
			else
			{
				RequestTheCurrentOwnerFromAuthority();
			}
			break;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			if (creator != null && object.Equals(creator, otherPlayer))
			{
				callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
				{
					callback.OnMyCreatorLeft();
				});
			}
			if (currentState == NetworkingState.ForcefullyTakingOver && object.Equals(currentOwner, otherPlayer))
			{
				callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
				{
					callback.OnMyOwnerLeft();
				});
			}
			if (!object.Equals(actualOwner, otherPlayer))
			{
				break;
			}
			if (fallbackOwner != null)
			{
				SetOwnership(fallbackOwner);
				if (object.Equals(fallbackOwner, PhotonNetwork.LocalPlayer))
				{
					ownershipRequestAccepted?.Invoke();
				}
				else
				{
					ownershipDenied?.Invoke();
				}
			}
			else if (object.Equals(currentMasterClient, PhotonNetwork.LocalPlayer))
			{
				ownershipRequestAccepted?.Invoke();
			}
			else
			{
				ownershipDenied?.Invoke();
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case NetworkingState.IsOwner:
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			break;
		}
	}

	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		switch (currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsClient:
			if (actualOwner == null && currentMasterClient == null)
			{
				SetOwnership(newMasterClient);
			}
			break;
		case NetworkingState.IsBlindClient:
			if (object.Equals(newMasterClient, PhotonNetwork.LocalPlayer))
			{
				SetOwnership(PhotonNetwork.LocalPlayer);
			}
			else
			{
				RequestTheCurrentOwnerFromAuthority();
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			break;
		}
		currentMasterClient = newMasterClient;
	}

	[PunRPC]
	public void RequestCurrentOwnerFromAuthorityRPC(PhotonMessageInfo info)
	{
		if (!PlayerHasAuthority(PhotonNetwork.LocalPlayer))
		{
			Debug.LogError("Got an assurance request but we arent the master client");
			return;
		}
		photonView.RPC("SetOwnershipFromMasterClient", info.Sender, actualOwner);
	}

	[PunRPC]
	public void TransferOwnershipFromToRPC([CanBeNull] Player player, string nonce, PhotonMessageInfo info)
	{
		Debug.Log($"TransferOwnershipFromToRPC player: {player.ActorNumber}, nonce: {nonce}, sender: {info.Sender.ActorNumber}");
		Debug.LogFormat("TransferOwnershipFromToRPC player: {0}, nonce: {1}, sender: {2}", player.ActorNumber, nonce, info.Sender.ActorNumber);
		if (!PlayerHasAuthority(PhotonNetwork.LocalPlayer) && photonView.OwnerActorNr != info.Sender.ActorNumber && currentOwner?.ActorNumber != info.Sender.ActorNumber && actualOwner?.ActorNumber != info.Sender.ActorNumber)
		{
			Debug.LogError("Got transfer request but the sender does not match the current owner");
			return;
		}
		if (currentOwner == null)
		{
			Debug.LogError("Tried to transfer ownership when the currentOwner is null. This happens if we just loaded in and haven't been told the current owner by the master client yet.");
			RequestTheCurrentOwnerFromAuthority();
			return;
		}
		if (currentOwner.ActorNumber != photonView.OwnerActorNr)
		{
			Debug.LogError("The stored actor number does not match the current actor number. Possible Attacker attempt or desync?");
			return;
		}
		if (actualOwner.ActorNumber == player.ActorNumber)
		{
			Debug.LogError("Tried to set the new owner to ourselves? This shouldn't happen.");
			return;
		}
		switch (currentState)
		{
		case NetworkingState.IsClient:
			SetOwnership(player);
			break;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			if (ownershipRequestNonce == nonce)
			{
				ownershipRequestNonce = "";
				SetOwnership(PhotonNetwork.LocalPlayer);
			}
			else
			{
				actualOwner = player;
			}
			break;
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			RequestTheCurrentOwnerFromAuthority();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	[PunRPC]
	public void SetOwnershipFromMasterClient([CanBeNull] Player player, PhotonMessageInfo info)
	{
		if (player == null)
		{
			Debug.LogError("SetOwnershipFromMasterClient: Player that was requested to be set is null");
			return;
		}
		Debug.LogFormat("SetOwnershipFromMasterClient player: {0}, sender: {1}", player.ActorNumber, info.Sender.ActorNumber);
		if (!PlayerHasAuthority(info.Sender))
		{
			Debug.LogError("Got Master client only request from a non masterclient, Ignoring.");
			GorillaNot.instance.SendReport("Sent an SetOwnershipFromMasterClient when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		NetworkingState networkingState;
		if (currentOwner == null)
		{
			networkingState = currentState;
			if (networkingState != NetworkingState.IsBlindClient)
			{
				_ = networkingState - 5;
				_ = 1;
			}
		}
		if (currentOwner != null && currentOwner?.ActorNumber != photonView.OwnerActorNr)
		{
			Debug.LogError("The stored actor number does not match the current actor number. Possible Attacker attempt or desync? Continuing regardless.\ncurrentOwner?.ActorNumber != photonView.OwnerActorNr\n" + $"{currentOwner?.ActorNumber} != {photonView.OwnerActorNr}", this);
		}
		if (object.Equals(actualOwner, player))
		{
			Debug.LogWarning("Tried to set the new owner to what it is already set too? This shouldn't happen. Continuing regardless", this);
		}
		networkingState = currentState;
		if ((uint)(networkingState - 3) <= 3u && object.Equals(player, PhotonNetwork.LocalPlayer))
		{
			ownershipRequestAccepted?.Invoke();
			SetOwnership(player);
			return;
		}
		switch (currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsBlindClient:
		case NetworkingState.IsClient:
			SetOwnership(player);
			break;
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			actualOwner = player;
			currentState = NetworkingState.ForcefullyTakingOver;
			ownershipRequestNonce = Guid.NewGuid().ToString();
			photonView.RPC("OwnershipRequested", actualOwner, ownershipRequestNonce);
			break;
		case NetworkingState.ForcefullyTakingOver:
			actualOwner = player;
			currentState = NetworkingState.ForcefullyTakingOver;
			break;
		case NetworkingState.RequestingOwnershipWaitingForSight:
			SetOwnership(PhotonNetwork.LocalPlayer);
			currentState = NetworkingState.RequestingOwnership;
			ownershipRequestNonce = Guid.NewGuid().ToString();
			photonView.RPC("OwnershipRequested", actualOwner, ownershipRequestNonce);
			break;
		case NetworkingState.RequestingOwnership:
			SetOwnership(PhotonNetwork.LocalPlayer);
			currentState = NetworkingState.RequestingOwnership;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	[PunRPC]
	public void OnMasterClientAssistedTakeoverRequest(string nonce, PhotonMessageInfo info)
	{
		Debug.Log($"OnMasterClientAssistedTakeoverRequest id: {info.Sender?.ActorNumber} nonce: {nonce}");
		bool flag = true;
		foreach (IRequestableOwnershipGuardCallbacks callbacks in callbacksList)
		{
			if (!callbacks.OnMasterClientAssistedTakeoverRequest(actualOwner, info.Sender))
			{
				flag = false;
			}
		}
		if (!flag)
		{
			photonView.RPC("OwnershipRequestDenied", info.Sender, nonce);
		}
		else
		{
			TransferOwnership(info.Sender, nonce);
			Debug.Log($"OnMasterClientAssistedTakeover: actualOwner: {actualOwner} refused to give up ownership, We are stealing ownership and giving it to {info.Sender}", this);
		}
	}

	[PunRPC]
	public void OwnershipRequested(string nonce, PhotonMessageInfo info)
	{
		Debug.Log($"OwnershipRequested id: {info.Sender?.ActorNumber} nonce: {nonce}");
		bool flag = true;
		foreach (IRequestableOwnershipGuardCallbacks callbacks in callbacksList)
		{
			if (!callbacks.OnOwnershipRequest(info.Sender))
			{
				flag = false;
			}
		}
		if (!flag)
		{
			photonView.RPC("OwnershipRequestDenied", info.Sender, nonce);
		}
		else
		{
			TransferOwnership(info.Sender, nonce);
		}
	}

	private void TransferOwnershipWithID(int id)
	{
		TransferOwnership(PhotonNetwork.CurrentRoom.GetPlayer(id));
	}

	public void TransferOwnership(Player player, string Nonce = "")
	{
		if (photonView.IsMine)
		{
			SetOwnership(player);
			photonView.RPC("TransferOwnershipFromToRPC", RpcTarget.Others, player, Nonce);
		}
		else if (PlayerHasAuthority(PhotonNetwork.LocalPlayer))
		{
			SetOwnership(player);
			photonView.RPC("SetOwnershipFromMasterClient", RpcTarget.Others, player);
		}
		else
		{
			Debug.LogError("Tried to transfer ownership when im not the owner or a master client");
		}
	}

	public void RequestTheCurrentOwnerFromAuthority()
	{
		photonView.RPC("RequestCurrentOwnerFromAuthorityRPC", GetAuthoritativePlayer());
	}

	protected void SetCurrentOwner(Player player)
	{
		if (player == null)
		{
			currentOwner = null;
		}
		else
		{
			currentOwner = player;
		}
		PhotonView[] array = photonViews;
		foreach (PhotonView photonView in array)
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
		if (!object.Equals(player, currentOwner) && !dontPropigate)
		{
			callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks actualOwner)
			{
				actualOwner.OnOwnershipTransferred(player, currentOwner);
			});
		}
		SetCurrentOwner(player);
		if (isLocalOnly)
		{
			return;
		}
		actualOwner = player;
		if (player != null)
		{
			if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				currentState = NetworkingState.IsOwner;
			}
			else
			{
				currentState = NetworkingState.IsClient;
			}
		}
	}

	public Player GetAuthoritativePlayer()
	{
		if (giveCreatorAbsoluteAuthority)
		{
			return creator;
		}
		return PhotonNetwork.MasterClient;
	}

	public bool PlayerHasAuthority(Player player)
	{
		return object.Equals(GetAuthoritativePlayer(), player);
	}

	[PunRPC]
	public void OwnershipRequestDenied(string nonce, PhotonMessageInfo info)
	{
		if (info.Sender.ActorNumber != actualOwner?.ActorNumber && !PlayerHasAuthority(info.Sender))
		{
			Debug.LogError("OwnershipRequestDenied came from not the current owner and not the master client");
			return;
		}
		Debug.Log($"OwnershipRequestDenied id: {info.Sender?.ActorNumber} nonce: {nonce}");
		if (attemptMasterAssistedTakeoverOnDeny)
		{
			ownershipRequestNonce = Guid.NewGuid().ToString();
			photonView.RPC("OnMasterClientAssistedTakeoverRequest", RpcTarget.MasterClient, ownershipRequestNonce);
			attemptMasterAssistedTakeoverOnDeny = false;
			return;
		}
		ownershipDenied?.Invoke();
		ownershipDenied = null;
		switch (currentState)
		{
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			currentState = NetworkingState.IsClient;
			SetOwnership(actualOwner);
			break;
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			photonView.RPC("OwnershipRequested", actualOwner, ownershipRequestNonce);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case NetworkingState.IsOwner:
		case NetworkingState.IsBlindClient:
		case NetworkingState.IsClient:
			break;
		}
	}

	public IEnumerator RequestTimeout()
	{
		Debug.Log($"Timeout request started...  {currentState} ");
		yield return new WaitForSecondsRealtime(2f);
		Debug.Log($"Timeout request ended! {currentState} ");
		switch (currentState)
		{
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			currentState = NetworkingState.IsClient;
			SetOwnership(actualOwner);
			break;
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			photonView.RPC("OwnershipRequested", actualOwner, ownershipRequestNonce);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case NetworkingState.IsOwner:
		case NetworkingState.IsBlindClient:
		case NetworkingState.IsClient:
			break;
		}
	}

	public void RequestOwnership(Action onRequestSuccess, Action onRequestFailed)
	{
		switch (currentState)
		{
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			ownershipDenied = (Action)Delegate.Combine(ownershipDenied, onRequestFailed);
			StartCoroutine("RequestTimeout");
			break;
		case NetworkingState.IsClient:
			ownershipDenied = (Action)Delegate.Combine(ownershipDenied, onRequestFailed);
			ownershipRequestNonce = Guid.NewGuid().ToString();
			currentState = NetworkingState.RequestingOwnership;
			photonView.RPC("OwnershipRequested", actualOwner, ownershipRequestNonce);
			StartCoroutine("RequestTimeout");
			break;
		case NetworkingState.IsBlindClient:
			ownershipDenied = (Action)Delegate.Combine(ownershipDenied, onRequestFailed);
			currentState = NetworkingState.RequestingOwnershipWaitingForSight;
			StartCoroutine("RequestTimeout");
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case NetworkingState.IsOwner:
			break;
		}
	}

	public void RequestOwnershipImmediately(Action onRequestFailed)
	{
		Debug.Log("WorldShareable RequestOwnershipImmediately");
		if (PlayerHasAuthority(PhotonNetwork.LocalPlayer))
		{
			RequestOwnershipImmediatelyWithGuaranteedAuthority();
			return;
		}
		switch (currentState)
		{
		case NetworkingState.IsOwner:
			_ = PhotonNetwork.InRoom;
			break;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			ownershipDenied = (Action)Delegate.Combine(ownershipDenied, onRequestFailed);
			currentState = NetworkingState.ForcefullyTakingOver;
			StartCoroutine("RequestTimeout");
			break;
		case NetworkingState.IsClient:
			ownershipDenied = (Action)Delegate.Combine(ownershipDenied, onRequestFailed);
			ownershipRequestNonce = Guid.NewGuid().ToString();
			currentState = NetworkingState.ForcefullyTakingOver;
			SetOwnership(PhotonNetwork.LocalPlayer, isLocalOnly: true);
			photonView.RPC("OwnershipRequested", actualOwner, ownershipRequestNonce);
			StartCoroutine("RequestTimeout");
			break;
		case NetworkingState.IsBlindClient:
			ownershipDenied = (Action)Delegate.Combine(ownershipDenied, onRequestFailed);
			currentState = NetworkingState.ForcefullyTakingOverWaitingForSight;
			SetOwnership(PhotonNetwork.LocalPlayer, isLocalOnly: true);
			RequestTheCurrentOwnerFromAuthority();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public void RequestOwnershipImmediatelyWithGuaranteedAuthority()
	{
		Debug.Log("WorldShareable RequestOwnershipImmediatelyWithGuaranteedAuthority");
		if (!PlayerHasAuthority(PhotonNetwork.LocalPlayer))
		{
			Debug.LogError("Tried to request ownership immediately with guaranteed authority without acutely having authority ");
		}
		switch (currentState)
		{
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			currentState = NetworkingState.ForcefullyTakingOver;
			StartCoroutine("RequestTimeout");
			break;
		case NetworkingState.IsClient:
			currentState = NetworkingState.ForcefullyTakingOver;
			SetOwnership(PhotonNetwork.LocalPlayer, isLocalOnly: true);
			photonView.RPC("SetOwnershipFromMasterClient", RpcTarget.All, PhotonNetwork.LocalPlayer);
			StartCoroutine("RequestTimeout");
			break;
		case NetworkingState.IsBlindClient:
			currentState = NetworkingState.ForcefullyTakingOverWaitingForSight;
			SetOwnership(PhotonNetwork.LocalPlayer, isLocalOnly: true);
			RequestTheCurrentOwnerFromAuthority();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case NetworkingState.IsOwner:
			break;
		}
	}

	public void Validate(SelfValidationResult result)
	{
	}

	public void AddCallbackTarget(IRequestableOwnershipGuardCallbacks callbackObject)
	{
		if (!callbacksList.Contains(callbackObject))
		{
			callbacksList.Add(callbackObject);
			if (currentOwner != null)
			{
				callbackObject.OnOwnershipTransferred(currentOwner, null);
			}
		}
	}

	public void RemoveCallbackTarget(IRequestableOwnershipGuardCallbacks callbackObject)
	{
		if (callbacksList.Contains(callbackObject))
		{
			callbacksList.Remove(callbackObject);
			if (currentOwner != null)
			{
				callbackObject.OnOwnershipTransferred(null, currentOwner);
			}
		}
	}

	public void SetCreator(Player player)
	{
		creator = player;
	}
}
