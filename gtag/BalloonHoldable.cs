using System;
using System.Runtime.CompilerServices;
using GorillaLocomotion.Swimming;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class BalloonHoldable : TransferrableObject, IFXContext
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
		if (NetworkSystem.Instance.InRoom)
		{
			if (this.worldShareableInstance != null)
			{
				return;
			}
			base.SpawnTransferableObjectViews();
		}
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

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (this.worldShareableInstance != null)
		{
			return;
		}
		base.SpawnTransferableObjectViews();
	}

	private bool ShouldSimulate()
	{
		return (base.Dropped() || base.InHand()) && this.balloonState == BalloonHoldable.BalloonStates.Normal;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		this.lineRenderer.enabled = false;
		this.EnableDynamics(false, false);
	}

	public override void PreDisable()
	{
		this.originalOwner = null;
		base.PreDisable();
	}

	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.balloonState = BalloonHoldable.BalloonStates.Normal;
		base.transform.localScale = Vector3.one;
	}

	protected override void OnWorldShareableItemSpawn()
	{
		WorldShareableItem worldShareableInstance = this.worldShareableInstance;
		if (worldShareableInstance != null)
		{
			worldShareableInstance.rpcCallBack = new Action(this.PopBalloonRemote);
			worldShareableInstance.onOwnerChangeCb = new WorldShareableItem.OnOwnerChangeDelegate(this.OnOwnerChangeCb);
			worldShareableInstance.EnableRemoteSync = this.ShouldSimulate();
		}
		this.originalOwner = worldShareableInstance.Target.owner;
	}

	public override void ResetToHome()
	{
		if (base.IsLocalObject() && this.worldShareableInstance != null && !this.worldShareableInstance.guard.isTrulyMine)
		{
			PhotonView photonView = PhotonView.Get(this.worldShareableInstance);
			if (photonView != null)
			{
				photonView.RPC("RPCWorldShareable", RpcTarget.Others, Array.Empty<object>());
			}
			this.worldShareableInstance.guard.RequestOwnershipImmediatelyWithGuaranteedAuthority();
		}
		this.PopBalloon();
		this.balloonState = BalloonHoldable.BalloonStates.WaitForReDock;
		base.ResetToHome();
	}

	protected override void PlayDestroyedOrDisabledEffect()
	{
		base.PlayDestroyedOrDisabledEffect();
		this.PlayPopBalloonFX();
	}

	protected override void OnItemDestroyedOrDisabled()
	{
		this.PlayPopBalloonFX();
		if (this.balloonDynamics)
		{
			this.balloonDynamics.ReParent();
		}
		base.transform.parent = base.DefaultAnchor();
		base.OnItemDestroyedOrDisabled();
	}

	private void PlayPopBalloonFX()
	{
		FXSystem.PlayFXForRig(FXType.BalloonPop, this, default(PhotonMessageInfo));
	}

	private void EnableDynamics(bool enable, bool forceKinematicOn = false)
	{
		bool flag = false;
		if (forceKinematicOn)
		{
			flag = true;
		}
		else if (NetworkSystem.Instance.InRoom && this.worldShareableInstance != null)
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
		if ((object.Equals(this.originalOwner, PhotonNetwork.LocalPlayer) || !NetworkSystem.Instance.InRoom) && NetworkSystem.Instance.InRoom && this.worldShareableInstance != null && !this.worldShareableInstance.guard.isTrulyMine)
		{
			this.worldShareableInstance.guard.RequestOwnershipImmediatelyWithGuaranteedAuthority();
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
		if (this.ShouldSimulate())
		{
			this.balloonState = BalloonHoldable.BalloonStates.Pop;
		}
	}

	public void OnOwnerChangeCb(Player newOwner, Player prevOwner)
	{
	}

	public override void OnOwnershipTransferred(Player newOwner, Player prevOwner)
	{
		base.OnOwnershipTransferred(newOwner, prevOwner);
		if (object.Equals(prevOwner, PhotonNetwork.LocalPlayer) && newOwner == null)
		{
			return;
		}
		if (!object.Equals(newOwner, PhotonNetwork.LocalPlayer))
		{
			this.EnableDynamics(false, true);
			return;
		}
		if (this.ShouldSimulate() && this.balloonDynamics)
		{
			this.balloonDynamics.EnableDynamics(true, false);
		}
		if (!this.rb)
		{
			return;
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
		this.balloonState = BalloonHoldable.BalloonStates.Pop;
	}

	private void RunLocalPopSM()
	{
		switch (this.balloonState)
		{
		case BalloonHoldable.BalloonStates.Normal:
			break;
		case BalloonHoldable.BalloonStates.Pop:
			this.timer = Time.time;
			this.PopBalloon();
			this.balloonState = BalloonHoldable.BalloonStates.WaitForOwnershipTransfer;
			this.lastOwnershipRequest = Time.time;
			return;
		case BalloonHoldable.BalloonStates.Waiting:
			if (Time.time - this.timer >= this.poppedTimerLength)
			{
				this.timer = Time.time;
				this.mesh.enabled = true;
				this.localScale = new Vector3(this.beginScale, this.beginScale, this.beginScale);
				base.transform.localScale = this.localScale;
				this.balloonInflatSource.Play();
				this.balloonState = BalloonHoldable.BalloonStates.Refilling;
				return;
			}
			break;
		case BalloonHoldable.BalloonStates.WaitForOwnershipTransfer:
			if (!NetworkSystem.Instance.InRoom)
			{
				this.balloonState = BalloonHoldable.BalloonStates.WaitForReDock;
				base.ReDock();
				return;
			}
			if (this.worldShareableInstance != null)
			{
				PhotonView photonView = PhotonView.Get(this.worldShareableInstance.gameObject);
				if (photonView != null && photonView.Owner == this.originalOwner)
				{
					this.balloonState = BalloonHoldable.BalloonStates.WaitForReDock;
					base.ReDock();
				}
				if (base.IsLocalObject() && this.lastOwnershipRequest + 5f < Time.time)
				{
					this.worldShareableInstance.guard.RequestOwnershipImmediatelyWithGuaranteedAuthority();
					this.lastOwnershipRequest = Time.time;
					return;
				}
			}
			break;
		case BalloonHoldable.BalloonStates.WaitForReDock:
			if (base.Attached())
			{
				base.ReDock();
				this.balloonState = BalloonHoldable.BalloonStates.Waiting;
				return;
			}
			break;
		case BalloonHoldable.BalloonStates.Refilling:
		{
			float num = Time.time - this.timer;
			if (num >= this.scaleTimerLength)
			{
				this.balloonState = BalloonHoldable.BalloonStates.Normal;
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
			else if (base.OnShoulder())
			{
				if (this.balloonDynamics && this.balloonDynamics.enabled)
				{
					this.EnableDynamics(false, false);
				}
				this.lineRenderer.enabled = false;
			}
		}
		if (base.InHand())
		{
			float num = ((this.targetRig != null) ? this.targetRig.transform.localScale.x : 1f);
			base.transform.localScale = Vector3.one * num;
		}
		if (this.worldShareableInstance != null && !this.worldShareableInstance.guard.isMine)
		{
			this.worldShareableInstance.EnableRemoteSync = this.ShouldSimulate();
		}
		if (this.balloonState != BalloonHoldable.BalloonStates.Normal)
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
		float num = ((this.targetRig != null) ? this.targetRig.transform.localScale.x : 1f);
		base.transform.localScale = Vector3.one * num;
		this.EnableDynamics(true, false);
		this.balloonDynamics.EnableDistanceConstraints(true, num);
		this.lineRenderer.enabled = true;
	}

	private void Release()
	{
		if (this.balloonDynamics == null)
		{
			return;
		}
		float num = ((this.targetRig != null) ? this.targetRig.transform.localScale.x : 1f);
		base.transform.localScale = Vector3.one * num;
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
					vector = Mathf.Min(this.balloonDynamics.maximumVelocity, vector.magnitude) * vector.normalized * this.balloonDynamics.balloonScale;
					vector2 = other.ClosestPointOnBounds(base.transform.position);
					this.rb.AddForceAtPosition(vector, vector2, ForceMode.VelocityChange);
					this.balloonBopSource.Play();
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
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (this.worldShareableInstance == null)
		{
			return;
		}
		RequestableOwnershipGuard component3 = PhotonView.Get(this.worldShareableInstance.gameObject).GetComponent<RequestableOwnershipGuard>();
		if (!component3.isTrulyMine && flag)
		{
			if (vector.magnitude > this.forceAppliedAsRemote.magnitude)
			{
				this.forceAppliedAsRemote = vector;
				this.collisionPtAsRemote = vector2;
			}
			component3.RequestOwnershipImmediately(delegate
			{
			});
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
		if (!NetworkSystem.Instance.InRoom)
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

	FXSystemSettings IFXContext.settings
	{
		get
		{
			return this.ownerRig.fxSettings;
		}
	}

	void IFXContext.OnPlayFX()
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(this.balloonPopFXPrefab);
		gameObject.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
		GorillaColorizableBase componentInChildren = gameObject.GetComponentInChildren<GorillaColorizableBase>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(this.balloonPopFXColor);
		}
	}

	public BalloonHoldable()
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

	private WaterVolume waterVolume;

	[DebugReadout]
	private BalloonHoldable.BalloonStates balloonState;

	public float lastOwnershipRequest;

	private enum BalloonStates
	{
		Normal,
		Pop,
		Waiting,
		WaitForOwnershipTransfer,
		WaitForReDock,
		Refilling
	}

	[CompilerGenerated]
	[Serializable]
	private sealed class <>c
	{
		// Note: this type is marked as 'beforefieldinit'.
		static <>c()
		{
		}

		public <>c()
		{
		}

		internal void <OnTriggerEnter>b__45_0()
		{
		}

		public static readonly BalloonHoldable.<>c <>9 = new BalloonHoldable.<>c();

		public static Action <>9__45_0;
	}
}
