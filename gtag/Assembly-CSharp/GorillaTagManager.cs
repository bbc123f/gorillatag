using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GorillaTagManager : GorillaGameManager, IInRoomCallbacks, IMatchmakingCallbacks, IPunObservable
{
	public override void Awake()
	{
		base.Awake();
		if (base.photonView.IsMine)
		{
			this.isCurrentlyTag = true;
		}
		Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
	}

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

	private float InterpolatedInfectedJumpMultiplier(int infectedCount)
	{
		if (this.currentPlayerArray.Length == 1)
		{
			return this.fastJumpMultiplier;
		}
		return (this.fastJumpMultiplier - this.slowJumpMultiplier) / (float)(this.currentPlayerArray.Length - 1) * (float)(this.currentPlayerArray.Length - infectedCount) + this.slowJumpMultiplier;
	}

	private float InterpolatedInfectedJumpSpeed(int infectedCount)
	{
		if (this.currentPlayerArray.Length == 1)
		{
			return this.fastJumpLimit;
		}
		return (this.fastJumpLimit - this.slowJumpLimit) / (float)(this.currentPlayerArray.Length - 1) * (float)(this.currentPlayerArray.Length - infectedCount) + this.slowJumpLimit;
	}

	private float InterpolatedNoobJumpMultiplier(int infectedCount)
	{
		if (this.currentPlayerArray.Length == 1)
		{
			return this.slowJumpMultiplier;
		}
		return (this.fastJumpMultiplier - this.slowJumpMultiplier) / (float)(this.currentPlayerArray.Length - 1) * (float)(infectedCount - 1) * 0.9f + this.slowJumpMultiplier;
	}

	private float InterpolatedNoobJumpSpeed(int infectedCount)
	{
		if (this.currentPlayerArray.Length == 1)
		{
			return this.slowJumpLimit;
		}
		return (this.fastJumpLimit - this.fastJumpLimit) / (float)(this.currentPlayerArray.Length - 1) * (float)(infectedCount - 1) * 0.9f + this.slowJumpLimit;
	}

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
				if (!this.taggingRig.CheckDistance(this.taggedRig.transform.position, this.tagDistanceThreshold))
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

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		bool isMine = base.photonView.IsMine;
	}

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

	public void ChangeCurrentIt(Player newCurrentIt, bool withTagFreeze = true)
	{
		this.lastTag = (double)Time.time;
		this.currentIt = newCurrentIt;
		this.UpdateTagState(withTagFreeze);
	}

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

	public void ClearInfectionState()
	{
		this.currentInfected.Clear();
		this.CopyInfectedListToArray();
		this.waitingToStartNextInfectionGame = false;
	}

	public bool IsGameModeTag()
	{
		if (!this.persistedIsTag)
		{
			this.persistedIsTagResult = (this.currentRoom.CustomProperties.TryGetValue("gameMode", out this.objRef) && !this.objRef.ToString().Contains("CASUAL") && !this.objRef.ToString().Contains("city"));
			this.persistedIsTag = true;
		}
		return this.persistedIsTagResult;
	}

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

	[PunRPC]
	public override void ReportTagRPC(Player taggedPlayer, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportTagRPC");
		this.ReportTag(taggedPlayer, info.Sender);
	}

	[PunRPC]
	public override void ReportContactWithLavaRPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportContactWithLavaRPC");
		this.ReportContactWithLava(info.Sender);
	}

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

	public override string GameMode()
	{
		if (!this.IsGameModeTag())
		{
			return "CASUAL";
		}
		return "INFECTION";
	}

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

	public float tagCoolDown = 5f;

	public int infectedModeThreshold = 4;

	public const byte ReportTagEvent = 1;

	public const byte ReportInfectionTagEvent = 2;

	public List<Player> currentInfected = new List<Player>();

	public int[] currentInfectedArray = new int[10];

	public Player currentIt;

	public Player lastInfectedPlayer;

	public double lastTag;

	public double timeInfectedGameEnded;

	public bool waitingToStartNextInfectionGame;

	public bool isCurrentlyTag;

	public bool isCasual;

	private int tempItInt;

	public object objRef;

	private int iterator1;

	private Player tempPlayer;

	private bool allInfected;

	private List<Player> returnPlayerList = new List<Player>();

	private bool persistedIsTag;

	private bool persistedIsTagResult;

	public float[] inspectorLocalPlayerSpeed;

	private VRRig taggingRig;

	private VRRig taggedRig;
}
