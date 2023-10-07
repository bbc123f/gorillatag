using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x020002AA RID: 682
	public class LurkerGhost : MonoBehaviourPunCallbacks, IPunObservable, IOnPhotonViewOwnerChange, IPhotonViewCallback
	{
		// Token: 0x060011BC RID: 4540 RVA: 0x00064AF9 File Offset: 0x00062CF9
		private void Awake()
		{
			this.possibleTargets = new List<Player>();
			this.targetPlayer = null;
			this.targetTransform = null;
			this.targetVRRig = null;
		}

		// Token: 0x060011BD RID: 4541 RVA: 0x00064B1B File Offset: 0x00062D1B
		private void Start()
		{
			this.waypointRegions = this.waypointsContainer.GetComponentsInChildren<ZoneBasedObject>();
			this.PickNextWaypoint();
			this.ChangeState(LurkerGhost.ghostState.patrol);
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x00064B3B File Offset: 0x00062D3B
		private void LateUpdate()
		{
			this.UpdateState();
			this.UpdateGhostVisibility();
		}

		// Token: 0x060011BF RID: 4543 RVA: 0x00064B4C File Offset: 0x00062D4C
		private void PickNextWaypoint()
		{
			if (this.waypoints.Count == 0 || this.lastWaypointRegion == null || !this.lastWaypointRegion.IsLocalPlayerInZone())
			{
				Debug.Log("Lurker selecting new region");
				ZoneBasedObject zoneBasedObject = ZoneBasedObject.SelectRandomEligible(this.waypointRegions, "");
				if (zoneBasedObject == null)
				{
					zoneBasedObject = this.lastWaypointRegion;
				}
				if (zoneBasedObject == null)
				{
					return;
				}
				this.lastWaypointRegion = zoneBasedObject;
				string str = "Lurker selected ";
				ZoneBasedObject zoneBasedObject2 = zoneBasedObject;
				Debug.Log(str + ((zoneBasedObject2 != null) ? zoneBasedObject2.ToString() : null));
				this.waypoints.Clear();
				foreach (object obj in zoneBasedObject.transform)
				{
					Transform item = (Transform)obj;
					this.waypoints.Add(item);
				}
			}
			int index = Random.Range(0, this.waypoints.Count);
			this.currentWaypoint = this.waypoints[index];
			this.targetRotation = Quaternion.LookRotation(this.currentWaypoint.position - base.transform.position);
			this.waypoints.RemoveAt(index);
			Debug.Log(string.Format("Lurker picks a new waypoint in {0} ({1}:{2})", this.currentWaypoint.parent, this.lastWaypointRegion.zones[0], ZoneManagement.IsInZone(this.lastWaypointRegion.zones[0])));
		}

		// Token: 0x060011C0 RID: 4544 RVA: 0x00064CD4 File Offset: 0x00062ED4
		private void Patrol()
		{
			Transform transform = this.currentWaypoint;
			base.transform.position = Vector3.MoveTowards(base.transform.position, transform.position, this.patrolSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 360f * Time.deltaTime);
		}

		// Token: 0x060011C1 RID: 4545 RVA: 0x00064D44 File Offset: 0x00062F44
		private void PlaySound(AudioClip clip, bool loop)
		{
			if (this.audioSource && this.audioSource.isPlaying)
			{
				this.audioSource.Stop();
			}
			if (this.audioSource && clip != null)
			{
				this.audioSource.clip = clip;
				this.audioSource.loop = loop;
				this.audioSource.Play();
			}
		}

		// Token: 0x060011C2 RID: 4546 RVA: 0x00064DB0 File Offset: 0x00062FB0
		private bool PickPlayer(float maxDistance)
		{
			if (base.photonView.IsMine)
			{
				this.possibleTargets.Clear();
				for (int i = 0; i < GorillaParent.instance.vrrigs.Count; i++)
				{
					if ((GorillaParent.instance.vrrigs[i].transform.position - base.transform.position).magnitude < maxDistance && GorillaParent.instance.vrrigs[i].creator != this.targetPlayer)
					{
						this.possibleTargets.Add(GorillaParent.instance.vrrigs[i].creator);
					}
				}
				this.targetPlayer = null;
				this.targetTransform = null;
				this.targetVRRig = null;
				if (this.possibleTargets.Count > 0)
				{
					int index = Random.Range(0, this.possibleTargets.Count);
					this.PickPlayer(this.possibleTargets[index]);
				}
			}
			else
			{
				this.targetPlayer = null;
				this.targetTransform = null;
				this.targetVRRig = null;
			}
			return this.targetPlayer != null && this.targetTransform != null;
		}

		// Token: 0x060011C3 RID: 4547 RVA: 0x00064EE4 File Offset: 0x000630E4
		private void PickPlayer(Player player)
		{
			int num = GorillaParent.instance.vrrigs.FindIndex((VRRig x) => x.creator != null && x.creator == player);
			if (num > -1 && num < GorillaParent.instance.vrrigs.Count)
			{
				this.targetPlayer = GorillaParent.instance.vrrigs[num].creator;
				this.targetTransform = GorillaParent.instance.vrrigs[num].head.rigTarget;
				this.targetVRRig = GorillaParent.instance.vrrigs[num];
			}
		}

		// Token: 0x060011C4 RID: 4548 RVA: 0x00064F8C File Offset: 0x0006318C
		private void SeekPlayer()
		{
			if (this.targetTransform == null)
			{
				this.ChangeState(LurkerGhost.ghostState.patrol);
				return;
			}
			this.targetPosition = this.targetTransform.position + this.targetTransform.forward.x0z() * this.seekAheadDistance;
			this.targetRotation = Quaternion.LookRotation(this.targetTransform.position - base.transform.position);
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, this.seekSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 720f * Time.deltaTime);
		}

		// Token: 0x060011C5 RID: 4549 RVA: 0x00065060 File Offset: 0x00063260
		private void ChargeAtPlayer()
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, this.chargeSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 720f * Time.deltaTime);
		}

		// Token: 0x060011C6 RID: 4550 RVA: 0x000650C8 File Offset: 0x000632C8
		private void UpdateGhostVisibility()
		{
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			case LurkerGhost.ghostState.seek:
			case LurkerGhost.ghostState.charge:
				if (this.targetPlayer == PhotonNetwork.LocalPlayer || this.passingPlayer == PhotonNetwork.LocalPlayer)
				{
					this.meshRenderer.sharedMaterial = this.visibleMaterial;
					this.bonesMeshRenderer.sharedMaterial = this.visibleMaterialBones;
					return;
				}
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			case LurkerGhost.ghostState.possess:
				if (this.targetPlayer == PhotonNetwork.LocalPlayer || this.passingPlayer == PhotonNetwork.LocalPlayer)
				{
					this.meshRenderer.sharedMaterial = this.visibleMaterial;
					this.bonesMeshRenderer.sharedMaterial = this.visibleMaterialBones;
					return;
				}
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			default:
				return;
			}
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x000651D8 File Offset: 0x000633D8
		private void HauntObjects()
		{
			Collider[] array = new Collider[20];
			int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.sphereColliderRadius, array);
			for (int i = 0; i < num; i++)
			{
				if (array[i].CompareTag("HauntedObject"))
				{
					UnityAction<GameObject> triggerHauntedObjects = this.TriggerHauntedObjects;
					if (triggerHauntedObjects != null)
					{
						triggerHauntedObjects(array[i].gameObject);
					}
				}
			}
		}

		// Token: 0x060011C8 RID: 4552 RVA: 0x0006523C File Offset: 0x0006343C
		private void ChangeState(LurkerGhost.ghostState newState)
		{
			this.currentState = newState;
			VRRig vrrig = null;
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.PlaySound(this.patrolAudio, true);
				this.passingPlayer = null;
				this.cooldownTimeRemaining = Random.Range(this.cooldownDuration, this.maxCooldownDuration);
				this.currentRepeatHuntTimes = 0;
				break;
			case LurkerGhost.ghostState.charge:
				this.PlaySound(this.huntAudio, false);
				this.targetPosition = this.targetTransform.position;
				this.targetRotation = Quaternion.LookRotation(this.targetTransform.position - base.transform.position);
				break;
			case LurkerGhost.ghostState.possess:
				if (this.targetPlayer == PhotonNetwork.LocalPlayer)
				{
					this.PlaySound(this.possessedAudio, true);
					GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
					GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
				}
				vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
				break;
			}
			Shader.SetGlobalFloat(this._BlackAndWhite, (float)((newState == LurkerGhost.ghostState.possess && this.targetPlayer == PhotonNetwork.LocalPlayer) ? 1 : 0));
			if (vrrig != this.lastHauntedVRRig && this.lastHauntedVRRig != null)
			{
				this.lastHauntedVRRig.IsHaunted = false;
			}
			if (vrrig != null)
			{
				vrrig.IsHaunted = true;
			}
			this.lastHauntedVRRig = vrrig;
			this.UpdateGhostVisibility();
		}

		// Token: 0x060011C9 RID: 4553 RVA: 0x000653B0 File Offset: 0x000635B0
		private void OnDestroy()
		{
			Shader.SetGlobalFloat(this._BlackAndWhite, 0f);
		}

		// Token: 0x060011CA RID: 4554 RVA: 0x000653C8 File Offset: 0x000635C8
		private void UpdateState()
		{
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.Patrol();
				if (base.photonView.IsMine)
				{
					if (Vector3.Distance(base.transform.position, this.currentWaypoint.position) < 0.2f)
					{
						this.PickNextWaypoint();
					}
					this.cooldownTimeRemaining -= Time.deltaTime;
					if (this.cooldownTimeRemaining <= 0f)
					{
						this.cooldownTimeRemaining = 0f;
						if (this.PickPlayer(this.maxHuntDistance))
						{
							this.ChangeState(LurkerGhost.ghostState.seek);
							return;
						}
					}
				}
				break;
			case LurkerGhost.ghostState.seek:
				this.SeekPlayer();
				if (base.photonView.IsMine && (this.targetPosition - base.transform.position).sqrMagnitude < this.seekCloseEnoughDistance * this.seekCloseEnoughDistance)
				{
					this.ChangeState(LurkerGhost.ghostState.charge);
					return;
				}
				break;
			case LurkerGhost.ghostState.charge:
				this.ChargeAtPlayer();
				if (base.photonView.IsMine && (this.targetPosition - base.transform.position).sqrMagnitude < 0.25f)
				{
					if ((this.targetTransform.position - this.targetPosition).magnitude < this.minCatchDistance)
					{
						this.ChangeState(LurkerGhost.ghostState.possess);
						return;
					}
					this.huntedPassedTime = 0f;
					this.ChangeState(LurkerGhost.ghostState.patrol);
					return;
				}
				break;
			case LurkerGhost.ghostState.possess:
				if (this.targetTransform != null)
				{
					float num = this.SpookyMagicNumbers.x + MathF.Abs(MathF.Sin(Time.time * this.SpookyMagicNumbers.y));
					float num2 = this.HauntedMagicNumbers.x * MathF.Sin(Time.time * this.HauntedMagicNumbers.y) + this.HauntedMagicNumbers.z * MathF.Sin(Time.time * this.HauntedMagicNumbers.w);
					float y = 0.5f + 0.5f * MathF.Sin(Time.time * this.SpookyMagicNumbers.z);
					Vector3 target = this.targetTransform.position + new Vector3(num * (float)Math.Sin((double)num2), y, num * (float)Math.Cos((double)num2));
					base.transform.position = Vector3.MoveTowards(base.transform.position, target, this.chargeSpeed);
					base.transform.rotation = Quaternion.LookRotation(base.transform.position - this.targetTransform.position);
				}
				if (base.photonView.IsMine)
				{
					this.huntedPassedTime += Time.deltaTime;
					if (this.huntedPassedTime >= this.PossessionDuration)
					{
						this.huntedPassedTime = 0f;
						if (this.hauntNeighbors && this.currentRepeatHuntTimes < this.maxRepeatHuntTimes && this.PickPlayer(this.maxRepeatHuntDistance))
						{
							this.currentRepeatHuntTimes++;
							this.ChangeState(LurkerGhost.ghostState.seek);
							return;
						}
						this.ChangeState(LurkerGhost.ghostState.patrol);
					}
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x060011CB RID: 4555 RVA: 0x000656DC File Offset: 0x000638DC
		void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			if (stream.IsWriting)
			{
				stream.SendNext(this.currentState);
				stream.SendNext(this.currentIndex);
				stream.SendNext(this.targetPlayer);
				stream.SendNext(this.targetPosition);
				return;
			}
			LurkerGhost.ghostState ghostState = this.currentState;
			this.currentState = (LurkerGhost.ghostState)stream.ReceiveNext();
			this.currentIndex = (int)stream.ReceiveNext();
			Player player = this.targetPlayer;
			this.targetPlayer = (Player)stream.ReceiveNext();
			this.targetPosition = (Vector3)stream.ReceiveNext();
			if (this.targetPlayer != player)
			{
				this.PickPlayer(this.targetPlayer);
			}
			if (ghostState != this.currentState || this.targetPlayer != player)
			{
				this.ChangeState(this.currentState);
			}
		}

		// Token: 0x060011CC RID: 4556 RVA: 0x000657C2 File Offset: 0x000639C2
		void IOnPhotonViewOwnerChange.OnOwnerChange(Player newOwner, Player previousOwner)
		{
			if (newOwner == PhotonNetwork.LocalPlayer)
			{
				this.ChangeState(this.currentState);
			}
		}

		// Token: 0x0400147C RID: 5244
		public float patrolSpeed = 3f;

		// Token: 0x0400147D RID: 5245
		public float seekSpeed = 6f;

		// Token: 0x0400147E RID: 5246
		public float chargeSpeed = 6f;

		// Token: 0x0400147F RID: 5247
		[Tooltip("Cooldown until the next time the ghost needs to hunt a new player")]
		public float cooldownDuration = 10f;

		// Token: 0x04001480 RID: 5248
		[Tooltip("Max Cooldown (randomized)")]
		public float maxCooldownDuration = 10f;

		// Token: 0x04001481 RID: 5249
		[Tooltip("How long the possession effects should last")]
		public float PossessionDuration = 15f;

		// Token: 0x04001482 RID: 5250
		[Tooltip("Hunted objects within this radius will get triggered ")]
		public float sphereColliderRadius = 2f;

		// Token: 0x04001483 RID: 5251
		[Tooltip("Maximum distance to the possible player to get hunted")]
		public float maxHuntDistance = 20f;

		// Token: 0x04001484 RID: 5252
		[Tooltip("Minimum distance from the player to start the possession effects")]
		public float minCatchDistance = 2f;

		// Token: 0x04001485 RID: 5253
		[Tooltip("Maximum distance to the possible player to get repeat hunted")]
		public float maxRepeatHuntDistance = 5f;

		// Token: 0x04001486 RID: 5254
		[Tooltip("Maximum times the lurker can haunt a nearby player before going back on cooldown")]
		public int maxRepeatHuntTimes = 3;

		// Token: 0x04001487 RID: 5255
		[Tooltip("Time in seconds before a haunted player can pass the lurker to another player by tagging")]
		public float tagCoolDown = 2f;

		// Token: 0x04001488 RID: 5256
		[Tooltip("UP & DOWN, IN & OUT")]
		public Vector3 SpookyMagicNumbers = new Vector3(1f, 1f, 1f);

		// Token: 0x04001489 RID: 5257
		[Tooltip("SPIN, SPIN, SPIN, SPIN")]
		public Vector4 HauntedMagicNumbers = new Vector4(1f, 2f, 3f, 1f);

		// Token: 0x0400148A RID: 5258
		[Tooltip("Haptic vibration when haunted by the ghost")]
		public float hapticStrength = 1f;

		// Token: 0x0400148B RID: 5259
		public float hapticDuration = 1.5f;

		// Token: 0x0400148C RID: 5260
		public GameObject waypointsContainer;

		// Token: 0x0400148D RID: 5261
		private ZoneBasedObject[] waypointRegions;

		// Token: 0x0400148E RID: 5262
		private ZoneBasedObject lastWaypointRegion;

		// Token: 0x0400148F RID: 5263
		private List<Transform> waypoints = new List<Transform>();

		// Token: 0x04001490 RID: 5264
		private Transform currentWaypoint;

		// Token: 0x04001491 RID: 5265
		public Material visibleMaterial;

		// Token: 0x04001492 RID: 5266
		public Material scryableMaterial;

		// Token: 0x04001493 RID: 5267
		public Material visibleMaterialBones;

		// Token: 0x04001494 RID: 5268
		public Material scryableMaterialBones;

		// Token: 0x04001495 RID: 5269
		public MeshRenderer meshRenderer;

		// Token: 0x04001496 RID: 5270
		public MeshRenderer bonesMeshRenderer;

		// Token: 0x04001497 RID: 5271
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04001498 RID: 5272
		public AudioClip patrolAudio;

		// Token: 0x04001499 RID: 5273
		public AudioClip huntAudio;

		// Token: 0x0400149A RID: 5274
		public AudioClip possessedAudio;

		// Token: 0x0400149B RID: 5275
		public ThrowableSetDressing scryingGlass;

		// Token: 0x0400149C RID: 5276
		public float scryingAngerAngle;

		// Token: 0x0400149D RID: 5277
		public float scryingAngerDelay;

		// Token: 0x0400149E RID: 5278
		public float seekAheadDistance;

		// Token: 0x0400149F RID: 5279
		public float seekCloseEnoughDistance;

		// Token: 0x040014A0 RID: 5280
		private float scryingAngerAfterTimestamp;

		// Token: 0x040014A1 RID: 5281
		private int currentRepeatHuntTimes;

		// Token: 0x040014A2 RID: 5282
		public UnityAction<GameObject> TriggerHauntedObjects;

		// Token: 0x040014A3 RID: 5283
		private readonly string handLayermask = "Gorilla Hand";

		// Token: 0x040014A4 RID: 5284
		private readonly string bodyLayerMask = "Gorilla Body Collider";

		// Token: 0x040014A5 RID: 5285
		private int currentIndex;

		// Token: 0x040014A6 RID: 5286
		private LurkerGhost.ghostState currentState;

		// Token: 0x040014A7 RID: 5287
		private float cooldownTimeRemaining;

		// Token: 0x040014A8 RID: 5288
		private List<Player> possibleTargets;

		// Token: 0x040014A9 RID: 5289
		private Player targetPlayer;

		// Token: 0x040014AA RID: 5290
		private Transform targetTransform;

		// Token: 0x040014AB RID: 5291
		private float huntedPassedTime;

		// Token: 0x040014AC RID: 5292
		private Vector3 targetPosition;

		// Token: 0x040014AD RID: 5293
		private Quaternion targetRotation;

		// Token: 0x040014AE RID: 5294
		private VRRig targetVRRig;

		// Token: 0x040014AF RID: 5295
		private ShaderHashId _BlackAndWhite = "_BlackAndWhite";

		// Token: 0x040014B0 RID: 5296
		private VRRig lastHauntedVRRig;

		// Token: 0x040014B1 RID: 5297
		private float nextTagTime;

		// Token: 0x040014B2 RID: 5298
		private Player passingPlayer;

		// Token: 0x040014B3 RID: 5299
		[SerializeField]
		private bool hauntNeighbors = true;

		// Token: 0x040014B4 RID: 5300
		[SerializeField]
		private bool hauntOnTag;

		// Token: 0x020004B2 RID: 1202
		private enum ghostState
		{
			// Token: 0x04001F76 RID: 8054
			patrol,
			// Token: 0x04001F77 RID: 8055
			seek,
			// Token: 0x04001F78 RID: 8056
			charge,
			// Token: 0x04001F79 RID: 8057
			possess
		}
	}
}
