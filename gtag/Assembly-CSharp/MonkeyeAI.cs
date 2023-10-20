using System;
using System.Collections.Generic;
using GorillaLocomotion;
using JetBrains.Annotations;
using Pathfinding;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000025 RID: 37
public class MonkeyeAI : MonoBehaviour
{
	// Token: 0x060000BC RID: 188 RVA: 0x00007710 File Offset: 0x00005910
	private string UserIdFromRig(VRRig rig)
	{
		if (rig == null)
		{
			return "";
		}
		if (!PhotonNetwork.InRoom)
		{
			if (rig == GorillaTagger.Instance.offlineVRRig)
			{
				return "-1";
			}
			Debug.Log("Not in a room but not targeting offline rig");
			return null;
		}
		else
		{
			if (rig == GorillaTagger.Instance.offlineVRRig)
			{
				return PhotonNetwork.LocalPlayer.UserId;
			}
			if (rig.creator == null)
			{
				return "";
			}
			return rig.creator.UserId;
		}
	}

	// Token: 0x060000BD RID: 189 RVA: 0x00007790 File Offset: 0x00005990
	private VRRig GetRig(string userId)
	{
		if (userId == "")
		{
			return null;
		}
		if (PhotonNetwork.InRoom || !(userId != "-1"))
		{
			foreach (VRRig vrrig in this.GetValidChoosableRigs())
			{
				if (!(vrrig == null))
				{
					Photon.Realtime.Player creator = vrrig.creator;
					if (creator != null && userId == creator.UserId)
					{
						return vrrig;
					}
				}
			}
			return null;
		}
		if (userId == "-1 " && GorillaTagger.Instance != null)
		{
			return GorillaTagger.Instance.offlineVRRig;
		}
		return null;
	}

	// Token: 0x060000BE RID: 190 RVA: 0x00007850 File Offset: 0x00005A50
	private float Distance2D(Vector3 a, Vector3 b)
	{
		Vector2 a2 = new Vector2(a.x, a.z);
		Vector2 b2 = new Vector2(b.x, b.z);
		return Vector2.Distance(a2, b2);
	}

