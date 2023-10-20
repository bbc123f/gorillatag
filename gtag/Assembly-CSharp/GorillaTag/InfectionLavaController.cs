using System;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200031C RID: 796
	public class InfectionLavaController : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
	{
		// Token: 0x1700017F RID: 383
		// (get) Token: 0x060015EB RID: 5611 RVA: 0x000790DA File Offset: 0x000772DA
		public static InfectionLavaController Instance
		{
			get
			{
				return InfectionLavaController.instance;
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x060015EC RID: 5612 RVA: 0x000790E1 File Offset: 0x000772E1
		public bool LavaCurrentlyActivated
		{
			get
			{
				return this.reliableState.state > InfectionLavaController.RisingLavaState.Drained;
			}
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x060015ED RID: 5613 RVA: 0x000790F1 File Offset: 0x000772F1
		public Plane LavaPlane
		{
			get
			{
				return new Plane(this.lavaSurfacePlaneTransform.up, this.lavaSurfacePlaneTransform.position);
			}
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x060015EE RID: 5614 RVA: 0x0007910E File Offset: 0x0007730E
		public Vector3 SurfaceCenter
		{
			get
			{
				return this.lavaSurfacePlaneTransform.position;
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x060015EF RID: 5615 RVA: 0x0007911C File Offset: 0x0007731C
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

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x060015F0 RID: 5616 RVA: 0x00079150 File Offset: 0x00077350
		private bool InCompetitiveQueue
		{
			get
			{
				object obj;
				return PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj) && obj.ToString().Contains("COMPETITIVE");
			}
		}

		// Token: 0x060015F1 RID: 5617 RVA: 0x00079188 File Offset: 0x00077388
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

		// Token: 0x060015F2 RID: 5618 RVA: 0x0007922F File Offset: 0x0007742F
		void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
		{
			if (InfectionLavaController.instance == this)
			{
				InfectionLavaController.instance = null;
			}
		}

		// Token: 0x060015F3 RID: 5619 RVA: 0x00079244 File Offset: 0x00077444
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

		// Token: 0x060015F4 RID: 5620 RVA: 0x00079310 File Offset: 0x00077510
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

		// Token: 0x060015F5 RID: 5621 RVA: 0x000793C4 File Offset: 0x000775C4
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

		// Token: 0x060015F6 RID: 5622 RVA: 0x00079414 File Offset: 0x00077614
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

		// Token: 0x060015F7 RID: 5623 RVA: 0x00079478 File Offset: 0x00077678
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

		// Token: 0x060015F8 RID: 5624 RVA: 0x00079530 File Offset: 0x00077730
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

		// Token: 0x060015F9 RID: 5625 RVA: 0x0007973C File Offset: 0x0007793C
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

		// Token: 0x060015FA RID: 5626 RVA: 0x00079984 File Offset: 0x00077B84
		private void UpdateLava(float fillProgress)
		{
			float z = Mathf.Lerp(this.lavaMeshMinScale, this.lavaMeshMaxScale, fillProgress);
			if (this.lavaMeshTransform != null)
			{
				this.lavaMeshTransform.localScale = new Vector3(this.lavaMeshTransform.localScale.x, this.lavaMeshTransform.localScale.y, z);
			}
		}

		// Token: 0x060015FB RID: 5627 RVA: 0x000799E4 File Offset: 0x00077BE4
		private void UpdateVolcanoActivationLava(float activationProgress)
		{
			this.activationProgessSmooth = Mathf.MoveTowards(this.activationProgessSmooth, activationProgress, this.lavaActivationVisualMovementProgressPerSecond * Time.deltaTime);
			this.lavaActivationRenderer.material.SetColor("_BaseColor", this.lavaActivationGradient.Evaluate(activationProgress));
			this.lavaActivationRenderer.transform.position = Vector3.Lerp(this.lavaActivationStartPos.position, this.lavaActivationEndPos.position, this.activationProgessSmooth);
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x00079A61 File Offset: 0x00077C61
		private void CheckLocalPlayerAgainstLava(double currentTime)
		{
			if (GorillaLocomotion.Player.Instance.InWater && GorillaLocomotion.Player.Instance.CurrentWaterVolume == this.lavaVolume)
			{
				this.LocalPlayerInLava(currentTime, false);
			}
		}

		// Token: 0x060015FD RID: 5629 RVA: 0x00079A8E File Offset: 0x00077C8E
		private void OnColliderEnteredLava(Collider collider)
		{
			if (collider == GorillaLocomotion.Player.Instance.bodyCollider)
			{
				this.LocalPlayerInLava(PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time), true);
			}
		}

		// Token: 0x060015FE RID: 5630 RVA: 0x00079AC0 File Offset: 0x00077CC0
		private void LocalPlayerInLava(double currentTime, bool enteredLavaThisFrame)
		{
			GorillaGameManager gorillaGameManager = GorillaGameManager.instance;
			if (gorillaGameManager != null && gorillaGameManager.LavaWouldAffectPlayer(PhotonNetwork.LocalPlayer, enteredLavaThisFrame) && (currentTime - this.lastTagSelfRPCTime > 0.5 || enteredLavaThisFrame))
			{
				this.lastTagSelfRPCTime = currentTime;
				PhotonView.Get(gorillaGameManager).RPC("ReportContactWithLavaRPC", RpcTarget.MasterClient, Array.Empty<object>());
			}
		}

		// Token: 0x060015FF RID: 5631 RVA: 0x00079B20 File Offset: 0x00077D20
		public void OnActivationLavaProjectileHit(SlingshotProjectile projectile, Collision collision)
		{
			if (projectile.gameObject.CompareTag(this.lavaRockProjectileTag))
			{
				this.AddLavaRock(projectile.projectileOwner.ActorNumber);
			}
		}

		// Token: 0x06001600 RID: 5632 RVA: 0x00079B48 File Offset: 0x00077D48
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

		// Token: 0x06001601 RID: 5633 RVA: 0x00079BC4 File Offset: 0x00077DC4
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

		// Token: 0x06001602 RID: 5634 RVA: 0x00079C28 File Offset: 0x00077E28
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

		// Token: 0x06001603 RID: 5635 RVA: 0x00079C84 File Offset: 0x00077E84
		private void StartEruption()
		{
			if (base.photonView.IsMine && this.reliableState.state == InfectionLavaController.RisingLavaState.Drained)
			{
				this.reliableState.state = InfectionLavaController.RisingLavaState.Erupting;
				this.reliableState.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			}
		}

		// Token: 0x06001604 RID: 5636 RVA: 0x00079CD8 File Offset: 0x00077ED8
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

		// Token: 0x06001605 RID: 5637 RVA: 0x00079F4B File Offset: 0x0007814B
		public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
		{
		}

		// Token: 0x06001606 RID: 5638 RVA: 0x00079F4D File Offset: 0x0007814D
		public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
		{
			this.RemoveVoteForVolcanoActivation(otherPlayer.ActorNumber);
		}

		// Token: 0x06001607 RID: 5639 RVA: 0x00079F5B File Offset: 0x0007815B
		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x00079F5D File Offset: 0x0007815D
		public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
		{
		}

		// Token: 0x06001609 RID: 5641 RVA: 0x00079F5F File Offset: 0x0007815F
		public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
		{
		}

		// Token: 0x040017FB RID: 6139
		private static InfectionLavaController instance;

		// Token: 0x040017FC RID: 6140
		[SerializeField]
		private float lavaMeshMinScale = 1f;

		// Token: 0x040017FD RID: 6141
		[SerializeField]
		private float lavaMeshMaxScale = 10f;

		// Token: 0x040017FE RID: 6142
		[SerializeField]
		private float eruptTime = 3f;

		// Token: 0x040017FF RID: 6143
		[SerializeField]
		private float riseTime = 10f;

		// Token: 0x04001800 RID: 6144
		[SerializeField]
		private float fullTime = 240f;

		// Token: 0x04001801 RID: 6145
		[SerializeField]
		private float drainTime = 10f;

		// Token: 0x04001802 RID: 6146
		[SerializeField]
		private float lagResolutionLavaProgressPerSecond = 0.2f;

		// Token: 0x04001803 RID: 6147
		[SerializeField]
		private AnimationCurve lavaProgressAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04001804 RID: 6148
		[Header("Volcano Activation")]
		[SerializeField]
		[Range(0f, 1f)]
		private float activationVotePercentageDefaultQueue = 0.42f;

		// Token: 0x04001805 RID: 6149
		[SerializeField]
		[Range(0f, 1f)]
		private float activationVotePercentageCompetitiveQueue = 0.6f;

		// Token: 0x04001806 RID: 6150
		[SerializeField]
		private Gradient lavaActivationGradient;

		// Token: 0x04001807 RID: 6151
		[SerializeField]
		private AnimationCurve lavaActivationRockProgressVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04001808 RID: 6152
		[SerializeField]
		private AnimationCurve lavaActivationDrainRateVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04001809 RID: 6153
		[SerializeField]
		private float lavaActivationVisualMovementProgressPerSecond = 1f;

		// Token: 0x0400180A RID: 6154
		[SerializeField]
		private bool debugLavaActivationVotes;

		// Token: 0x0400180B RID: 6155
		private Transform lavaMeshTransform;

		// Token: 0x0400180C RID: 6156
		private Transform lavaSurfacePlaneTransform;

		// Token: 0x0400180D RID: 6157
		private WaterVolume lavaVolume;

		// Token: 0x0400180E RID: 6158
		private MeshRenderer lavaActivationRenderer;

		// Token: 0x0400180F RID: 6159
		private Transform lavaActivationStartPos;

		// Token: 0x04001810 RID: 6160
		private Transform lavaActivationEndPos;

		// Token: 0x04001811 RID: 6161
		private SlingshotProjectileHitNotifier lavaActivationProjectileHitNotifier;

		// Token: 0x04001812 RID: 6162
		private VolcanoEffects[] volcanoEffects;

		// Token: 0x04001813 RID: 6163
		private InfectionLavaController.LavaSyncData reliableState;

		// Token: 0x04001814 RID: 6164
		private int[] lavaActivationVotePlayerIds = new int[10];

		// Token: 0x04001815 RID: 6165
		private int lavaActivationVoteCount;

		// Token: 0x04001816 RID: 6166
		private float localLagLavaProgressOffset;

		// Token: 0x04001817 RID: 6167
		private float lavaProgressLinear;

		// Token: 0x04001818 RID: 6168
		private float lavaProgressSmooth;

		// Token: 0x04001819 RID: 6169
		private double lastTagSelfRPCTime;

		// Token: 0x0400181A RID: 6170
		private string lavaRockProjectileTag = "LavaRockProjectile";

		// Token: 0x0400181B RID: 6171
		private double currentTime;

		// Token: 0x0400181C RID: 6172
		private double prevTime;

		// Token: 0x0400181D RID: 6173
		private float activationProgessSmooth;

		// Token: 0x02000504 RID: 1284
		public enum RisingLavaState
		{
			// Token: 0x040020EA RID: 8426
			Drained,
			// Token: 0x040020EB RID: 8427
			Erupting,
			// Token: 0x040020EC RID: 8428
			Rising,
			// Token: 0x040020ED RID: 8429
			Full,
			// Token: 0x040020EE RID: 8430
			Draining
		}

		// Token: 0x02000505 RID: 1285
		private struct LavaSyncData
		{
			// Token: 0x040020EF RID: 8431
			public InfectionLavaController.RisingLavaState state;

			// Token: 0x040020F0 RID: 8432
			public double stateStartTime;

			// Token: 0x040020F1 RID: 8433
			public double activationProgress;
		}
	}
}
