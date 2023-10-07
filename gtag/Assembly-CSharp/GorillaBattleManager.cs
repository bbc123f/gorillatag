using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000167 RID: 359
public class GorillaBattleManager : GorillaGameManager, IInRoomCallbacks, IMatchmakingCallbacks, IPunObservable
{
	// Token: 0x060008E3 RID: 2275 RVA: 0x00035F8D File Offset: 0x0003418D
	private void ActivateBattleBalloons(bool enable)
	{
		if (GorillaTagger.Instance.offlineVRRig != null)
		{
			GorillaTagger.Instance.offlineVRRig.battleBalloons.gameObject.SetActive(enable);
		}
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x00035FBB File Offset: 0x000341BB
	private bool HasFlag(GorillaBattleManager.BattleStatus state, GorillaBattleManager.BattleStatus statusFlag)
	{
		return (state & statusFlag) > GorillaBattleManager.BattleStatus.None;
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x00035FC3 File Offset: 0x000341C3
	public override string GameMode()
	{
		return "BATTLE";
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x00035FCC File Offset: 0x000341CC
	private void ActivateDefaultSlingShot()
	{
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if (offlineVRRig != null && !Slingshot.IsSlingShotEnabled())
		{
			CosmeticsController instance = CosmeticsController.instance;
			CosmeticsController.CosmeticItem itemFromDict = instance.GetItemFromDict("Slingshot");
			instance.ApplyCosmeticItemToSet(offlineVRRig.cosmeticSet, itemFromDict, true, false);
		}
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x00036016 File Offset: 0x00034216
	public override void Awake()
	{
		base.Awake();
		this.coroutineRunning = false;
		Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
		if (base.photonView.IsMine)
		{
			this.currentState = GorillaBattleManager.BattleState.NotEnoughPlayers;
		}
		this.ActivateBattleBalloons(true);
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x0003604F File Offset: 0x0003424F
	private void Transition(GorillaBattleManager.BattleState newState)
	{
		this.currentState = newState;
		Debug.Log("current state is: " + this.currentState.ToString());
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x00036078 File Offset: 0x00034278
	public void UpdateBattleState()
	{
		if (base.photonView.IsMine)
		{
			switch (this.currentState)
			{
			case GorillaBattleManager.BattleState.NotEnoughPlayers:
				if ((float)this.currentPlayerArray.Length >= this.playerMin)
				{
					this.Transition(GorillaBattleManager.BattleState.StartCountdown);
				}
				break;
			case GorillaBattleManager.BattleState.GameEnd:
				if (this.EndBattleGame())
				{
					this.Transition(GorillaBattleManager.BattleState.GameEndWaiting);
				}
				break;
			case GorillaBattleManager.BattleState.GameEndWaiting:
				if (this.BattleEnd())
				{
					this.Transition(GorillaBattleManager.BattleState.StartCountdown);
				}
				break;
			case GorillaBattleManager.BattleState.StartCountdown:
				this.RandomizeTeams();
				this.ActivateBattleBalloons(true);
				base.StartCoroutine(this.StartBattleCountdown());
				this.Transition(GorillaBattleManager.BattleState.CountingDownToStart);
				break;
			case GorillaBattleManager.BattleState.CountingDownToStart:
				if (!this.coroutineRunning)
				{
					this.Transition(GorillaBattleManager.BattleState.StartCountdown);
				}
				break;
			case GorillaBattleManager.BattleState.GameStart:
				this.StartBattle();
				this.Transition(GorillaBattleManager.BattleState.GameRunning);
				break;
			case GorillaBattleManager.BattleState.GameRunning:
				if (this.CheckForGameEnd())
				{
					this.Transition(GorillaBattleManager.BattleState.GameEnd);
				}
				if ((float)this.currentPlayerArray.Length < this.playerMin)
				{
					this.InitializePlayerStatus();
					this.ActivateBattleBalloons(false);
					this.Transition(GorillaBattleManager.BattleState.NotEnoughPlayers);
				}
				break;
			}
			this.UpdatePlayerStatus();
		}
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x00036184 File Offset: 0x00034384
	private bool CheckForGameEnd()
	{
		this.bcount = 0;
		this.rcount = 0;
		foreach (Player player in this.currentPlayerArray)
		{
			if (this.playerLives.TryGetValue(player.ActorNumber, out this.lives))
			{
				if (this.lives > 0 && this.playerStatusDict.TryGetValue(player.ActorNumber, out this.tempStatus))
				{
					if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.RedTeam))
					{
						this.rcount++;
					}
					else if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.BlueTeam))
					{
						this.bcount++;
					}
				}
			}
			else
			{
				this.playerLives.Add(player.ActorNumber, 0);
			}
		}
		return this.bcount == 0 || this.rcount == 0;
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x0003625C File Offset: 0x0003445C
	public IEnumerator StartBattleCountdown()
	{
		this.coroutineRunning = true;
		this.countDownTime = 5;
		while (this.countDownTime > 0)
		{
			try
			{
				foreach (Player player in PhotonNetwork.PlayerList)
				{
					this.playerLives[player.ActorNumber] = 3;
					PhotonView photonView = this.FindVRRigForPlayer(player);
					if (photonView != null)
					{
						photonView.RPC("PlayTagSound", player, new object[]
						{
							6,
							0.25f
						});
					}
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
			this.countDownTime--;
		}
		this.coroutineRunning = false;
		this.currentState = GorillaBattleManager.BattleState.GameStart;
		yield return null;
		yield break;
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x0003626C File Offset: 0x0003446C
	public void StartBattle()
	{
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			this.playerLives[player.ActorNumber] = 3;
			PhotonView photonView = this.FindVRRigForPlayer(player);
			if (photonView != null)
			{
				photonView.RPC("PlayTagSound", player, new object[]
				{
					7,
					0.5f
				});
			}
		}
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x000362DC File Offset: 0x000344DC
	private bool EndBattleGame()
	{
		if ((float)PhotonNetwork.PlayerList.Length >= this.playerMin)
		{
			foreach (Player player in PhotonNetwork.PlayerList)
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
			this.timeBattleEnded = Time.time;
			return true;
		}
		return false;
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x00036365 File Offset: 0x00034565
	public bool BattleEnd()
	{
		return Time.time > this.timeBattleEnded + this.tagCoolDown;
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x0003637B File Offset: 0x0003457B
	public bool SlingshotHit(Player myPlayer, Player otherPlayer)
	{
		return this.playerLives.TryGetValue(otherPlayer.ActorNumber, out this.lives) && this.lives > 0;
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x000363A4 File Offset: 0x000345A4
	[PunRPC]
	public void ReportSlingshotHit(Player taggedPlayer, Vector3 hitLocation, int projectileCount, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportSlingshotHit");
		if (!base.photonView.IsMine)
		{
			return;
		}
		if (this.currentState != GorillaBattleManager.BattleState.GameRunning)
		{
			return;
		}
		if (this.OnSameTeam(taggedPlayer, info.Sender))
		{
			return;
		}
		if (this.GetPlayerLives(taggedPlayer) > 0 && this.GetPlayerLives(info.Sender) > 0 && !this.PlayerInHitCooldown(taggedPlayer))
		{
			if (!this.playerHitTimes.TryGetValue(taggedPlayer.ActorNumber, out this.outHitTime))
			{
				this.playerHitTimes.Add(taggedPlayer.ActorNumber, Time.time);
			}
			else
			{
				this.playerHitTimes[taggedPlayer.ActorNumber] = Time.time;
			}
			Dictionary<int, int> dictionary = this.playerLives;
			int actorNumber = taggedPlayer.ActorNumber;
			int num = dictionary[actorNumber];
			dictionary[actorNumber] = num - 1;
			this.tempView = this.FindVRRigForPlayer(taggedPlayer);
			if (this.tempView != null)
			{
				this.tempView.RPC("PlayTagSound", RpcTarget.All, new object[]
				{
					0,
					0.25f
				});
				return;
			}
		}
		else if (this.GetPlayerLives(info.Sender) == 0 && this.GetPlayerLives(taggedPlayer) > 0)
		{
			this.tempStatus = this.GetPlayerStatus(taggedPlayer);
			if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Normal) && !this.PlayerInHitCooldown(taggedPlayer) && !this.PlayerInStunCooldown(taggedPlayer))
			{
				if (!this.playerStunTimes.TryGetValue(taggedPlayer.ActorNumber, out this.outHitTime))
				{
					this.playerStunTimes.Add(taggedPlayer.ActorNumber, Time.time);
				}
				else
				{
					this.playerStunTimes[taggedPlayer.ActorNumber] = Time.time;
				}
				this.tempView = this.FindVRRigForPlayer(taggedPlayer);
				if (this.tempView != null)
				{
					this.tempView.RPC("SetSlowedTime", taggedPlayer, null);
					this.tempView.RPC("PlayTagSound", RpcTarget.All, new object[]
					{
						5,
						0.125f
					});
				}
			}
		}
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x000365BC File Offset: 0x000347BC
	public override void ReportContactWithLava(Player player)
	{
		if (!base.photonView.IsMine || this.currentState != GorillaBattleManager.BattleState.GameRunning)
		{
			return;
		}
		if (this.GetPlayerLives(player) > 0)
		{
			this.playerLives[player.ActorNumber] = 0;
			this.tempView = this.FindVRRigForPlayer(player);
			if (this.tempView != null)
			{
				this.tempView.RPC("PlayTagSound", RpcTarget.All, new object[]
				{
					0,
					0.25f
				});
			}
		}
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x00036644 File Offset: 0x00034844
	public override bool LavaWouldAffectPlayer(Player player, bool enteredLavaThisFrame)
	{
		return this.playerLives.TryGetValue(player.ActorNumber, out this.lives) && this.lives > 0;
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x0003666C File Offset: 0x0003486C
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (base.photonView.IsMine)
		{
			if (this.currentState == GorillaBattleManager.BattleState.GameRunning)
			{
				this.playerLives.Add(newPlayer.ActorNumber, 0);
			}
			else
			{
				this.playerLives.Add(newPlayer.ActorNumber, 3);
			}
			this.playerStatusDict.Add(newPlayer.ActorNumber, GorillaBattleManager.BattleStatus.None);
			this.CopyBattleDictToArray();
			this.AddPlayerToCorrectTeam(newPlayer);
		}
		this.playerProjectiles.Add(newPlayer, new List<GorillaGameManager.ProjectileInfo>());
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x000366EC File Offset: 0x000348EC
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (this.playerLives.ContainsKey(otherPlayer.ActorNumber))
		{
			this.playerLives.Remove(otherPlayer.ActorNumber);
		}
		if (this.playerStatusDict.ContainsKey(otherPlayer.ActorNumber))
		{
			this.playerStatusDict.Remove(otherPlayer.ActorNumber);
		}
		if (this.playerProjectiles.ContainsKey(otherPlayer))
		{
			this.playerProjectiles.Remove(otherPlayer);
		}
		this.playerVRRigDict.Remove(otherPlayer.ActorNumber);
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x00036777 File Offset: 0x00034977
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x0003677C File Offset: 0x0003497C
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		if (stream.IsWriting)
		{
			this.CopyBattleDictToArray();
			for (int i = 0; i < this.playerLivesArray.Length; i++)
			{
				stream.SendNext(this.playerActorNumberArray[i]);
				stream.SendNext(this.playerLivesArray[i]);
				stream.SendNext(this.playerStatusArray[i]);
			}
			stream.SendNext((int)this.currentState);
			return;
		}
		for (int j = 0; j < this.playerLivesArray.Length; j++)
		{
			this.playerActorNumberArray[j] = (int)stream.ReceiveNext();
			this.playerLivesArray[j] = (int)stream.ReceiveNext();
			this.playerStatusArray[j] = (GorillaBattleManager.BattleStatus)stream.ReceiveNext();
		}
		this.currentState = (GorillaBattleManager.BattleState)stream.ReceiveNext();
		this.CopyArrayToBattleDict();
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x00036868 File Offset: 0x00034A68
	public override int MyMatIndex(Player forPlayer)
	{
		this.tempStatus = this.GetPlayerStatus(forPlayer);
		if (this.tempStatus != GorillaBattleManager.BattleStatus.None)
		{
			if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.RedTeam))
			{
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Normal))
				{
					return 8;
				}
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Hit))
				{
					return 9;
				}
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Stunned))
				{
					return 10;
				}
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Grace))
				{
					return 10;
				}
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Eliminated))
				{
					return 11;
				}
			}
			else
			{
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Normal))
				{
					return 4;
				}
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Hit))
				{
					return 5;
				}
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Stunned))
				{
					return 6;
				}
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Grace))
				{
					return 6;
				}
				if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Eliminated))
				{
					return 7;
				}
			}
		}
		return 0;
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x00036954 File Offset: 0x00034B54
	public override float[] LocalPlayerSpeed()
	{
		if (this.playerStatusDict.TryGetValue(PhotonNetwork.LocalPlayer.ActorNumber, out this.tempStatus))
		{
			if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Normal))
			{
				this.playerSpeed[0] = 6.5f;
				this.playerSpeed[1] = 1.1f;
				return this.playerSpeed;
			}
			if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Stunned))
			{
				this.playerSpeed[0] = 2f;
				this.playerSpeed[1] = 0.5f;
				return this.playerSpeed;
			}
			if (this.HasFlag(this.tempStatus, GorillaBattleManager.BattleStatus.Eliminated))
			{
				this.playerSpeed[0] = this.fastJumpLimit;
				this.playerSpeed[1] = this.fastJumpMultiplier;
				return this.playerSpeed;
			}
		}
		this.playerSpeed[0] = 6.5f;
		this.playerSpeed[1] = 1.1f;
		return this.playerSpeed;
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x00036A35 File Offset: 0x00034C35
	public override void Update()
	{
		base.Update();
		if (base.photonView.IsMine)
		{
			this.UpdateBattleState();
		}
		this.ActivateDefaultSlingShot();
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x00036A58 File Offset: 0x00034C58
	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
		foreach (int num in this.playerLives.Keys)
		{
			this.playerInList = false;
			Player[] currentPlayerArray = this.currentPlayerArray;
			for (int i = 0; i < currentPlayerArray.Length; i++)
			{
				if (currentPlayerArray[i].ActorNumber == num)
				{
					this.playerInList = true;
				}
			}
			if (!this.playerInList)
			{
				this.playerLives.Remove(num);
			}
		}
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x00036AF4 File Offset: 0x00034CF4
	public int GetPlayerLives(Player player)
	{
		if (player == null)
		{
			return 0;
		}
		if (this.playerLives.TryGetValue(player.ActorNumber, out this.outLives))
		{
			return this.outLives;
		}
		return 0;
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x00036B1C File Offset: 0x00034D1C
	public bool PlayerInHitCooldown(Player player)
	{
		float num;
		return this.playerHitTimes.TryGetValue(player.ActorNumber, out num) && num + this.hitCooldown > Time.time;
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x00036B50 File Offset: 0x00034D50
	public bool PlayerInStunCooldown(Player player)
	{
		float num;
		return this.playerStunTimes.TryGetValue(player.ActorNumber, out num) && num + this.hitCooldown + this.stunGracePeriod > Time.time;
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x00036B8A File Offset: 0x00034D8A
	public GorillaBattleManager.BattleStatus GetPlayerStatus(Player player)
	{
		if (this.playerStatusDict.TryGetValue(player.ActorNumber, out this.tempStatus))
		{
			return this.tempStatus;
		}
		return GorillaBattleManager.BattleStatus.None;
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x00036BAD File Offset: 0x00034DAD
	public bool OnRedTeam(GorillaBattleManager.BattleStatus status)
	{
		return this.HasFlag(status, GorillaBattleManager.BattleStatus.RedTeam);
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x00036BB8 File Offset: 0x00034DB8
	public bool OnRedTeam(Player player)
	{
		GorillaBattleManager.BattleStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnRedTeam(playerStatus);
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x00036BD4 File Offset: 0x00034DD4
	public bool OnBlueTeam(GorillaBattleManager.BattleStatus status)
	{
		return this.HasFlag(status, GorillaBattleManager.BattleStatus.BlueTeam);
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x00036BE0 File Offset: 0x00034DE0
	public bool OnBlueTeam(Player player)
	{
		GorillaBattleManager.BattleStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnBlueTeam(playerStatus);
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x00036BFC File Offset: 0x00034DFC
	public bool OnNoTeam(GorillaBattleManager.BattleStatus status)
	{
		return !this.OnRedTeam(status) && !this.OnBlueTeam(status);
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x00036C14 File Offset: 0x00034E14
	public bool OnNoTeam(Player player)
	{
		GorillaBattleManager.BattleStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnNoTeam(playerStatus);
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x00036C30 File Offset: 0x00034E30
	public override bool LocalCanTag(Player myPlayer, Player otherPlayer)
	{
		return false;
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x00036C34 File Offset: 0x00034E34
	public bool OnSameTeam(GorillaBattleManager.BattleStatus playerA, GorillaBattleManager.BattleStatus playerB)
	{
		bool flag = this.OnRedTeam(playerA) && this.OnRedTeam(playerB);
		bool flag2 = this.OnBlueTeam(playerA) && this.OnBlueTeam(playerB);
		return flag || flag2;
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x00036C6C File Offset: 0x00034E6C
	public bool OnSameTeam(Player myPlayer, Player otherPlayer)
	{
		GorillaBattleManager.BattleStatus playerStatus = this.GetPlayerStatus(myPlayer);
		GorillaBattleManager.BattleStatus playerStatus2 = this.GetPlayerStatus(otherPlayer);
		return this.OnSameTeam(playerStatus, playerStatus2);
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x00036C94 File Offset: 0x00034E94
	public bool LocalCanHit(Player myPlayer, Player otherPlayer)
	{
		bool flag = !this.OnSameTeam(myPlayer, otherPlayer);
		bool flag2 = this.GetPlayerLives(otherPlayer) != 0;
		return flag && flag2;
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x00036CBC File Offset: 0x00034EBC
	private void CopyBattleDictToArray()
	{
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			this.playerLivesArray[i] = 0;
			this.playerActorNumberArray[i] = 0;
		}
		this.keyValuePairs = this.playerLives.ToArray<KeyValuePair<int, int>>();
		int num = 0;
		while (num < this.playerLivesArray.Length && num < this.keyValuePairs.Length)
		{
			this.playerActorNumberArray[num] = this.keyValuePairs[num].Key;
			this.playerLivesArray[num] = this.keyValuePairs[num].Value;
			this.playerStatusArray[num] = this.GetPlayerStatus(PhotonNetwork.LocalPlayer.Get(this.keyValuePairs[num].Key));
			num++;
		}
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x00036D78 File Offset: 0x00034F78
	private void CopyArrayToBattleDict()
	{
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			if (this.playerActorNumberArray[i] != 0)
			{
				if (this.playerLives.TryGetValue(this.playerActorNumberArray[i], out this.outLives))
				{
					this.playerLives[this.playerActorNumberArray[i]] = this.playerLivesArray[i];
				}
				else
				{
					this.playerLives.Add(this.playerActorNumberArray[i], this.playerLivesArray[i]);
				}
				if (this.playerStatusDict.ContainsKey(this.playerActorNumberArray[i]))
				{
					this.playerStatusDict[this.playerActorNumberArray[i]] = this.playerStatusArray[i];
				}
				else
				{
					this.playerStatusDict.Add(this.playerActorNumberArray[i], this.playerStatusArray[i]);
				}
			}
		}
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x00036E4B File Offset: 0x0003504B
	private GorillaBattleManager.BattleStatus SetFlag(GorillaBattleManager.BattleStatus currState, GorillaBattleManager.BattleStatus flag)
	{
		return currState | flag;
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x00036E50 File Offset: 0x00035050
	private GorillaBattleManager.BattleStatus SetFlagExclusive(GorillaBattleManager.BattleStatus currState, GorillaBattleManager.BattleStatus flag)
	{
		return flag;
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x00036E53 File Offset: 0x00035053
	private GorillaBattleManager.BattleStatus ClearFlag(GorillaBattleManager.BattleStatus currState, GorillaBattleManager.BattleStatus flag)
	{
		return currState & ~flag;
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x00036E59 File Offset: 0x00035059
	private bool FlagIsSet(GorillaBattleManager.BattleStatus currState, GorillaBattleManager.BattleStatus flag)
	{
		return (currState & flag) > GorillaBattleManager.BattleStatus.None;
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x00036E64 File Offset: 0x00035064
	public void RandomizeTeams()
	{
		int[] array = new int[this.currentPlayerArray.Length];
		for (int i = 0; i < this.currentPlayerArray.Length; i++)
		{
			array[i] = i;
		}
		Random rand = new Random();
		int[] array2 = (from x in array
		orderby rand.Next()
		select x).ToArray<int>();
		GorillaBattleManager.BattleStatus battleStatus = (rand.Next(0, 2) == 0) ? GorillaBattleManager.BattleStatus.RedTeam : GorillaBattleManager.BattleStatus.BlueTeam;
		GorillaBattleManager.BattleStatus battleStatus2 = (battleStatus == GorillaBattleManager.BattleStatus.RedTeam) ? GorillaBattleManager.BattleStatus.BlueTeam : GorillaBattleManager.BattleStatus.RedTeam;
		for (int j = 0; j < this.currentPlayerArray.Length; j++)
		{
			GorillaBattleManager.BattleStatus value = (array2[j] % 2 == 0) ? battleStatus2 : battleStatus;
			this.playerStatusDict[this.currentPlayerArray[j].ActorNumber] = value;
		}
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x00036F28 File Offset: 0x00035128
	public void AddPlayerToCorrectTeam(Player newPlayer)
	{
		this.rcount = 0;
		for (int i = 0; i < this.currentPlayerArray.Length; i++)
		{
			if (this.playerStatusDict.ContainsKey(this.currentPlayerArray[i].ActorNumber))
			{
				GorillaBattleManager.BattleStatus state = this.playerStatusDict[this.currentPlayerArray[i].ActorNumber];
				this.rcount = (this.HasFlag(state, GorillaBattleManager.BattleStatus.RedTeam) ? (this.rcount + 1) : this.rcount);
			}
		}
		if ((this.currentPlayerArray.Length - 1) / 2 == this.rcount)
		{
			this.playerStatusDict[newPlayer.ActorNumber] = ((Random.Range(0, 2) == 0) ? this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaBattleManager.BattleStatus.RedTeam) : this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaBattleManager.BattleStatus.BlueTeam));
			return;
		}
		if (this.rcount <= (this.currentPlayerArray.Length - 1) / 2)
		{
			this.playerStatusDict[newPlayer.ActorNumber] = this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaBattleManager.BattleStatus.RedTeam);
		}
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x00037040 File Offset: 0x00035240
	private void InitializePlayerStatus()
	{
		this.keyValuePairsStatus = this.playerStatusDict.ToArray<KeyValuePair<int, GorillaBattleManager.BattleStatus>>();
		foreach (KeyValuePair<int, GorillaBattleManager.BattleStatus> keyValuePair in this.keyValuePairsStatus)
		{
			this.playerStatusDict[keyValuePair.Key] = GorillaBattleManager.BattleStatus.Normal;
		}
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x00037090 File Offset: 0x00035290
	private void UpdatePlayerStatus()
	{
		this.keyValuePairsStatus = this.playerStatusDict.ToArray<KeyValuePair<int, GorillaBattleManager.BattleStatus>>();
		foreach (KeyValuePair<int, GorillaBattleManager.BattleStatus> keyValuePair in this.keyValuePairsStatus)
		{
			GorillaBattleManager.BattleStatus battleStatus = this.HasFlag(this.playerStatusDict[keyValuePair.Key], GorillaBattleManager.BattleStatus.RedTeam) ? GorillaBattleManager.BattleStatus.RedTeam : GorillaBattleManager.BattleStatus.BlueTeam;
			if (this.playerLives.TryGetValue(keyValuePair.Key, out this.outLives) && this.outLives == 0)
			{
				this.playerStatusDict[keyValuePair.Key] = (battleStatus | GorillaBattleManager.BattleStatus.Eliminated);
			}
			else if (this.playerHitTimes.TryGetValue(keyValuePair.Key, out this.outHitTime) && this.outHitTime + this.hitCooldown > Time.time)
			{
				this.playerStatusDict[keyValuePair.Key] = (battleStatus | GorillaBattleManager.BattleStatus.Hit);
			}
			else if (this.playerStunTimes.TryGetValue(keyValuePair.Key, out this.outHitTime))
			{
				if (this.outHitTime + this.hitCooldown > Time.time)
				{
					this.playerStatusDict[keyValuePair.Key] = (battleStatus | GorillaBattleManager.BattleStatus.Stunned);
				}
				else if (this.outHitTime + this.hitCooldown + this.stunGracePeriod > Time.time)
				{
					this.playerStatusDict[keyValuePair.Key] = (battleStatus | GorillaBattleManager.BattleStatus.Grace);
				}
				else
				{
					this.playerStatusDict[keyValuePair.Key] = (battleStatus | GorillaBattleManager.BattleStatus.Normal);
				}
			}
			else
			{
				this.playerStatusDict[keyValuePair.Key] = (battleStatus | GorillaBattleManager.BattleStatus.Normal);
			}
		}
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x0003721C File Offset: 0x0003541C
	public override void OnDisable()
	{
		base.OnDisable();
		if (Slingshot.IsSlingShotEnabled())
		{
			CosmeticsController instance = CosmeticsController.instance;
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			if (offlineVRRig.cosmeticSet.HasItem("Slingshot"))
			{
				instance.RemoveCosmeticItemFromSet(offlineVRRig.cosmeticSet, "Slingshot", true);
			}
		}
		this.ActivateBattleBalloons(false);
	}

	// Token: 0x04000AED RID: 2797
	private float playerMin = 2f;

	// Token: 0x04000AEE RID: 2798
	public float tagCoolDown = 5f;

	// Token: 0x04000AEF RID: 2799
	public Dictionary<int, int> playerLives = new Dictionary<int, int>();

	// Token: 0x04000AF0 RID: 2800
	public Dictionary<int, GorillaBattleManager.BattleStatus> playerStatusDict = new Dictionary<int, GorillaBattleManager.BattleStatus>();

	// Token: 0x04000AF1 RID: 2801
	public Dictionary<int, float> playerHitTimes = new Dictionary<int, float>();

	// Token: 0x04000AF2 RID: 2802
	public Dictionary<int, float> playerStunTimes = new Dictionary<int, float>();

	// Token: 0x04000AF3 RID: 2803
	public int[] playerActorNumberArray = new int[10];

	// Token: 0x04000AF4 RID: 2804
	public int[] playerLivesArray = new int[10];

	// Token: 0x04000AF5 RID: 2805
	public GorillaBattleManager.BattleStatus[] playerStatusArray = new GorillaBattleManager.BattleStatus[10];

	// Token: 0x04000AF6 RID: 2806
	public bool teamBattle = true;

	// Token: 0x04000AF7 RID: 2807
	public int countDownTime;

	// Token: 0x04000AF8 RID: 2808
	private float timeBattleEnded;

	// Token: 0x04000AF9 RID: 2809
	public float hitCooldown = 3f;

	// Token: 0x04000AFA RID: 2810
	public float stunGracePeriod = 2f;

	// Token: 0x04000AFB RID: 2811
	public object objRef;

	// Token: 0x04000AFC RID: 2812
	private bool playerInList;

	// Token: 0x04000AFD RID: 2813
	private bool coroutineRunning;

	// Token: 0x04000AFE RID: 2814
	private int lives;

	// Token: 0x04000AFF RID: 2815
	private int outLives;

	// Token: 0x04000B00 RID: 2816
	private int bcount;

	// Token: 0x04000B01 RID: 2817
	private int rcount;

	// Token: 0x04000B02 RID: 2818
	private int randInt;

	// Token: 0x04000B03 RID: 2819
	private float outHitTime;

	// Token: 0x04000B04 RID: 2820
	private PhotonView tempView;

	// Token: 0x04000B05 RID: 2821
	private KeyValuePair<int, int>[] keyValuePairs;

	// Token: 0x04000B06 RID: 2822
	private KeyValuePair<int, GorillaBattleManager.BattleStatus>[] keyValuePairsStatus;

	// Token: 0x04000B07 RID: 2823
	private GorillaBattleManager.BattleStatus tempStatus;

	// Token: 0x04000B08 RID: 2824
	private GorillaBattleManager.BattleState currentState;

	// Token: 0x02000418 RID: 1048
	public enum BattleStatus
	{
		// Token: 0x04001CFC RID: 7420
		RedTeam = 1,
		// Token: 0x04001CFD RID: 7421
		BlueTeam,
		// Token: 0x04001CFE RID: 7422
		Normal = 4,
		// Token: 0x04001CFF RID: 7423
		Hit = 8,
		// Token: 0x04001D00 RID: 7424
		Stunned = 16,
		// Token: 0x04001D01 RID: 7425
		Grace = 32,
		// Token: 0x04001D02 RID: 7426
		Eliminated = 64,
		// Token: 0x04001D03 RID: 7427
		None = 0
	}

	// Token: 0x02000419 RID: 1049
	private enum BattleState
	{
		// Token: 0x04001D05 RID: 7429
		NotEnoughPlayers,
		// Token: 0x04001D06 RID: 7430
		GameEnd,
		// Token: 0x04001D07 RID: 7431
		GameEndWaiting,
		// Token: 0x04001D08 RID: 7432
		StartCountdown,
		// Token: 0x04001D09 RID: 7433
		CountingDownToStart,
		// Token: 0x04001D0A RID: 7434
		GameStart,
		// Token: 0x04001D0B RID: 7435
		GameRunning
	}
}