	// Token: 0x060000BF RID: 191 RVA: 0x00007888 File Offset: 0x00005A88
	private Transform PickRandomPatrolPoint()
	{
		int num;
		do
		{
			num = Random.Range(0, this.patrolPts.Count);
		}
		while (num == this.patrolIdx);
		this.patrolIdx = num;
		return this.patrolPts[num];
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x000078C8 File Offset: 0x00005AC8
	private void PickNewPath(bool pathFinished = false)
	{
		if (this.calculatingPath)
		{
			return;
		}
		this.currentWaypoint = 0;
		switch (this.replState.state)
		{
		case MonkeyeAI_ReplState.EStates.Patrolling:
			if (this.patrolCount == this.maxPatrols)
			{
				this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
				this.targetPosition = this.PickRandomPatrolPoint().position;
				this.patrolCount = 0;
			}
			else
			{
				this.targetPosition = this.PickRandomPatrolPoint().position;
				this.patrolCount++;
			}
			break;
		case MonkeyeAI_ReplState.EStates.Chasing:
		{
			Vector3 position = base.transform.position;
			VRRig vrrig;
			if (this.ClosestPlayer(position, out vrrig) && vrrig != this.targetRig)
			{
				this.SetTargetPlayer(vrrig);
			}
			if (this.targetRig == null)
			{
				this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
				this.targetPosition = this.sleepPt.position;
			}
			else
			{
				this.targetPosition = this.targetRig.transform.position;
			}
			break;
		}
		case MonkeyeAI_ReplState.EStates.ReturnToSleepPt:
			this.targetPosition = this.sleepPt.position;
			break;
		}
		this.calculatingPath = true;
		this.seeker.StartPath(base.transform.position, this.targetPosition, new OnPathDelegate(this.OnPathComplete));
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x00007A0C File Offset: 0x00005C0C
	private void Awake()
	{
		this._view = base.GetComponent<PhotonView>();
		this.lazerFx = base.GetComponent<Monkeye_LazerFX>();
		this.animController = base.GetComponent<Animator>();
		this.layerBase = this.animController.GetLayerIndex("Base_Layer");
		this.layerForward = this.animController.GetLayerIndex("MoveFwdAddPose");
		this.layerLeft = this.animController.GetLayerIndex("TurnLAddPose");
		this.layerRight = this.animController.GetLayerIndex("TurnRAddPose");
		this.seeker = base.GetComponent<Seeker>();
		this.renderer = this.portalFx.GetComponent<Renderer>();
		this.portalMatPropBlock = new MaterialPropertyBlock();
		this.monkEyeMatPropBlock = new MaterialPropertyBlock();
		this.layerMask = (LayerMask.GetMask(new string[]
		{
			"Default"
		}) | LayerMask.GetMask(new string[]
		{
			"Gorilla Object"
		}));
		this.SetDefaultAttackState();
		this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		if (this.monkEyeColor.a != 0f)
		{
			this.monkEyeMatPropBlock.SetVector(MonkeyeAI.ColorShaderProp, this.monkEyeColor);
			this.skinnedMeshRenderer.SetPropertyBlock(this.monkEyeMatPropBlock);
		}
		base.InvokeRepeating("AntiOverlapAssurance", 0f, 0.5f);
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x00007B5C File Offset: 0x00005D5C
	private void OnPathComplete(Path path_)
	{
		this.path = path_;
		this.currentWaypoint = 0;
		if (this.path.vectorPath.Count < 1)
		{
			base.transform.position = this.sleepPt.position;
			base.transform.rotation = this.sleepPt.rotation;
			this.path = null;
		}
		else
		{
			this.nextWaypointDistance = this.Distance2D(base.transform.position, this.path.vectorPath[this.currentWaypoint]);
		}
		this.reachedEndOfPath = false;
		this.calculatingPath = false;
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x00007BFC File Offset: 0x00005DFC
	private void FollowPath()
	{
		if (this.path == null || this.currentWaypoint >= this.path.vectorPath.Count)
		{
			this.PickNewPath(false);
			if (this.path == null)
			{
				return;
			}
		}
		if (this.Distance2D(base.transform.position, this.path.vectorPath[this.currentWaypoint]) < 0.01f)
		{
			if (this.currentWaypoint + 1 == this.path.vectorPath.Count)
			{
				this.PickNewPath(true);
				return;
			}
			this.currentWaypoint++;
		}
		Vector3 normalized = (this.path.vectorPath[this.currentWaypoint] - base.transform.position).normalized;
		normalized.y = 0f;
		if (this.animController.GetCurrentAnimatorStateInfo(0).IsName("Move"))
		{
			Vector3 a = normalized * this.speed;
			base.transform.position += a * Time.deltaTime;
		}
		Mathf.Clamp01(Vector3.Dot(base.transform.forward, normalized) / 1.5707964f);
		if (Mathf.Sign(Vector3.Cross(base.transform.forward, normalized).y) > 0f)
		{
			this.animController.SetLayerWeight(this.layerRight, 0f);
		}
		else
		{
			this.animController.SetLayerWeight(this.layerLeft, 0f);
		}
		this.animController.SetLayerWeight(this.layerForward, 0f);
		Vector3 forward = Vector3.RotateTowards(base.transform.forward, normalized, this.rotationSpeed * Time.deltaTime, 0f);
		base.transform.rotation = Quaternion.LookRotation(forward);
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x00007DD4 File Offset: 0x00005FD4
	private bool PlayerNear(VRRig rig, float dist, out float playerDist)
	{
		if (rig == null)
		{
			playerDist = float.PositiveInfinity;
			return false;
		}
		playerDist = this.Distance2D(rig.transform.position, base.transform.position);
		return playerDist < dist && Physics.RaycastNonAlloc(new Ray(base.transform.position, rig.transform.position - base.transform.position), this.rayResults, playerDist, this.layerMask) <= 0;
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x00007E61 File Offset: 0x00006061
	private void Sleeping()
	{
		this.audioSource.volume = Mathf.Min(this.sleepLoopVolume, this.audioSource.volume + Time.deltaTime / this.sleepLoopFadeInTime);
		this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
		this.PickNewPath(false);
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x00007EA0 File Offset: 0x000060A0
	private bool ClosestPlayer(in Vector3 myPos, out VRRig outRig)
	{
		float num = float.MaxValue;
		outRig = null;
		foreach (VRRig vrrig in this.GetValidChoosableRigs())
		{
			float num2 = 0f;
			if (this.PlayerNear(vrrig, this.chaseDistance, out num2) && num2 < num)
			{
				num = num2;
				outRig = vrrig;
			}
		}
		return num != float.MaxValue;
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x00007F20 File Offset: 0x00006120
	private bool CheckForChase()
	{
		foreach (VRRig vrrig in this.GetValidChoosableRigs())
		{
			float num = 0f;
			if (this.PlayerNear(vrrig, this.wakeDistance, out num))
			{
				this.SetTargetPlayer(vrrig);
				this.SetState(MonkeyeAI_ReplState.EStates.Chasing);
				this.PickNewPath(false);
				return true;
			}
		}
		return false;
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x00007FA0 File Offset: 0x000061A0
	private void Patrolling()
	{
		this.audioSource.volume = Mathf.Min(this.patrolLoopVolume, this.audioSource.volume + Time.deltaTime / this.patrolLoopFadeInTime);
		if (this.path == null)
		{
			this.PickNewPath(false);
		}
		this.CheckForChase();
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x00007FF4 File Offset: 0x000061F4
	private void Chasing()
	{
		this.audioSource.volume = Mathf.Min(this.chaseLoopVolume, this.audioSource.volume + Time.deltaTime / this.chaseLoopFadeInTime);
		this.PickNewPath(false);
		if (this.targetRig == null)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
			return;
		}
		if (this.Distance2D(base.transform.position, this.targetRig.transform.position) < this.attackDistance)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.BeginAttack);
			return;
		}
	}

	// Token: 0x060000CA RID: 202 RVA: 0x00008080 File Offset: 0x00006280
	private void ReturnToSleepPt()
	{
		if (this.path == null)
		{
			this.PickNewPath(false);
		}
		if (this.CheckForChase())
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Chasing);
			return;
		}
		if (this.Distance2D(base.transform.position, this.sleepPt.position) < 0.01f)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		}
	}

	// Token: 0x060000CB RID: 203 RVA: 0x000080D8 File Offset: 0x000062D8
	private void UpdateClientState()
	{
		if (this.wasConnectedToRoom && !PhotonNetwork.IsConnected)
		{
			this.SetDefaultState();
		}
		if (!this.replState.floorEnabled)
		{
			if (!PhotonNetwork.InRoom)
			{
				if (this.replState.userId == "-1")
				{
					this.collMan.DisableFloorForFrame();
				}
			}
			else if (this.replState.userId == PhotonNetwork.LocalPlayer.UserId)
			{
				this.collMan.DisableFloorForFrame();
			}
		}
		if (this.portalFx.activeSelf != this.replState.portalEnabled)
		{
			this.portalFx.SetActive(this.replState.portalEnabled);
		}
		this.portalFx.transform.position = new Vector3(this.replState.attackPos.x, this.portalFx.transform.position.y, this.replState.attackPos.z);
		this.replState.timer -= Time.deltaTime;
		if (this.replState.timer < 0f)
		{
			this.replState.timer = 0f;
		}
		VRRig rig = this.GetRig(this.replState.userId);
		if (this.replState.state >= MonkeyeAI_ReplState.EStates.BeginAttack)
		{
			if (rig == null)
			{
				this.lazerFx.DisableLazer();
			}
			else if (this.replState.state < MonkeyeAI_ReplState.EStates.DropPlayer)
			{
				this.lazerFx.EnableLazer(this.eyeBones, rig);
			}
			else
			{
				this.lazerFx.DisableLazer();
			}
		}
		else
		{
			this.lazerFx.DisableLazer();
		}
		if (this.replState.portalEnabled)
		{
			this.portalColor.a = this.replState.alpha;
			this.portalMatPropBlock.SetVector(MonkeyeAI.tintColorShaderProp, this.portalColor);
			this.renderer.SetPropertyBlock(this.portalMatPropBlock);
		}
		if (GorillaTagger.Instance.offlineVRRig == rig && this.replState.freezePlayer)
		{
			GorillaLocomotion.Player.Instance.SetMaximumSlipThisFrame();
			Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
			Vector3 vector = rigidbody.velocity;
			rigidbody.velocity = new Vector3(vector.x * Time.deltaTime * 4f, Mathf.Min(vector.y, 0f), vector.x * Time.deltaTime * 4f);
		}
		this.animController.SetInteger(MonkeyeAI.animStateID, (int)this.replState.state);
	}

	// Token: 0x060000CC RID: 204 RVA: 0x0000835C File Offset: 0x0000655C
	private void SetDefaultState()
	{
		this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		this.SetDefaultAttackState();
	}

	// Token: 0x060000CD RID: 205 RVA: 0x0000836C File Offset: 0x0000656C
	private void SetDefaultAttackState()
	{
		this.replState.floorEnabled = true;
		this.replState.timer = 0f;
		this.replState.userId = "";
		this.replState.attackPos = base.transform.position;
		this.replState.portalEnabled = false;
		this.replState.freezePlayer = false;
		this.replState.alpha = 0f;
	}

	// Token: 0x060000CE RID: 206 RVA: 0x000083E3 File Offset: 0x000065E3
	private void ExitAttackState()
	{
		this.SetDefaultAttackState();
		this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
	}

	// Token: 0x060000CF RID: 207 RVA: 0x000083F4 File Offset: 0x000065F4
	private void BeginAttack()
	{
		this.path = null;
		this.replState.freezePlayer = true;
		if (this.replState.timer <= 0f)
		{
			this.audioSource.PlayOneShot(this.attackSound, this.attackVolume);
			this.replState.timer = this.openFloorTime;
			this.replState.portalEnabled = true;
			this.SetState(MonkeyeAI_ReplState.EStates.OpenFloor);
		}
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x00008464 File Offset: 0x00006664
	private void OpenFloor()
	{
		this.replState.alpha = Mathf.Lerp(0f, 1f, 1f - Mathf.Clamp01(this.replState.timer / this.openFloorTime));
		if (this.replState.timer <= 0f)
		{
			this.replState.timer = this.dropPlayerTime;
			this.replState.floorEnabled = false;
			this.SetState(MonkeyeAI_ReplState.EStates.DropPlayer);
		}
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x000084DE File Offset: 0x000066DE
	private void DropPlayer()
	{
		if (this.replState.timer <= 0f)
		{
			this.replState.timer = this.dropPlayerTime;
			this.replState.floorEnabled = true;
			this.SetState(MonkeyeAI_ReplState.EStates.CloseFloor);
		}
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x00008516 File Offset: 0x00006716
	private void CloseFloor()
	{
		if (this.replState.timer <= 0f)
		{
			this.ExitAttackState();
		}
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x00008530 File Offset: 0x00006730
	private void ValidateChasingRig()
	{
		if (this.targetRig == null)
		{
			this.SetTargetPlayer(null);
			return;
		}
		bool flag = false;
		foreach (VRRig vrrig in this.GetValidChoosableRigs())
		{
			if (vrrig == this.targetRig)
			{
				flag = true;
				this.SetTargetPlayer(vrrig);
				break;
			}
		}
		if (!flag)
		{
			this.SetTargetPlayer(null);
		}
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x000085B8 File Offset: 0x000067B8
	public void SetState(MonkeyeAI_ReplState.EStates state_)
	{
		this.replState.state = state_;
		this.animController.SetInteger(MonkeyeAI.animStateID, (int)this.replState.state);
		switch (this.replState.state)
		{
		case MonkeyeAI_ReplState.EStates.Sleeping:
			this.audioSource.clip = this.sleepLoopSound;
			this.audioSource.volume = 0f;
			this.audioSource.Play();
			return;
		case MonkeyeAI_ReplState.EStates.Patrolling:
			this.audioSource.clip = this.patrolLoopSound;
			this.audioSource.loop = true;
			this.audioSource.volume = 0f;
			this.audioSource.Play();
			this.patrolCount = 0;
			return;
		case MonkeyeAI_ReplState.EStates.Chasing:
			this.audioSource.loop = true;
			this.audioSource.volume = 0f;
			this.audioSource.clip = this.chaseLoopSound;
			this.audioSource.Play();
			return;
		case MonkeyeAI_ReplState.EStates.ReturnToSleepPt:
		case MonkeyeAI_ReplState.EStates.GoToSleep:
			break;
		case MonkeyeAI_ReplState.EStates.BeginAttack:
			this.replState.attackPos = ((this.targetRig != null) ? this.targetRig.transform.position : base.transform.position);
			this.replState.timer = this.beginAttackTime;
			break;
		default:
			return;
		}
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x00008704 File Offset: 0x00006904
	public List<VRRig> GetValidChoosableRigs()
	{
		this.validRigs.Clear();
		foreach (VRRig vrrig in this.playerCollection.containedRigs)
		{
			if ((PhotonNetwork.InRoom || vrrig.isOfflineVRRig) && !(vrrig == null))
			{
				this.validRigs.Add(vrrig);
			}
		}
		return this.validRigs;
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x0000878C File Offset: 0x0000698C
	private void Update()
	{
		this.UpdateClientState();
		if (!this._view.IsMine && PhotonNetwork.InRoom)
		{
			this.path = null;
			return;
		}
		if (!this.playerCollection.gameObject.activeInHierarchy)
		{
			Photon.Realtime.Player player = null;
			float num = float.PositiveInfinity;
			foreach (VRRig vrrig in this.playersInRoomCollection.containedRigs)
			{
				if (!(vrrig == null))
				{
					float num2 = Vector3.Distance(base.transform.position, vrrig.transform.position);
					if (num2 < num)
					{
						player = vrrig.creator;
						num = num2;
					}
				}
			}
			if (num > 6f)
			{
				return;
			}
			this.path = null;
			if (player == null)
			{
				return;
			}
			this._view.GetComponent<RequestableOwnershipGuard>().TransferOwnership(player, "");
			this.replState.GetComponent<RequestableOwnershipGuard>().TransferOwnership(player, "");
			return;
		}
		else
		{
			this.ValidateChasingRig();
			switch (this.replState.state)
			{
			case MonkeyeAI_ReplState.EStates.Sleeping:
				this.Sleeping();
				break;
			case MonkeyeAI_ReplState.EStates.Patrolling:
				this.Patrolling();
				break;
			case MonkeyeAI_ReplState.EStates.Chasing:
				this.Chasing();
				break;
			case MonkeyeAI_ReplState.EStates.ReturnToSleepPt:
				this.ReturnToSleepPt();
				break;
			case MonkeyeAI_ReplState.EStates.BeginAttack:
				this.BeginAttack();
				break;
			case MonkeyeAI_ReplState.EStates.OpenFloor:
				this.OpenFloor();
				break;
			case MonkeyeAI_ReplState.EStates.DropPlayer:
				this.DropPlayer();
				break;
			case MonkeyeAI_ReplState.EStates.CloseFloor:
				this.CloseFloor();
				break;
			}
			if (this.path == null)
			{
				return;
			}
			this.FollowPath();
			this.velocity = base.transform.position - this.prevPosition;
			this.prevPosition = base.transform.position;
			return;
		}
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00008954 File Offset: 0x00006B54
	protected void LateUpdate()
	{
		this.wasConnectedToRoom = PhotonNetwork.InRoom;
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00008964 File Offset: 0x00006B64
	private void AntiOverlapAssurance()
	{
		if ((!this._view.IsMine && PhotonNetwork.InRoom) || !this.playerCollection.gameObject.activeInHierarchy)
		{
			return;
		}
		foreach (MonkeyeAI monkeyeAI in this.playerCollection.monkeyeAis)
		{
			if (!(monkeyeAI == this) && Vector3.Distance(base.transform.position, monkeyeAI.transform.position) < this.overlapRadius && (double)Vector3.Dot(base.transform.forward, monkeyeAI.transform.forward) > 0.2)
			{
				MonkeyeAI_ReplState.EStates state = this.replState.state;
				if (state != MonkeyeAI_ReplState.EStates.Patrolling)
				{
					if (state == MonkeyeAI_ReplState.EStates.Chasing)
					{
						if (monkeyeAI.replState.state == MonkeyeAI_ReplState.EStates.Chasing)
						{
							this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
						}
					}
				}
				else
				{
					this.PickNewPath(false);
				}
			}
		}
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x00008A70 File Offset: 0x00006C70
	private void SetTargetPlayer([CanBeNull] VRRig rig)
	{
		if (rig == null)
		{
			this.replState.userId = "";
			this.replState.freezePlayer = false;
			this.replState.floorEnabled = true;
			this.replState.portalEnabled = false;
			this.targetRig = null;
			return;
		}
		this.replState.userId = this.UserIdFromRig(rig);
		this.targetRig = rig;
	}

	// Token: 0x040000E0 RID: 224
	public List<Transform> patrolPts;

	// Token: 0x040000E1 RID: 225
	public Transform sleepPt;

	// Token: 0x040000E2 RID: 226
	private int patrolIdx = -1;

	// Token: 0x040000E3 RID: 227
	private int patrolCount;

	// Token: 0x040000E4 RID: 228
	private Vector3 targetPosition;

	// Token: 0x040000E5 RID: 229
	private MaterialPropertyBlock portalMatPropBlock;

	// Token: 0x040000E6 RID: 230
	private MaterialPropertyBlock monkEyeMatPropBlock;

	// Token: 0x040000E7 RID: 231
	private Renderer renderer;

	// Token: 0x040000E8 RID: 232
	private AIDestinationSetter aiDest;

	// Token: 0x040000E9 RID: 233
	private AIPath aiPath;

	// Token: 0x040000EA RID: 234
	private AILerp aiLerp;

	// Token: 0x040000EB RID: 235
	private Seeker seeker;

	// Token: 0x040000EC RID: 236
	private Path path;

	// Token: 0x040000ED RID: 237
	private int currentWaypoint;

	// Token: 0x040000EE RID: 238
	private float nextWaypointDistance;

	// Token: 0x040000EF RID: 239
	private bool reachedEndOfPath;

	// Token: 0x040000F0 RID: 240
	private bool calculatingPath;

	// Token: 0x040000F1 RID: 241
	private Monkeye_LazerFX lazerFx;

	// Token: 0x040000F2 RID: 242
	private Animator animController;

	// Token: 0x040000F3 RID: 243
	private RaycastHit[] rayResults = new RaycastHit[1];

	// Token: 0x040000F4 RID: 244
	private LayerMask layerMask;

	// Token: 0x040000F5 RID: 245
	private bool wasConnectedToRoom;

	// Token: 0x040000F6 RID: 246
	public SkinnedMeshRenderer skinnedMeshRenderer;

	// Token: 0x040000F7 RID: 247
	public MazePlayerCollection playerCollection;

	// Token: 0x040000F8 RID: 248
	public PlayerCollection playersInRoomCollection;

	// Token: 0x040000F9 RID: 249
	private List<VRRig> validRigs = new List<VRRig>();

	// Token: 0x040000FA RID: 250
	public ColliderEnabledManager collMan;

	// Token: 0x040000FB RID: 251
	public GameObject portalFx;

	// Token: 0x040000FC RID: 252
	public Transform[] eyeBones;

	// Token: 0x040000FD RID: 253
	public float speed = 0.1f;

	// Token: 0x040000FE RID: 254
	public float rotationSpeed = 1f;

	// Token: 0x040000FF RID: 255
	public float wakeDistance = 1f;

	// Token: 0x04000100 RID: 256
	public float chaseDistance = 3f;

	// Token: 0x04000101 RID: 257
	public float attackDistance = 0.1f;

	// Token: 0x04000102 RID: 258
	public float beginAttackTime = 1f;

	// Token: 0x04000103 RID: 259
	public float openFloorTime = 3f;

	// Token: 0x04000104 RID: 260
	public float dropPlayerTime = 1f;

	// Token: 0x04000105 RID: 261
	public float closeFloorTime = 1f;

	// Token: 0x04000106 RID: 262
	public Color portalColor;

	// Token: 0x04000107 RID: 263
	public Color gorillaPortalColor;

	// Token: 0x04000108 RID: 264
	public Color monkEyeColor;

	// Token: 0x04000109 RID: 265
	public int maxPatrols = 4;

	// Token: 0x0400010A RID: 266
	private VRRig targetRig;

	// Token: 0x0400010B RID: 267
	public MonkeyeAI_ReplState replState;

	// Token: 0x0400010C RID: 268
	private int layerBase;

	// Token: 0x0400010D RID: 269
	private int layerForward = 1;

	// Token: 0x0400010E RID: 270
	private int layerLeft = 2;

	// Token: 0x0400010F RID: 271
	private int layerRight = 3;

	// Token: 0x04000110 RID: 272
	private static readonly int EmissionColorShaderProp = Shader.PropertyToID("_EmissionColor");

	// Token: 0x04000111 RID: 273
	private static readonly int ColorShaderProp = Shader.PropertyToID("_BaseColor");

	// Token: 0x04000112 RID: 274
	private static readonly int tintColorShaderProp = Shader.PropertyToID("_TintColor");

	// Token: 0x04000113 RID: 275
	private static readonly int animStateID = Animator.StringToHash("state");

	// Token: 0x04000114 RID: 276
	private Vector3 prevPosition;

	// Token: 0x04000115 RID: 277
	private Vector3 velocity;

	// Token: 0x04000116 RID: 278
	public AudioSource audioSource;

	// Token: 0x04000117 RID: 279
	public AudioClip sleepLoopSound;

	// Token: 0x04000118 RID: 280
	public float sleepLoopVolume = 0.5f;

	// Token: 0x04000119 RID: 281
	public float sleepLoopFadeInTime = 3f;

	// Token: 0x0400011A RID: 282
	[FormerlySerializedAs("moveLoopSound")]
	public AudioClip patrolLoopSound;

	// Token: 0x0400011B RID: 283
	public float patrolLoopVolume = 0.5f;

	// Token: 0x0400011C RID: 284
	public float patrolLoopFadeInTime = 1f;

	// Token: 0x0400011D RID: 285
	public AudioClip chaseLoopSound;

	// Token: 0x0400011E RID: 286
	public float chaseLoopVolume = 0.5f;

	// Token: 0x0400011F RID: 287
	public float chaseLoopFadeInTime = 0.05f;

	// Token: 0x04000120 RID: 288
	public AudioClip attackSound;

	// Token: 0x04000121 RID: 289
	public float attackVolume = 0.5f;

	// Token: 0x04000122 RID: 290
	private PhotonView _view;

	// Token: 0x04000123 RID: 291
	public float overlapRadius;
}
