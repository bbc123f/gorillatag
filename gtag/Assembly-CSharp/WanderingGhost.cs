using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001C4 RID: 452
public class WanderingGhost : MonoBehaviourPunCallbacks, IPunObservable, IOnPhotonViewOwnerChange, IPhotonViewCallback
{
	// Token: 0x06000B76 RID: 2934 RVA: 0x000467A8 File Offset: 0x000449A8
	private void Start()
	{
		this.waypointRegions = this.waypointsContainer.GetComponentsInChildren<ZoneBasedObject>();
		this.idlePassedTime = 0f;
		this.idlSoundPlayed = false;
		this.handLayerMaskId = LayerMask.NameToLayer(this.handLayermask);
		this.bodyLayerMaskId = LayerMask.NameToLayer(this.bodyLayerMask);
		ThrowableSetDressing[] array = this.allFlowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].anchor.position = this.flowerDisabledPosition;
		}
		base.Invoke("DelayedStart", 0.5f);
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x00046832 File Offset: 0x00044A32
	private void DelayedStart()
	{
		this.PickNextWaypoint();
		base.transform.position = this.currentWaypoint._transform.position;
		this.PickNextWaypoint();
		this.ChangeState(WanderingGhost.ghostState.patrol);
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x00046864 File Offset: 0x00044A64
	private void LateUpdate()
	{
		this.UpdateState();
		this.hoverVelocity -= this.mrenderer.transform.localPosition * this.hoverRectifyForce * Time.deltaTime;
		this.hoverVelocity += Random.insideUnitSphere * this.hoverRandomForce * Time.deltaTime;
		this.hoverVelocity = Vector3.MoveTowards(this.hoverVelocity, Vector3.zero, this.hoverDrag * Time.deltaTime);
		this.mrenderer.transform.localPosition += this.hoverVelocity * Time.deltaTime;
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x00046928 File Offset: 0x00044B28
	private void PickNextWaypoint()
	{
		if (this.waypoints.Count == 0 || this.lastWaypointRegion == null || !this.lastWaypointRegion.IsLocalPlayerInZone())
		{
			ZoneBasedObject zoneBasedObject = ZoneBasedObject.SelectRandomEligible(this.waypointRegions, this.debugForceWaypointRegion);
			if (zoneBasedObject == null)
			{
				zoneBasedObject = this.lastWaypointRegion;
			}
			if (zoneBasedObject == null)
			{
				return;
			}
			this.lastWaypointRegion = zoneBasedObject;
			this.waypoints.Clear();
			foreach (object obj in zoneBasedObject.transform)
			{
				Transform transform = (Transform)obj;
				this.waypoints.Add(new WanderingGhost.Waypoint(transform.name.Contains("_v_"), transform));
			}
		}
		int index = Random.Range(0, this.waypoints.Count);
		this.currentWaypoint = this.waypoints[index];
		this.waypoints.RemoveAt(index);
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x00046A38 File Offset: 0x00044C38
	private void Patrol()
	{
		this.idlSoundPlayed = false;
		this.idlePassedTime = 0f;
		this.mrenderer.sharedMaterial = this.scryableMaterial;
		Transform transform = this.currentWaypoint._transform;
		base.transform.position = Vector3.MoveTowards(base.transform.position, transform.position, this.patrolSpeed * Time.deltaTime);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(transform.position - base.transform.position), 360f * Time.deltaTime);
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x00046AE4 File Offset: 0x00044CE4
	private bool MaybeHideGhost()
	{
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.sphereColliderRadius, this.hitColliders);
		for (int i = 0; i < num; i++)
		{
			if (this.hitColliders[i].gameObject.layer == this.handLayerMaskId || this.hitColliders[i].gameObject.layer == this.bodyLayerMaskId)
			{
				this.ChangeState(WanderingGhost.ghostState.patrol);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x00046B58 File Offset: 0x00044D58
	private void ChangeState(WanderingGhost.ghostState newState)
	{
		this.currentState = newState;
		this.mrenderer.sharedMaterial = ((newState == WanderingGhost.ghostState.idle) ? this.visibleMaterial : this.scryableMaterial);
		if (newState == WanderingGhost.ghostState.patrol)
		{
			this.audioSource.Stop();
			this.audioSource.volume = this.patrolVolume;
			this.audioSource.clip = this.patrolAudio;
			this.audioSource.Play();
			return;
		}
		if (newState != WanderingGhost.ghostState.idle)
		{
			return;
		}
		this.audioSource.Stop();
		this.audioSource.volume = this.idleVolume;
		this.audioSource.PlayOneShot(this.appearAudio.GetRandomItem<AudioClip>());
		if (PhotonNetwork.IsMasterClient)
		{
			this.SpawnFlowerNearby();
		}
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x00046C0C File Offset: 0x00044E0C
	private void UpdateState()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		WanderingGhost.ghostState ghostState = this.currentState;
		if (ghostState != WanderingGhost.ghostState.patrol)
		{
			if (ghostState != WanderingGhost.ghostState.idle)
			{
				return;
			}
			this.idlePassedTime += Time.deltaTime;
			if (this.idlePassedTime >= this.idleStayDuration || this.MaybeHideGhost())
			{
				this.PickNextWaypoint();
				this.ChangeState(WanderingGhost.ghostState.patrol);
			}
		}
		else
		{
			if (this.currentWaypoint._transform == null)
			{
				this.PickNextWaypoint();
				return;
			}
			this.Patrol();
			if (Vector3.Distance(base.transform.position, this.currentWaypoint._transform.position) < 0.2f)
			{
				if (this.currentWaypoint._visible)
				{
					this.ChangeState(WanderingGhost.ghostState.idle);
					return;
				}
				this.PickNextWaypoint();
				return;
			}
		}
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x00046CC8 File Offset: 0x00044EC8
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

	// Token: 0x06000B7F RID: 2943 RVA: 0x00046D2C File Offset: 0x00044F2C
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.currentState);
			return;
		}
		WanderingGhost.ghostState ghostState = this.currentState;
		this.currentState = (WanderingGhost.ghostState)stream.ReceiveNext();
		if (ghostState != this.currentState)
		{
			this.ChangeState(this.currentState);
		}
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x00046D8C File Offset: 0x00044F8C
	void IOnPhotonViewOwnerChange.OnOwnerChange(Player newOwner, Player previousOwner)
	{
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.ChangeState(this.currentState);
		}
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x00046DA4 File Offset: 0x00044FA4
	private void SpawnFlowerNearby()
	{
		Vector3 position = base.transform.position + Vector3.down * 0.25f;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(base.transform.position + Random.insideUnitCircle.x0y() * this.flowerSpawnRadius, Vector3.down), out raycastHit, 3f, this.flowerGroundMask))
		{
			position = raycastHit.point;
		}
		ThrowableSetDressing throwableSetDressing = null;
		int num = 0;
		foreach (ThrowableSetDressing throwableSetDressing2 in this.allFlowers)
		{
			if (!throwableSetDressing2.InHand())
			{
				num++;
				if (Random.Range(0, num) == 0)
				{
					throwableSetDressing = throwableSetDressing2;
				}
			}
		}
		if (throwableSetDressing != null)
		{
			if (!throwableSetDressing.IsLocalOwnedWorldShareable)
			{
				throwableSetDressing.WorldShareableRequestOwnership();
			}
			throwableSetDressing.SetWillTeleport();
			throwableSetDressing.transform.position = position;
			throwableSetDressing.StartRespawnTimer(this.flowerSpawnDuration);
		}
	}

	// Token: 0x04000EEE RID: 3822
	public float patrolSpeed = 3f;

	// Token: 0x04000EEF RID: 3823
	public float idleStayDuration = 5f;

	// Token: 0x04000EF0 RID: 3824
	public float sphereColliderRadius = 2f;

	// Token: 0x04000EF1 RID: 3825
	public ThrowableSetDressing[] allFlowers;

	// Token: 0x04000EF2 RID: 3826
	public Vector3 flowerDisabledPosition;

	// Token: 0x04000EF3 RID: 3827
	public float flowerSpawnRadius;

	// Token: 0x04000EF4 RID: 3828
	public float flowerSpawnDuration;

	// Token: 0x04000EF5 RID: 3829
	public LayerMask flowerGroundMask;

	// Token: 0x04000EF6 RID: 3830
	public MeshRenderer mrenderer;

	// Token: 0x04000EF7 RID: 3831
	public Material visibleMaterial;

	// Token: 0x04000EF8 RID: 3832
	public Material scryableMaterial;

	// Token: 0x04000EF9 RID: 3833
	public GameObject waypointsContainer;

	// Token: 0x04000EFA RID: 3834
	private ZoneBasedObject[] waypointRegions;

	// Token: 0x04000EFB RID: 3835
	private ZoneBasedObject lastWaypointRegion;

	// Token: 0x04000EFC RID: 3836
	private List<WanderingGhost.Waypoint> waypoints = new List<WanderingGhost.Waypoint>();

	// Token: 0x04000EFD RID: 3837
	private WanderingGhost.Waypoint currentWaypoint;

	// Token: 0x04000EFE RID: 3838
	public string debugForceWaypointRegion;

	// Token: 0x04000EFF RID: 3839
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000F00 RID: 3840
	public AudioClip[] appearAudio;

	// Token: 0x04000F01 RID: 3841
	public float idleVolume;

	// Token: 0x04000F02 RID: 3842
	public AudioClip patrolAudio;

	// Token: 0x04000F03 RID: 3843
	public float patrolVolume;

	// Token: 0x04000F04 RID: 3844
	private readonly string defaultLayerMask = "Default";

	// Token: 0x04000F05 RID: 3845
	private readonly string handLayermask = "Gorilla Hand";

	// Token: 0x04000F06 RID: 3846
	private readonly string bodyLayerMask = "Gorilla Body Collider";

	// Token: 0x04000F07 RID: 3847
	private WanderingGhost.ghostState currentState;

	// Token: 0x04000F08 RID: 3848
	private float idlePassedTime;

	// Token: 0x04000F09 RID: 3849
	private bool idlSoundPlayed;

	// Token: 0x04000F0A RID: 3850
	public UnityAction<GameObject> TriggerHauntedObjects;

	// Token: 0x04000F0B RID: 3851
	private int handLayerMaskId;

	// Token: 0x04000F0C RID: 3852
	private int bodyLayerMaskId;

	// Token: 0x04000F0D RID: 3853
	private Vector3 hoverVelocity;

	// Token: 0x04000F0E RID: 3854
	public float hoverRectifyForce;

	// Token: 0x04000F0F RID: 3855
	public float hoverRandomForce;

	// Token: 0x04000F10 RID: 3856
	public float hoverDrag;

	// Token: 0x04000F11 RID: 3857
	private const int maxColliders = 10;

	// Token: 0x04000F12 RID: 3858
	private Collider[] hitColliders = new Collider[10];

	// Token: 0x02000455 RID: 1109
	[Serializable]
	public struct Waypoint
	{
		// Token: 0x06001CFD RID: 7421 RVA: 0x00099F4B File Offset: 0x0009814B
		public Waypoint(bool visible, Transform tr)
		{
			this._visible = visible;
			this._transform = tr;
		}

		// Token: 0x04001DED RID: 7661
		[Tooltip("The ghost will be visible when its reached to this waypoint")]
		public bool _visible;

		// Token: 0x04001DEE RID: 7662
		public Transform _transform;
	}

	// Token: 0x02000456 RID: 1110
	private enum ghostState
	{
		// Token: 0x04001DF0 RID: 7664
		patrol,
		// Token: 0x04001DF1 RID: 7665
		idle
	}
}
