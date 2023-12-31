﻿using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GorillaHuntManager : GorillaGameManager, IInRoomCallbacks, IMatchmakingCallbacks, IPunObservable
{
	public override string GameMode()
	{
		return "HUNT";
	}

	public override void Awake()
	{
		base.Awake();
		Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
		GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(true);
	}

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

	public void CleanUpHunt()
	{
		if (base.photonView.IsMine)
		{
			this.currentHunted.Clear();
			this.currentTarget.Clear();
			this.CopyHuntDataListToArray();
		}
	}

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

	public bool IsTargetOf(Player huntingPlayer, Player huntedPlayer)
	{
		return !this.currentHunted.Contains(huntingPlayer) && !this.currentHunted.Contains(huntedPlayer) && this.currentTarget.Contains(huntingPlayer) && this.currentTarget.Contains(huntedPlayer) && huntedPlayer == this.GetTargetOf(huntingPlayer);
	}

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

	public override bool LavaWouldAffectPlayer(Player player, bool enteredLavaThisFrame)
	{
		return !this.waitingToStartNextHuntGame && !this.currentHunted.Contains(player) && this.currentTarget.Contains(player);
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		bool isMine = base.photonView.IsMine;
	}

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

	public void CopyRoomDataToLocalData()
	{
		this.waitingToStartNextHuntGame = false;
		this.UpdateHuntState();
	}

	[PunRPC]
	public override void ReportTagRPC(Player taggedPlayer, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportTagRPC");
		this.ReportTag(taggedPlayer, info.Sender);
	}

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

	public override int MyMatIndex(Player forPlayer)
	{
		if (this.currentHunted.Contains(forPlayer) || (this.huntStarted && this.GetTargetOf(forPlayer) == null))
		{
			return 3;
		}
		return 0;
	}

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

	public float tagCoolDown = 5f;

	public int[] currentHuntedArray = new int[10];

	public List<Player> currentHunted = new List<Player>();

	public int[] currentTargetArray = new int[10];

	public List<Player> currentTarget = new List<Player>();

	public bool huntStarted;

	public bool waitingToStartNextHuntGame;

	public bool inStartCountdown;

	public int countDownTime;

	public double timeHuntGameEnded;

	public float timeLastSlowTagged;

	public object objRef;

	private int iterator1;

	private Player tempRandPlayer;

	private int tempRandIndex;

	private int notHuntedCount;

	private int tempTargetIndex;

	private Player tempPlayer;

	private int copyListToArrayIndex;

	private int copyArrayToListIndex;
}
