using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020001E4 RID: 484
public class PhotonUserCache : IInRoomCallbacks, IMatchmakingCallbacks
{
	// Token: 0x06000C75 RID: 3189 RVA: 0x0004B988 File Offset: 0x00049B88
	[RuntimeInitializeOnLoadMethod]
	private static void Initialize()
	{
		PhotonUserCache.gUserCache = new PhotonUserCache();
		PhotonNetwork.AddCallbackTarget(PhotonUserCache.gUserCache);
	}

	// Token: 0x06000C76 RID: 3190 RVA: 0x0004B99E File Offset: 0x00049B9E
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

	// Token: 0x06000C77 RID: 3191 RVA: 0x0004B9CB File Offset: 0x00049BCB
	public static bool TryGetPlayerObject(int actorNumber, out GameObject playerObject)
	{
		PhotonUserCache.gUserCache.SafeRefresh();
		return PhotonUserCache.gUserCache._playerObjects.TryGetValue(actorNumber, out playerObject);
	}

	// Token: 0x06000C78 RID: 3192 RVA: 0x0004B9E8 File Offset: 0x00049BE8
	private void SafeRefresh()
	{
		if (this._needsRefresh)
		{
			this.Refresh(false);
		}
	}

	// Token: 0x06000C79 RID: 3193 RVA: 0x0004B9FC File Offset: 0x00049BFC
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

	// Token: 0x06000C7A RID: 3194 RVA: 0x0004BA88 File Offset: 0x00049C88
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
		this.Refresh(false);
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x0004BA91 File Offset: 0x00049C91
	void IMatchmakingCallbacks.OnJoinedRoom()
	{
		this.Refresh(true);
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x0004BA9A File Offset: 0x00049C9A
	void IMatchmakingCallbacks.OnLeftRoom()
	{
		this._playerObjects.Clear();
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x0004BAA7 File Offset: 0x00049CA7
	void IInRoomCallbacks.OnPlayerLeftRoom(Player player)
	{
		if (!this._playerObjects.ContainsKey(player.ActorNumber))
		{
			return;
		}
		this._playerObjects.Remove(player.ActorNumber);
	}

	// Token: 0x06000C7E RID: 3198 RVA: 0x0004BACF File Offset: 0x00049CCF
	void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06000C7F RID: 3199 RVA: 0x0004BAD1 File Offset: 0x00049CD1
	void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x0004BAD3 File Offset: 0x00049CD3
	void IMatchmakingCallbacks.OnCreatedRoom()
	{
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x0004BAD5 File Offset: 0x00049CD5
	void IMatchmakingCallbacks.OnPreLeavingRoom()
	{
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x0004BAD7 File Offset: 0x00049CD7
	void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06000C83 RID: 3203 RVA: 0x0004BAD9 File Offset: 0x00049CD9
	void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	// Token: 0x06000C84 RID: 3204 RVA: 0x0004BADB File Offset: 0x00049CDB
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable changedProperties)
	{
	}

	// Token: 0x06000C85 RID: 3205 RVA: 0x0004BADD File Offset: 0x00049CDD
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player player, Hashtable changedProperties)
	{
	}

	// Token: 0x06000C86 RID: 3206 RVA: 0x0004BADF File Offset: 0x00049CDF
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
	}

	// Token: 0x04000FF3 RID: 4083
	private static PhotonUserCache gUserCache;

	// Token: 0x04000FF4 RID: 4084
	private readonly Dictionary<int, GameObject> _playerObjects = new Dictionary<int, GameObject>();

	// Token: 0x04000FF5 RID: 4085
	private TimeSince _timeSinceLastRefresh;

	// Token: 0x04000FF6 RID: 4086
	private bool _needsRefresh;
}
