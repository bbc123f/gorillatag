using System;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GorillaTag
{
	public class InfectionLavaController : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
	{
		public static InfectionLavaController Instance
		{
			get
			{
				return InfectionLavaController.instance;
			}
		}

		public bool LavaCurrentlyActivated
		{
			get
			{
				return this.reliableState.state > InfectionLavaController.RisingLavaState.Drained;
			}
		}

		public Plane LavaPlane
		{
			get
			{
				return new Plane(this.lavaSurfacePlaneTransform.up, this.lavaSurfacePlaneTransform.position);
			}
		}

		public Vector3 SurfaceCenter
		{
			get
			{
				return this.lavaSurfacePlaneTransform.position;
			}
		}

		private int PlayerCount
		{
			get
			{
				int num = 1;
				GorillaGameManager gorillaGameManager = GorillaGameManager.instance;
				if (gorillaGameManager != null && gorillaGameManager.currentPlayerArray != null)
				{
					num = gorillaGameManager.currentPlayerArray.Length;
				}
				return num;
			}
		}

		private bool InCompetitiveQueue
		{
			get
			{
				object obj;
				return PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj) && obj.ToString().Contains("COMPETITIVE");
			}
		}

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

		void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
		{
			if (InfectionLavaController.instance == this)
			{
				InfectionLavaController.instance = null;
			}
		}

		private void Awake()
		{
			if (this.lavaVolume != null)
			{
				this.lavaVolume.ColliderEnteredWater += this.OnColliderEnteredLava;
			}
			if (this.lavaActivationProjectileHitNotifier != null)
			{
				this.lavaActivationProjectileHitNotifier.OnProjectileHit += this.OnActivationLavaProjectileHit;
			}
		}

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
					float num = (this.InCompetitiveQueue ? this.activationVotePercentageCompetitiveQueue : this.activationVotePercentageDefaultQueue);
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

		private void UpdateLocalState(double currentTime, InfectionLavaController.LavaSyncData syncData)
		{
			switch (syncData.state)
			{
			default:
			{
				this.lavaProgressLinear = 0f;
				this.lavaProgressSmooth = 0f;
				float num = (float)(currentTime - syncData.stateStartTime);
				foreach (VolcanoEffects volcanoEffects in this.volcanoEffects)
				{
					if (volcanoEffects != null)
					{
						volcanoEffects.UpdateDrainedState(num);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Erupting:
			{
				this.lavaProgressLinear = 0f;
				this.lavaProgressSmooth = 0f;
				float num2 = (float)(currentTime - syncData.stateStartTime);
				float num3 = Mathf.Clamp01(num2 / this.eruptTime);
				foreach (VolcanoEffects volcanoEffects2 in this.volcanoEffects)
				{
					if (volcanoEffects2 != null)
					{
						volcanoEffects2.UpdateEruptingState(num2, this.eruptTime - num2, num3);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Rising:
			{
				float num4 = (float)(currentTime - syncData.stateStartTime) / this.riseTime;
				this.lavaProgressLinear = Mathf.Clamp01(num4);
				this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
				float num5 = (float)(currentTime - syncData.stateStartTime);
				foreach (VolcanoEffects volcanoEffects3 in this.volcanoEffects)
				{
					if (volcanoEffects3 != null)
					{
						volcanoEffects3.UpdateRisingState(num5, this.riseTime - num5, this.lavaProgressLinear);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Full:
			{
				this.lavaProgressLinear = 1f;
				this.lavaProgressSmooth = 1f;
				float num6 = (float)(currentTime - syncData.stateStartTime);
				float num7 = Mathf.Clamp01(this.fullTime / num6);
				foreach (VolcanoEffects volcanoEffects4 in this.volcanoEffects)
				{
					if (volcanoEffects4 != null)
					{
						volcanoEffects4.UpdateFullState(num6, this.fullTime - num6, num7);
					}
				}
				return;
			}
			case InfectionLavaController.RisingLavaState.Draining:
			{
				float num8 = (float)(currentTime - syncData.stateStartTime);
				float num9 = Mathf.Clamp01(num8 / this.drainTime);
				this.lavaProgressLinear = 1f - num9;
				this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
				foreach (VolcanoEffects volcanoEffects5 in this.volcanoEffects)
				{
					if (volcanoEffects5 != null)
					{
						volcanoEffects5.UpdateDrainingState(num8, this.riseTime - num8, num9);
					}
				}
				return;
			}
			}
		}

		private void UpdateLava(float fillProgress)
		{
			float num = Mathf.Lerp(this.lavaMeshMinScale, this.lavaMeshMaxScale, fillProgress);
			if (this.lavaMeshTransform != null)
			{
				this.lavaMeshTransform.localScale = new Vector3(this.lavaMeshTransform.localScale.x, this.lavaMeshTransform.localScale.y, num);
			}
		}

		private void UpdateVolcanoActivationLava(float activationProgress)
		{
			this.activationProgessSmooth = Mathf.MoveTowards(this.activationProgessSmooth, activationProgress, this.lavaActivationVisualMovementProgressPerSecond * Time.deltaTime);
			this.lavaActivationRenderer.material.SetColor("_BaseColor", this.lavaActivationGradient.Evaluate(activationProgress));
			this.lavaActivationRenderer.transform.position = Vector3.Lerp(this.lavaActivationStartPos.position, this.lavaActivationEndPos.position, this.activationProgessSmooth);
		}

		private void CheckLocalPlayerAgainstLava(double currentTime)
		{
			if (GorillaLocomotion.Player.Instance.InWater && GorillaLocomotion.Player.Instance.CurrentWaterVolume == this.lavaVolume)
			{
				this.LocalPlayerInLava(currentTime, false);
			}
		}

		private void OnColliderEnteredLava(WaterVolume volume, Collider collider)
		{
			if (collider == GorillaLocomotion.Player.Instance.bodyCollider)
			{
				this.LocalPlayerInLava(PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time), true);
			}
		}

		private void LocalPlayerInLava(double currentTime, bool enteredLavaThisFrame)
		{
			GorillaGameManager gorillaGameManager = GorillaGameManager.instance;
			if (gorillaGameManager != null && gorillaGameManager.CanAffectPlayer(PhotonNetwork.LocalPlayer, enteredLavaThisFrame) && (currentTime - this.lastTagSelfRPCTime > 0.5 || enteredLavaThisFrame))
			{
				this.lastTagSelfRPCTime = currentTime;
				GameMode.ReportHit();
			}
		}

		public void OnActivationLavaProjectileHit(SlingshotProjectile projectile, Collision collision)
		{
			if (projectile.gameObject.CompareTag(this.lavaRockProjectileTag))
			{
				this.AddLavaRock(projectile.projectileOwner.ActorNumber);
			}
		}

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

		private void StartEruption()
		{
			if (base.photonView.IsMine && this.reliableState.state == InfectionLavaController.RisingLavaState.Drained)
			{
				this.reliableState.state = InfectionLavaController.RisingLavaState.Erupting;
				this.reliableState.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			}
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

		public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
		{
		}

		public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
		{
			this.RemoveVoteForVolcanoActivation(otherPlayer.ActorNumber);
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

		[OnEnterPlay_SetNull]
		private static InfectionLavaController instance;

		[SerializeField]
		private float lavaMeshMinScale = 1f;

		[SerializeField]
		private float lavaMeshMaxScale = 10f;

		[SerializeField]
		private float eruptTime = 3f;

		[SerializeField]
		private float riseTime = 10f;

		[SerializeField]
		private float fullTime = 240f;

		[SerializeField]
		private float drainTime = 10f;

		[SerializeField]
		private float lagResolutionLavaProgressPerSecond = 0.2f;

		[SerializeField]
		private AnimationCurve lavaProgressAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[Header("Volcano Activation")]
		[SerializeField]
		[Range(0f, 1f)]
		private float activationVotePercentageDefaultQueue = 0.42f;

		[SerializeField]
		[Range(0f, 1f)]
		private float activationVotePercentageCompetitiveQueue = 0.6f;

		[SerializeField]
		private Gradient lavaActivationGradient;

		[SerializeField]
		private AnimationCurve lavaActivationRockProgressVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[SerializeField]
		private AnimationCurve lavaActivationDrainRateVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[SerializeField]
		private float lavaActivationVisualMovementProgressPerSecond = 1f;

		[SerializeField]
		private bool debugLavaActivationVotes;

		[Header("Scene References")]
		[SerializeField]
		private Transform lavaMeshTransform;

		[SerializeField]
		private Transform lavaSurfacePlaneTransform;

		[SerializeField]
		private WaterVolume lavaVolume;

		[SerializeField]
		private MeshRenderer lavaActivationRenderer;

		[SerializeField]
		private Transform lavaActivationStartPos;

		[SerializeField]
		private Transform lavaActivationEndPos;

		[SerializeField]
		private SlingshotProjectileHitNotifier lavaActivationProjectileHitNotifier;

		[SerializeField]
		private VolcanoEffects[] volcanoEffects;

		private InfectionLavaController.LavaSyncData reliableState;

		private int[] lavaActivationVotePlayerIds = new int[10];

		private int lavaActivationVoteCount;

		private float localLagLavaProgressOffset;

		private float lavaProgressLinear;

		private float lavaProgressSmooth;

		private double lastTagSelfRPCTime;

		private string lavaRockProjectileTag = "LavaRockProjectile";

		private double currentTime;

		private double prevTime;

		private float activationProgessSmooth;

		public enum RisingLavaState
		{
			Drained,
			Erupting,
			Rising,
			Full,
			Draining
		}

		private struct LavaSyncData
		{
			public InfectionLavaController.RisingLavaState state;

			public double stateStartTime;

			public double activationProgress;
		}
	}
}
