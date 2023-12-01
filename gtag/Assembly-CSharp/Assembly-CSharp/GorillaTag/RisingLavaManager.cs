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
	public class RisingLavaManager : MonoBehaviourPun, IPunObservable, IInRoomCallbacks
	{
		private bool RefreshWaterAvailable
		{
			get
			{
				return this.reliableState.state == RisingLavaManager.RisingLavaState.Drained || this.reliableState.state == RisingLavaManager.RisingLavaState.Erupting || (this.reliableState.state == RisingLavaManager.RisingLavaState.Rising && this.lavaProgress < this.lavaProgressToDisableRefreshWater) || (this.reliableState.state == RisingLavaManager.RisingLavaState.Draining && this.lavaProgress < this.lavaProgressToEnableRefreshWater);
			}
		}

		public RisingLavaManager.RisingLavaState GameState
		{
			get
			{
				return this.reliableState.state;
			}
		}

		public float LavaProgress
		{
			get
			{
				return this.lavaProgress;
			}
		}

		public float LavaProgressLinear
		{
			get
			{
				return this.lavaProgressLinear;
			}
		}

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

		private void OnColliderExitedLava(Collider collider)
		{
		}

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

		private void OnColliderExitedRefreshWater(Collider collider)
		{
		}

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

		public void RestartButtonPressed()
		{
			if (base.photonView.IsMine)
			{
				this.RestartGame();
				return;
			}
			base.photonView.RPC("RestartGameRPC", RpcTarget.MasterClient, Array.Empty<object>());
		}

		public void SetSpeedButtonPressedFast()
		{
			this.SetRiseTimeButtonPressed(RisingLavaManager.LavaSpeed.Fast);
		}

		public void SetSpeedButtonPressedMedium()
		{
			this.SetRiseTimeButtonPressed(RisingLavaManager.LavaSpeed.Medium);
		}

		public void SetSpeedButtonPressedSlow()
		{
			this.SetRiseTimeButtonPressed(RisingLavaManager.LavaSpeed.Slow);
		}

		public void SetSpeedButtonPressedExtraSlow()
		{
			this.SetRiseTimeButtonPressed(RisingLavaManager.LavaSpeed.ExtraSlow);
		}

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

		[PunRPC]
		public void PlayerEnteredGameAreaRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerEnteredGameAreaRPC");
			this.PlayerEnteredGameArea(info.Sender.ActorNumber);
		}

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

		[PunRPC]
		public void PlayerExitedGameAreaRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerExitedGameAreaRPC");
			this.PlayerExitedGameArea(info.Sender.ActorNumber);
		}

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

		[PunRPC]
		public void PlayerTouchedLavaRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerTouchedLavaRPC");
			this.PlayerTouchedLava(info.Sender.ActorNumber);
		}

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

		[PunRPC]
		private void PlayerTouchedRefreshWaterRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerTouchedRefreshWaterRPC");
			this.PlayerTouchedRefreshWater(info.Sender.ActorNumber);
		}

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

		[PunRPC]
		private void ValidateLocalPlayerWaterBalloonHitRPC(int playerId, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "ValidateLocalPlayerWaterBalloonHitRPC");
			if (playerId == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				this.ValidateLocalPlayerWaterBalloonHit(playerId);
			}
		}

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

		[PunRPC]
		private void PlayerHitByWaterBalloonRPC(int playerId, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "PlayerHitByWaterBalloonRPC");
			this.PlayerHitByWaterBalloon(playerId);
		}

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

		[PunRPC]
		private void RestartGameRPC(PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "RestartGameRPC");
			this.RestartGame();
		}

		[PunRPC]
		private void SetRiseTimeRPC(RisingLavaManager.LavaSpeed lavaSpeed, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SetRiseTimeRPC");
			this.SetRiseTime(lavaSpeed);
		}

		private void SetRiseTime(RisingLavaManager.LavaSpeed lavaSpeed)
		{
			if (base.photonView.IsMine)
			{
				this.nextRoundLavaSpeed = lavaSpeed;
				this.RefreshSpeedSelectButtons();
			}
		}

		public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
		{
		}

		public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
		{
			this.PlayerExitedGameArea(otherPlayer.ActorNumber);
		}

		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
		{
		}

		public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
		{
		}

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

		[CompilerGenerated]
		internal static void <OnPhotonSerializeView>g__SendPlayerGameState|93_0(PhotonStream photonStream, ref RisingLavaManager.PlayerGameState gameState)
		{
			photonStream.SendNext(gameState.playerId);
			photonStream.SendNext(gameState.touchedLava);
			photonStream.SendNext(gameState.touchedLavaAtProgress);
		}

		[CompilerGenerated]
		internal static RisingLavaManager.PlayerGameState <OnPhotonSerializeView>g__ReceivePlayerGameState|93_1(PhotonStream photonStream)
		{
			RisingLavaManager.PlayerGameState result;
			result.playerId = (int)photonStream.ReceiveNext();
			result.touchedLava = (bool)photonStream.ReceiveNext();
			result.touchedLavaAtProgress = (float)photonStream.ReceiveNext();
			return result;
		}

		[OnEnterPlay_SetNull]
		public static volatile RisingLavaManager instance;

		[SerializeField]
		private float minScale = 1f;

		[SerializeField]
		private float maxScale = 10f;

		[SerializeField]
		private float riseTimeFast = 30f;

		[SerializeField]
		private float riseTimeMedium = 60f;

		[SerializeField]
		private float riseTimeSlow = 120f;

		[SerializeField]
		private float riseTimeExtraSlow = 240f;

		[SerializeField]
		private float preDrainWaitTime = 3f;

		[SerializeField]
		private float maxFullTime = 5f;

		[SerializeField]
		private float drainTime = 10f;

		[SerializeField]
		private float fullyDrainedWaitTime = 3f;

		[SerializeField]
		private float lagResolutionLavaProgressPerSecond = 0.2f;

		[SerializeField]
		private AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[SerializeField]
		private Transform lavaMeshTransform;

		[SerializeField]
		private WaterVolume lavaVolume;

		[SerializeField]
		private WaterVolume entryLavaVolume;

		[SerializeField]
		private WaterVolume refreshWaterVolume;

		[SerializeField]
		private float lavaProgressToDisableRefreshWater = 0.18f;

		[SerializeField]
		private float lavaProgressToEnableRefreshWater = 0.08f;

		[SerializeField]
		private Transform entryWayLavaMeshTransform;

		[SerializeField]
		private float entryLavaMaxScale = 5f;

		[SerializeField]
		private Vector2 entryLavaScaleSyncOpeningTop = Vector2.zero;

		[SerializeField]
		private Vector2 entryLavaScaleSyncOpeningBottom = Vector2.zero;

		[SerializeField]
		private List<RisingLavaManager.DisableByLava> disableByLavaList = new List<RisingLavaManager.DisableByLava>();

		[SerializeField]
		public GameObject waterBalloonPrefab;

		[SerializeField]
		private Text gameStateDisplay;

		[SerializeField]
		private Text gameScoreboard;

		[SerializeField]
		private Text gameStartCountdown;

		[SerializeField]
		private GorillaPressableButton lavaSpeedButtonFast;

		[SerializeField]
		private GorillaPressableButton lavaSpeedButtonMedium;

		[SerializeField]
		private GorillaPressableButton lavaSpeedButtonSlow;

		[SerializeField]
		private GorillaPressableButton lavaSpeedButtonExtraSlow;

		[SerializeField]
		private float infrequentUpdatePeriod = 3f;

		[SerializeField]
		private bool debugDrawPlayerGameState;

		private Photon.Realtime.Player[] allPlayersInRoom;

		private RisingLavaManager.PlayerGameState[] inGamePlayerStates = new RisingLavaManager.PlayerGameState[10];

		private int inGamePlayerCount;

		private int lastWinnerId = -1;

		private string lastWinnerName = "None";

		private List<RisingLavaManager.PlayerGameState> sortedPlayerStates = new List<RisingLavaManager.PlayerGameState>();

		private RisingLavaManager.LavaSyncData reliableState;

		private RisingLavaManager.LavaSpeed nextRoundLavaSpeed = RisingLavaManager.LavaSpeed.Slow;

		private float riseTime = 120f;

		private float lavaProgress;

		private float lavaProgressLinear;

		private float localLagLavaProgressOffset;

		private double lastInfrequentUpdateTime = -10.0;

		private int overlappingWaterVolumes;

		private float[] riseTimeLookup;

		public enum RisingLavaState
		{
			Drained,
			Erupting,
			Rising,
			Full,
			PreDrainDelay,
			Draining
		}

		[Serializable]
		public struct PlayerGameState
		{
			public int playerId;

			public bool touchedLava;

			public float touchedLavaAtProgress;
		}

		private struct LavaSyncData
		{
			public RisingLavaManager.RisingLavaState state;

			public double stateStartTime;

			public float stateStartLavaProgressLinear;
		}

		private enum LavaSpeed
		{
			Fast,
			Medium,
			Slow,
			ExtraSlow
		}

		[Serializable]
		private struct DisableByLava
		{
			public Transform transform;

			public float lavaScaleToDisable;

			public float lavaScaleToReenable;
		}
	}
}
