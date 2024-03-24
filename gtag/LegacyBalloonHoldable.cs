using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LegacyBalloonHoldable : LegacyTransferrableObject
{
	protected override void Awake()
	{
		base.Awake();
		this.balloonDynamics = base.GetComponent<BalloonDynamics>();
		this.mesh = base.GetComponent<MeshRenderer>();
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.itemState = (TransferrableObject.ItemStates)0;
		this.rb = base.GetComponent<Rigidbody>();
	}

	protected override void Start()
	{
		base.Start();
		this.EnableDynamics(false, false);
	}

	public override void OnEnable()
	{
		base.OnEnable();
		this.EnableDynamics(false, false);
		this.mesh.enabled = true;
		this.lineRenderer.enabled = false;
		if (base.InHand())
		{
			this.Grab();
			return;
		}
		if (base.Dropped())
		{
			this.Release();
		}
	}

	private bool ShouldSimulate()
	{
		return (base.Dropped() || base.InHand()) && this.balloonState == LegacyBalloonHoldable.BalloonStates.Normal;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		this.lineRenderer.enabled = false;
		this.EnableDynamics(false, false);
	}

	protected override void OnWorldShareableItemSpawn()
	{
		LegacyWorldShareableItem component = this.worldShareableInstance.GetComponent<LegacyWorldShareableItem>();
		if (component != null)
		{
			component.rpcCallBack = new LegacyWorldShareableItem.Delegate(this.PopBalloonRemote);
			component.onOwnerChangeCb = new LegacyWorldShareableItem.OnOwnerChangeDelegate(this.OnOwnerChangeCb);
			component.EnableRemoteSync = this.ShouldSimulate();
		}
		this.originalOwner = component.Target.owner;
	}

	protected override void OnWorldShareableItemDeallocated(Player player)
	{
		if (player == this.originalOwner || player == PhotonNetwork.LocalPlayer)
		{
			if (this.originalOwner != PhotonNetwork.LocalPlayer)
			{
				this.PlayPopBalloonFX();
				if (this.balloonDynamics)
				{
					this.balloonDynamics.ReParent();
				}
				base.transform.parent = base.DefaultAnchor();
				Object.Destroy(base.gameObject);
			}
			else
			{
				this.OwnerPopBalloon();
			}
		}
		if (player != PhotonNetwork.LocalPlayer && PhotonNetwork.InRoom && this.originalOwner == PhotonNetwork.LocalPlayer && this.worldShareableInstance != null)
		{
			PhotonView.Get(this.worldShareableInstance.gameObject).TransferOwnership(PhotonNetwork.LocalPlayer);
		}
	}

	private void PlayPopBalloonFX()
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(this.balloonPopFXPrefab);
		gameObject.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
		GorillaColorizableBase componentInChildren = gameObject.GetComponentInChildren<GorillaColorizableBase>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(this.balloonPopFXColor);
		}
	}

	private void EnableDynamics(bool enable, bool forceKinematicOn = false)
	{
		bool flag = false;
		if (forceKinematicOn)
		{
			flag = true;
		}
		else if (PhotonNetwork.InRoom && this.worldShareableInstance != null)
		{
			PhotonView photonView = PhotonView.Get(this.worldShareableInstance.gameObject);
			if (photonView != null && !photonView.IsMine)
			{
				flag = true;
			}
		}
		if (this.balloonDynamics)
		{
			this.balloonDynamics.EnableDynamics(enable, flag);
		}
	}

	private void PopBalloon()
	{
		this.PlayPopBalloonFX();
		this.EnableDynamics(false, false);
		this.mesh.enabled = false;
		this.lineRenderer.enabled = false;
		if (this.gripInteractor != null)
		{
			this.gripInteractor.gameObject.SetActive(false);
		}
		if ((this.originalOwner == PhotonNetwork.LocalPlayer || !PhotonNetwork.InRoom) && PhotonNetwork.InRoom && this.worldShareableInstance != null)
		{
			PhotonView photonView = PhotonView.Get(this.worldShareableInstance.gameObject);
			if (photonView != null && !photonView.IsMine)
			{
				photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
			}
		}
		if (this.balloonDynamics != null)
		{
			this.balloonDynamics.ReParent();
			this.EnableDynamics(false, false);
		}
		if (this.IsMyItem())
		{
			if (base.InLeftHand())
			{
				EquipmentInteractor.instance.ReleaseLeftHand();
			}
			if (base.InRightHand())
			{
				EquipmentInteractor.instance.ReleaseRightHand();
			}
		}
	}

	public void PopBalloonRemote()
	{
		this.balloonState = LegacyBalloonHoldable.BalloonStates.Pop;
	}

	public void OnOwnerChangeCb(Player newOwner, Player prevOwner)
	{
		if (newOwner != PhotonNetwork.LocalPlayer)
		{
			this.EnableDynamics(false, true);
			return;
		}
		if (this.ShouldSimulate() && this.balloonDynamics)
		{
			this.balloonDynamics.EnableDynamics(true, false);
		}
		this.rb.AddForceAtPosition(this.forceAppliedAsRemote, this.collisionPtAsRemote, ForceMode.VelocityChange);
		this.forceAppliedAsRemote = Vector3.zero;
		this.collisionPtAsRemote = Vector3.zero;
	}

	private void OwnerPopBalloon()
	{
		if (this.worldShareableInstance != null)
		{
			PhotonView photonView = PhotonView.Get(this.worldShareableInstance);
			if (photonView != null)
			{
				photonView.RPC("RPCWorldShareable", RpcTarget.Others, Array.Empty<object>());
			}
		}
		this.balloonState = LegacyBalloonHoldable.BalloonStates.Pop;
	}

	private void RunLocalPopSM()
	{
		switch (this.balloonState)
		{
		case LegacyBalloonHoldable.BalloonStates.Normal:
			break;
		case LegacyBalloonHoldable.BalloonStates.Pop:
			this.timer = Time.time;
			this.PopBalloon();
			this.balloonState = LegacyBalloonHoldable.BalloonStates.WaitForOwnershipTransfer;
			return;
		case LegacyBalloonHoldable.BalloonStates.Waiting:
			if (Time.time - this.timer >= this.poppedTimerLength)
			{
				this.timer = Time.time;
				this.mesh.enabled = true;
				this.localScale = new Vector3(this.beginScale, this.beginScale, this.beginScale);
				base.transform.localScale = this.localScale;
				this.balloonInflatSource.Play();
				this.balloonState = LegacyBalloonHoldable.BalloonStates.Refilling;
				return;
			}
			break;
		case LegacyBalloonHoldable.BalloonStates.WaitForOwnershipTransfer:
			if (!PhotonNetwork.InRoom)
			{
				this.balloonState = LegacyBalloonHoldable.BalloonStates.WaitForReDock;
				base.ReDock();
				return;
			}
			if (this.worldShareableInstance != null)
			{
				PhotonView photonView = PhotonView.Get(this.worldShareableInstance.gameObject);
				if (photonView != null && photonView.Owner == this.originalOwner)
				{
					this.balloonState = LegacyBalloonHoldable.BalloonStates.WaitForReDock;
					base.ReDock();
					return;
				}
			}
			break;
		case LegacyBalloonHoldable.BalloonStates.WaitForReDock:
			if (base.Attached())
			{
				base.ReDock();
				this.balloonState = LegacyBalloonHoldable.BalloonStates.Waiting;
				return;
			}
			break;
		case LegacyBalloonHoldable.BalloonStates.Refilling:
		{
			float num = Time.time - this.timer;
			if (num >= this.scaleTimerLength)
			{
				this.balloonState = LegacyBalloonHoldable.BalloonStates.Normal;
				if (this.gripInteractor != null)
				{
					this.gripInteractor.gameObject.SetActive(true);
				}
			}
			num = Mathf.Clamp01(num / this.scaleTimerLength);
			float num2 = Mathf.Lerp(this.beginScale, 1f, num);
			this.localScale = new Vector3(num2, num2, num2);
			base.transform.localScale = this.localScale;
			break;
		}
		default:
			return;
		}
	}

	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.previousState != this.currentState || Time.frameCount == this.enabledOnFrame)
		{
			if (base.InHand())
			{
				this.Grab();
			}
			else if (base.Dropped())
			{
				this.Release();
			}
			else if (base.OnShoulder() && this.balloonDynamics && this.balloonDynamics.enabled)
			{
				this.EnableDynamics(false, false);
			}
		}
		if (this.worldShareableInstance != null)
		{
			LegacyWorldShareableItem component = this.worldShareableInstance.GetComponent<LegacyWorldShareableItem>();
			if (component != null)
			{
				PhotonView photonView = PhotonView.Get(component);
				if (photonView != null && !photonView.IsMine)
				{
					component.EnableRemoteSync = this.ShouldSimulate();
				}
			}
		}
		if (this.balloonState != LegacyBalloonHoldable.BalloonStates.Normal)
		{
			this.RunLocalPopSM();
		}
	}

	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
	}

	private void Grab()
	{
		if (this.balloonDynamics == null)
		{
			return;
		}
		this.EnableDynamics(true, false);
		this.balloonDynamics.EnableDistanceConstraints(true, 1f);
		this.lineRenderer.enabled = true;
	}

	private void Release()
	{
		if (this.balloonDynamics == null)
		{
			return;
		}
		this.EnableDynamics(true, false);
		this.balloonDynamics.EnableDistanceConstraints(false, 1f);
		this.lineRenderer.enabled = false;
	}

	public void OnTriggerEnter(Collider other)
	{
		if (!this.ShouldSimulate())
		{
			return;
		}
		bool flag = other.gameObject.IsOnLayer(UnityLayer.GorillaHand);
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		if (flag)
		{
			TransformFollow component = other.gameObject.GetComponent<TransformFollow>();
			if (component)
			{
				Vector3 vector3 = (component.transform.position - component.prevPos) / Time.deltaTime;
				if (this.rb)
				{
					vector = vector3 * this.bopSpeed;
					vector = Mathf.Min(this.balloonDynamics.maximumVelocity, vector.magnitude) * vector.normalized;
					vector2 = other.ClosestPointOnBounds(base.transform.position);
					this.rb.AddForceAtPosition(vector, vector2, ForceMode.VelocityChange);
					GorillaTriggerColliderHandIndicator component2 = other.GetComponent<GorillaTriggerColliderHandIndicator>();
					if (component2 != null)
					{
						float num = GorillaTagger.Instance.tapHapticStrength / 4f;
						float fixedDeltaTime = Time.fixedDeltaTime;
						GorillaTagger.Instance.StartVibration(component2.isLeftHand, num, fixedDeltaTime);
					}
				}
			}
		}
		this.balloonBopSource.Play();
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (this.worldShareableInstance == null)
		{
			return;
		}
		PhotonView photonView = PhotonView.Get(this.worldShareableInstance.gameObject);
		if (!photonView.IsMine)
		{
			if (flag)
			{
				if (vector.magnitude > this.forceAppliedAsRemote.magnitude)
				{
					this.forceAppliedAsRemote = vector;
					this.collisionPtAsRemote = vector2;
				}
				photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
			}
			return;
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (!this.ShouldSimulate())
		{
			return;
		}
		this.balloonBopSource.Play();
		if (!collision.gameObject.IsOnLayer(UnityLayer.GorillaThrowable))
		{
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			this.OwnerPopBalloon();
			return;
		}
		if (this.worldShareableInstance == null)
		{
			return;
		}
		if (PhotonView.Get(this.worldShareableInstance.gameObject).IsMine)
		{
			this.OwnerPopBalloon();
		}
	}

	public LegacyBalloonHoldable()
	{
	}

	private BalloonDynamics balloonDynamics;

	private MeshRenderer mesh;

	private LineRenderer lineRenderer;

	private Rigidbody rb;

	private Player originalOwner;

	public GameObject balloonPopFXPrefab;

	public Color balloonPopFXColor;

	private float timer;

	public float scaleTimerLength = 2f;

	public float poppedTimerLength = 2.5f;

	public float beginScale = 0.1f;

	public float bopSpeed = 1f;

	private Vector3 localScale;

	public AudioSource balloonBopSource;

	public AudioSource balloonInflatSource;

	private Vector3 forceAppliedAsRemote;

	private Vector3 collisionPtAsRemote;

	private LegacyBalloonHoldable.BalloonStates balloonState;

	private enum BalloonStates
	{
		Normal,
		Pop,
		Waiting,
		WaitForOwnershipTransfer,
		WaitForReDock,
		Refilling
	}
}
