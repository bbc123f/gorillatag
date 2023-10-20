using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000190 RID: 400
public class HalloweenGhostChaser : MonoBehaviourPunCallbacks, IInRoomCallbacks, IPunObservable, IOnPhotonViewOwnerChange, IPhotonViewCallback
{
	// Token: 0x06000A34 RID: 2612 RVA: 0x0003EFED File Offset: 0x0003D1ED
	private void Awake()
	{
		this.spawnIndex = 0;
		this.targetPlayer = null;
		this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
		this.grabTime = -this.minGrabCooldown;
		this.possibleTarget = new List<Player>();
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x0003F01C File Offset: 0x0003D21C
	private void InitializeGhost()
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			this.lastHeadAngleTime = 0f;
			this.nextHeadAngleTime = this.lastHeadAngleTime + Random.value * this.maxTimeToNextHeadAngle;
			this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
			this.ghostBody.transform.localPosition = Vector3.zero;
			base.transform.eulerAngles = Vector3.zero;
			this.lastSpeedIncreased = 0f;
			this.currentSpeed = 0f;
		}
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x0003F0BC File Offset: 0x0003D2BC
	private void LateUpdate()
	{
		if (!PhotonNetwork.InRoom)
		{
			this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
			this.UpdateState();
			return;
		}
		if (base.photonView.IsMine)
		{
			HalloweenGhostChaser.ChaseState chaseState = this.currentState;
			switch (chaseState)
			{
			case HalloweenGhostChaser.ChaseState.Dormant:
				if (Time.time >= this.nextTimeToChasePlayer)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.InitialRise;
				}
				if (Time.time >= this.lastSummonCheck + this.summoningDuration)
				{
					this.lastSummonCheck = Time.time;
					this.possibleTarget.Clear();
					int num = 0;
					int i = 0;
					while (i < this.spawnTransforms.Length)
					{
						int num2 = 0;
						for (int j = 0; j < GorillaParent.instance.vrrigs.Count; j++)
						{
							if ((GorillaParent.instance.vrrigs[j].transform.position - this.spawnTransforms[i].position).magnitude < this.summonDistance)
							{
								this.possibleTarget.Add(GorillaParent.instance.vrrigs[j].creator);
								num2++;
								if (num2 >= this.summonCount)
								{
									break;
								}
							}
						}
						if (num2 >= this.summonCount)
						{
							if (!this.wasSurroundedLastCheck)
							{
								this.wasSurroundedLastCheck = true;
								break;
							}
							this.wasSurroundedLastCheck = false;
							this.isSummoned = true;
							this.currentState = HalloweenGhostChaser.ChaseState.Gong;
							break;
						}
						else
						{
							num++;
							i++;
						}
					}
					if (num == this.spawnTransforms.Length)
					{
						this.wasSurroundedLastCheck = false;
					}
				}
				break;
			case HalloweenGhostChaser.ChaseState.InitialRise:
				if (Time.time > this.timeRiseStarted + this.totalTimeToRise)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.Chasing;
				}
				break;
			case (HalloweenGhostChaser.ChaseState)3:
				break;
			case HalloweenGhostChaser.ChaseState.Gong:
				if (Time.time > this.timeGongStarted + this.gongDuration)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.InitialRise;
				}
				break;
			default:
				if (chaseState != HalloweenGhostChaser.ChaseState.Chasing)
				{
					if (chaseState == HalloweenGhostChaser.ChaseState.Grabbing)
					{
						if (Time.time > this.grabTime + this.grabDuration)
						{
							this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
						}
					}
				}
				else
				{
					if (this.followTarget == null || this.targetPlayer == null)
					{
						this.ChooseRandomTarget();
					}
					if (!(this.followTarget == null) && (this.followTarget.position - this.ghostBody.transform.position).magnitude < this.catchDistance)
					{
						this.currentState = HalloweenGhostChaser.ChaseState.Grabbing;
					}
				}
				break;
			}
		}
		if (this.lastState != this.currentState)
		{
			this.OnChangeState(this.currentState);
			this.lastState = this.currentState;
		}
		this.UpdateState();
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x0003F354 File Offset: 0x0003D554
	public void UpdateState()
	{
		HalloweenGhostChaser.ChaseState chaseState = this.currentState;
		switch (chaseState)
		{
		case HalloweenGhostChaser.ChaseState.Dormant:
			this.isSummoned = false;
			if (this.ghostMaterial.color == this.summonedColor)
			{
				this.ghostMaterial.color = this.defaultColor;
				return;
			}
			break;
		case HalloweenGhostChaser.ChaseState.InitialRise:
			if (PhotonNetwork.InRoom)
			{
				if (base.photonView.IsMine)
				{
					this.RiseHost();
				}
				this.MoveHead();
				return;
			}
			break;
		case (HalloweenGhostChaser.ChaseState)3:
		case HalloweenGhostChaser.ChaseState.Gong:
			break;
		default:
			if (chaseState != HalloweenGhostChaser.ChaseState.Chasing)
			{
				if (chaseState != HalloweenGhostChaser.ChaseState.Grabbing)
				{
					return;
				}
				if (PhotonNetwork.InRoom)
				{
					if (this.targetPlayer == PhotonNetwork.LocalPlayer)
					{
						this.RiseGrabbedLocalPlayer();
					}
					this.GrabBodyShared();
					this.MoveHead();
				}
			}
			else if (PhotonNetwork.InRoom)
			{
				if (base.photonView.IsMine)
				{
					this.ChaseHost();
				}
				this.MoveBodyShared();
				this.MoveHead();
				return;
			}
			break;
		}
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x0003F42C File Offset: 0x0003D62C
	private void OnChangeState(HalloweenGhostChaser.ChaseState newState)
	{
		switch (newState)
		{
		case HalloweenGhostChaser.ChaseState.Dormant:
			if (this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(false);
			}
			if (base.photonView.IsMine)
			{
				this.targetPlayer = null;
				this.InitializeGhost();
			}
			else
			{
				this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
			}
			this.SetInitialRotations();
			return;
		case HalloweenGhostChaser.ChaseState.InitialRise:
			this.timeRiseStarted = Time.time;
			if (!this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(true);
			}
			if (base.photonView.IsMine)
			{
				if (!this.isSummoned)
				{
					this.currentSpeed = 0f;
					this.ChooseRandomTarget();
					this.SetInitialSpawnPoint();
				}
				else
				{
					this.currentSpeed = 3f;
				}
			}
			if (this.isSummoned)
			{
				this.laugh.volume = 0.25f;
				this.laugh.PlayOneShot(this.deepLaugh);
				this.ghostMaterial.color = this.summonedColor;
			}
			else
			{
				this.laugh.volume = 0.25f;
				this.laugh.Play();
				this.ghostMaterial.color = this.defaultColor;
			}
			this.SetInitialRotations();
			return;
		case (HalloweenGhostChaser.ChaseState)3:
			break;
		case HalloweenGhostChaser.ChaseState.Gong:
			if (!this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(true);
			}
			if (base.photonView.IsMine)
			{
				this.ChooseRandomTarget();
				this.SetInitialSpawnPoint();
				base.transform.position = this.spawnTransforms[this.spawnIndex].position;
			}
			this.timeGongStarted = Time.time;
			this.laugh.volume = 1f;
			this.laugh.PlayOneShot(this.gong);
			this.isSummoned = true;
			return;
		default:
			if (newState != HalloweenGhostChaser.ChaseState.Chasing)
			{
				if (newState != HalloweenGhostChaser.ChaseState.Grabbing)
				{
					return;
				}
				if (!this.ghostBody.activeSelf)
				{
					this.ghostBody.SetActive(true);
				}
				this.grabTime = Time.time;
				if (this.isSummoned)
				{
					this.laugh.volume = 0.25f;
					this.laugh.PlayOneShot(this.deepLaugh);
				}
				else
				{
					this.laugh.volume = 0.25f;
					this.laugh.Play();
				}
				this.leftArm.localEulerAngles = this.leftArmGrabbingLocal;
				this.rightArm.localEulerAngles = this.rightArmGrabbingLocal;
				this.leftHand.localEulerAngles = this.leftHandGrabbingLocal;
				this.rightHand.localEulerAngles = this.rightHandGrabbingLocal;
				this.ghostBody.transform.localPosition = this.ghostOffsetGrabbingLocal;
				this.ghostBody.transform.localEulerAngles = this.ghostGrabbingEulerRotation;
				VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
				if (vrrig != null)
				{
					this.followTarget = vrrig.transform;
					return;
				}
			}
			else
			{
				if (!this.ghostBody.activeSelf)
				{
					this.ghostBody.SetActive(true);
				}
				this.ResetPath();
			}
			break;
		}
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x0003F724 File Offset: 0x0003D924
	private void SetInitialSpawnPoint()
	{
		float num = 1000f;
		this.spawnIndex = 0;
		if (this.followTarget == null)
		{
			return;
		}
		for (int i = 0; i < this.spawnTransforms.Length; i++)
		{
			float magnitude = (this.followTarget.position - this.spawnTransformOffsets[i].position).magnitude;
			if (magnitude < num)
			{
				num = magnitude;
				this.spawnIndex = i;
			}
		}
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x0003F794 File Offset: 0x0003D994
	private void ChooseRandomTarget()
	{
		int num = -1;
		if (this.possibleTarget.Count >= this.summonCount)
		{
			int randomTarget = Random.Range(0, this.possibleTarget.Count);
			num = GorillaParent.instance.vrrigs.FindIndex((VRRig x) => x.creator != null && x.creator == this.possibleTarget[randomTarget]);
			this.currentSpeed = 3f;
		}
		if (num == -1)
		{
			num = Random.Range(0, GorillaParent.instance.vrrigs.Count);
		}
		this.possibleTarget.Clear();
		if (num < GorillaParent.instance.vrrigs.Count)
		{
			this.targetPlayer = GorillaParent.instance.vrrigs[num].creator;
			this.followTarget = GorillaParent.instance.vrrigs[num].head.rigTarget;
			NavMeshHit navMeshHit;
			this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, 5f, 1);
			return;
		}
		this.targetPlayer = null;
		this.followTarget = null;
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x0003F8AC File Offset: 0x0003DAAC
	private void SetInitialRotations()
	{
		this.leftArm.localEulerAngles = Vector3.zero;
		this.rightArm.localEulerAngles = Vector3.zero;
		this.leftHand.localEulerAngles = this.leftHandStartingLocal;
		this.rightHand.localEulerAngles = this.rightHandStartingLocal;
		this.ghostBody.transform.localPosition = Vector3.zero;
		this.ghostBody.transform.localEulerAngles = this.ghostStartingEulerRotation;
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x0003F928 File Offset: 0x0003DB28
	private void MoveHead()
	{
		if (Time.time > this.nextHeadAngleTime)
		{
			this.skullTransform.localEulerAngles = this.headEulerAngles[Random.Range(0, this.headEulerAngles.Length)];
			this.lastHeadAngleTime = Time.time;
			this.nextHeadAngleTime = this.lastHeadAngleTime + Mathf.Max(Random.value * this.maxTimeToNextHeadAngle, 0.05f);
		}
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x0003F994 File Offset: 0x0003DB94
	private void RiseHost()
	{
		if (Time.time < this.timeRiseStarted + this.totalTimeToRise)
		{
			if (this.spawnIndex == -1)
			{
				this.spawnIndex = 0;
			}
			base.transform.position = this.spawnTransforms[this.spawnIndex].position + Vector3.up * (Time.time - this.timeRiseStarted) / this.totalTimeToRise * this.riseDistance;
			base.transform.rotation = this.spawnTransforms[this.spawnIndex].rotation;
		}
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x0003FA30 File Offset: 0x0003DC30
	private void RiseGrabbedLocalPlayer()
	{
		if (Time.time > this.grabTime + this.minGrabCooldown)
		{
			this.grabTime = Time.time;
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
			GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}
		if (Time.time < this.grabTime + this.grabDuration)
		{
			GorillaTagger.Instance.rigidbody.velocity = Vector3.up * this.grabSpeed;
			EquipmentInteractor.instance.ForceStopClimbing();
		}
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x0003FAE0 File Offset: 0x0003DCE0
	public void UpdateFollowPath(Vector3 destination, float currentSpeed)
	{
		if (this.path == null)
		{
			this.GetNewPath(destination);
		}
		this.points[this.points.Count - 1] = destination;
		Vector3 vector = this.points[this.currentTargetIdx];
		base.transform.position = Vector3.MoveTowards(base.transform.position, vector, currentSpeed * Time.deltaTime);
		Vector3 eulerAngles = Quaternion.LookRotation(vector - base.transform.position).eulerAngles;
		if (Mathf.Abs(eulerAngles.x) > 45f)
		{
			eulerAngles.x = 0f;
		}
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		if (this.currentTargetIdx + 1 < this.points.Count && (base.transform.position - vector).sqrMagnitude < 0.1f)
		{
			if (this.nextPathTimestamp <= Time.time)
			{
				this.GetNewPath(destination);
				return;
			}
			this.currentTargetIdx++;
		}
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x0003FBF0 File Offset: 0x0003DDF0
	private void GetNewPath(Vector3 destination)
	{
		this.path = new NavMeshPath();
		NavMeshHit navMeshHit;
		NavMesh.SamplePosition(base.transform.position, out navMeshHit, 5f, 1);
		NavMeshHit navMeshHit2;
		this.targetIsOnNavMesh = NavMesh.SamplePosition(destination, out navMeshHit2, 5f, 1);
		NavMesh.CalculatePath(navMeshHit.position, navMeshHit2.position, -1, this.path);
		this.points = new List<Vector3>();
		foreach (Vector3 a in this.path.corners)
		{
			this.points.Add(a + Vector3.up * this.heightAboveNavmesh);
		}
		this.points.Add(destination);
		this.currentTargetIdx = 0;
		this.nextPathTimestamp = Time.time + 2f;
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x0003FCC4 File Offset: 0x0003DEC4
	public void ResetPath()
	{
		this.path = null;
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x0003FCD0 File Offset: 0x0003DED0
	private void ChaseHost()
	{
		if (this.followTarget != null)
		{
			if (Time.time > this.lastSpeedIncreased + this.velocityIncreaseTime)
			{
				this.lastSpeedIncreased = Time.time;
				this.currentSpeed += this.velocityStep;
			}
			if (this.targetIsOnNavMesh)
			{
				this.UpdateFollowPath(this.followTarget.position, this.currentSpeed);
				return;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.followTarget.position, this.currentSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.LookRotation(this.followTarget.position - base.transform.position, Vector3.up);
		}
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x0003FDA4 File Offset: 0x0003DFA4
	private void MoveBodyShared()
	{
		this.noisyOffset = new Vector3(Mathf.PerlinNoise(Time.time, 0f) - 0.5f, Mathf.PerlinNoise(Time.time, 10f) - 0.5f, Mathf.PerlinNoise(Time.time, 20f) - 0.5f);
		this.childGhost.localPosition = this.noisyOffset;
		this.leftArm.localEulerAngles = this.noisyOffset * 20f;
		this.rightArm.localEulerAngles = this.noisyOffset * -20f;
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x0003FE42 File Offset: 0x0003E042
	private void GrabBodyShared()
	{
		if (this.followTarget != null)
		{
			base.transform.rotation = this.followTarget.rotation;
			base.transform.position = this.followTarget.position;
		}
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x0003FE80 File Offset: 0x0003E080
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.targetPlayer);
			stream.SendNext(this.currentState);
			stream.SendNext(this.spawnIndex);
			stream.SendNext(this.currentSpeed);
			stream.SendNext(this.isSummoned);
			return;
		}
		this.targetPlayer = (Player)stream.ReceiveNext();
		this.currentState = (HalloweenGhostChaser.ChaseState)stream.ReceiveNext();
		this.spawnIndex = (int)stream.ReceiveNext();
		this.currentSpeed = (float)stream.ReceiveNext();
		this.isSummoned = (bool)stream.ReceiveNext();
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x0003FF3B File Offset: 0x0003E13B
	void IOnPhotonViewOwnerChange.OnOwnerChange(Player newOwner, Player previousOwner)
	{
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.OnChangeState(this.currentState);
		}
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x0003FF51 File Offset: 0x0003E151
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (PhotonNetwork.IsMasterClient)
		{
			this.InitializeGhost();
			return;
		}
		this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
	}

	// Token: 0x04000C9C RID: 3228
	public float heightAboveNavmesh = 0.5f;

	// Token: 0x04000C9D RID: 3229
	public Transform followTarget;

	// Token: 0x04000C9E RID: 3230
	public Transform childGhost;

	// Token: 0x04000C9F RID: 3231
	public float velocityStep = 1f;

	// Token: 0x04000CA0 RID: 3232
	public float currentSpeed;

	// Token: 0x04000CA1 RID: 3233
	public float velocityIncreaseTime = 20f;

	// Token: 0x04000CA2 RID: 3234
	public float riseDistance = 2f;

	// Token: 0x04000CA3 RID: 3235
	public float summonDistance = 5f;

	// Token: 0x04000CA4 RID: 3236
	public float timeEncircled;

	// Token: 0x04000CA5 RID: 3237
	public float lastSummonCheck;

	// Token: 0x04000CA6 RID: 3238
	public float timeGongStarted;

	// Token: 0x04000CA7 RID: 3239
	public float summoningDuration = 30f;

	// Token: 0x04000CA8 RID: 3240
	public float summoningCheckCountdown = 5f;

	// Token: 0x04000CA9 RID: 3241
	public float gongDuration = 5f;

	// Token: 0x04000CAA RID: 3242
	public int summonCount = 5;

	// Token: 0x04000CAB RID: 3243
	public bool wasSurroundedLastCheck;

	// Token: 0x04000CAC RID: 3244
	public AudioSource laugh;

	// Token: 0x04000CAD RID: 3245
	public List<Player> possibleTarget;

	// Token: 0x04000CAE RID: 3246
	public AudioClip defaultLaugh;

	// Token: 0x04000CAF RID: 3247
	public AudioClip deepLaugh;

	// Token: 0x04000CB0 RID: 3248
	public AudioClip gong;

	// Token: 0x04000CB1 RID: 3249
	public Vector3 noisyOffset;

	// Token: 0x04000CB2 RID: 3250
	public Vector3 leftArmGrabbingLocal;

	// Token: 0x04000CB3 RID: 3251
	public Vector3 rightArmGrabbingLocal;

	// Token: 0x04000CB4 RID: 3252
	public Vector3 leftHandGrabbingLocal;

	// Token: 0x04000CB5 RID: 3253
	public Vector3 rightHandGrabbingLocal;

	// Token: 0x04000CB6 RID: 3254
	public Vector3 leftHandStartingLocal;

	// Token: 0x04000CB7 RID: 3255
	public Vector3 rightHandStartingLocal;

	// Token: 0x04000CB8 RID: 3256
	public Vector3 ghostOffsetGrabbingLocal;

	// Token: 0x04000CB9 RID: 3257
	public Vector3 ghostStartingEulerRotation;

	// Token: 0x04000CBA RID: 3258
	public Vector3 ghostGrabbingEulerRotation;

	// Token: 0x04000CBB RID: 3259
	public float maxTimeToNextHeadAngle;

	// Token: 0x04000CBC RID: 3260
	public float lastHeadAngleTime;

	// Token: 0x04000CBD RID: 3261
	public float nextHeadAngleTime;

	// Token: 0x04000CBE RID: 3262
	public float nextTimeToChasePlayer;

	// Token: 0x04000CBF RID: 3263
	public float maxNextTimeToChasePlayer;

	// Token: 0x04000CC0 RID: 3264
	public float timeRiseStarted;

	// Token: 0x04000CC1 RID: 3265
	public float totalTimeToRise;

	// Token: 0x04000CC2 RID: 3266
	public float catchDistance;

	// Token: 0x04000CC3 RID: 3267
	public float grabTime;

	// Token: 0x04000CC4 RID: 3268
	public float grabDuration;

	// Token: 0x04000CC5 RID: 3269
	public float grabSpeed = 1f;

	// Token: 0x04000CC6 RID: 3270
	public float minGrabCooldown;

	// Token: 0x04000CC7 RID: 3271
	public float lastSpeedIncreased;

	// Token: 0x04000CC8 RID: 3272
	public Vector3[] headEulerAngles;

	// Token: 0x04000CC9 RID: 3273
	public Transform skullTransform;

	// Token: 0x04000CCA RID: 3274
	public Transform leftArm;

	// Token: 0x04000CCB RID: 3275
	public Transform rightArm;

	// Token: 0x04000CCC RID: 3276
	public Transform leftHand;

	// Token: 0x04000CCD RID: 3277
	public Transform rightHand;

	// Token: 0x04000CCE RID: 3278
	public Transform[] spawnTransforms;

	// Token: 0x04000CCF RID: 3279
	public Transform[] spawnTransformOffsets;

	// Token: 0x04000CD0 RID: 3280
	public Player targetPlayer;

	// Token: 0x04000CD1 RID: 3281
	public GameObject ghostBody;

	// Token: 0x04000CD2 RID: 3282
	public HalloweenGhostChaser.ChaseState currentState;

	// Token: 0x04000CD3 RID: 3283
	public HalloweenGhostChaser.ChaseState lastState;

	// Token: 0x04000CD4 RID: 3284
	public int spawnIndex;

	// Token: 0x04000CD5 RID: 3285
	public Player grabbedPlayer;

	// Token: 0x04000CD6 RID: 3286
	public Material ghostMaterial;

	// Token: 0x04000CD7 RID: 3287
	public Color defaultColor;

	// Token: 0x04000CD8 RID: 3288
	public Color summonedColor;

	// Token: 0x04000CD9 RID: 3289
	public bool isSummoned;

	// Token: 0x04000CDA RID: 3290
	private bool targetIsOnNavMesh;

	// Token: 0x04000CDB RID: 3291
	private const float navMeshSampleRange = 5f;

	// Token: 0x04000CDC RID: 3292
	[Tooltip("Haptic vibration when chased by lucy")]
	public float hapticStrength = 1f;

	// Token: 0x04000CDD RID: 3293
	public float hapticDuration = 1.5f;

	// Token: 0x04000CDE RID: 3294
	private NavMeshPath path;

	// Token: 0x04000CDF RID: 3295
	public List<Vector3> points;

	// Token: 0x04000CE0 RID: 3296
	public int currentTargetIdx;

	// Token: 0x04000CE1 RID: 3297
	private float nextPathTimestamp;

	// Token: 0x02000439 RID: 1081
	public enum ChaseState
	{
		// Token: 0x04001D8A RID: 7562
		Dormant = 1,
		// Token: 0x04001D8B RID: 7563
		InitialRise,
		// Token: 0x04001D8C RID: 7564
		Gong = 4,
		// Token: 0x04001D8D RID: 7565
		Chasing = 8,
		// Token: 0x04001D8E RID: 7566
		Grabbing = 16
	}
}
