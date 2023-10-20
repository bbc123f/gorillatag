using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000186 RID: 390
public class GorillaTagManager : GorillaGameManager, IInRoomCallbacks, IMatchmakingCallbacks, IPunObservable
{
	// Token: 0x060009FB RID: 2555 RVA: 0x0003DBBD File Offset: 0x0003BDBD
	public override void Awake()
	{
		base.Awake();
		if (base.photonView.IsMine)
		{
			this.isCurrentlyTag = true;
		}
		Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x0003DBE8 File Offset: 0x0003BDE8
	public void UpdateState()
	{
		if (base.photonView.IsMine)
		{
			if (!this.IsGameModeTag())
			{
				if (this.currentInfected.Count > 0)
				{
					this.ClearInfectionState();
				}
				return;
			}
			if (this.isCurrentlyTag && this.currentIt == null)
			{
				int num = Random.Range(0, this.currentPlayerArray.Length);
				this.ChangeCurrentIt(this.currentPlayerArray[num], false);
			}
			else if (this.isCurrentlyTag && this.currentPlayerArray.Length >= this.infectedModeThreshold)
			{
				this.SetisCurrentlyTag(false);
				this.ClearInfectionState();
				int num2 = Random.Range(0, this.currentPlayerArray.Length);
				this.AddInfectedPlayer(this.currentPlayerArray[num2]);
				this.lastInfectedPlayer = this.currentPlayerArray[num2];
			}
			else if (!this.isCurrentlyTag && this.currentPlayerArray.Length < this.infectedModeThreshold)
			{
				this.ClearInfectionState();
				this.lastInfectedPlayer = null;
				this.SetisCurrentlyTag(true);
				int num3 = Random.Range(0, this.currentPlayerArray.Length);
				this.ChangeCurrentIt(this.currentPlayerArray[num3], false);
			}
			else if (!this.isCurrentlyTag && this.currentInfected.Count == 0)
			{
				int num4 = Random.Range(0, this.CurrentInfectionPlayers().Length);
				this.AddInfectedPlayer(this.CurrentInfectionPlayers()[num4]);
			}
			else if (!this.isCurrentlyTag)
			{
				this.UpdateInfectionState();
			}
			this.CopyInfectedListToArray();
		}
	}

	// Token: 0x060009FD RID: 2557 RVA: 0x0003DD40 File Offset: 0x0003BF40
	public override void InfrequentUpdate()
	{
		if (base.photonView.IsMine)
		{
			base.InfrequentUpdate();
			this.UpdateState();
			PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out this.obj);
			if (this.obj.ToString().Contains("HUNT") || this.obj.ToString().Contains("BATTLE"))
			{
				Object.Destroy(this);
			}
		}
		this.currentPlayerArray = PhotonNetwork.PlayerList;
		this.inspectorLocalPlayerSpeed = this.LocalPlayerSpeed();
	}

