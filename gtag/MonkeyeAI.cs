using System;
using System.Collections.Generic;
using GorillaLocomotion;
using JetBrains.Annotations;
using Pathfinding;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

public class MonkeyeAI : MonoBehaviour
{
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

	private float Distance2D(Vector3 a, Vector3 b)
	{
		Vector2 vector = new Vector2(a.x, a.z);
		Vector2 vector2 = new Vector2(b.x, b.z);
		return Vector2.Distance(vector, vector2);
	}

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
		this.layerMask = UnityLayer.Default.ToLayerMask() | UnityLayer.GorillaObject.ToLayerMask();
		this.SetDefaultAttackState();
		this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		if (this.monkEyeColor.a != 0f)
		{
			this.monkEyeMatPropBlock.SetVector(MonkeyeAI.ColorShaderProp, this.monkEyeColor);
			this.skinnedMeshRenderer.SetPropertyBlock(this.monkEyeMatPropBlock);
		}
		base.InvokeRepeating("AntiOverlapAssurance", 0f, 0.5f);
	}

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

	private void FollowPath()
	{
		if (this.path == null || this.currentWaypoint >= this.path.vectorPath.Count || this.currentWaypoint < 0)
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
			Vector3 vector = normalized * this.speed;
			base.transform.position += vector * Time.deltaTime;
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
		Vector3 vector2 = Vector3.RotateTowards(base.transform.forward, normalized, this.rotationSpeed * Time.deltaTime, 0f);
		base.transform.rotation = Quaternion.LookRotation(vector2);
	}

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

	private void Sleeping()
	{
		this.audioSource.volume = Mathf.Min(this.sleepLoopVolume, this.audioSource.volume + Time.deltaTime / this.sleepLoopFadeInTime);
		this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
		this.PickNewPath(false);
	}

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

	private void Patrolling()
	{
		this.audioSource.volume = Mathf.Min(this.patrolLoopVolume, this.audioSource.volume + Time.deltaTime / this.patrolLoopFadeInTime);
		if (this.path == null)
		{
			this.PickNewPath(false);
		}
		this.CheckForChase();
	}

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

	private void UpdateClientState()
	{
		if (this.wasConnectedToRoom && !PhotonNetwork.IsConnected)
		{
			this.SetDefaultState();
		}
		if (ColliderEnabledManager.instance != null && !this.replState.floorEnabled)
		{
			if (!PhotonNetwork.InRoom)
			{
				if (this.replState.userId == "-1")
				{
					ColliderEnabledManager.instance.DisableFloorForFrame();
				}
			}
			else if (this.replState.userId == PhotonNetwork.LocalPlayer.UserId)
			{
				ColliderEnabledManager.instance.DisableFloorForFrame();
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
			global::GorillaLocomotion.Player.Instance.SetMaximumSlipThisFrame();
			Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
			Vector3 vector = rigidbody.velocity;
			rigidbody.velocity = new Vector3(vector.x * Time.deltaTime * 4f, Mathf.Min(vector.y, 0f), vector.x * Time.deltaTime * 4f);
		}
		this.animController.SetInteger(MonkeyeAI.animStateID, (int)this.replState.state);
	}

	private void SetDefaultState()
	{
		this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		this.SetDefaultAttackState();
	}

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

	private void ExitAttackState()
	{
		this.SetDefaultAttackState();
		this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
	}

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

	private void DropPlayer()
	{
		if (this.replState.timer <= 0f)
		{
			this.replState.timer = this.dropPlayerTime;
			this.replState.floorEnabled = true;
			this.SetState(MonkeyeAI_ReplState.EStates.CloseFloor);
		}
	}

	private void CloseFloor()
	{
		if (this.replState.timer <= 0f)
		{
			this.ExitAttackState();
		}
	}

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

	protected void LateUpdate()
	{
		this.wasConnectedToRoom = PhotonNetwork.InRoom;
	}

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

	public List<Transform> patrolPts;

	public Transform sleepPt;

	private int patrolIdx = -1;

	private int patrolCount;

	private Vector3 targetPosition;

	private MaterialPropertyBlock portalMatPropBlock;

	private MaterialPropertyBlock monkEyeMatPropBlock;

	private Renderer renderer;

	private AIDestinationSetter aiDest;

	private AIPath aiPath;

	private AILerp aiLerp;

	private Seeker seeker;

	private Path path;

	private int currentWaypoint;

	private float nextWaypointDistance;

	private bool reachedEndOfPath;

	private bool calculatingPath;

	private Monkeye_LazerFX lazerFx;

	private Animator animController;

	private RaycastHit[] rayResults = new RaycastHit[1];

	private LayerMask layerMask;

	private bool wasConnectedToRoom;

	public SkinnedMeshRenderer skinnedMeshRenderer;

	public MazePlayerCollection playerCollection;

	public PlayerCollection playersInRoomCollection;

	private List<VRRig> validRigs = new List<VRRig>();

	public GameObject portalFx;

	public Transform[] eyeBones;

	public float speed = 0.1f;

	public float rotationSpeed = 1f;

	public float wakeDistance = 1f;

	public float chaseDistance = 3f;

	public float attackDistance = 0.1f;

	public float beginAttackTime = 1f;

	public float openFloorTime = 3f;

	public float dropPlayerTime = 1f;

	public float closeFloorTime = 1f;

	public Color portalColor;

	public Color gorillaPortalColor;

	public Color monkEyeColor;

	public int maxPatrols = 4;

	private VRRig targetRig;

	public MonkeyeAI_ReplState replState;

	private int layerBase;

	private int layerForward = 1;

	private int layerLeft = 2;

	private int layerRight = 3;

	private static readonly int EmissionColorShaderProp = Shader.PropertyToID("_EmissionColor");

	private static readonly int ColorShaderProp = Shader.PropertyToID("_BaseColor");

	private static readonly int tintColorShaderProp = Shader.PropertyToID("_TintColor");

	private static readonly int animStateID = Animator.StringToHash("state");

	private Vector3 prevPosition;

	private Vector3 velocity;

	public AudioSource audioSource;

	public AudioClip sleepLoopSound;

	public float sleepLoopVolume = 0.5f;

	public float sleepLoopFadeInTime = 3f;

	[FormerlySerializedAs("moveLoopSound")]
	public AudioClip patrolLoopSound;

	public float patrolLoopVolume = 0.5f;

	public float patrolLoopFadeInTime = 1f;

	public AudioClip chaseLoopSound;

	public float chaseLoopVolume = 0.5f;

	public float chaseLoopFadeInTime = 0.05f;

	public AudioClip attackSound;

	public float attackVolume = 0.5f;

	private PhotonView _view;

	public float overlapRadius;
}
