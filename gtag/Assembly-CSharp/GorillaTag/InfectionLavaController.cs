using System;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200031A RID: 794
	public class InfectionLavaController : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
	{
		// Token: 0x1700017D RID: 381
		// (get) Token: 0x060015E2 RID: 5602 RVA: 0x00078BF2 File Offset: 0x00076DF2
		public static InfectionLavaController Instance
		{
			get
			{
				return InfectionLavaController.instance;
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x060015E3 RID: 5603 RVA: 0x00078BF9 File Offset: 0x00076DF9
		public bool LavaCurrentlyActivated
		{
			get
			{
				return this.reliableState.state > InfectionLavaController.RisingLavaState.Drained;
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x060015E4 RID: 5604 RVA: 0x00078C09 File Offset: 0x00076E09
		public Plane LavaPlane
		{
			get
			{
				return new Plane(this.lavaSurfacePlaneTransform.up, this.lavaSurfacePlaneTransform.position);
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x060015E5 RID: 5605 RVA: 0x00078C26 File Offset: 0x00076E26
		public Vector3 SurfaceCenter
		{
			get
			{
				return this.lavaSurfacePlaneTransform.position;
			}
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x060015E6 RID: 5606 RVA: 0x00078C34 File Offset: 0x00076E34
		private int PlayerCount
		{
			get
			{
				int result = 1;
				GorillaGameManager gorillaGameManager = GorillaGameManager.instance;
				if (gorillaGameManager != null && gorillaGameManager.currentPlayerArray != null)
				{
					result = gorillaGameManager.currentPlayerArray.Length;
				}
				return result;
			}
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x060015E7 RID: 5607 RVA: 0x00078C68 File Offset: 0x00076E68
		private bool InCompetitiveQueue
		{
			get
			{
				object obj;
				return PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj) && obj.ToString().Contains("COMPETITIVE");
			}
		}

		// Token: 0x060015E8 RID: 5608 RVA: 0x00078CA0 File Offset: 0x00076EA0
		void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
		{
			if (!(InfectionLavaController.instance != null) || !(InfectionLavaController.instance != this))
			{
				InfectionLavaController.instance = this;
				base.photonView.AddCallbackTarget(this);
				if (GorillaParent.instance != null && base.transform != null)
				{
					base.transform.parent = GorillaParent.instance.transform;
				}
				return;
			}
			if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
			{
				Debug.Log("Existing InfectionLavaController! I'm host. Destroying newly created manager");
				PhotonNetwork.Destroy(base.gameObject);
				return;
			}
			Debug.Log("Existing PhotonNetwork! I'm not host. Destroying newly created manager");
			Object.Destroy(base.gameObject);
		}

		// Token: 0x060015E9 RID: 5609 RVA: 0x00078D47 File Offset: 0x00076F47
		void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
		{
			if (InfectionLavaController.instance == this)
			{
				InfectionLavaController.instance = null;
			}
		}

		// Token: 0x060015EA RID: 5610 RVA: 0x00078D5C File Offset: 0x00076F5C
		private void Awake()
		{
			GorillaGameModeReferences gorillaGameModeReferences = GorillaGameModeReferences.Instance;
			this.IfNullThenLogAndDisableSelf(gorillaGameModeReferences, "GorillaGameModeReferences.Instance", -1);
			this.lavaMeshTransform = gorillaGameModeReferences.lavaMeshTransform;
			this.lavaSurfacePlaneTransform = gorillaGameModeReferences.lavaSurfacePlaneTransform;
			this.lavaVolume = gorillaGameModeReferences.lavaVolume;
			this.lavaActivationRenderer = gorillaGameModeReferences.lavaActivationRenderer;
			this.lavaActivationStartPos = gorillaGameModeReferences.lavaActivationStartPos;
			this.lavaActivationEndPos = gorillaGameModeReferences.lavaActivationEndPos;
			this.lavaActivationProjectileHitNotifier = gorillaGameModeReferences.lavaActivationProjectileHitNotifier;
			this.volcanoEffects = gorillaGameModeReferences.volcanoEffects;
			if (this.lavaVolume != null)
			{
				this.lavaVolume.ColliderEnteredWater += this.OnColliderEnteredLava;
			}
			if (this.lavaActivationProjectileHitNotifier != null)
			{
				this.lavaActivationProjectileHitNotifier.OnProjectileHit += this.OnActivationLavaProjectileHit;
			}
		}

		// Token: 0x060015EB RID: 5611 RVA: 0x00078E28 File Offset: 0x00077028
		protected void OnEnable()
		{
			this.IfNullThenLogAndDisableSelf(this.lavaMeshTransform, "lavaMeshTransform", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaSurfacePlaneTransform, "lavaSurfacePlaneTransform", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaVolume, "lavaVolume", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaActivationRenderer, "lavaActivationRenderer", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaActivationStartPos, "lavaActivationStartPos", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaActivationEndPos, "lavaActivationEndPos", -1);
			this.IfNullThenLogAndDisableSelf(this.lavaActivationProjectileHitNotifier, "lavaActivationProjectileHitNotifier", -1);
			for (int i = 0; i < this.volcanoEffects.Length; i++)
			{
				this.IfNullThenLogAndDisableSelf(this.volcanoEffects[i], "volcanoEffects", i);
			}
		}

		// Token: 0x060015EC RID: 5612 RVA: 0x00078EDC File Offset: 0x000770DC
		private void IfNullThenLogAndDisableSelf(Object obj, string fieldName, int index = -1)
		{
			if (obj != null)
			{
				return;
			}
			fieldName = ((index != -1) ? string.Format("{0}[{1}]", fieldName, index) : fieldName);
			Debug.LogError("InfectionLavaController: Disabling self because reference `" + fieldName + "` is null.", this);
			base.enabled = false;
		}

		// Token: 0x060015ED RID: 5613 RVA: 0x00078F2C File Offset: 0x0007712C
		private void OnDestroy()
		{
			this.UpdateLava(0f);
			if (this.lavaVolume != null)
			{
				this.lavaVolume.ColliderEnteredWater -= this.OnColliderEnteredLava;
			}
			if (this.lavaActivationProjectileHitNotifier != null)
			{
				this.lavaActivationProjectileHitNotifier.OnProjectileHit -= this.OnActivationLavaProjectileHit;
			}
		}

		// Token: 0x060015EE RID: 5614 RVA: 0x00078F90 File Offset: 0x00077190
		private void Update()
		{
			this.prevTime = this.currentTime;
			this.currentTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			if (base.photonView.IsMine)
			{
				this.UpdateReliableState(this.currentTime, ref this.reliableState);
			}
			this.UpdateLocalState(this.currentTime, this.reliableState);
			this.localLagLavaProgressOffset = Mathf.MoveTowards(this.localLagLavaProgressOffset, 0f, this.lagResolutionLavaProgressPerSecond * Time.deltaTime);
			this.UpdateLava(this.lavaProgressSmooth + this.localLagLavaProgressOffset);
			this.UpdateVolcanoActivationLava((float)this.reliableState.activationProgress);
			this.CheckLocalPlayerAgainstLava(this.currentTime);
		}

		// Token: 0x060015EF RID: 5615 RVA: 0x00079048 File Offset: 0x00077248
		private void UpdateReliableState(double currentTime, ref InfectionLavaController.LavaSyncData syncData)
		{
			if (currentTime < syncData.stateStartTime)
			{
				syncData.stateStartTime = currentTime;
			}
			switch (syncData.state)
			{
			default:
				if (syncData.activationProgress > 1.0)
				{
					float playerCount = (float)this.PlayerCount;
					float num = this.InCompetitiveQueue ? this.activationVotePercentageCompetitiveQueue : this.activationVotePercentageDefaultQueue;
					int num2 = Mathf.RoundToInt(playerCount * num);
					if (this.lavaActivationVoteCount >= num2)
					{
						for (int i = 0; i < this.lavaActivationVoteCount; i++)
						{
							this.lavaActivationVotePlayerIds[i] = 0;
						}
						this.lavaActivationVoteCount = 0;
						syncData.state = InfectionLavaController.RisingLavaState.Erupting;
						syncData.stateStartTime = currentTime;
						syncData.activationProgress = 1.0;
						return;
					}
				}
				else
				{
					float num3 = Mathf.Clamp((float)(currentTime - this.prevTime), 0f, 0.1f);
					double activationProgress = syncData.activationProgress;
					syncData.activationProgress = (double)Mathf.MoveTowards((float)syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * num3);
					if (activationProgress > 0.0 && syncData.activationProgress <= 5E-324)
					{
						VolcanoEffects[] array = this.volcanoEffects;
						for (int j = 0; j < array.Length; j++)
						{
							array[j].OnVolcanoBellyEmpty();
						}
						return;
					}
				}
				break;
			case InfectionLavaController.RisingLavaState.Erupting:
				if (currentTime > syncData.stateStartTime + (double)this.eruptTime)
				{
					syncData.state = InfectionLavaController.RisingLavaState.Rising;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case InfectionLavaController.RisingLavaState.Rising:
				if (currentTime > syncData.stateStartTime + (double)this.riseTime)
				{
					syncData.state = InfectionLavaController.RisingLavaState.Full;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case InfectionLavaController.RisingLavaState.Full:
				if (currentTime > syncData.stateStartTime + (double)this.fullTime)
				{
					syncData.state = InfectionLavaController.RisingLavaState.Draining;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case InfectionLavaController.RisingLavaState.Draining:
				syncData.activationProgress = (double)Mathf.MoveTowards((float)syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * Time.deltaTime);
				if (currentTime > syncData.stateStartTime + (double)this.drainTime)
				{
					syncData.state = InfectionLavaController.RisingLavaState.Drained;
					syncData.stateStartTime = currentTime;
				}
				break;
			}
		}

		// Token: 0x060015F0 RID: 5616 RVA: 0x00079254 File Offset: 0x00077454
		private void UpdateLocalState(double currentTime, InfectionLavaController.LavaSyncData syncData)
		{
			switch (syncData.state)
			{
			default:
			{
				this.lavaProgressLinear = 0f;
				this.lavaProgressSmooth = 0f;
				float time = (float)(currentTime - syncData.stateStartTime);
				foreach (VolcanoEffects volcanoEffects in this.volcanoEffects)
				{
					if (volcanoEffects != null)
					{
						volcanoEffects.UpdateDrainedState(time);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Erupting:
			{
				this.lavaProgressLinear = 0f;
				this.lavaProgressSmooth = 0f;
				float num = (float)(currentTime - syncData.stateStartTime);
				float progress = Mathf.Clamp01(num / this.eruptTime);
				foreach (VolcanoEffects volcanoEffects2 in this.volcanoEffects)
				{
					if (volcanoEffects2 != null)
					{
						volcanoEffects2.UpdateEruptingState(num, this.eruptTime - num, progress);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Rising:
			{
				float value = (float)(currentTime - syncData.stateStartTime) / this.riseTime;
				this.lavaProgressLinear = Mathf.Clamp01(value);
				this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
				float num2 = (float)(currentTime - syncData.stateStartTime);
				foreach (VolcanoEffects volcanoEffects3 in this.volcanoEffects)
				{
					if (volcanoEffects3 != null)
					{
						volcanoEffects3.UpdateRisingState(num2, this.riseTime - num2, this.lavaProgressLinear);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Full:
			{
				this.lavaProgressLinear = 1f;
				this.lavaProgressSmooth = 1f;
				float num3 = (float)(currentTime - syncData.stateStartTime);
				float progress2 = Mathf.Clamp01(this.fullTime / num3);
				foreach (VolcanoEffects volcanoEffects4 in this.volcanoEffects)
				{
					if (volcanoEffects4 != null)
					{
						volcanoEffects4.UpdateFullState(num3, this.fullTime - num3, progress2);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Draining:
			{
				float num4 = (float)(currentTime - syncData.stateStartTime);
				float num5 = Mathf.Clamp01(num4 / this.drainTime);
				this.lavaProgressLinear = 1f - num5;
				this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
				foreach (VolcanoEffects volcanoEffects5 in this.volcanoEffects)
				{
					if (volcanoEffects5 != null)
					{
						volcanoEffects5.UpdateDrainingState(num4, this.riseTime - num4, num5);
					}
				}
				return;
			}
			}
		}

		// Token: 0x060015F1 RID: 5617 RVA: 0x0007949C File Offset: 0x0007769C
		private void UpdateLava(float fillProgress)
		{
			float z = Mathf.Lerp(this.lavaMeshMinScale, this.lavaMeshMaxScale, fillProgress);
			if (this.lavaMeshTransform != null)
			{
				this.lavaMeshTransform.localScale = new Vector3(this.lavaMeshTransform.localScale.x, this.lavaMeshTransform.localScale.y, z);
			}
		}

		// Token: 0x060015F2 RID: 5618 RVA: 0x000794FC File Offset: 0x000776FC
		private void UpdateVolcanoActivationLava(float activationProgress)
		{
			this.activationProgessSmooth = Mathf.MoveTowards(this.activationProgessSmooth, activationProgress, this.lavaActivationVisualMovementProgressPerSecond * Time.deltaTime);
			this.lavaActivationRenderer.material.SetColor("_BaseColor", this.lavaActivationGradient.Evaluate(activationProgress));
			this.lavaActivationRenderer.transform.position = Vector3.Lerp(this.lavaActivationStartPos.position, this.lavaActivationEndPos.position, this.activationProgessSmooth);
		}

		// Token: 0x060015F3 RID: 5619 RVA: 0x00079579 File Offset: 0x00077779
		private void CheckLocalPlayerAgainstLava(double currentTime)
		{
			if (GorillaLocomotion.Player.Instance.InWater && GorillaLocomotion.Player.Instance.CurrentWaterVolume == this.lavaVolume)
			{
				this.LocalPlayerInLava(currentTime, false);
			}
		}

		// Token: 0x060015F4 RID: 5620 RVA: 0x000795A6 File Offset: 0x000777A6
		private void OnColliderEnteredLava(Collider collider)
		{
			if (collider == GorillaLocomotion.Player.Instance.bodyCollider)
			{
				this.LocalPlayerInLava(PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time), true);
			}
		}

		// Token: 0x060015F5 RID: 5621 RVA: 0x000795D8 File Offset: 0x000777D8
		private void LocalPlayerInLava(double currentTime, bool enteredLavaThisFrame)
		{
			GorillaGameManager gorillaGameManager = GorillaGameManager.instance;
			if (gorillaGameManager != null && gorillaGameManager.LavaWouldAffectPlayer(PhotonNetwork.LocalPlayer, enteredLavaThisFrame) && (currentTime - this.lastTagSelfRPCTime > 0.5 || enteredLavaThisFrame))
			{
				this.lastTagSelfRPCTime = currentTime;
				PhotonView.Get(gorillaGameManager).RPC("ReportContactWithLavaRPC", RpcTarget.MasterClient, Array.Empty<object>());
			}
		}

		// Token: 0x060015F6 RID: 5622 RVA: 0x00079638 File Offset: 0x00077838
		public void OnActivationLavaProjectileHit(SlingshotProjectile projectile, Collision collision)
		{
			if (projectile.gameObject.CompareTag(this.lavaRockProjectileTag))
			{
				this.AddLavaRock(projectile.projectileOwner.ActorNumber);
			}
		}

		// Token: 0x060015F7 RID: 5623 RVA: 0x00079660 File Offset: 0x00077860
		private void AddLavaRock(int playerId)
		{
			if (base.photonView.IsMine && this.reliableState.state == InfectionLavaController.RisingLavaState.Drained)
			{
				float num = this.lavaActivationRockProgressVsPlayerCount.Evaluate((float)this.PlayerCount);
				this.reliableState.activationProgress = this.reliableState.activationProgress + (double)num;
				this.AddVoteForVolcanoActivation(playerId);
				VolcanoEffects[] array = this.volcanoEffects;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].OnStoneAccepted(this.reliableState.activationProgress);
				}
			}
		}

		// Token: 0x060015F8 RID: 5624 RVA: 0x000796DC File Offset: 0x000778DC
		private void AddVoteForVolcanoActivation(int playerId)
		{
			if (base.photonView.IsMine && this.lavaActivationVoteCount < 10)
			{
				bool flag = false;
				for (int i = 0; i < this.lavaActivationVoteCount; i++)
				{
					if (this.lavaActivationVotePlayerIds[i] == playerId)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					this.lavaActivationVotePlayerIds[this.lavaActivationVoteCount] = playerId;
					this.lavaActivationVoteCount++;
				}
			}
		}

		// Token: 0x060015F9 RID: 5625 RVA: 0x00079740 File Offset: 0x00077940
		private void RemoveVoteForVolcanoActivation(int playerId)
		{
			if (base.photonView.IsMine)
			{
				for (int i = 0; i < this.lavaActivationVoteCount; i++)
				{
					if (this.lavaActivationVotePlayerIds[i] == playerId)
					{
						this.lavaActivationVotePlayerIds[i] = this.lavaActivationVotePlayerIds[this.lavaActivationVoteCount - 1];
						this.lavaActivationVoteCount--;
						return;
					}
				}
			}
		}

		// Token: 0x060015FA RID: 5626 RVA: 0x0007979C File Offset: 0x0007799C
		private void StartEruption()
		{
			if (base.photonView.IsMine && this.reliableState.state == InfectionLavaController.RisingLavaState.Drained)
			{
				this.reliableState.state = InfectionLavaController.RisingLavaState.Erupting;
				this.reliableState.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			}
		}

		// Token: 0x060015FB RID: 5627 RVA: 0x000797F0 File Offset: 0x000779F0
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
				stream.SendNext(this.reliableState.activationProgress);
				stream.SendNext(this.lavaActivationVoteCount);
				stream.SendNext(this.lavaActivationVotePlayerIds[0]);
				stream.SendNext(this.lavaActivationVotePlayerIds[1]);
				stream.SendNext(this.lavaActivationVotePlayerIds[2]);
				stream.SendNext(this.lavaActivationVotePlayerIds[3]);
				stream.SendNext(this.lavaActivationVotePlayerIds[4]);
				stream.SendNext(this.lavaActivationVotePlayerIds[5]);
				stream.SendNext(this.lavaActivationVotePlayerIds[6]);
				stream.SendNext(this.lavaActivationVotePlayerIds[7]);
				stream.SendNext(this.lavaActivationVotePlayerIds[8]);
				stream.SendNext(this.lavaActivationVotePlayerIds[9]);
				return;
			}
			this.reliableState.state = (InfectionLavaController.RisingLavaState)((int)stream.ReceiveNext());
			this.reliableState.stateStartTime = (double)stream.ReceiveNext();
			this.reliableState.activationProgress = (double)stream.ReceiveNext();
			this.lavaActivationVoteCount = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[0] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[1] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[2] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[3] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[4] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[5] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[6] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[7] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[8] = (int)stream.ReceiveNext();
			this.lavaActivationVotePlayerIds[9] = (int)stream.ReceiveNext();
			float num = this.lavaProgressSmooth;
			this.UpdateLocalState((double)((float)PhotonNetwork.Time), this.reliableState);
			this.localLagLavaProgressOffset = num - this.lavaProgressSmooth;
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x00079A63 File Offset: 0x00077C63
		public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
		{
		}

		// Token: 0x060015FD RID: 5629 RVA: 0x00079A65 File Offset: 0x00077C65
		public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
		{
			this.RemoveVoteForVolcanoActivation(otherPlayer.ActorNumber);
		}

		// Token: 0x060015FE RID: 5630 RVA: 0x00079A73 File Offset: 0x00077C73
		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		// Token: 0x060015FF RID: 5631 RVA: 0x00079A75 File Offset: 0x00077C75
		public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
		{
		}

		// Token: 0x06001600 RID: 5632 RVA: 0x00079A77 File Offset: 0x00077C77
		public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
		{
		}

		// Token: 0x040017EE RID: 6126
		private static InfectionLavaController instance;

		// Token: 0x040017EF RID: 6127
		[SerializeField]
		private float lavaMeshMinScale = 1f;

		// Token: 0x040017F0 RID: 6128
		[SerializeField]
		private float lavaMeshMaxScale = 10f;

		// Token: 0x040017F1 RID: 6129
		[SerializeField]
		private float eruptTime = 3f;

		// Token: 0x040017F2 RID: 6130
		[SerializeField]
		private float riseTime = 10f;

		// Token: 0x040017F3 RID: 6131
		[SerializeField]
		private float fullTime = 240f;

		// Token: 0x040017F4 RID: 6132
		[SerializeField]
		private float drainTime = 10f;

		// Token: 0x040017F5 RID: 6133
		[SerializeField]
		private float lagResolutionLavaProgressPerSecond = 0.2f;

		// Token: 0x040017F6 RID: 6134
		[SerializeField]
		private AnimationCurve lavaProgressAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040017F7 RID: 6135
		[Header("Volcano Activation")]
		[SerializeField]
		[Range(0f, 1f)]
		private float activationVotePercentageDefaultQueue = 0.42f;

		// Token: 0x040017F8 RID: 6136
		[SerializeField]
		[Range(0f, 1f)]
		private float activationVotePercentageCompetitiveQueue = 0.6f;

		// Token: 0x040017F9 RID: 6137
		[SerializeField]
		private Gradient lavaActivationGradient;

		// Token: 0x040017FA RID: 6138
		[SerializeField]
		private AnimationCurve lavaActivationRockProgressVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040017FB RID: 6139
		[SerializeField]
		private AnimationCurve lavaActivationDrainRateVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040017FC RID: 6140
		[SerializeField]
		private float lavaActivationVisualMovementProgressPerSecond = 1f;

		// Token: 0x040017FD RID: 6141
		[SerializeField]
		private bool debugLavaActivationVotes;

		// Token: 0x040017FE RID: 6142
		private Transform lavaMeshTransform;

		// Token: 0x040017FF RID: 6143
		private Transform lavaSurfacePlaneTransform;

		// Token: 0x04001800 RID: 6144
		private WaterVolume lavaVolume;

		// Token: 0x04001801 RID: 6145
		private MeshRenderer lavaActivationRenderer;

		// Token: 0x04001802 RID: 6146
		private Transform lavaActivationStartPos;

		// Token: 0x04001803 RID: 6147
		private Transform lavaActivationEndPos;

		// Token: 0x04001804 RID: 6148
		private SlingshotProjectileHitNotifier lavaActivationProjectileHitNotifier;

		// Token: 0x04001805 RID: 6149
		private VolcanoEffects[] volcanoEffects;

		// Token: 0x04001806 RID: 6150
		private InfectionLavaController.LavaSyncData reliableState;

		// Token: 0x04001807 RID: 6151
		private int[] lavaActivationVotePlayerIds = new int[10];

		// Token: 0x04001808 RID: 6152
		private int lavaActivationVoteCount;

		// Token: 0x04001809 RID: 6153
		private float localLagLavaProgressOffset;

		// Token: 0x0400180A RID: 6154
		private float lavaProgressLinear;

		// Token: 0x0400180B RID: 6155
		private float lavaProgressSmooth;

		// Token: 0x0400180C RID: 6156
		private double lastTagSelfRPCTime;

		// Token: 0x0400180D RID: 6157
		private string lavaRockProjectileTag = "LavaRockProjectile";

		// Token: 0x0400180E RID: 6158
		private double currentTime;

		// Token: 0x0400180F RID: 6159
		private double prevTime;

		// Token: 0x04001810 RID: 6160
		private float activationProgessSmooth;

		// Token: 0x02000502 RID: 1282
		public enum RisingLavaState
		{
			// Token: 0x040020DD RID: 8413
			Drained,
			// Token: 0x040020DE RID: 8414
			Erupting,
			// Token: 0x040020DF RID: 8415
			Rising,
			// Token: 0x040020E0 RID: 8416
			Full,
			// Token: 0x040020E1 RID: 8417
			Draining
		}

		// Token: 0x02000503 RID: 1283
		private struct LavaSyncData
		{
			// Token: 0x040020E2 RID: 8418
			public InfectionLavaController.RisingLavaState state;

			// Token: 0x040020E3 RID: 8419
			public double stateStartTime;

			// Token: 0x040020E4 RID: 8420
			public double activationProgress;
		}
	}
}
