using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CjLib;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaTag
{
	// Token: 0x0200031C RID: 796
	public class RisingLavaManager : MonoBehaviourPun, IPunObservable, IInRoomCallbacks
	{
		// Token: 0x17000183 RID: 387
		// (get) Token: 0x0600160B RID: 5643 RVA: 0x0007A19C File Offset: 0x0007839C
		private bool RefreshWaterAvailable
		{
			get
			{
				return this.reliableState.state == RisingLavaManager.RisingLavaState.Drained || this.reliableState.state == RisingLavaManager.RisingLavaState.Erupting || (this.reliableState.state == RisingLavaManager.RisingLavaState.Rising && this.lavaProgress < this.lavaProgressToDisableRefreshWater) || (this.reliableState.state == RisingLavaManager.RisingLavaState.Draining && this.lavaProgress < this.lavaProgressToEnableRefreshWater);
			}
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x0600160C RID: 5644 RVA: 0x0007A200 File Offset: 0x00078400
		public RisingLavaManager.RisingLavaState GameState
		{
			get
			{
				return this.reliableState.state;
			}
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x0600160D RID: 5645 RVA: 0x0007A20D File Offset: 0x0007840D
		public float LavaProgress
		{
			get
			{
				return this.lavaProgress;
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x0600160E RID: 5646 RVA: 0x0007A215 File Offset: 0x00078415
		public float LavaProgressLinear
		{
			get
			{
				return this.lavaProgressLinear;
			}
		}

		// Token: 0x0600160F RID: 5647 RVA: 0x0007A220 File Offset: 0x00078420
		private void Awake()
		{
			if (RisingLavaManager.instance == null)
			{
				RisingLavaManager.instance = this;
			}
			else if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient && GorillaGameManager.instance != this)
			{
				Debug.LogError("Dupicate RisingLavaManager singleton detected");
				PhotonNetwork.Destroy(base.photonView);
			}
			this.riseTimeLookup = new float[]
			{
				this.riseTimeFast,
				this.riseTimeMedium,
				this.riseTimeSlow,
				this.riseTimeExtraSlow
			};
			this.riseTime = this.riseTimeLookup[(int)this.nextRoundLavaSpeed];
			this.RefreshSpeedSelectButtons();
			this.allPlayersInRoom = PhotonNetwork.PlayerList;
			this.lavaVolume.ColliderEnteredVolume += this.OnColliderEnteredVolume;
			this.lavaVolume.ColliderExitedVolume += this.OnColliderExitedVolume;
			this.lavaVolume.ColliderEnteredWater += this.OnColliderEnteredLava;
			this.lavaVolume.ColliderExitedWater += this.OnColliderExitedLava;
			this.entryLavaVolume.ColliderEnteredVolume += this.OnColliderEnteredVolume;
			this.entryLavaVolume.ColliderExitedVolume += this.OnColliderExitedVolume;
			this.entryLavaVolume.ColliderEnteredWater += this.OnColliderEnteredLava;
			this.entryLavaVolume.ColliderExitedWater += this.OnColliderExitedLava;
			this.refreshWaterVolume.ColliderEnteredWater += this.OnColliderEnteredRefreshWater;
			this.refreshWaterVolume.ColliderExitedWater += this.OnColliderExitedRefreshWater;
		}

		// Token: 0x06001610 RID: 5648 RVA: 0x0007A3B4 File Offset: 0x000785B4
		private void OnDestroy()
		{
			if (this.lavaVolume != null)
			{
				this.lavaVolume.ColliderEnteredVolume -= this.OnColliderEnteredVolume;
				this.lavaVolume.ColliderExitedVolume -= this.OnColliderExitedVolume;
				this.lavaVolume.ColliderEnteredWater -= this.OnColliderEnteredLava;
				this.lavaVolume.ColliderExitedWater -= this.OnColliderExitedLava;
			}
			if (this.entryLavaVolume != null)
			{
				this.entryLavaVolume.ColliderEnteredVolume -= this.OnColliderEnteredVolume;
				this.entryLavaVolume.ColliderExitedVolume -= this.OnColliderExitedVolume;
				this.entryLavaVolume.ColliderEnteredWater -= this.OnColliderEnteredLava;
				this.entryLavaVolume.ColliderExitedWater -= this.OnColliderExitedLava;
			}
			if (this.refreshWaterVolume != null)
			{
				this.refreshWaterVolume.ColliderEnteredWater -= this.OnColliderEnteredRefreshWater;
				this.refreshWaterVolume.ColliderExitedWater -= this.OnColliderExitedRefreshWater;
			}
		}

		// Token: 0x06001611 RID: 5649 RVA: 0x0007A4D4 File Offset: 0x000786D4
		private void Update()
		{
			double num = PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time);
			this.lastInfrequentUpdateTime = ((this.lastInfrequentUpdateTime > num) ? num : this.lastInfrequentUpdateTime);
			if (num > this.lastInfrequentUpdateTime + (double)this.infrequentUpdatePeriod)
			{
				this.InfrequentUpdate();
				this.lastInfrequentUpdateTime = (double)((float)num);
			}
			if (base.photonView.IsMine)
			{
				this.UpdateReliableState(num, ref this.reliableState);
			}
			this.UpdateLocalState(num, this.reliableState);
			this.localLagLavaProgressOffset = Mathf.MoveTowards(this.localLagLavaProgressOffset, 0f, this.lagResolutionLavaProgressPerSecond * Time.deltaTime);
			this.UpdateLava(this.lavaProgress + this.localLagLavaProgressOffset);
			this.UpdateRefreshWater();
			this.DisableObjectsInContactWithLava(this.lavaMeshTransform.localScale.z);
			this.UpdateGameUI(num);
			if (this.debugDrawPlayerGameState)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					Photon.Realtime.Player player = null;
					if (PhotonNetwork.CurrentRoom != null)
					{
						player = PhotonNetwork.CurrentRoom.GetPlayer(this.inGamePlayerStates[i].playerId, false);
					}
					else if (this.inGamePlayerStates[i].playerId == PhotonNetwork.LocalPlayer.ActorNumber)
					{
						player = PhotonNetwork.LocalPlayer;
					}
					RigContainer rigContainer;
					if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer) && rigContainer.Rig != null)
					{
						float num2 = 0.03f;
						DebugUtil.DrawSphere(rigContainer.Rig.transform.position + Vector3.up * 0.5f * num2, 0.16f * num2, 12, 12, this.inGamePlayerStates[i].touchedLava ? Color.red : Color.green, true, DebugUtil.Style.SolidColor);
					}
				}
			}
		}

		// Token: 0x06001612 RID: 5650 RVA: 0x0007A6A4 File Offset: 0x000788A4
		private void InfrequentUpdate()
		{
			this.allPlayersInRoom = PhotonNetwork.PlayerList;
			if (base.photonView.IsMine)
			{
				for (int i = this.inGamePlayerCount - 1; i >= 0; i--)
				{
					int playerId = this.inGamePlayerStates[i].playerId;
					bool flag = false;
					for (int j = 0; j < this.allPlayersInRoom.Length; j++)
					{
						if (this.allPlayersInRoom[j].ActorNumber == playerId)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						if (i < this.inGamePlayerCount - 1)
						{
							this.inGamePlayerStates[i] = this.inGamePlayerStates[this.inGamePlayerCount - 1];
						}
						this.inGamePlayerStates[this.inGamePlayerCount - 1] = default(RisingLavaManager.PlayerGameState);
						this.inGamePlayerCount--;
					}
				}
			}
		}

		// Token: 0x06001613 RID: 5651 RVA: 0x0007A774 File Offset: 0x00078974
		private bool PlayerInGame(Photon.Realtime.Player player)
		{
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (this.inGamePlayerStates[i].playerId == player.ActorNumber)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001614 RID: 5652 RVA: 0x0007A7B0 File Offset: 0x000789B0
		private void UpdateReliableState(double currentTime, ref RisingLavaManager.LavaSyncData syncData)
		{
			if (currentTime < syncData.stateStartTime)
			{
				syncData.stateStartTime = currentTime;
			}
			switch (syncData.state)
			{
			default:
				if (this.<UpdateReliableState>g__GetAlivePlayerCount|67_0() > 0)
				{
					syncData.stateStartLavaProgressLinear = 0f;
					syncData.state = RisingLavaManager.RisingLavaState.Erupting;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case RisingLavaManager.RisingLavaState.Erupting:
				if (currentTime > syncData.stateStartTime + (double)this.fullyDrainedWaitTime)
				{
					this.riseTime = this.riseTimeLookup[(int)this.nextRoundLavaSpeed];
					syncData.stateStartLavaProgressLinear = 0f;
					syncData.state = RisingLavaManager.RisingLavaState.Rising;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case RisingLavaManager.RisingLavaState.Rising:
				if (this.<UpdateReliableState>g__GetAlivePlayerCount|67_0() <= 0)
				{
					this.UpdateWinner();
					syncData.stateStartLavaProgressLinear = Mathf.Clamp01((float)((currentTime - syncData.stateStartTime) / (double)this.riseTime));
					syncData.state = RisingLavaManager.RisingLavaState.PreDrainDelay;
					syncData.stateStartTime = currentTime;
					return;
				}
				if (currentTime > syncData.stateStartTime + (double)this.riseTime)
				{
					syncData.stateStartLavaProgressLinear = 1f;
					syncData.state = RisingLavaManager.RisingLavaState.Full;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case RisingLavaManager.RisingLavaState.Full:
				if (this.<UpdateReliableState>g__GetAlivePlayerCount|67_0() <= 0 || currentTime > syncData.stateStartTime + (double)this.maxFullTime)
				{
					this.UpdateWinner();
					syncData.stateStartLavaProgressLinear = 1f;
					syncData.state = RisingLavaManager.RisingLavaState.PreDrainDelay;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case RisingLavaManager.RisingLavaState.PreDrainDelay:
				if (currentTime > syncData.stateStartTime + (double)this.preDrainWaitTime)
				{
					syncData.state = RisingLavaManager.RisingLavaState.Draining;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case RisingLavaManager.RisingLavaState.Draining:
			{
				double num = (1.0 - (double)syncData.stateStartLavaProgressLinear) * (double)this.drainTime;
				if (currentTime + num > syncData.stateStartTime + (double)this.drainTime)
				{
					syncData.stateStartLavaProgressLinear = 0f;
					syncData.state = RisingLavaManager.RisingLavaState.Drained;
					syncData.stateStartTime = currentTime;
				}
				break;
			}
			}
		}

		// Token: 0x06001615 RID: 5653 RVA: 0x0007A964 File Offset: 0x00078B64
		private void UpdateLocalState(double currentTime, RisingLavaManager.LavaSyncData syncData)
		{
			switch (syncData.state)
			{
			default:
				this.lavaProgressLinear = 0f;
				this.lavaProgress = 0f;
				return;
			case RisingLavaManager.RisingLavaState.Rising:
			{
				double num = (currentTime - syncData.stateStartTime) / (double)this.riseTime;
				this.lavaProgressLinear = Mathf.Clamp01((float)num);
				this.lavaProgress = this.animationCurve.Evaluate(this.lavaProgressLinear);
				return;
			}
			case RisingLavaManager.RisingLavaState.Full:
				this.lavaProgressLinear = 1f;
				this.lavaProgress = 1f;
				return;
			case RisingLavaManager.RisingLavaState.PreDrainDelay:
				this.lavaProgressLinear = syncData.stateStartLavaProgressLinear;
				this.lavaProgress = this.animationCurve.Evaluate(this.lavaProgressLinear);
				return;
			case RisingLavaManager.RisingLavaState.Draining:
			{
				double num2 = (1.0 - (double)syncData.stateStartLavaProgressLinear) * (double)this.drainTime;
				double num3 = (currentTime + num2 - syncData.stateStartTime) / (double)this.drainTime;
				this.lavaProgressLinear = Mathf.Clamp01((float)(1.0 - num3));
				this.lavaProgress = this.animationCurve.Evaluate(this.lavaProgressLinear);
				return;
			}
			}
		}

		// Token: 0x06001616 RID: 5654 RVA: 0x0007AA7C File Offset: 0x00078C7C
		private void UpdateLava(float fillProgress)
		{
			float num = Mathf.Lerp(this.minScale, this.maxScale, fillProgress);
			this.lavaMeshTransform.localScale = new Vector3(1f, 1f, num);
			if (this.entryWayLavaMeshTransform != null)
			{
				float z;
				if (num < this.entryLavaScaleSyncOpeningBottom.y)
				{
					z = this.entryLavaScaleSyncOpeningBottom.x;
				}
				else if (num < this.entryLavaScaleSyncOpeningTop.y)
				{
					float t = Mathf.InverseLerp(this.entryLavaScaleSyncOpeningBottom.y, this.entryLavaScaleSyncOpeningTop.y, num);
					z = Mathf.Lerp(this.entryLavaScaleSyncOpeningBottom.x, this.entryLavaScaleSyncOpeningTop.x, t);
				}
				else
				{
					float t2 = Mathf.InverseLerp(this.entryLavaScaleSyncOpeningTop.y, this.maxScale, num);
					z = Mathf.Lerp(this.entryLavaScaleSyncOpeningTop.x, this.entryLavaMaxScale, t2);
				}
				this.entryWayLavaMeshTransform.localScale = new Vector3(this.entryWayLavaMeshTransform.localScale.x, this.entryWayLavaMeshTransform.localScale.y, z);
			}
		}

		// Token: 0x06001617 RID: 5655 RVA: 0x0007AB90 File Offset: 0x00078D90
		private void DisableObjectsInContactWithLava(float lavaScale)
		{
			if (this.reliableState.state == RisingLavaManager.RisingLavaState.Rising)
			{
				for (int i = 0; i < this.disableByLavaList.Count; i++)
				{
					if (lavaScale > this.disableByLavaList[i].lavaScaleToDisable && this.disableByLavaList[i].transform != null)
					{
						this.disableByLavaList[i].transform.gameObject.SetActive(false);
					}
				}
				return;
			}
			if (this.reliableState.state == RisingLavaManager.RisingLavaState.Draining)
			{
				for (int j = 0; j < this.disableByLavaList.Count; j++)
				{
					if (lavaScale < this.disableByLavaList[j].lavaScaleToReenable && this.disableByLavaList[j].transform != null)
					{
						this.disableByLavaList[j].transform.gameObject.SetActive(true);
					}
				}
			}
		}

		// Token: 0x06001618 RID: 5656 RVA: 0x0007AC78 File Offset: 0x00078E78
		private void UpdateGameUI(double currentTime)
		{
			if (this.reliableState.state == RisingLavaManager.RisingLavaState.Erupting)
			{
				int num = Mathf.CeilToInt((float)(this.reliableState.stateStartTime + (double)this.fullyDrainedWaitTime - currentTime));
				this.gameStartCountdown.text = num.ToString();
				return;
			}
			if (this.reliableState.state == RisingLavaManager.RisingLavaState.Rising && currentTime < 1.0 + this.reliableState.stateStartTime)
			{
				this.gameStartCountdown.text = "ERUPT";
				return;
			}
			this.gameStartCountdown.text = "";
		}

		// Token: 0x06001619 RID: 5657 RVA: 0x0007AD0C File Offset: 0x00078F0C
		private void UpdateWinner()
		{
			float num = -1f;
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (!this.inGamePlayerStates[i].touchedLava)
				{
					this.lastWinnerId = this.inGamePlayerStates[i].playerId;
					break;
				}
				if (this.inGamePlayerStates[i].touchedLavaAtProgress > num)
				{
					num = this.inGamePlayerStates[i].touchedLavaAtProgress;
					this.lastWinnerId = this.inGamePlayerStates[i].playerId;
				}
			}
			this.RefreshWinnerName();
		}

		// Token: 0x0600161A RID: 5658 RVA: 0x0007ADA0 File Offset: 0x00078FA0
		private void RefreshWinnerName()
		{
			Photon.Realtime.Player playerFromId = this.GetPlayerFromId(this.lastWinnerId);
			if (playerFromId != null)
			{
				this.lastWinnerName = playerFromId.NickName;
				return;
			}
			this.lastWinnerName = "None";
		}

		// Token: 0x0600161B RID: 5659 RVA: 0x0007ADD5 File Offset: 0x00078FD5
		private Photon.Realtime.Player GetPlayerFromId(int id)
		{
			if (PhotonNetwork.CurrentRoom != null)
			{
				return PhotonNetwork.CurrentRoom.GetPlayer(id, false);
			}
			if (id == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				return PhotonNetwork.LocalPlayer;
			}
			return null;
		}

		// Token: 0x0600161C RID: 5660 RVA: 0x0007AE00 File Offset: 0x00079000
		private void UpdateRefreshWater()
		{
			if (this.RefreshWaterAvailable && !this.refreshWaterVolume.gameObject.activeSelf)
			{
				this.refreshWaterVolume.gameObject.SetActive(true);
				return;
			}
			if (!this.RefreshWaterAvailable && this.refreshWaterVolume.gameObject.activeSelf)
			{
				this.refreshWaterVolume.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x0007AE64 File Offset: 0x00079064
		private void ResetGame()
		{
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				RisingLavaManager.PlayerGameState playerGameState = this.inGamePlayerStates[i];
				playerGameState.touchedLava = false;
				playerGameState.touchedLavaAtProgress = -1f;
				this.inGamePlayerStates[i] = playerGameState;
			}
		}

		// Token: 0x0600161E RID: 5662 RVA: 0x0007AEB0 File Offset: 0x000790B0
		public void RestartGame()
		{
			if (base.photonView.IsMine)
			{
				this.riseTime = this.riseTimeLookup[(int)this.nextRoundLavaSpeed];
				this.reliableState.state = RisingLavaManager.RisingLavaState.Rising;
				this.reliableState.stateStartTime = (double)(PhotonNetwork.InRoom ? ((float)PhotonNetwork.Time) : Time.time);
				this.ResetGame();
			}
		}

		// Token: 0x0600161F RID: 5663 RVA: 0x0007AF10 File Offset: 0x00079110
		public bool GetMaterialIfPlayerInGame(int playerActorNumber, out int materialIndex)
		{
			int i = 0;
			while (i < this.inGamePlayerCount)
			{
				if (this.inGamePlayerStates[i].playerId == playerActorNumber)
				{
					if (this.inGamePlayerStates[i].touchedLava)
					{
						materialIndex = 2;
						return true;
					}
					materialIndex = 0;
					return true;
				}
				else
				{
					i++;
				}
			}
			materialIndex = 0;
			return false;
		}

		// Token: 0x06001620 RID: 5664 RVA: 0x0007AF64 File Offset: 0x00079164
		private void OnColliderEnteredVolume(Collider collider)
		{
			if (collider == GorillaLocomotion.Player.Instance.bodyCollider)
			{
				this.overlappingWaterVolumes++;
				if (base.photonView.IsMine)
				{
					this.PlayerEnteredGameArea(PhotonNetwork.LocalPlayer.ActorNumber);
					return;
				}
				base.photonView.RPC("PlayerEnteredGameAreaRPC", RpcTarget.MasterClient, Array.Empty<object>());
			}
		}

		// Token: 0x06001621 RID: 5665 RVA: 0x0007AFC8 File Offset: 0x000791C8
		private void OnColliderExitedVolume(Collider collider)
		{
			if (collider == GorillaLocomotion.Player.Instance.bodyCollider)
			{
				this.overlappingWaterVolumes--;
				if (this.overlappingWaterVolumes <= 0)
				{
					this.overlappingWaterVolumes = 0;
					if (base.photonView.IsMine)
					{
						this.PlayerExitedGameArea(PhotonNetwork.LocalPlayer.ActorNumber);
						return;
					}
					base.photonView.RPC("PlayerExitedGameAreaRPC", RpcTarget.MasterClient, Array.Empty<object>());
				}
			}
		}

		// Token: 0x06001622 RID: 5666 RVA: 0x0007B03C File Offset: 0x0007923C
		private void OnColliderEnteredLava(Collider collider)
		{
			if (collider == GorillaLocomotion.Player.Instance.bodyCollider)
			{
				if (base.photonView.IsMine)
				{
					this.PlayerTouchedLava(PhotonNetwork.LocalPlayer.ActorNumber);
					return;
				}
				base.photonView.RPC("PlayerTouchedLavaRPC", RpcTarget.MasterClient, Array.Empty<object>());
			}
		}

		// Token: 0x06001623 RID: 5667 RVA: 0x0007B08F File Offset: 0x0007928F
		private void OnColliderExitedLava(Collider collider)
		{
		}

		// Token: 0x06001624 RID: 5668 RVA: 0x0007B094 File Offset: 0x00079294
		private void OnColliderEnteredRefreshWater(Collider collider)
		{
			if (collider == GorillaLocomotion.Player.Instance.bodyCollider)
			{
				if (base.photonView.IsMine)
				{
					this.PlayerTouchedRefreshWater(PhotonNetwork.LocalPlayer.ActorNumber);
					return;
				}
				base.photonView.RPC("PlayerTouchedRefreshWaterRPC", RpcTarget.MasterClient, Array.Empty<object>());
			}
		}

		// Token: 0x06001625 RID: 5669 RVA: 0x0007B0E7 File Offset: 0x000792E7
		private void OnColliderExitedRefreshWater(Collider collider)
		{
		}

		// Token: 0x06001626 RID: 5670 RVA: 0x0007B0EC File Offset: 0x000792EC
		public void OnWaterBalloonHitPlayer(Photon.Realtime.Player hitPlayer)
		{
			bool flag = false;
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (this.inGamePlayerStates[i].playerId == hitPlayer.ActorNumber)
				{
					flag = true;
				}
			}
			if (flag)
			{
				if (hitPlayer == PhotonNetwork.LocalPlayer)
				{
					this.ValidateLocalPlayerWaterBalloonHit(hitPlayer.ActorNumber);
					return;
				}
				base.photonView.RPC("ValidateLocalPlayerWaterBalloonHitRPC", RpcTarget.Others, new object[]
				{
					hitPlayer.ActorNumber
				});
			}
		}

		// Token: 0x06001627 RID: 5671 RVA: 0x0007B164 File Offset: 0x00079364
		public void RestartButtonPressed()
		{
			if (base.photonView.IsMine)
			{
				this.RestartGame();
				return;
			}
			base.photonView.RPC("RestartGameRPC", RpcTarget.MasterClient, Array.Empty<object>());
		}

		// Token: 0x06001628 RID: 5672 RVA: 0x0007B190 File Offset: 0x00079390
		public void SetSpeedButtonPressedFast()
		{
			this.SetRiseTimeButtonPressed(RisingLavaManager.LavaSpeed.Fast);
		}

		// Token: 0x06001629 RID: 5673 RVA: 0x0007B199 File Offset: 0x00079399
		public void SetSpeedButtonPressedMedium()
		{
			this.SetRiseTimeButtonPressed(RisingLavaManager.LavaSpeed.Medium);
		}

		// Token: 0x0600162A RID: 5674 RVA: 0x0007B1A2 File Offset: 0x000793A2
		public void SetSpeedButtonPressedSlow()
		{
			this.SetRiseTimeButtonPressed(RisingLavaManager.LavaSpeed.Slow);
		}

		// Token: 0x0600162B RID: 5675 RVA: 0x0007B1AB File Offset: 0x000793AB
		public void SetSpeedButtonPressedExtraSlow()
		{
			this.SetRiseTimeButtonPressed(RisingLavaManager.LavaSpeed.ExtraSlow);
		}

		// Token: 0x0600162C RID: 5676 RVA: 0x0007B1B4 File Offset: 0x000793B4
		private void RefreshSpeedSelectButtons()
		{
			this.lavaSpeedButtonFast.isOn = (this.nextRoundLavaSpeed == RisingLavaManager.LavaSpeed.Fast);
			this.lavaSpeedButtonFast.UpdateColor();
			this.lavaSpeedButtonMedium.isOn = (this.nextRoundLavaSpeed == RisingLavaManager.LavaSpeed.Medium);
			this.lavaSpeedButtonMedium.UpdateColor();
			this.lavaSpeedButtonSlow.isOn = (this.nextRoundLavaSpeed == RisingLavaManager.LavaSpeed.Slow);
			this.lavaSpeedButtonSlow.UpdateColor();
			this.lavaSpeedButtonExtraSlow.isOn = (this.nextRoundLavaSpeed == RisingLavaManager.LavaSpeed.ExtraSlow);
			this.lavaSpeedButtonExtraSlow.UpdateColor();
		}

		// Token: 0x0600162D RID: 5677 RVA: 0x0007B23D File Offset: 0x0007943D
		private void SetRiseTimeButtonPressed(RisingLavaManager.LavaSpeed lavaSpeed)
		{
			if (base.photonView.IsMine)
			{
				this.SetRiseTime(lavaSpeed);
				return;
			}
			base.photonView.RPC("SetRiseTimeRPC", RpcTarget.MasterClient, new object[]
			{
				lavaSpeed
			});
		}

		// Token: 0x0600162E RID: 5678 RVA: 0x0007B274 File Offset: 0x00079474
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			if (stream.IsWriting)
			{
				stream.SendNext((int)this.reliableState.state);
				stream.SendNext(this.reliableState.stateStartTime);
				stream.SendNext(this.reliableState.stateStartLavaProgressLinear);
				stream.SendNext((int)this.nextRoundLavaSpeed);
				stream.SendNext(this.riseTime);
				stream.SendNext(this.lastWinnerId);
				stream.SendNext(this.inGamePlayerCount);
				RisingLavaManager.<OnPhotonSerializeView>g__SendPlayerGameState|93_0(stream, ref this.inGamePlayerStates[0]);
				RisingLavaManager.<OnPhotonSerializeView>g__SendPlayerGameState|93_0(stream, ref this.inGamePlayerStates[1]);
				RisingLavaManager.<OnPhotonSerializeView>g__SendPlayerGameState|93_0(stream, ref this.inGamePlayerStates[2]);
				RisingLavaManager.<OnPhotonSerializeView>g__SendPlayerGameState|93_0(stream, ref this.inGamePlayerStates[3]);
				RisingLavaManager.<OnPhotonSerializeView>g__SendPlayerGameState|93_0(stream, ref this.inGamePlayerStates[4]);
				RisingLavaManager.<OnPhotonSerializeView>g__SendPlayerGameState|93_0(stream, ref this.inGamePlayerStates[5]);
				RisingLavaManager.<OnPhotonSerializeView>g__SendPlayerGameState|93_0(stream, ref this.inGamePlayerStates[6]);
				RisingLavaManager.<OnPhotonSerializeView>g__SendPlayerGameState|93_0(stream, ref this.inGamePlayerStates[7]);
				RisingLavaManager.<OnPhotonSerializeView>g__SendPlayerGameState|93_0(stream, ref this.inGamePlayerStates[8]);
				RisingLavaManager.<OnPhotonSerializeView>g__SendPlayerGameState|93_0(stream, ref this.inGamePlayerStates[9]);
				return;
			}
			int num = this.lastWinnerId;
			RisingLavaManager.LavaSpeed lavaSpeed = this.nextRoundLavaSpeed;
			this.reliableState.state = (RisingLavaManager.RisingLavaState)((int)stream.ReceiveNext());
			this.reliableState.stateStartTime = (double)stream.ReceiveNext();
			this.reliableState.stateStartLavaProgressLinear = (float)stream.ReceiveNext();
			this.nextRoundLavaSpeed = (RisingLavaManager.LavaSpeed)((int)stream.ReceiveNext());
			this.riseTime = (float)stream.ReceiveNext();
			this.lastWinnerId = (int)stream.ReceiveNext();
			this.inGamePlayerCount = (int)stream.ReceiveNext();
			this.inGamePlayerStates[0] = RisingLavaManager.<OnPhotonSerializeView>g__ReceivePlayerGameState|93_1(stream);
			this.inGamePlayerStates[1] = RisingLavaManager.<OnPhotonSerializeView>g__ReceivePlayerGameState|93_1(stream);
			this.inGamePlayerStates[2] = RisingLavaManager.<OnPhotonSerializeView>g__ReceivePlayerGameState|93_1(stream);
			this.inGamePlayerStates[3] = RisingLavaManager.<OnPhotonSerializeView>g__ReceivePlayerGameState|93_1(stream);
			this.inGamePlayerStates[4] = RisingLavaManager.<OnPhotonSerializeView>g__ReceivePlayerGameState|93_1(stream);
			this.inGamePlayerStates[5] = RisingLavaManager.<OnPhotonSerializeView>g__ReceivePlayerGameState|93_1(stream);
			this.inGamePlayerStates[6] = RisingLavaManager.<OnPhotonSerializeView>g__ReceivePlayerGameState|93_1(stream);
			this.inGamePlayerStates[7] = RisingLavaManager.<OnPhotonSerializeView>g__ReceivePlayerGameState|93_1(stream);
			this.inGamePlayerStates[8] = RisingLavaManager.<OnPhotonSerializeView>g__ReceivePlayerGameState|93_1(stream);
			this.inGamePlayerStates[9] = RisingLavaManager.<OnPhotonSerializeView>g__ReceivePlayerGameState|93_1(stream);
			float num2 = this.lavaProgress;
			this.UpdateLocalState((double)((float)PhotonNetwork.Time), this.reliableState);
			this.localLagLavaProgressOffset = num2 - this.lavaProgress;
			if (num != this.lastWinnerId)
			{
				this.RefreshWinnerName();
			}
			if (lavaSpeed != this.nextRoundLavaSpeed)
			{
				this.RefreshSpeedSelectButtons();
			}
		}

		// Token: 0x0600162F RID: 5679 RVA: 0x0007B563 File Offset: 0x00079763
		[PunRPC]
		public void PlayerEnteredGameAreaRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerEnteredGameAreaRPC");
			this.PlayerEnteredGameArea(info.Sender.ActorNumber);
		}

		// Token: 0x06001630 RID: 5680 RVA: 0x0007B584 File Offset: 0x00079784
		private void PlayerEnteredGameArea(int pId)
		{
			if (base.photonView.IsMine)
			{
				bool flag = false;
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == pId)
					{
						flag = true;
						break;
					}
				}
				if (!flag && this.inGamePlayerCount < 10)
				{
					bool touchedLava = false;
					this.inGamePlayerStates[this.inGamePlayerCount] = new RisingLavaManager.PlayerGameState
					{
						playerId = pId,
						touchedLava = touchedLava,
						touchedLavaAtProgress = -1f
					};
					this.inGamePlayerCount++;
				}
			}
		}

		// Token: 0x06001631 RID: 5681 RVA: 0x0007B61C File Offset: 0x0007981C
		[PunRPC]
		public void PlayerExitedGameAreaRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerExitedGameAreaRPC");
			this.PlayerExitedGameArea(info.Sender.ActorNumber);
		}

		// Token: 0x06001632 RID: 5682 RVA: 0x0007B63C File Offset: 0x0007983C
		private void PlayerExitedGameArea(int playerId)
		{
			if (base.photonView.IsMine)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == playerId)
					{
						this.inGamePlayerStates[i] = this.inGamePlayerStates[this.inGamePlayerCount - 1];
						this.inGamePlayerCount--;
						return;
					}
				}
			}
		}

		// Token: 0x06001633 RID: 5683 RVA: 0x0007B6A9 File Offset: 0x000798A9
		[PunRPC]
		public void PlayerTouchedLavaRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerTouchedLavaRPC");
			this.PlayerTouchedLava(info.Sender.ActorNumber);
		}

		// Token: 0x06001634 RID: 5684 RVA: 0x0007B6C8 File Offset: 0x000798C8
		private void PlayerTouchedLava(int playerId)
		{
			if (base.photonView.IsMine)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == playerId)
					{
						RisingLavaManager.PlayerGameState playerGameState = this.inGamePlayerStates[i];
						if (!playerGameState.touchedLava)
						{
							playerGameState.touchedLavaAtProgress = this.lavaProgressLinear;
						}
						playerGameState.touchedLava = true;
						this.inGamePlayerStates[i] = playerGameState;
						return;
					}
				}
			}
		}

		// Token: 0x06001635 RID: 5685 RVA: 0x0007B73F File Offset: 0x0007993F
		[PunRPC]
		private void PlayerTouchedRefreshWaterRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerTouchedRefreshWaterRPC");
			this.PlayerTouchedRefreshWater(info.Sender.ActorNumber);
		}

		// Token: 0x06001636 RID: 5686 RVA: 0x0007B760 File Offset: 0x00079960
		private void PlayerTouchedRefreshWater(int playerId)
		{
			if (base.photonView.IsMine && this.RefreshWaterAvailable)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == playerId)
					{
						RisingLavaManager.PlayerGameState playerGameState = this.inGamePlayerStates[i];
						playerGameState.touchedLava = false;
						playerGameState.touchedLavaAtProgress = -1f;
						this.inGamePlayerStates[i] = playerGameState;
						return;
					}
				}
			}
		}

		// Token: 0x06001637 RID: 5687 RVA: 0x0007B7D6 File Offset: 0x000799D6
		[PunRPC]
		private void ValidateLocalPlayerWaterBalloonHitRPC(int playerId, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "ValidateLocalPlayerWaterBalloonHitRPC");
			if (playerId == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				this.ValidateLocalPlayerWaterBalloonHit(playerId);
			}
		}

		// Token: 0x06001638 RID: 5688 RVA: 0x0007B7F8 File Offset: 0x000799F8
		private void ValidateLocalPlayerWaterBalloonHit(int playerId)
		{
			if (playerId == PhotonNetwork.LocalPlayer.ActorNumber && !GorillaLocomotion.Player.Instance.InWater)
			{
				if (base.photonView.IsMine)
				{
					this.PlayerHitByWaterBalloon(PhotonNetwork.LocalPlayer.ActorNumber);
					return;
				}
				base.photonView.RPC("PlayerHitByWaterBalloonRPC", RpcTarget.MasterClient, new object[]
				{
					PhotonNetwork.LocalPlayer.ActorNumber
				});
			}
		}

		// Token: 0x06001639 RID: 5689 RVA: 0x0007B865 File Offset: 0x00079A65
		[PunRPC]
		private void PlayerHitByWaterBalloonRPC(int playerId, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerHitByWaterBalloonRPC");
			this.PlayerHitByWaterBalloon(playerId);
		}

		// Token: 0x0600163A RID: 5690 RVA: 0x0007B87C File Offset: 0x00079A7C
		private void PlayerHitByWaterBalloon(int playerId)
		{
			if (base.photonView.IsMine)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == playerId)
					{
						RisingLavaManager.PlayerGameState playerGameState = this.inGamePlayerStates[i];
						playerGameState.touchedLava = false;
						playerGameState.touchedLavaAtProgress = -1f;
						this.inGamePlayerStates[i] = playerGameState;
						return;
					}
				}
			}
		}

		// Token: 0x0600163B RID: 5691 RVA: 0x0007B8EA File Offset: 0x00079AEA
		[PunRPC]
		private void RestartGameRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RestartGameRPC");
			this.RestartGame();
		}

		// Token: 0x0600163C RID: 5692 RVA: 0x0007B8FD File Offset: 0x00079AFD
		[PunRPC]
		private void SetRiseTimeRPC(RisingLavaManager.LavaSpeed lavaSpeed, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SetRiseTimeRPC");
			this.SetRiseTime(lavaSpeed);
		}

		// Token: 0x0600163D RID: 5693 RVA: 0x0007B911 File Offset: 0x00079B11
		private void SetRiseTime(RisingLavaManager.LavaSpeed lavaSpeed)
		{
			if (base.photonView.IsMine)
			{
				this.nextRoundLavaSpeed = lavaSpeed;
				this.RefreshSpeedSelectButtons();
			}
		}

		// Token: 0x0600163E RID: 5694 RVA: 0x0007B92D File Offset: 0x00079B2D
		public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
		{
		}

		// Token: 0x0600163F RID: 5695 RVA: 0x0007B92F File Offset: 0x00079B2F
		public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
		{
			this.PlayerExitedGameArea(otherPlayer.ActorNumber);
		}

		// Token: 0x06001640 RID: 5696 RVA: 0x0007B93D File Offset: 0x00079B3D
		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		// Token: 0x06001641 RID: 5697 RVA: 0x0007B93F File Offset: 0x00079B3F
		public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
		{
		}

		// Token: 0x06001642 RID: 5698 RVA: 0x0007B941 File Offset: 0x00079B41
		public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
		{
		}

		// Token: 0x06001644 RID: 5700 RVA: 0x0007BA88 File Offset: 0x00079C88
		[CompilerGenerated]
		private int <UpdateReliableState>g__GetAlivePlayerCount|67_0()
		{
			int num = 0;
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (!this.inGamePlayerStates[i].touchedLava)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06001645 RID: 5701 RVA: 0x0007BAC0 File Offset: 0x00079CC0
		[CompilerGenerated]
		internal static void <OnPhotonSerializeView>g__SendPlayerGameState|93_0(PhotonStream photonStream, ref RisingLavaManager.PlayerGameState gameState)
		{
			photonStream.SendNext(gameState.playerId);
			photonStream.SendNext(gameState.touchedLava);
			photonStream.SendNext(gameState.touchedLavaAtProgress);
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x0007BAF8 File Offset: 0x00079CF8
		[CompilerGenerated]
		internal static RisingLavaManager.PlayerGameState <OnPhotonSerializeView>g__ReceivePlayerGameState|93_1(PhotonStream photonStream)
		{
			RisingLavaManager.PlayerGameState result;
			result.playerId = (int)photonStream.ReceiveNext();
			result.touchedLava = (bool)photonStream.ReceiveNext();
			result.touchedLavaAtProgress = (float)photonStream.ReceiveNext();
			return result;
		}

		// Token: 0x0400181E RID: 6174
		public static volatile RisingLavaManager instance;

		// Token: 0x0400181F RID: 6175
		[SerializeField]
		private float minScale = 1f;

		// Token: 0x04001820 RID: 6176
		[SerializeField]
		private float maxScale = 10f;

		// Token: 0x04001821 RID: 6177
		[SerializeField]
		private float riseTimeFast = 30f;

		// Token: 0x04001822 RID: 6178
		[SerializeField]
		private float riseTimeMedium = 60f;

		// Token: 0x04001823 RID: 6179
		[SerializeField]
		private float riseTimeSlow = 120f;

		// Token: 0x04001824 RID: 6180
		[SerializeField]
		private float riseTimeExtraSlow = 240f;

		// Token: 0x04001825 RID: 6181
		[SerializeField]
		private float preDrainWaitTime = 3f;

		// Token: 0x04001826 RID: 6182
		[SerializeField]
		private float maxFullTime = 5f;

		// Token: 0x04001827 RID: 6183
		[SerializeField]
		private float drainTime = 10f;

		// Token: 0x04001828 RID: 6184
		[SerializeField]
		private float fullyDrainedWaitTime = 3f;

		// Token: 0x04001829 RID: 6185
		[SerializeField]
		private float lagResolutionLavaProgressPerSecond = 0.2f;

		// Token: 0x0400182A RID: 6186
		[SerializeField]
		private AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400182B RID: 6187
		[SerializeField]
		private Transform lavaMeshTransform;

		// Token: 0x0400182C RID: 6188
		[SerializeField]
		private WaterVolume lavaVolume;

		// Token: 0x0400182D RID: 6189
		[SerializeField]
		private WaterVolume entryLavaVolume;

		// Token: 0x0400182E RID: 6190
		[SerializeField]
		private WaterVolume refreshWaterVolume;

		// Token: 0x0400182F RID: 6191
		[SerializeField]
		private float lavaProgressToDisableRefreshWater = 0.18f;

		// Token: 0x04001830 RID: 6192
		[SerializeField]
		private float lavaProgressToEnableRefreshWater = 0.08f;

		// Token: 0x04001831 RID: 6193
		[SerializeField]
		private Transform entryWayLavaMeshTransform;

		// Token: 0x04001832 RID: 6194
		[SerializeField]
		private float entryLavaMaxScale = 5f;

		// Token: 0x04001833 RID: 6195
		[SerializeField]
		private Vector2 entryLavaScaleSyncOpeningTop = Vector2.zero;

		// Token: 0x04001834 RID: 6196
		[SerializeField]
		private Vector2 entryLavaScaleSyncOpeningBottom = Vector2.zero;

		// Token: 0x04001835 RID: 6197
		[SerializeField]
		private List<RisingLavaManager.DisableByLava> disableByLavaList = new List<RisingLavaManager.DisableByLava>();

		// Token: 0x04001836 RID: 6198
		[SerializeField]
		public GameObject waterBalloonPrefab;

		// Token: 0x04001837 RID: 6199
		[SerializeField]
		private Text gameStateDisplay;

		// Token: 0x04001838 RID: 6200
		[SerializeField]
		private Text gameScoreboard;

		// Token: 0x04001839 RID: 6201
		[SerializeField]
		private Text gameStartCountdown;

		// Token: 0x0400183A RID: 6202
		[SerializeField]
		private GorillaPressableButton lavaSpeedButtonFast;

		// Token: 0x0400183B RID: 6203
		[SerializeField]
		private GorillaPressableButton lavaSpeedButtonMedium;

		// Token: 0x0400183C RID: 6204
		[SerializeField]
		private GorillaPressableButton lavaSpeedButtonSlow;

		// Token: 0x0400183D RID: 6205
		[SerializeField]
		private GorillaPressableButton lavaSpeedButtonExtraSlow;

		// Token: 0x0400183E RID: 6206
		[SerializeField]
		private float infrequentUpdatePeriod = 3f;

		// Token: 0x0400183F RID: 6207
		[SerializeField]
		private bool debugDrawPlayerGameState;

		// Token: 0x04001840 RID: 6208
		private Photon.Realtime.Player[] allPlayersInRoom;

		// Token: 0x04001841 RID: 6209
		private RisingLavaManager.PlayerGameState[] inGamePlayerStates = new RisingLavaManager.PlayerGameState[10];

		// Token: 0x04001842 RID: 6210
		private int inGamePlayerCount;

		// Token: 0x04001843 RID: 6211
		private int lastWinnerId = -1;

		// Token: 0x04001844 RID: 6212
		private string lastWinnerName = "None";

		// Token: 0x04001845 RID: 6213
		private List<RisingLavaManager.PlayerGameState> sortedPlayerStates = new List<RisingLavaManager.PlayerGameState>();

		// Token: 0x04001846 RID: 6214
		private RisingLavaManager.LavaSyncData reliableState;

		// Token: 0x04001847 RID: 6215
		private RisingLavaManager.LavaSpeed nextRoundLavaSpeed = RisingLavaManager.LavaSpeed.Slow;

		// Token: 0x04001848 RID: 6216
		private float riseTime = 120f;

		// Token: 0x04001849 RID: 6217
		private float lavaProgress;

		// Token: 0x0400184A RID: 6218
		private float lavaProgressLinear;

		// Token: 0x0400184B RID: 6219
		private float localLagLavaProgressOffset;

		// Token: 0x0400184C RID: 6220
		private double lastInfrequentUpdateTime = -10.0;

		// Token: 0x0400184D RID: 6221
		private int overlappingWaterVolumes;

		// Token: 0x0400184E RID: 6222
		private float[] riseTimeLookup;

		// Token: 0x02000505 RID: 1285
		public enum RisingLavaState
		{
			// Token: 0x040020EC RID: 8428
			Drained,
			// Token: 0x040020ED RID: 8429
			Erupting,
			// Token: 0x040020EE RID: 8430
			Rising,
			// Token: 0x040020EF RID: 8431
			Full,
			// Token: 0x040020F0 RID: 8432
			PreDrainDelay,
			// Token: 0x040020F1 RID: 8433
			Draining
		}

		// Token: 0x02000506 RID: 1286
		[Serializable]
		public struct PlayerGameState
		{
			// Token: 0x040020F2 RID: 8434
			public int playerId;

			// Token: 0x040020F3 RID: 8435
			public bool touchedLava;

			// Token: 0x040020F4 RID: 8436
			public float touchedLavaAtProgress;
		}

		// Token: 0x02000507 RID: 1287
		private struct LavaSyncData
		{
			// Token: 0x040020F5 RID: 8437
			public RisingLavaManager.RisingLavaState state;

			// Token: 0x040020F6 RID: 8438
			public double stateStartTime;

			// Token: 0x040020F7 RID: 8439
			public float stateStartLavaProgressLinear;
		}

		// Token: 0x02000508 RID: 1288
		private enum LavaSpeed
		{
			// Token: 0x040020F9 RID: 8441
			Fast,
			// Token: 0x040020FA RID: 8442
			Medium,
			// Token: 0x040020FB RID: 8443
			Slow,
			// Token: 0x040020FC RID: 8444
			ExtraSlow
		}

		// Token: 0x02000509 RID: 1289
		[Serializable]
		private struct DisableByLava
		{
			// Token: 0x040020FD RID: 8445
			public Transform transform;

			// Token: 0x040020FE RID: 8446
			public float lavaScaleToDisable;

			// Token: 0x040020FF RID: 8447
			public float lavaScaleToReenable;
		}
	}
}