	// Token: 0x060009FE RID: 2558 RVA: 0x0003DDCC File Offset: 0x0003BFCC
	public IEnumerator InfectionEnd()
	{
		while ((double)Time.time < this.timeInfectedGameEnded + (double)this.tagCoolDown)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (!this.isCurrentlyTag && this.waitingToStartNextInfectionGame)
		{
			Player[] array = this.CurrentInfectionPlayers();
			int num = Random.Range(0, array.Length);
			int num2 = 0;
			while (num2 < 10 && array[num] == this.lastInfectedPlayer)
			{
				num = Random.Range(0, array.Length);
				num2++;
			}
			this.ClearInfectionState();
			this.AddInfectedPlayer(array[num]);
			this.lastInfectedPlayer = array[num];
			this.lastTag = (double)Time.time;
		}
		yield return null;
		yield break;
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x0003DDDC File Offset: 0x0003BFDC
	public void UpdateInfectionState()
	{
		if (base.photonView.IsMine)
		{
			this.allInfected = true;
			foreach (Player player in this.CurrentInfectionPlayers())
			{
				if (this.FindVRRigForPlayer(player) != null && !this.currentInfected.Contains(player))
				{
					this.allInfected = false;
				}
			}
			if (!this.isCurrentlyTag && !this.waitingToStartNextInfectionGame && this.allInfected)
			{
				this.EndInfectionGame();
			}
		}
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x0003DE58 File Offset: 0x0003C058
	public void UpdateTagState(bool withTagFreeze = true)
	{
		if (base.photonView.IsMine)
		{
			foreach (Player player in this.currentPlayerArray)
			{
				PhotonView photonView = this.FindVRRigForPlayer(player);
				if (photonView != null && this.currentIt == player)
				{
					if (withTagFreeze)
					{
						photonView.RPC("SetTaggedTime", player, null);
					}
					else
					{
						photonView.RPC("SetJoinTaggedTime", player, null);
					}
					photonView.RPC("PlayTagSound", RpcTarget.All, new object[]
					{
						0,
						0.25f
					});
				}
			}
		}
	}

	// Token: 0x06000A01 RID: 2561 RVA: 0x0003DEEC File Offset: 0x0003C0EC
	private void EndInfectionGame()
	{
		if (base.photonView.IsMine)
		{
			foreach (Player player in this.CurrentInfectionPlayers())
			{
				PhotonView photonView = this.FindVRRigForPlayer(player);
				if (photonView != null)
				{
					photonView.RPC("SetTaggedTime", player, null);
					photonView.RPC("PlayTagSound", player, new object[]
					{
						2,
						0.25f
					});
				}
			}
			this.waitingToStartNextInfectionGame = true;
			this.timeInfectedGameEnded = (double)Time.time;
			base.StartCoroutine(this.InfectionEnd());
		}
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x0003DF88 File Offset: 0x0003C188
	private Player[] CurrentInfectionPlayers()
	{
		this.returnPlayerList.Clear();
		foreach (Player player in this.currentPlayerArray)
		{
			if (!player.CustomProperties.TryGetValue("didTutorial", out this.obj) || (bool)this.obj)
			{
				this.returnPlayerList.Add(player);
			}
		}
		return this.returnPlayerList.ToArray();
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x0003DFF8 File Offset: 0x0003C1F8
	public override bool LocalCanTag(Player myPlayer, Player otherPlayer)
	{
		if (!this.IsGameModeTag())
		{
			return false;
		}
		if (this.isCurrentlyTag)
		{
			return myPlayer == this.currentIt && myPlayer != otherPlayer;
		}
		return this.currentInfected.Contains(myPlayer) && !this.currentInfected.Contains(otherPlayer);
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x0003E049 File Offset: 0x0003C249
	private float InterpolatedInfectedJumpMultiplier(int infectedCount)
	{
		if (this.currentPlayerArray.Length == 1)
		{
			return this.fastJumpMultiplier;
		}
		return (this.fastJumpMultiplier - this.slowJumpMultiplier) / (float)(this.currentPlayerArray.Length - 1) * (float)(this.currentPlayerArray.Length - infectedCount) + this.slowJumpMultiplier;
	}

	// Token: 0x06000A05 RID: 2565 RVA: 0x0003E089 File Offset: 0x0003C289
	private float InterpolatedInfectedJumpSpeed(int infectedCount)
	{
		if (this.currentPlayerArray.Length == 1)
		{
			return this.fastJumpLimit;
		}
		return (this.fastJumpLimit - this.slowJumpLimit) / (float)(this.currentPlayerArray.Length - 1) * (float)(this.currentPlayerArray.Length - infectedCount) + this.slowJumpLimit;
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x0003E0C9 File Offset: 0x0003C2C9
	private float InterpolatedNoobJumpMultiplier(int infectedCount)
	{
		if (this.currentPlayerArray.Length == 1)
		{
			return this.slowJumpMultiplier;
		}
		return (this.fastJumpMultiplier - this.slowJumpMultiplier) / (float)(this.currentPlayerArray.Length - 1) * (float)(infectedCount - 1) * 0.9f + this.slowJumpMultiplier;
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x0003E108 File Offset: 0x0003C308
	private float InterpolatedNoobJumpSpeed(int infectedCount)
	{
		if (this.currentPlayerArray.Length == 1)
		{
			return this.slowJumpLimit;
		}
		return (this.fastJumpLimit - this.fastJumpLimit) / (float)(this.currentPlayerArray.Length - 1) * (float)(infectedCount - 1) * 0.9f + this.slowJumpLimit;
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x0003E148 File Offset: 0x0003C348
	public override void ReportTag(Player taggedPlayer, Player taggingPlayer)
	{
		if (base.photonView.IsMine && this.IsGameModeTag())
		{
			this.taggingRig = this.FindPlayerVRRig(taggingPlayer);
			this.taggedRig = this.FindPlayerVRRig(taggedPlayer);
			if (this.taggingRig == null || this.taggedRig == null)
			{
				return;
			}
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			WebFlags flags = new WebFlags(1);
			raiseEventOptions.Flags = flags;
			byte b = 0;
			int num = 0;
			if (this.isCurrentlyTag)
			{
				if (taggingPlayer == this.currentIt && taggingPlayer != taggedPlayer && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
				{
					this.ChangeCurrentIt(taggedPlayer, true);
					new Hashtable();
					this.lastTag = (double)Time.time;
					b = 1;
				}
			}
			else if (this.currentInfected.Contains(taggingPlayer) && !this.currentInfected.Contains(taggedPlayer) && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
			{
				if ((this.taggingRig.transform.position - this.taggedRig.transform.position).magnitude > this.tagDistanceThreshold)
				{
					GorillaNot.instance.SendReport("extremely far tag", taggingPlayer.UserId, taggingPlayer.NickName);
				}
				else
				{
					this.AddInfectedPlayer(taggedPlayer);
					num = this.currentInfected.Count;
				}
				b = 2;
			}
			if (b == 1)
			{
				object[] eventContent = new object[]
				{
					taggingPlayer.UserId,
					taggedPlayer.UserId
				};
				PhotonNetwork.RaiseEvent(1, eventContent, raiseEventOptions, SendOptions.SendReliable);
				return;
			}
			if (b == 2)
			{
				object[] eventContent2 = new object[]
				{
					taggingPlayer.UserId,
					taggedPlayer.UserId,
					num
				};
				PhotonNetwork.RaiseEvent(2, eventContent2, raiseEventOptions, SendOptions.SendReliable);
			}
		}
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x0003E31C File Offset: 0x0003C51C
	public override void ReportContactWithLava(Player taggedPlayer)
	{
		if (base.photonView.IsMine && this.IsGameModeTag())
		{
			this.taggedRig = this.FindPlayerVRRig(taggedPlayer);
			if (this.taggedRig == null || this.waitingToStartNextInfectionGame || (double)Time.time < this.timeInfectedGameEnded + (double)(2f * this.tagCoolDown))
			{
				return;
			}
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			WebFlags flags = new WebFlags(1);
			raiseEventOptions.Flags = flags;
			byte b = 0;
			int num = 0;
			if (this.isCurrentlyTag)
			{
				this.ChangeCurrentIt(taggedPlayer, false);
				new Hashtable();
				b = 1;
			}
			else if (!this.currentInfected.Contains(taggedPlayer))
			{
				this.AddInfectedPlayer(taggedPlayer, false);
				num = this.currentInfected.Count;
				b = 2;
			}
			if (b == 1)
			{
				object[] eventContent = new object[]
				{
					taggedPlayer.UserId,
					taggedPlayer.UserId
				};
				PhotonNetwork.RaiseEvent(1, eventContent, raiseEventOptions, SendOptions.SendReliable);
				return;
			}
			if (b == 2)
			{
				object[] eventContent2 = new object[]
				{
					taggedPlayer.UserId,
					taggedPlayer.UserId,
					num
				};
				PhotonNetwork.RaiseEvent(2, eventContent2, raiseEventOptions, SendOptions.SendReliable);
			}
		}
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x0003E440 File Offset: 0x0003C640
	public override bool LavaWouldAffectPlayer(Player player, bool enteredLavaThisFrame)
	{
		if (!this.IsGameModeTag())
		{
			return false;
		}
		if (this.isCurrentlyTag)
		{
			return this.currentIt != player && enteredLavaThisFrame;
		}
		return !this.waitingToStartNextInfectionGame && (double)Time.time >= this.timeInfectedGameEnded + (double)(2f * this.tagCoolDown) && !this.currentInfected.Contains(player);
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x0003E4A3 File Offset: 0x0003C6A3
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		bool isMine = base.photonView.IsMine;
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x0003E4B4 File Offset: 0x0003C6B4
	public override void NewVRRig(Player player, int vrrigPhotonViewID, bool didntDoTutorial)
	{
		base.NewVRRig(player, vrrigPhotonViewID, didntDoTutorial);
		if (base.photonView.IsMine && this.IsGameModeTag())
		{
			bool flag = this.isCurrentlyTag;
			this.UpdateState();
			if (!flag && !this.isCurrentlyTag)
			{
				if (!didntDoTutorial)
				{
					this.AddInfectedPlayer(player, false);
				}
				this.UpdateInfectionState();
			}
		}
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x0003E508 File Offset: 0x0003C708
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (base.photonView.IsMine && this.IsGameModeTag())
		{
			if (this.isCurrentlyTag && otherPlayer == this.currentIt)
			{
				int num = Random.Range(0, this.currentPlayerArray.Length);
				this.ChangeCurrentIt(this.currentPlayerArray[num], false);
			}
			else if (!this.isCurrentlyTag && this.currentPlayerArray.Length >= this.infectedModeThreshold)
			{
				while (this.currentInfected.Contains(otherPlayer))
				{
					this.currentInfected.Remove(otherPlayer);
				}
				this.CopyInfectedListToArray();
				this.UpdateInfectionState();
			}
			this.UpdateState();
		}
		this.playerVRRigDict.Remove(otherPlayer.ActorNumber);
		this.playerCosmeticsLookup.Remove(otherPlayer.ActorNumber);
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x0003E5D0 File Offset: 0x0003C7D0
	private void CopyInfectedListToArray()
	{
		this.iterator1 = 0;
		while (this.iterator1 < this.currentInfectedArray.Length)
		{
			this.currentInfectedArray[this.iterator1] = 0;
			this.iterator1++;
		}
		this.iterator1 = this.currentInfected.Count - 1;
		while (this.iterator1 >= 0)
		{
			if (this.currentInfected[this.iterator1] == null)
			{
				this.currentInfected.RemoveAt(this.iterator1);
			}
			this.iterator1--;
		}
		this.iterator1 = 0;
		while (this.iterator1 < this.currentInfected.Count)
		{
			this.currentInfectedArray[this.iterator1] = this.currentInfected[this.iterator1].ActorNumber;
			this.iterator1++;
		}
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x0003E6B0 File Offset: 0x0003C8B0
	private void CopyInfectedArrayToList()
	{
		this.currentInfected.Clear();
		int num = 0;
		this.iterator1 = 0;
		while (this.iterator1 < this.currentInfectedArray.Length)
		{
			if (this.currentInfectedArray[this.iterator1] != 0)
			{
				this.tempPlayer = PhotonNetwork.LocalPlayer.Get(this.currentInfectedArray[this.iterator1]);
				if (this.tempPlayer != null)
				{
					num++;
					this.currentInfected.Add(this.tempPlayer);
				}
			}
			this.iterator1++;
		}
		Debug.Log("current infected players count is " + num.ToString());
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x0003E750 File Offset: 0x0003C950
	public void ChangeCurrentIt(Player newCurrentIt, bool withTagFreeze = true)
	{
		this.lastTag = (double)Time.time;
		this.currentIt = newCurrentIt;
		this.UpdateTagState(withTagFreeze);
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x0003E76C File Offset: 0x0003C96C
	public void SetisCurrentlyTag(bool newTagSetting)
	{
		if (newTagSetting)
		{
			this.isCurrentlyTag = true;
		}
		else
		{
			this.isCurrentlyTag = false;
		}
		foreach (Player player in this.currentPlayerArray)
		{
			if (this.FindVRRigForPlayer(player) != null)
			{
				this.FindVRRigForPlayer(player).RPC("PlayTagSound", player, new object[]
				{
					2,
					0.25f
				});
			}
		}
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x0003E7E4 File Offset: 0x0003C9E4
	public void AddInfectedPlayer(Player infectedPlayer)
	{
		if (base.photonView.IsMine)
		{
			this.currentInfected.Add(infectedPlayer);
			this.CopyInfectedListToArray();
			PhotonView photonView = this.FindVRRigForPlayer(infectedPlayer);
			photonView.RPC("SetTaggedTime", infectedPlayer, null);
			photonView.RPC("PlayTagSound", RpcTarget.All, new object[]
			{
				0,
				0.25f
			});
			this.UpdateInfectionState();
		}
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x0003E854 File Offset: 0x0003CA54
	public void AddInfectedPlayer(Player infectedPlayer, bool withTagStop)
	{
		if (base.photonView.IsMine)
		{
			this.currentInfected.Add(infectedPlayer);
			this.CopyInfectedListToArray();
			PhotonView photonView = this.FindVRRigForPlayer(infectedPlayer);
			if (!withTagStop)
			{
				photonView.RPC("SetJoinTaggedTime", infectedPlayer, null);
			}
			else
			{
				photonView.RPC("SetTaggedTime", infectedPlayer, null);
			}
			photonView.RPC("PlayTagSound", RpcTarget.All, new object[]
			{
				0,
				0.25f
			});
			this.UpdateInfectionState();
		}
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x0003E8D5 File Offset: 0x0003CAD5
	public void ClearInfectionState()
	{
		this.currentInfected.Clear();
		this.CopyInfectedListToArray();
		this.waitingToStartNextInfectionGame = false;
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x0003E8F0 File Offset: 0x0003CAF0
	public bool IsGameModeTag()
	{
		if (!this.persistedIsTag)
		{
			this.persistedIsTagResult = (this.currentRoom.CustomProperties.TryGetValue("gameMode", out this.objRef) && !this.objRef.ToString().Contains("CASUAL") && !this.objRef.ToString().Contains("city"));
			this.persistedIsTag = true;
		}
		return this.persistedIsTagResult;
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x0003E968 File Offset: 0x0003CB68
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		if (base.photonView.IsMine)
		{
			foreach (Player player in this.currentPlayerArray)
			{
				if (this.playerCosmeticsLookup.TryGetValue(player.ActorNumber, out this.tempString) && this.tempString == "BANNED")
				{
					Debug.Log("this guy needs banned: " + player.NickName);
					PhotonNetwork.CloseConnection(player);
				}
			}
			this.CopyRoomDataToLocalData();
			this.UpdateState();
		}
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x0003E9F0 File Offset: 0x0003CBF0
	public void CopyRoomDataToLocalData()
	{
		this.lastTag = 0.0;
		this.timeInfectedGameEnded = 0.0;
		this.waitingToStartNextInfectionGame = false;
		if (this.IsGameModeTag())
		{
			if (this.isCurrentlyTag)
			{
				this.UpdateTagState(true);
				return;
			}
			this.UpdateInfectionState();
		}
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x0003EA40 File Offset: 0x0003CC40
	[PunRPC]
	public override void ReportTagRPC(Player taggedPlayer, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportTagRPC");
		this.ReportTag(taggedPlayer, info.Sender);
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x0003EA5A File Offset: 0x0003CC5A
	[PunRPC]
	public override void ReportContactWithLavaRPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportContactWithLavaRPC");
		this.ReportContactWithLava(info.Sender);
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x0003EA74 File Offset: 0x0003CC74
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.isCurrentlyTag);
			stream.SendNext((this.currentIt != null) ? this.currentIt.ActorNumber : 0);
			stream.SendNext(this.currentInfectedArray[0]);
			stream.SendNext(this.currentInfectedArray[1]);
			stream.SendNext(this.currentInfectedArray[2]);
			stream.SendNext(this.currentInfectedArray[3]);
			stream.SendNext(this.currentInfectedArray[4]);
			stream.SendNext(this.currentInfectedArray[5]);
			stream.SendNext(this.currentInfectedArray[6]);
			stream.SendNext(this.currentInfectedArray[7]);
			stream.SendNext(this.currentInfectedArray[8]);
			stream.SendNext(this.currentInfectedArray[9]);
			return;
		}
		this.isCurrentlyTag = (bool)stream.ReceiveNext();
		this.tempItInt = (int)stream.ReceiveNext();
		this.currentIt = ((this.tempItInt != 0) ? PhotonNetwork.LocalPlayer.Get(this.tempItInt) : null);
		this.currentInfectedArray[0] = (int)stream.ReceiveNext();
		this.currentInfectedArray[1] = (int)stream.ReceiveNext();
		this.currentInfectedArray[2] = (int)stream.ReceiveNext();
		this.currentInfectedArray[3] = (int)stream.ReceiveNext();
		this.currentInfectedArray[4] = (int)stream.ReceiveNext();
		this.currentInfectedArray[5] = (int)stream.ReceiveNext();
		this.currentInfectedArray[6] = (int)stream.ReceiveNext();
		this.currentInfectedArray[7] = (int)stream.ReceiveNext();
		this.currentInfectedArray[8] = (int)stream.ReceiveNext();
		this.currentInfectedArray[9] = (int)stream.ReceiveNext();
		this.CopyInfectedArrayToList();
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x0003EC94 File Offset: 0x0003CE94
	public override string GameMode()
	{
		if (!this.IsGameModeTag())
		{
			return "CASUAL";
		}
		return "INFECTION";
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x0003ECA9 File Offset: 0x0003CEA9
	public override int MyMatIndex(Player forPlayer)
	{
		if (!this.IsGameModeTag())
		{
			return 0;
		}
		if (this.isCurrentlyTag && forPlayer == this.currentIt)
		{
			return 1;
		}
		if (this.currentInfected.Contains(forPlayer))
		{
			return 2;
		}
		return 0;
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x0003ECDC File Offset: 0x0003CEDC
	public override float[] LocalPlayerSpeed()
	{
		if (!this.IsGameModeTag())
		{
			this.playerSpeed[0] = this.slowJumpLimit;
			this.playerSpeed[1] = this.slowJumpMultiplier;
			return this.playerSpeed;
		}
		if (this.isCurrentlyTag)
		{
			if (PhotonNetwork.LocalPlayer == this.currentIt)
			{
				this.playerSpeed[0] = this.fastJumpLimit;
				this.playerSpeed[1] = this.fastJumpMultiplier;
				return this.playerSpeed;
			}
			this.playerSpeed[0] = this.slowJumpLimit;
			this.playerSpeed[1] = this.slowJumpMultiplier;
			return this.playerSpeed;
		}
		else
		{
			if (this.currentInfected.Contains(PhotonNetwork.LocalPlayer))
			{
				this.playerSpeed[0] = this.InterpolatedInfectedJumpSpeed(this.currentInfected.Count);
				this.playerSpeed[1] = this.InterpolatedInfectedJumpMultiplier(this.currentInfected.Count);
				return this.playerSpeed;
			}
			this.playerSpeed[0] = this.InterpolatedNoobJumpSpeed(this.currentInfected.Count);
			this.playerSpeed[1] = this.InterpolatedNoobJumpMultiplier(this.currentInfected.Count);
			return this.playerSpeed;
		}
	}

	// Token: 0x04000C6C RID: 3180
	public float tagCoolDown = 5f;

	// Token: 0x04000C6D RID: 3181
	public int infectedModeThreshold = 4;

	// Token: 0x04000C6E RID: 3182
	public const byte ReportTagEvent = 1;

	// Token: 0x04000C6F RID: 3183
	public const byte ReportInfectionTagEvent = 2;

	// Token: 0x04000C70 RID: 3184
	public List<Player> currentInfected = new List<Player>();

	// Token: 0x04000C71 RID: 3185
	public int[] currentInfectedArray = new int[10];

	// Token: 0x04000C72 RID: 3186
	public Player currentIt;

	// Token: 0x04000C73 RID: 3187
	public Player lastInfectedPlayer;

	// Token: 0x04000C74 RID: 3188
	public double lastTag;

	// Token: 0x04000C75 RID: 3189
	public double timeInfectedGameEnded;

	// Token: 0x04000C76 RID: 3190
	public bool waitingToStartNextInfectionGame;

	// Token: 0x04000C77 RID: 3191
	public bool isCurrentlyTag;

	// Token: 0x04000C78 RID: 3192
	public bool isCasual;

	// Token: 0x04000C79 RID: 3193
	private int tempItInt;

	// Token: 0x04000C7A RID: 3194
	public object objRef;

	// Token: 0x04000C7B RID: 3195
	private int iterator1;

	// Token: 0x04000C7C RID: 3196
	private Player tempPlayer;

	// Token: 0x04000C7D RID: 3197
	private bool allInfected;

	// Token: 0x04000C7E RID: 3198
	private List<Player> returnPlayerList = new List<Player>();

	// Token: 0x04000C7F RID: 3199
	private bool persistedIsTag;

	// Token: 0x04000C80 RID: 3200
	private bool persistedIsTagResult;

	// Token: 0x04000C81 RID: 3201
	public float[] inspectorLocalPlayerSpeed;

	// Token: 0x04000C82 RID: 3202
	private VRRig taggingRig;

	// Token: 0x04000C83 RID: 3203
	private VRRig taggedRig;
}
