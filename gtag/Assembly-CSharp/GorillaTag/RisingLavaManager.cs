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
	// Token: 0x0200031E RID: 798
	public class RisingLavaManager : MonoBehaviourPun, IPunObservable, IInRoomCallbacks
	{
		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06001614 RID: 5652 RVA: 0x0007A684 File Offset: 0x00078884
		private bool RefreshWaterAvailable
		{
			get
			{
				return this.reliableState.state == RisingLavaManager.RisingLavaState.Drained || this.reliableState.state == RisingLavaManager.RisingLavaState.Erupting || (this.reliableState.state == RisingLavaManager.RisingLavaState.Rising && this.lavaProgress < this.lavaProgressToDisableRefreshWater) || (this.reliableState.state == RisingLavaManager.RisingLavaState.Draining && this.lavaProgress < this.lavaProgressToEnableRefreshWater);
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06001615 RID: 5653 RVA: 0x0007A6E8 File Offset: 0x000788E8
		public RisingLavaManager.RisingLavaState GameState
		{
			get
			{
				return this.reliableState.state;
			}
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06001616 RID: 5654 RVA: 0x0007A6F5 File Offset: 0x000788F5
		public float LavaProgress
		{
			get
			{
				return this.lavaProgress;
			}
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06001617 RID: 5655 RVA: 0x0007A6FD File Offset: 0x000788FD
		public float LavaProgressLinear
		{
			get
			{
				return this.lavaProgressLinear;
			}
		}

		// Token: 0x06001618 RID: 5656 RVA: 0x0007A708 File Offset: 0x00078908
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

		// Token: 0x06001619 RID: 5657 RVA: 0x0007A89C File Offset: 0x00078A9C
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

		// Token: 0x0600161A RID: 5658 RVA: 0x0007A9BC File Offset: 0x00078BBC
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

		// Token: 0x0600161B RID: 5659 RVA: 0x0007AB8C File Offset: 0x00078D8C
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

		// Token: 0x0600161C RID: 5660 RVA: 0x0007AC5C File Offset: 0x00078E5C
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

		// Token: 0x0600161D RID: 5661 RVA: 0x0007AC98 File Offset: 0x00078E98
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

		// Token: 0x0600161E RID: 5662 RVA: 0x0007AE4C File Offset: 0x0007904C
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

		// Token: 0x0600161F RID: 5663 RVA: 0x0007AF64 File Offset: 0x00079164
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

		// Token: 0x06001620 RID: 5664 RVA: 0x0007B078 File Offset: 0x00079278
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

		// Token: 0x06001621 RID: 5665 RVA: 0x0007B160 File Offset: 0x00079360
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

		// Token: 0x06001622 RID: 5666 RVA: 0x0007B1F4 File Offset: 0x000793F4
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

		// Token: 0x06001623 RID: 5667 RVA: 0x0007B288 File Offset: 0x00079488
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

		// Token: 0x06001624 RID: 5668 RVA: 0x0007B2BD File Offset: 0x000794BD
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

		// Token: 0x06001625 RID: 5669 RVA: 0x0007B2E8 File Offset: 0x000794E8
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

		// Token: 0x06001626 RID: 5670 RVA: 0x0007B34C File Offset: 0x0007954C
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

		// Token: 0x06001627 RID: 5671 RVA: 0x0007B398 File Offset: 0x00079598
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

		// Token: 0x06001628 RID: 5672 RVA: 0x0007B3F8 File Offset: 0x000795F8
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

		// Token: 0x06001629 RID: 5673 RVA: 0x0007B44C File Offset: 0x0007964C
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

		// Token: 0x0600162A RID: 5674 RVA: 0x0007B4B0 File Offset: 0x000796B0
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

		// Token: 0x0600162B RID: 5675 RVA: 0x0007B524 File Offset: 0x00079724
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

		// Token: 0x0600162C RID: 5676 RVA: 0x0007B577 File Offset: 0x00079777
		private void OnColliderExitedLava(Collider collider)
		{
		}

		// Token: 0x0600162D RID: 5677 RVA: 0x0007B57C File Offset: 0x0007977C
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

		// Token: 0x0600162E RID: 5678 RVA: 0x0007B5CF File Offset: 0x000797CF
		private void OnColliderExitedRefreshWater(Collider collider)
		{
		}

		// Token: 0x0600162F RID: 5679 RVA: 0x0007B5D4 File Offset: 0x000797D4
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

		// Token: 0x06001630 RID: 5680 RVA: 0x0007B64C File Offset: 0x0007984C
		public void RestartButtonPressed()
		{
			if (base.photonView.IsMine)
			{
				this.RestartGame();
				return;
			}
			base.photonView.RPC("RestartGameRPC", RpcTarget.MasterClient, Array.Empty<object>());
		}

		// Token: 0x06001631 RID: 5681 RVA: 0x0007B678 File Offset: 0x00079878
		public void SetSpeedButtonPressedFast()
		{
			this.SetRiseTimeButtonPressed(RisingLavaManager.LavaSpeed.Fast);
		}

		// Token: 0x06001632 RID: 5682 RVA: 0x0007B681 File Offset: 0x00079881
		public void SetSpeedButtonPressedMedium()
		{
			this.SetRiseTimeButtonPressed(RisingLavaManager.LavaSpeed.Medium);
		}

		// Token: 0x06001633 RID: 5683 RVA: 0x0007B68A File Offset: 0x0007988A
		public void SetSpeedButtonPressedSlow()
		{
			this.SetRiseTimeButtonPressed(RisingLavaManager.LavaSpeed.Slow);
		}

		// Token: 0x06001634 RID: 5684 RVA: 0x0007B693 File Offset: 0x00079893
		public void SetSpeedButtonPressedExtraSlow()
		{
			this.SetRiseTimeButtonPressed(RisingLavaManager.LavaSpeed.ExtraSlow);
		}

		// Token: 0x06001635 RID: 5685 RVA: 0x0007B69C File Offset: 0x0007989C
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

		// Token: 0x06001636 RID: 5686 RVA: 0x0007B725 File Offset: 0x00079925
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

		// Token: 0x06001637 RID: 5687 RVA: 0x0007B75C File Offset: 0x0007995C
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

		// Token: 0x06001638 RID: 5688 RVA: 0x0007BA4B File Offset: 0x00079C4B
		[PunRPC]
		public void PlayerEnteredGameAreaRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerEnteredGameAreaRPC");
			this.PlayerEnteredGameArea(info.Sender.ActorNumber);
		}

		// Token: 0x06001639 RID: 5689 RVA: 0x0007BA6C File Offset: 0x00079C6C
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

		// Token: 0x0600163A RID: 5690 RVA: 0x0007BB04 File Offset: 0x00079D04
		[PunRPC]
		public void PlayerExitedGameAreaRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerExitedGameAreaRPC");
			this.PlayerExitedGameArea(info.Sender.ActorNumber);
		}

		// Token: 0x0600163B RID: 5691 RVA: 0x0007BB24 File Offset: 0x00079D24
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

		// Token: 0x0600163C RID: 5692 RVA: 0x0007BB91 File Offset: 0x00079D91
		[PunRPC]
		public void PlayerTouchedLavaRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerTouchedLavaRPC");
			this.PlayerTouchedLava(info.Sender.ActorNumber);
		}

		// Token: 0x0600163D RID: 5693 RVA: 0x0007BBB0 File Offset: 0x00079DB0
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

		// Token: 0x0600163E RID: 5694 RVA: 0x0007BC27 File Offset: 0x00079E27
		[PunRPC]
		private void PlayerTouchedRefreshWaterRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerTouchedRefreshWaterRPC");
			this.PlayerTouchedRefreshWater(info.Sender.ActorNumber);
		}

		// Token: 0x0600163F RID: 5695 RVA: 0x0007BC48 File Offset: 0x00079E48
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

		// Token: 0x06001640 RID: 5696 RVA: 0x0007BCBE File Offset: 0x00079EBE
		[PunRPC]
		private void ValidateLocalPlayerWaterBalloonHitRPC(int playerId, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "ValidateLocalPlayerWaterBalloonHitRPC");
			if (playerId == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				this.ValidateLocalPlayerWaterBalloonHit(playerId);
			}
		}

		// Token: 0x06001641 RID: 5697 RVA: 0x0007BCE0 File Offset: 0x00079EE0
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

		// Token: 0x06001642 RID: 5698 RVA: 0x0007BD4D File Offset: 0x00079F4D
		[PunRPC]
		private void PlayerHitByWaterBalloonRPC(int playerId, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerHitByWaterBalloonRPC");
			this.PlayerHitByWaterBalloon(playerId);
		}

		// Token: 0x06001643 RID: 5699 RVA: 0x0007BD64 File Offset: 0x00079F64
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

		// Token: 0x06001644 RID: 5700 RVA: 0x0007BDD2 File Offset: 0x00079FD2
		[PunRPC]
		private void RestartGameRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RestartGameRPC");
			this.RestartGame();
		}

		// Token: 0x06001645 RID: 5701 RVA: 0x0007BDE5 File Offset: 0x00079FE5
		[PunRPC]
		private void SetRiseTimeRPC(RisingLavaManager.LavaSpeed lavaSpeed, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SetRiseTimeRPC");
			this.SetRiseTime(lavaSpeed);
		}

		// Token: 0x06001646 RID: 5702 RVA: 0x0007BDF9 File Offset: 0x00079FF9
		private void SetRiseTime(RisingLavaManager.LavaSpeed lavaSpeed)
		{
			if (base.photonView.IsMine)
			{
				this.nextRoundLavaSpeed = lavaSpeed;
				this.RefreshSpeedSelectButtons();
			}
		}

		// Token: 0x06001647 RID: 5703 RVA: 0x0007BE15 File Offset: 0x0007A015
		public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
		{
		}

		// Token: 0x06001648 RID: 5704 RVA: 0x0007BE17 File Offset: 0x0007A017
		public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
		{
			this.PlayerExitedGameArea(otherPlayer.ActorNumber);
		}

		// Token: 0x06001649 RID: 5705 RVA: 0x0007BE25 File Offset: 0x0007A025
		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		// Token: 0x0600164A RID: 5706 RVA: 0x0007BE27 File Offset: 0x0007A027
		public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
		{
		}

		// Token: 0x0600164B RID: 5707 RVA: 0x0007BE29 File Offset: 0x0007A029
		public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
		{
		}

		// Token: 0x0600164D RID: 5709 RVA: 0x0007BF70 File Offset: 0x0007A170
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

		// Token: 0x0600164E RID: 5710 RVA: 0x0007BFA8 File Offset: 0x0007A1A8
		[CompilerGenerated]
		internal static void <OnPhotonSerializeView>g__SendPlayerGameState|93_0(PhotonStream photonStream, ref RisingLavaManager.PlayerGameState gameState)
		{
			photonStream.SendNext(gameState.playerId);
			photonStream.SendNext(gameState.touchedLava);
			photonStream.SendNext(gameState.touchedLavaAtProgress);
		}

		// Token: 0x0600164F RID: 5711 RVA: 0x0007BFE0 File Offset: 0x0007A1E0
		[CompilerGenerated]
		internal static RisingLavaManager.PlayerGameState <OnPhotonSerializeView>g__ReceivePlayerGameState|93_1(PhotonStream photonStream)
		{
			RisingLavaManager.PlayerGameState result;
			result.playerId = (int)photonStream.ReceiveNext();
			result.touchedLava = (bool)photonStream.ReceiveNext();
			result.touchedLavaAtProgress = (float)photonStream.ReceiveNext();
			return result;
		}

		// Token: 0x0400182B RID: 6187
		public static volatile RisingLavaManager instance;

		// Token: 0x0400182C RID: 6188
		[SerializeField]
		private float minScale = 1f;

		// Token: 0x0400182D RID: 6189
		[SerializeField]
		private float maxScale = 10f;

		// Token: 0x0400182E RID: 6190
		[SerializeField]
		private float riseTimeFast = 30f;

		// Token: 0x0400182F RID: 6191
		[SerializeField]
		private float riseTimeMedium = 60f;

		// Token: 0x04001830 RID: 6192
		[SerializeField]
		private float riseTimeSlow = 120f;

		// Token: 0x04001831 RID: 6193
		[SerializeField]
		private float riseTimeExtraSlow = 240f;

		// Token: 0x04001832 RID: 6194
		[SerializeField]
		private float preDrainWaitTime = 3f;

		// Token: 0x04001833 RID: 6195
		[SerializeField]
		private float maxFullTime = 5f;

		// Token: 0x04001834 RID: 6196
		[SerializeField]
		private float drainTime = 10f;

		// Token: 0x04001835 RID: 6197
		[SerializeField]
		private float fullyDrainedWaitTime = 3f;

		// Token: 0x04001836 RID: 6198
		[SerializeField]
		private float lagResolutionLavaProgressPerSecond = 0.2f;

		// Token: 0x04001837 RID: 6199
		[SerializeField]
		private AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04001838 RID: 6200
		[SerializeField]
		private Transform lavaMeshTransform;

		// Token: 0x04001839 RID: 6201
		[SerializeField]
		private WaterVolume lavaVolume;

		// Token: 0x0400183A RID: 6202
		[SerializeField]
		private WaterVolume entryLavaVolume;

		// Token: 0x0400183B RID: 6203
		[SerializeField]
		private WaterVolume refreshWaterVolume;

		// Token: 0x0400183C RID: 6204
		[SerializeField]
		private float lavaProgressToDisableRefreshWater = 0.18f;

		// Token: 0x0400183D RID: 6205
		[SerializeField]
		private float lavaProgressToEnableRefreshWater = 0.08f;

		// Token: 0x0400183E RID: 6206
		[SerializeField]
		private Transform entryWayLavaMeshTransform;

		// Token: 0x0400183F RID: 6207
		[SerializeField]
		private float entryLavaMaxScale = 5f;

		// Token: 0x04001840 RID: 6208
		[SerializeField]
		private Vector2 entryLavaScaleSyncOpeningTop = Vector2.zero;

		// Token: 0x04001841 RID: 6209
		[SerializeField]
		private Vector2 entryLavaScaleSyncOpeningBottom = Vector2.zero;

		// Token: 0x04001842 RID: 6210
		[SerializeField]
		private List<RisingLavaManager.DisableByLava> disableByLavaList = new List<RisingLavaManager.DisableByLava>();

		// Token: 0x04001843 RID: 6211
		[SerializeField]
		public GameObject waterBalloonPrefab;

		// Token: 0x04001844 RID: 6212
		[SerializeField]
		private Text gameStateDisplay;

		// Token: 0x04001845 RID: 6213
		[SerializeField]
		private Text gameScoreboard;

		// Token: 0x04001846 RID: 6214
		[SerializeField]
		private Text gameStartCountdown;

		// Token: 0x04001847 RID: 6215
		[SerializeField]
		private GorillaPressableButton lavaSpeedButtonFast;

		// Token: 0x04001848 RID: 6216
		[SerializeField]
		private GorillaPressableButton lavaSpeedButtonMedium;

		// Token: 0x04001849 RID: 6217
		[SerializeField]
		private GorillaPressableButton lavaSpeedButtonSlow;

		// Token: 0x0400184A RID: 6218
		[SerializeField]
		private GorillaPressableButton lavaSpeedButtonExtraSlow;

		// Token: 0x0400184B RID: 6219
		[SerializeField]
		private float infrequentUpdatePeriod = 3f;

		// Token: 0x0400184C RID: 6220
		[SerializeField]
		private bool debugDrawPlayerGameState;

		// Token: 0x0400184D RID: 6221
		private Photon.Realtime.Player[] allPlayersInRoom;

		// Token: 0x0400184E RID: 6222
		private RisingLavaManager.PlayerGameState[] inGamePlayerStates = new RisingLavaManager.PlayerGameState[10];

		// Token: 0x0400184F RID: 6223
		private int inGamePlayerCount;

		// Token: 0x04001850 RID: 6224
		private int lastWinnerId = -1;

		// Token: 0x04001851 RID: 6225
		private string lastWinnerName = "None";

		// Token: 0x04001852 RID: 6226
		private List<RisingLavaManager.PlayerGameState> sortedPlayerStates = new List<RisingLavaManager.PlayerGameState>();

		// Token: 0x04001853 RID: 6227
		private RisingLavaManager.LavaSyncData reliableState;

		// Token: 0x04001854 RID: 6228
		private RisingLavaManager.LavaSpeed nextRoundLavaSpeed = RisingLavaManager.LavaSpeed.Slow;

		// Token: 0x04001855 RID: 6229
		private float riseTime = 120f;

		// Token: 0x04001856 RID: 6230
		private float lavaProgress;

		// Token: 0x04001857 RID: 6231
		private float lavaProgressLinear;

		// Token: 0x04001858 RID: 6232
		private float localLagLavaProgressOffset;

		// Token: 0x04001859 RID: 6233
		private double lastInfrequentUpdateTime = -10.0;

		// Token: 0x0400185A RID: 6234
		private int overlappingWaterVolumes;

		// Token: 0x0400185B RID: 6235
		private float[] riseTimeLookup;

		// Token: 0x02000507 RID: 1287
		public enum RisingLavaState
		{
			// Token: 0x040020F9 RID: 8441
			Drained,
			// Token: 0x040020FA RID: 8442
			Erupting,
			// Token: 0x040020FB RID: 8443
			Rising,
			// Token: 0x040020FC RID: 8444
			Full,
			// Token: 0x040020FD RID: 8445
			PreDrainDelay,
			// Token: 0x040020FE RID: 8446
			Draining
		}

		// Token: 0x02000508 RID: 1288
		[Serializable]
		public struct PlayerGameState
		{
			// Token: 0x040020FF RID: 8447
			public int playerId;

			// Token: 0x04002100 RID: 8448
			public bool touchedLava;

			// Token: 0x04002101 RID: 8449
			public float touchedLavaAtProgress;
		}

		// Token: 0x02000509 RID: 1289
		private struct LavaSyncData
		{
			// Token: 0x04002102 RID: 8450
			public RisingLavaManager.RisingLavaState state;

			// Token: 0x04002103 RID: 8451
			public double stateStartTime;

			// Token: 0x04002104 RID: 8452
			public float stateStartLavaProgressLinear;
		}

		// Token: 0x0200050A RID: 1290
		private enum LavaSpeed
		{
			// Token: 0x04002106 RID: 8454
			Fast,
			// Token: 0x04002107 RID: 8455
			Medium,
			// Token: 0x04002108 RID: 8456
			Slow,
			// Token: 0x04002109 RID: 8457
			ExtraSlow
		}

		// Token: 0x0200050B RID: 1291
		[Serializable]
		private struct DisableByLava
		{
			// Token: 0x0400210A RID: 8458
			public Transform transform;

			// Token: 0x0400210B RID: 8459
			public float lavaScaleToDisable;

			// Token: 0x0400210C RID: 8460
			public float lavaScaleToReenable;
		}
	}
}
