using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GorillaTagManager : GorillaGameManager
{
	public override void StartPlaying()
	{
		base.StartPlaying();
		if (PhotonNetwork.IsMasterClient)
		{
			for (int i = 0; i < this.currentInfected.Count; i++)
			{
				this.tempPlayer = this.currentInfected[i];
				if (this.tempPlayer == null || !this.tempPlayer.InRoom())
				{
					this.currentInfected.RemoveAt(i);
					i--;
				}
			}
			if (this.currentIt != null && !this.currentIt.InRoom())
			{
				this.currentIt = null;
			}
			if (this.lastInfectedPlayer != null && !this.lastInfectedPlayer.InRoom())
			{
				this.lastInfectedPlayer = null;
			}
			this.UpdateState();
		}
	}

	public override void StopPlaying()
	{
		base.StopPlaying();
		base.StopAllCoroutines();
	}

	public override void Reset()
	{
		base.Reset();
		for (int i = 0; i < this.currentInfectedArray.Length; i++)
		{
			this.currentInfectedArray[i] = 0;
		}
		this.currentInfected.Clear();
		this.lastTag = 0.0;
		this.timeInfectedGameEnded = 0.0;
		this.allInfected = false;
		this.isCurrentlyTag = false;
		this.waitingToStartNextInfectionGame = false;
		this.currentIt = null;
		this.lastInfectedPlayer = null;
	}

	public void UpdateState()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			if (GameMode.ParticipatingPlayers.Count < 1)
			{
				this.isCurrentlyTag = true;
				this.ClearInfectionState();
				this.lastInfectedPlayer = null;
				this.currentIt = null;
				return;
			}
			if (this.isCurrentlyTag && this.currentIt == null)
			{
				int index = Random.Range(0, GameMode.ParticipatingPlayers.Count);
				this.ChangeCurrentIt(GameMode.ParticipatingPlayers[index], false);
			}
			else if (this.isCurrentlyTag && GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold)
			{
				this.SetisCurrentlyTag(false);
				this.ClearInfectionState();
				int index2 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
				this.AddInfectedPlayer(GameMode.ParticipatingPlayers[index2]);
				this.lastInfectedPlayer = GameMode.ParticipatingPlayers[index2];
			}
			else if (!this.isCurrentlyTag && GameMode.ParticipatingPlayers.Count < this.infectedModeThreshold)
			{
				this.ClearInfectionState();
				this.lastInfectedPlayer = null;
				this.SetisCurrentlyTag(true);
				int index3 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
				this.ChangeCurrentIt(GameMode.ParticipatingPlayers[index3], false);
			}
			else if (!this.isCurrentlyTag && this.currentInfected.Count == 0)
			{
				int index4 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
				this.AddInfectedPlayer(GameMode.ParticipatingPlayers[index4]);
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
		base.InfrequentUpdate();
		if (PhotonNetwork.IsMasterClient)
		{
			this.UpdateState();
		}
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
			this.ClearInfectionState();
			List<Player> participatingPlayers = GameMode.ParticipatingPlayers;
			if (participatingPlayers.Count > 0)
			{
				int index = Random.Range(0, participatingPlayers.Count);
				int num = 0;
				while (num < 10 && participatingPlayers[index] == this.lastInfectedPlayer)
				{
					index = Random.Range(0, participatingPlayers.Count);
					num++;
				}
				this.AddInfectedPlayer(participatingPlayers[index]);
				this.lastInfectedPlayer = participatingPlayers[index];
				this.lastTag = (double)Time.time;
			}
		}
		yield return null;
		yield break;
	}

	public void UpdateInfectionState()
	{
		if (!PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			return;
		}
		this.allInfected = true;
		foreach (Player item in GameMode.ParticipatingPlayers)
		{
			if (!this.currentInfected.Contains(item))
			{
				this.allInfected = false;
				break;
			}
		}
		if (!this.isCurrentlyTag && !this.waitingToStartNextInfectionGame && this.allInfected)
		{
			this.EndInfectionGame();
		}
	}

	public void UpdateTagState(bool withTagFreeze = true)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		foreach (Player player in GameMode.ParticipatingPlayers)
		{
			if (this.currentIt == player)
			{
				if (withTagFreeze)
				{
					RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, player);
				}
				else
				{
					RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.JoinedTaggedTime, player);
				}
				RoomSystem.SendSoundEffectOnOther(0, 0.25f, player);
				break;
			}
		}
	}

	private void EndInfectionGame()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			foreach (Player player in GameMode.ParticipatingPlayers)
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, player);
				RoomSystem.SendSoundEffectToPlayer(2, 0.25f, player);
			}
			this.waitingToStartNextInfectionGame = true;
			this.timeInfectedGameEnded = (double)Time.time;
			base.StartCoroutine(this.InfectionEnd());
		}
	}

	public override bool LocalCanTag(Player myPlayer, Player otherPlayer)
	{
		if (this.isCurrentlyTag)
		{
			return myPlayer == this.currentIt && myPlayer != otherPlayer;
		}
		return this.currentInfected.Contains(myPlayer) && !this.currentInfected.Contains(otherPlayer);
	}

	private float InterpolatedInfectedJumpMultiplier(int infectedCount)
	{
		if (GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.fastJumpMultiplier;
		}
		return (this.fastJumpMultiplier - this.slowJumpMultiplier) / (float)(GameMode.ParticipatingPlayers.Count - 1) * (float)(GameMode.ParticipatingPlayers.Count - infectedCount) + this.slowJumpMultiplier;
	}

	private float InterpolatedInfectedJumpSpeed(int infectedCount)
	{
		if (GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.fastJumpLimit;
		}
		return (this.fastJumpLimit - this.slowJumpLimit) / (float)(GameMode.ParticipatingPlayers.Count - 1) * (float)(GameMode.ParticipatingPlayers.Count - infectedCount) + this.slowJumpLimit;
	}

	private float InterpolatedNoobJumpMultiplier(int infectedCount)
	{
		if (GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.slowJumpMultiplier;
		}
		return (this.fastJumpMultiplier - this.slowJumpMultiplier) / (float)(GameMode.ParticipatingPlayers.Count - 1) * (float)(infectedCount - 1) * 0.9f + this.slowJumpMultiplier;
	}

	private float InterpolatedNoobJumpSpeed(int infectedCount)
	{
		if (GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.slowJumpLimit;
		}
		return (this.fastJumpLimit - this.fastJumpLimit) / (float)(GameMode.ParticipatingPlayers.Count - 1) * (float)(infectedCount - 1) * 0.9f + this.slowJumpLimit;
	}

	public override void ReportTag(Player taggedPlayer, Player taggingPlayer)
	{
		if (PhotonNetwork.LocalPlayer.IsMasterClient)
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
				if (!this.taggingRig.CheckDistance(this.taggedRig.transform.position, 6f) && !this.taggingRig.CheckTagDistanceRollback(this.taggedRig, 6f, 0.2f))
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

	public override void HitPlayer(Player taggedPlayer)
	{
		if (PhotonNetwork.LocalPlayer.IsMasterClient)
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

	public override bool CanAffectPlayer(Player player, bool thisFrame)
	{
		if (this.isCurrentlyTag)
		{
			return this.currentIt != player && thisFrame;
		}
		return !this.waitingToStartNextInfectionGame && (double)Time.time >= this.timeInfectedGameEnded + (double)(2f * this.tagCoolDown) && !this.currentInfected.Contains(player);
	}

	public bool IsInfected(Player player)
	{
		if (this.isCurrentlyTag)
		{
			return this.currentIt == player;
		}
		return !this.waitingToStartNextInfectionGame && (double)Time.time >= this.timeInfectedGameEnded + (double)(2f * this.tagCoolDown) && this.currentInfected.Contains(player);
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		bool isMasterClient = PhotonNetwork.LocalPlayer.IsMasterClient;
	}

	public override void NewVRRig(Player player, int vrrigPhotonViewID, bool didTutorial)
	{
		base.NewVRRig(player, vrrigPhotonViewID, didTutorial);
		if (PhotonNetwork.IsMasterClient)
		{
			bool flag = this.isCurrentlyTag;
			this.UpdateState();
			if (!flag && !this.isCurrentlyTag)
			{
				if (didTutorial)
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
		if (PhotonNetwork.IsMasterClient)
		{
			if (this.isCurrentlyTag && otherPlayer == this.currentIt)
			{
				if (GameMode.ParticipatingPlayers.Count > 0)
				{
					int index = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.ChangeCurrentIt(GameMode.ParticipatingPlayers[index], false);
				}
			}
			else if (!this.isCurrentlyTag && GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold)
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
		RoomSystem.SendSoundEffectAll(2, 0.25f);
	}

	public void AddInfectedPlayer(Player infectedPlayer)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.currentInfected.Add(infectedPlayer);
			this.CopyInfectedListToArray();
			RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, infectedPlayer);
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, infectedPlayer);
			this.UpdateInfectionState();
		}
	}

	public void AddInfectedPlayer(Player infectedPlayer, bool withTagStop)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.currentInfected.Add(infectedPlayer);
			this.CopyInfectedListToArray();
			if (!withTagStop)
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.JoinedTaggedTime, infectedPlayer);
			}
			else
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, infectedPlayer);
			}
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, infectedPlayer);
			this.UpdateInfectionState();
		}
	}

	public void ClearInfectionState()
	{
		this.currentInfected.Clear();
		this.CopyInfectedListToArray();
		this.waitingToStartNextInfectionGame = false;
	}

	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		if (PhotonNetwork.IsMasterClient)
		{
			this.CopyRoomDataToLocalData();
			this.UpdateState();
		}
	}

	public void CopyRoomDataToLocalData()
	{
		this.lastTag = 0.0;
		this.timeInfectedGameEnded = 0.0;
		this.waitingToStartNextInfectionGame = false;
		if (this.isCurrentlyTag)
		{
			this.UpdateTagState(true);
			return;
		}
		this.UpdateInfectionState();
	}

	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
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
	}

	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
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

	public override GameModeType GameType()
	{
		return GameModeType.Infection;
	}

	public override string GameModeName()
	{
		return "INFECTION";
	}

	public override int MyMatIndex(Player forPlayer)
	{
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

	public GorillaTagManager()
	{
	}

	public float tagCoolDown = 5f;

	public int infectedModeThreshold = 4;

	public const byte ReportTagEvent = 1;

	public const byte ReportInfectionTagEvent = 2;

	public List<Player> currentInfected = new List<Player>(10);

	public int[] currentInfectedArray = new int[10];

	public Player currentIt;

	public Player lastInfectedPlayer;

	public double lastTag;

	public double timeInfectedGameEnded;

	public bool waitingToStartNextInfectionGame;

	public bool isCurrentlyTag;

	private int tempItInt;

	private int iterator1;

	private Player tempPlayer;

	private bool allInfected;

	public float[] inspectorLocalPlayerSpeed;

	private VRRig taggingRig;

	private VRRig taggedRig;

	[CompilerGenerated]
	private sealed class <InfectionEnd>d__24 : IEnumerator<object>, IEnumerator, IDisposable
	{
		[DebuggerHidden]
		public <InfectionEnd>d__24(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			GorillaTagManager gorillaTagManager = this;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				break;
			case 1:
				this.<>1__state = -1;
				break;
			case 2:
				this.<>1__state = -1;
				return false;
			default:
				return false;
			}
			if ((double)Time.time >= gorillaTagManager.timeInfectedGameEnded + (double)gorillaTagManager.tagCoolDown)
			{
				if (!gorillaTagManager.isCurrentlyTag && gorillaTagManager.waitingToStartNextInfectionGame)
				{
					gorillaTagManager.ClearInfectionState();
					List<Player> participatingPlayers = GameMode.ParticipatingPlayers;
					if (participatingPlayers.Count > 0)
					{
						int index = Random.Range(0, participatingPlayers.Count);
						int num2 = 0;
						while (num2 < 10 && participatingPlayers[index] == gorillaTagManager.lastInfectedPlayer)
						{
							index = Random.Range(0, participatingPlayers.Count);
							num2++;
						}
						gorillaTagManager.AddInfectedPlayer(participatingPlayers[index]);
						gorillaTagManager.lastInfectedPlayer = participatingPlayers[index];
						gorillaTagManager.lastTag = (double)Time.time;
					}
				}
				this.<>2__current = null;
				this.<>1__state = 2;
				return true;
			}
			this.<>2__current = new WaitForSeconds(0.1f);
			this.<>1__state = 1;
			return true;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		private int <>1__state;

		private object <>2__current;

		public GorillaTagManager <>4__this;
	}
}
