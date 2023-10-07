using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000179 RID: 377
public class GorillaHuntManager : GorillaGameManager, IInRoomCallbacks, IMatchmakingCallbacks, IPunObservable
{
	// Token: 0x06000974 RID: 2420 RVA: 0x0003969B File Offset: 0x0003789B
	public override string GameMode()
	{
		return "HUNT";
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x000396A2 File Offset: 0x000378A2
	public override void Awake()
	{
		base.Awake();
		Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
		GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(true);
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x000396D0 File Offset: 0x000378D0
	public void UpdateState()
	{
		if (base.photonView.IsMine)
		{
			if (PhotonNetwork.CurrentRoom.PlayerCount <= 3)
			{
				this.CleanUpHunt();
				this.huntStarted = false;
				this.waitingToStartNextHuntGame = false;
				this.iterator1 = 0;
				while (this.iterator1 < PhotonNetwork.PlayerList.Length)
				{
					this.FindVRRigForPlayer(PhotonNetwork.PlayerList[this.iterator1]).RPC("PlayTagSound", PhotonNetwork.PlayerList[this.iterator1], new object[]
					{
						0,
						0.25f
					});
					this.iterator1++;
				}
				return;
			}
			if (PhotonNetwork.CurrentRoom.PlayerCount > 3 && !this.huntStarted && !this.waitingToStartNextHuntGame && !this.inStartCountdown)
			{
				base.StartCoroutine(this.StartHuntCountdown());
				return;
			}
			this.UpdateHuntState();
		}
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x000397B2 File Offset: 0x000379B2
	public void CleanUpHunt()
	{
		if (base.photonView.IsMine)
		{
			this.currentHunted.Clear();
			this.currentTarget.Clear();
			this.CopyHuntDataListToArray();
		}
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x000397DD File Offset: 0x000379DD
	public IEnumerator StartHuntCountdown()
	{
		if (base.photonView.IsMine && !this.inStartCountdown)
		{
			this.inStartCountdown = true;
			this.countDownTime = 5;
			this.CleanUpHunt();
			while (this.countDownTime > 0)
			{
				yield return new WaitForSeconds(1f);
				this.countDownTime--;
			}
			this.StartHunt();
		}
		yield return null;
		yield break;
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x000397EC File Offset: 0x000379EC
	public void StartHunt()
	{
		if (base.photonView.IsMine)
		{
			this.huntStarted = true;
			this.waitingToStartNextHuntGame = false;
			this.countDownTime = 0;
			this.inStartCountdown = false;
			this.CleanUpHunt();
			this.iterator1 = 0;
			while (this.iterator1 < PhotonNetwork.PlayerList.Length)
			{
				if (this.currentTarget.Count < 10)
				{
					this.currentTarget.Add(PhotonNetwork.PlayerList[this.iterator1]);
					this.FindVRRigForPlayer(PhotonNetwork.PlayerList[this.iterator1]).RPC("PlayTagSound", PhotonNetwork.PlayerList[this.iterator1], new object[]
					{
						0,
						0.25f
					});
				}
				this.iterator1++;
			}
			this.RandomizePlayerList(ref this.currentTarget);
			this.CopyHuntDataListToArray();
		}
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x000398D0 File Offset: 0x00037AD0
	public void RandomizePlayerList(ref List<Player> listToRandomize)
	{
		for (int i = 0; i < listToRandomize.Count - 1; i++)
		{
			this.tempRandIndex = Random.Range(i, listToRandomize.Count);
			this.tempRandPlayer = listToRandomize[i];
			listToRandomize[i] = listToRandomize[this.tempRandIndex];
			listToRandomize[this.tempRandIndex] = this.tempRandPlayer;
		}
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x0003993A File Offset: 0x00037B3A
	public IEnumerator HuntEnd()
	{
		if (base.photonView.IsMine)
		{
			while ((double)Time.time < this.timeHuntGameEnded + (double)this.tagCoolDown)
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (this.waitingToStartNextHuntGame)
			{
				base.StartCoroutine(this.StartHuntCountdown());
			}
			yield return null;
		}
		yield return null;
		yield break;
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x0003994C File Offset: 0x00037B4C
	public void UpdateHuntState()
	{
		if (base.photonView.IsMine)
		{
			this.CopyHuntDataListToArray();
			this.notHuntedCount = 0;
			foreach (Player item in PhotonNetwork.PlayerList)
			{
				if (this.currentTarget.Contains(item) && !this.currentHunted.Contains(item))
				{
					this.notHuntedCount++;
				}
			}
			if (this.notHuntedCount <= 2 && this.huntStarted)
			{
				this.EndHuntGame();
			}
		}
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x000399CC File Offset: 0x00037BCC
	private void EndHuntGame()
	{
		if (base.photonView.IsMine)
		{
			foreach (Player player in PhotonNetwork.PlayerList)
			{
				PhotonView photonView = this.FindVRRigForPlayer(player);
				photonView.RPC("SetTaggedTime", player, null);
				photonView.RPC("PlayTagSound", player, new object[]
				{
					2,
					0.25f
				});
			}
			this.huntStarted = false;
			this.timeHuntGameEnded = (double)Time.time;
			this.waitingToStartNextHuntGame = true;
			base.StartCoroutine(this.HuntEnd());
		}
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x00039A60 File Offset: 0x00037C60
	public override bool LocalCanTag(Player myPlayer, Player otherPlayer)
	{
		if (this.waitingToStartNextHuntGame || this.countDownTime > 0 || GorillaTagger.Instance.currentStatus == GorillaTagger.StatusEffect.Frozen)
		{
			return false;
		}
		if (this.currentHunted.Contains(myPlayer) && !this.currentHunted.Contains(otherPlayer) && Time.time > this.timeLastSlowTagged + 1f)
		{
			this.timeLastSlowTagged = Time.time;
			return true;
		}
		return this.IsTargetOf(myPlayer, otherPlayer);
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x00039AD8 File Offset: 0x00037CD8
	public override void ReportTag(Player taggedPlayer, Player taggingPlayer)
	{
		if (base.photonView.IsMine && !this.waitingToStartNextHuntGame)
		{
			PhotonView photonView = this.FindVRRigForPlayer(taggedPlayer);
			if ((this.currentHunted.Contains(taggingPlayer) || !this.currentTarget.Contains(taggingPlayer)) && !this.currentHunted.Contains(taggedPlayer) && this.currentTarget.Contains(taggedPlayer))
			{
				photonView.RPC("SetSlowedTime", taggedPlayer, null);
				photonView.RPC("PlayTagSound", RpcTarget.All, new object[]
				{
					5,
					0.125f
				});
				return;
			}
			if (this.IsTargetOf(taggingPlayer, taggedPlayer))
			{
				photonView.RPC("SetTaggedTime", taggedPlayer, null);
				photonView.RPC("PlayTagSound", RpcTarget.All, new object[]
				{
					0,
					0.25f
				});
				this.currentHunted.Add(taggedPlayer);
				this.CopyHuntDataListToArray();
				this.UpdateHuntState();
			}
		}
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x00039BD0 File Offset: 0x00037DD0
	public bool IsTargetOf(Player huntingPlayer, Player huntedPlayer)
	{
		return !this.currentHunted.Contains(huntingPlayer) && !this.currentHunted.Contains(huntedPlayer) && this.currentTarget.Contains(huntingPlayer) && this.currentTarget.Contains(huntedPlayer) && huntedPlayer == this.GetTargetOf(huntingPlayer);
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x00039C24 File Offset: 0x00037E24
	public Player GetTargetOf(Player player)
	{
		if (this.currentHunted.Contains(player) || !this.currentTarget.Contains(player))
		{
			return null;
		}
		this.tempTargetIndex = this.currentTarget.IndexOf(player);
		for (int num = (this.tempTargetIndex + 1) % this.currentTarget.Count; num != this.tempTargetIndex; num = (num + 1) % this.currentTarget.Count)
		{
			if (this.currentTarget[num] == player)
			{
				return null;
			}
			if (!this.currentHunted.Contains(this.currentTarget[num]) && this.currentTarget[num] != null)
			{
				return this.currentTarget[num];
			}
		}
		return null;
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x00039CD8 File Offset: 0x00037ED8
	public override void ReportContactWithLava(Player taggedPlayer)
	{
		if (base.photonView.IsMine && !this.waitingToStartNextHuntGame)
		{
			PhotonView photonView = this.FindVRRigForPlayer(taggedPlayer);
			if (!this.currentHunted.Contains(taggedPlayer) && this.currentTarget.Contains(taggedPlayer))
			{
				photonView.RPC("SetTaggedTime", taggedPlayer, null);
				photonView.RPC("PlayTagSound", RpcTarget.All, new object[]
				{
					0,
					0.25f
				});
				this.currentHunted.Add(taggedPlayer);
				this.CopyHuntDataListToArray();
				this.UpdateHuntState();
			}
		}
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x00039D6B File Offset: 0x00037F6B
	public override bool LavaWouldAffectPlayer(Player player, bool enteredLavaThisFrame)
	{
		return !this.waitingToStartNextHuntGame && !this.currentHunted.Contains(player) && this.currentTarget.Contains(player);
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x00039D91 File Offset: 0x00037F91
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		bool isMine = base.photonView.IsMine;
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x00039D9F File Offset: 0x00037F9F
	public override void NewVRRig(Player player, int vrrigPhotonViewID, bool didntDoTutorial)
	{
		base.NewVRRig(player, vrrigPhotonViewID, didntDoTutorial);
		if (base.photonView.IsMine)
		{
			if (!this.waitingToStartNextHuntGame && this.huntStarted)
			{
				this.currentHunted.Add(player);
				this.CopyHuntDataListToArray();
			}
			this.UpdateState();
		}
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x00039DE0 File Offset: 0x00037FE0
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (base.photonView.IsMine)
		{
			if (this.currentTarget.Contains(otherPlayer))
			{
				this.currentTarget.Remove(otherPlayer);
				this.CopyHuntDataListToArray();
			}
			if (this.currentHunted.Contains(otherPlayer))
			{
				this.currentHunted.Remove(otherPlayer);
				this.CopyHuntDataListToArray();
			}
			this.UpdateState();
		}
		this.playerVRRigDict.Remove(otherPlayer.ActorNumber);
		this.playerCosmeticsLookup.Remove(otherPlayer.ActorNumber);
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x00039E70 File Offset: 0x00038070
	private void CopyHuntDataListToArray()
	{
		this.copyListToArrayIndex = 0;
		while (this.copyListToArrayIndex < 10)
		{
			this.currentHuntedArray[this.copyListToArrayIndex] = 0;
			this.currentTargetArray[this.copyListToArrayIndex] = 0;
			this.copyListToArrayIndex++;
		}
		this.copyListToArrayIndex = this.currentHunted.Count - 1;
		while (this.copyListToArrayIndex >= 0)
		{
			if (this.currentHunted[this.copyListToArrayIndex] == null)
			{
				this.currentHunted.RemoveAt(this.copyListToArrayIndex);
			}
			this.copyListToArrayIndex--;
		}
		this.copyListToArrayIndex = this.currentTarget.Count - 1;
		while (this.copyListToArrayIndex >= 0)
		{
			if (this.currentTarget[this.copyListToArrayIndex] == null)
			{
				this.currentTarget.RemoveAt(this.copyListToArrayIndex);
			}
			this.copyListToArrayIndex--;
		}
		this.copyListToArrayIndex = 0;
		while (this.copyListToArrayIndex < this.currentHunted.Count)
		{
			this.currentHuntedArray[this.copyListToArrayIndex] = this.currentHunted[this.copyListToArrayIndex].ActorNumber;
			this.copyListToArrayIndex++;
		}
		this.copyListToArrayIndex = 0;
		while (this.copyListToArrayIndex < this.currentTarget.Count)
		{
			this.currentTargetArray[this.copyListToArrayIndex] = this.currentTarget[this.copyListToArrayIndex].ActorNumber;
			this.copyListToArrayIndex++;
		}
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x00039FF4 File Offset: 0x000381F4
	private void CopyHuntDataArrayToList()
	{
		this.currentTarget.Clear();
		this.copyArrayToListIndex = 0;
		while (this.copyArrayToListIndex < this.currentTargetArray.Length)
		{
			if (this.currentTargetArray[this.copyArrayToListIndex] != 0)
			{
				this.tempPlayer = PhotonNetwork.LocalPlayer.Get(this.currentTargetArray[this.copyArrayToListIndex]);
				if (this.tempPlayer != null)
				{
					this.currentTarget.Add(this.tempPlayer);
				}
			}
			this.copyArrayToListIndex++;
		}
		this.currentHunted.Clear();
		this.copyArrayToListIndex = 0;
		while (this.copyArrayToListIndex < this.currentHuntedArray.Length)
		{
			if (this.currentHuntedArray[this.copyArrayToListIndex] != 0)
			{
				this.tempPlayer = PhotonNetwork.LocalPlayer.Get(this.currentHuntedArray[this.copyArrayToListIndex]);
				if (this.tempPlayer != null)
				{
					this.currentHunted.Add(this.tempPlayer);
				}
			}
			this.copyArrayToListIndex++;
		}
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x0003A0F0 File Offset: 0x000382F0
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		if (base.photonView.IsMine)
		{
			foreach (Player player in PhotonNetwork.PlayerList)
			{
				if (this.playerCosmeticsLookup.TryGetValue(player.ActorNumber, out this.tempString) && this.tempString == "BANNED")
				{
					PhotonNetwork.CloseConnection(player);
				}
			}
			this.CopyRoomDataToLocalData();
			this.UpdateState();
		}
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x0003A160 File Offset: 0x00038360
	public void CopyRoomDataToLocalData()
	{
		this.waitingToStartNextHuntGame = false;
		this.UpdateHuntState();
	}

	// Token: 0x0600098B RID: 2443 RVA: 0x0003A16F File Offset: 0x0003836F
	[PunRPC]
	public override void ReportTagRPC(Player taggedPlayer, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportTagRPC");
		this.ReportTag(taggedPlayer, info.Sender);
	}

	// Token: 0x0600098C RID: 2444 RVA: 0x0003A18C File Offset: 0x0003838C
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.currentHuntedArray[0]);
			stream.SendNext(this.currentHuntedArray[1]);
			stream.SendNext(this.currentHuntedArray[2]);
			stream.SendNext(this.currentHuntedArray[3]);
			stream.SendNext(this.currentHuntedArray[4]);
			stream.SendNext(this.currentHuntedArray[5]);
			stream.SendNext(this.currentHuntedArray[6]);
			stream.SendNext(this.currentHuntedArray[7]);
			stream.SendNext(this.currentHuntedArray[8]);
			stream.SendNext(this.currentHuntedArray[9]);
			stream.SendNext(this.currentTargetArray[0]);
			stream.SendNext(this.currentTargetArray[1]);
			stream.SendNext(this.currentTargetArray[2]);
			stream.SendNext(this.currentTargetArray[3]);
			stream.SendNext(this.currentTargetArray[4]);
			stream.SendNext(this.currentTargetArray[5]);
			stream.SendNext(this.currentTargetArray[6]);
			stream.SendNext(this.currentTargetArray[7]);
			stream.SendNext(this.currentTargetArray[8]);
			stream.SendNext(this.currentTargetArray[9]);
			stream.SendNext(this.huntStarted);
			stream.SendNext(this.waitingToStartNextHuntGame);
			stream.SendNext(this.countDownTime);
			return;
		}
		this.currentHuntedArray[0] = (int)stream.ReceiveNext();
		this.currentHuntedArray[1] = (int)stream.ReceiveNext();
		this.currentHuntedArray[2] = (int)stream.ReceiveNext();
		this.currentHuntedArray[3] = (int)stream.ReceiveNext();
		this.currentHuntedArray[4] = (int)stream.ReceiveNext();
		this.currentHuntedArray[5] = (int)stream.ReceiveNext();
		this.currentHuntedArray[6] = (int)stream.ReceiveNext();
		this.currentHuntedArray[7] = (int)stream.ReceiveNext();
		this.currentHuntedArray[8] = (int)stream.ReceiveNext();
		this.currentHuntedArray[9] = (int)stream.ReceiveNext();
		this.currentTargetArray[0] = (int)stream.ReceiveNext();
		this.currentTargetArray[1] = (int)stream.ReceiveNext();
		this.currentTargetArray[2] = (int)stream.ReceiveNext();
		this.currentTargetArray[3] = (int)stream.ReceiveNext();
		this.currentTargetArray[4] = (int)stream.ReceiveNext();
		this.currentTargetArray[5] = (int)stream.ReceiveNext();
		this.currentTargetArray[6] = (int)stream.ReceiveNext();
		this.currentTargetArray[7] = (int)stream.ReceiveNext();
		this.currentTargetArray[8] = (int)stream.ReceiveNext();
		this.currentTargetArray[9] = (int)stream.ReceiveNext();
		this.huntStarted = (bool)stream.ReceiveNext();
		this.waitingToStartNextHuntGame = (bool)stream.ReceiveNext();
		this.countDownTime = (int)stream.ReceiveNext();
		this.CopyHuntDataArrayToList();
	}

	// Token: 0x0600098D RID: 2445 RVA: 0x0003A51B File Offset: 0x0003871B
	public override int MyMatIndex(Player forPlayer)
	{
		if (this.currentHunted.Contains(forPlayer) || (this.huntStarted && this.GetTargetOf(forPlayer) == null))
		{
			return 3;
		}
		return 0;
	}

	// Token: 0x0600098E RID: 2446 RVA: 0x0003A540 File Offset: 0x00038740
	public override float[] LocalPlayerSpeed()
	{
		if (this.currentHunted.Contains(PhotonNetwork.LocalPlayer) || (this.huntStarted && this.GetTargetOf(PhotonNetwork.LocalPlayer) == null))
		{
			return new float[]
			{
				8.5f,
				1.3f
			};
		}
		if (GorillaTagger.Instance.currentStatus == GorillaTagger.StatusEffect.Slowed)
		{
			return new float[]
			{
				5.5f,
				0.9f
			};
		}
		return new float[]
		{
			6.5f,
			1.1f
		};
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x0003A5C8 File Offset: 0x000387C8
	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
		if (base.photonView.IsMine)
		{
			PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out this.obj);
			if (this.obj.ToString().Contains("CASUAL") || this.obj.ToString().Contains("INFECTION"))
			{
				Object.Destroy(this);
			}
		}
	}

	// Token: 0x04000B9B RID: 2971
	public float tagCoolDown = 5f;

	// Token: 0x04000B9C RID: 2972
	public int[] currentHuntedArray = new int[10];

	// Token: 0x04000B9D RID: 2973
	public List<Player> currentHunted = new List<Player>();

	// Token: 0x04000B9E RID: 2974
	public int[] currentTargetArray = new int[10];

	// Token: 0x04000B9F RID: 2975
	public List<Player> currentTarget = new List<Player>();

	// Token: 0x04000BA0 RID: 2976
	public bool huntStarted;

	// Token: 0x04000BA1 RID: 2977
	public bool waitingToStartNextHuntGame;

	// Token: 0x04000BA2 RID: 2978
	public bool inStartCountdown;

	// Token: 0x04000BA3 RID: 2979
	public int countDownTime;

	// Token: 0x04000BA4 RID: 2980
	public double timeHuntGameEnded;

	// Token: 0x04000BA5 RID: 2981
	public float timeLastSlowTagged;

	// Token: 0x04000BA6 RID: 2982
	public object objRef;

	// Token: 0x04000BA7 RID: 2983
	private int iterator1;

	// Token: 0x04000BA8 RID: 2984
	private Player tempRandPlayer;

	// Token: 0x04000BA9 RID: 2985
	private int tempRandIndex;

	// Token: 0x04000BAA RID: 2986
	private int notHuntedCount;

	// Token: 0x04000BAB RID: 2987
	private int tempTargetIndex;

	// Token: 0x04000BAC RID: 2988
	private Player tempPlayer;

	// Token: 0x04000BAD RID: 2989
	private int copyListToArrayIndex;

	// Token: 0x04000BAE RID: 2990
	private int copyArrayToListIndex;
}
