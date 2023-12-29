using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonUserCache : IInRoomCallbacks, IMatchmakingCallbacks
{
	[RuntimeInitializeOnLoadMethod]
	private static void Initialize()
	{
		PhotonUserCache.gUserCache = new PhotonUserCache();
		PhotonNetwork.AddCallbackTarget(PhotonUserCache.gUserCache);
	}

	public static void Invalidate()
	{
		if (PhotonUserCache.gUserCache != null)
		{
			PhotonNetwork.RemoveCallbackTarget(PhotonUserCache.gUserCache);
			PhotonUserCache.gUserCache._playerObjects.Clear();
			PhotonUserCache.gUserCache = null;
		}
		PhotonUserCache.Initialize();
	}

	public static bool TryGetPlayerObject(int actorNumber, out GameObject playerObject)
	{
		PhotonUserCache.gUserCache.SafeRefresh();
		return PhotonUserCache.gUserCache._playerObjects.TryGetValue(actorNumber, out playerObject);
	}

	private void SafeRefresh()
	{
		if (this._needsRefresh)
		{
			this.Refresh(false);
		}
	}

	private void Refresh(bool force = false)
	{
		bool flag = this._timeSinceLastRefresh < 3f;
		if (flag)
		{
			this._needsRefresh = true;
			if (!force)
			{
				return;
			}
		}
		foreach (PhotonView photonView in Object.FindObjectsOfType<PhotonView>())
		{
			Player owner = photonView.Owner;
			if (owner != null)
			{
				this._playerObjects[owner.ActorNumber] = photonView.gameObject;
			}
		}
		if (!flag && this._needsRefresh)
		{
			this._needsRefresh = false;
		}
		this._timeSinceLastRefresh = 0f;
	}

	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
		this.Refresh(false);
	}

	void IMatchmakingCallbacks.OnJoinedRoom()
	{
		this.Refresh(true);
	}

	void IMatchmakingCallbacks.OnLeftRoom()
	{
		this._playerObjects.Clear();
	}

	void IInRoomCallbacks.OnPlayerLeftRoom(Player player)
	{
		if (!this._playerObjects.ContainsKey(player.ActorNumber))
		{
			return;
		}
		this._playerObjects.Remove(player.ActorNumber);
	}

	void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
	{
	}

	void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
	{
	}

	void IMatchmakingCallbacks.OnCreatedRoom()
	{
	}

	void IMatchmakingCallbacks.OnPreLeavingRoom()
	{
	}

	void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
	{
	}

	void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable changedProperties)
	{
	}

	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player player, Hashtable changedProperties)
	{
	}

	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
	}

	private static PhotonUserCache gUserCache;

	private readonly Dictionary<int, GameObject> _playerObjects = new Dictionary<int, GameObject>();

	private TimeSince _timeSinceLastRefresh;

	private bool _needsRefresh;
}
