using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020001E3 RID: 483
public class PhotonUserCache : IInRoomCallbacks, IMatchmakingCallbacks
{
	// Token: 0x06000C6F RID: 3183 RVA: 0x0004B720 File Offset: 0x00049920
	[RuntimeInitializeOnLoadMethod]
	private static void Initialize()
	{
		PhotonUserCache.gUserCache = new PhotonUserCache();
		PhotonNetwork.AddCallbackTarget(PhotonUserCache.gUserCache);
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x0004B736 File Offset: 0x00049936
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

	// Token: 0x06000C71 RID: 3185 RVA: 0x0004B763 File Offset: 0x00049963
	public static bool TryGetPlayerObject(int actorNumber, out GameObject playerObject)
	{
		PhotonUserCache.gUserCache.SafeRefresh();
		return PhotonUserCache.gUserCache._playerObjects.TryGetValue(actorNumber, out playerObject);
	}

	// Token: 0x06000C72 RID: 3186 RVA: 0x0004B780 File Offset: 0x00049980
	private void SafeRefresh()
	{
		if (this._needsRefresh)
		{
			this.Refresh(false);
		}
	}

	// Token: 0x06000C73 RID: 3187 RVA: 0x0004B794 File Offset: 0x00049994
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

	// Token: 0x06000C74 RID: 3188 RVA: 0x0004B820 File Offset: 0x00049A20
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
		this.Refresh(false);
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x0004B829 File Offset: 0x00049A29
	void IMatchmakingCallbacks.OnJoinedRoom()
	{
		this.Refresh(true);
	}

	// Token: 0x06000C76 RID: 3190 RVA: 0x0004B832 File Offset: 0x00049A32
	void IMatchmakingCallbacks.OnLeftRoom()
	{
		this._playerObjects.Clear();
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x0004B83F File Offset: 0x00049A3F
	void IInRoomCallbacks.OnPlayerLeftRoom(Player player)
	{
		if (!this._playerObjects.ContainsKey(player.ActorNumber))
		{
			return;
		}
		this._playerObjects.Remove(player.ActorNumber);
	}

	// Token: 0x06000C78 RID: 3192 RVA: 0x0004B867 File Offset: 0x00049A67
	void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06000C79 RID: 3193 RVA: 0x0004B869 File Offset: 0x00049A69
	void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x0004B86B File Offset: 0x00049A6B
	void IMatchmakingCallbacks.OnCreatedRoom()
	{
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x0004B86D File Offset: 0x00049A6D
	void IMatchmakingCallbacks.OnPreLeavingRoom()
	{
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x0004B86F File Offset: 0x00049A6F
	void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x0004B871 File Offset: 0x00049A71
	void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	// Token: 0x06000C7E RID: 3198 RVA: 0x0004B873 File Offset: 0x00049A73
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable changedProperties)
	{
	}

	// Token: 0x06000C7F RID: 3199 RVA: 0x0004B875 File Offset: 0x00049A75
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player player, Hashtable changedProperties)
	{
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x0004B877 File Offset: 0x00049A77
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
	}

	// Token: 0x04000FEF RID: 4079
	private static PhotonUserCache gUserCache;

	// Token: 0x04000FF0 RID: 4080
	private readonly Dictionary<int, GameObject> _playerObjects = new Dictionary<int, GameObject>();

	// Token: 0x04000FF1 RID: 4081
	private TimeSince _timeSinceLastRefresh;

	// Token: 0x04000FF2 RID: 4082
	private bool _needsRefresh;
}
