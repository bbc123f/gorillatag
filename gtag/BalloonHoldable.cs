using GorillaLocomotion.Swimming;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class BalloonHoldable : TransferrableObject, IFXContext
{
	private enum BalloonStates
	{
		Normal,
		Pop,
		Waiting,
		WaitForOwnershipTransfer,
		WaitForReDock,
		Refilling
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
	private BalloonStates balloonState;

	public float lastOwnershipRequest;

	FXSystemSettings IFXContext.settings => ownerRig.fxSettings;

	protected override void Awake()
	{
		base.Awake();
		balloonDynamics = GetComponent<BalloonDynamics>();
		mesh = GetComponent<MeshRenderer>();
		lineRenderer = GetComponent<LineRenderer>();
		itemState = (ItemStates)0;
		rb = GetComponent<Rigidbody>();
	}

	protected override void Start()
	{
		base.Start();
		EnableDynamics(enable: false);
	}

	public override void OnEnable()
	{
		base.OnEnable();
		EnableDynamics(enable: false);
		mesh.enabled = true;
		lineRenderer.enabled = false;
		if (PhotonNetwork.InRoom)
		{
			if (worldShareableInstance != null)
			{
				return;
			}
			SpawnTransferableObjectViews();
		}
		if (InHand())
		{
			Grab();
		}
		else if (Dropped())
		{
			Release();
		}
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (!(worldShareableInstance != null))
		{
			SpawnTransferableObjectViews();
		}
	}

	private bool ShouldSimulate()
	{
		if (Dropped() || InHand())
		{
			return balloonState == BalloonStates.Normal;
		}
		return false;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		lineRenderer.enabled = false;
		EnableDynamics(enable: false);
	}

	public override void PreDisable()
	{
		originalOwner = null;
		base.PreDisable();
	}

	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		balloonState = BalloonStates.Normal;
		base.transform.localScale = Vector3.one;
	}

	protected override void OnWorldShareableItemSpawn()
	{
		WorldShareableItem worldShareableItem = worldShareableInstance;
		if (worldShareableItem != null)
		{
			worldShareableItem.rpcCallBack = PopBalloonRemote;
			worldShareableItem.onOwnerChangeCb = OnOwnerChangeCb;
			worldShareableItem.EnableRemoteSync = ShouldSimulate();
		}
		originalOwner = worldShareableItem.Target.owner;
	}

	public override void ResetToHome()
	{
		if (IsLocalObject() && worldShareableInstance != null && !worldShareableInstance.guard.isTrulyMine)
		{
			PhotonView photonView = PhotonView.Get(worldShareableInstance);
			if (photonView != null)
			{
				photonView.RPC("RPCWorldShareable", RpcTarget.Others);
			}
			worldShareableInstance.guard.RequestOwnershipImmediatelyWithGuaranteedAuthority();
		}
		PopBalloon();
		balloonState = BalloonStates.WaitForReDock;
		base.ResetToHome();
	}

	protected override void PlayDestroyedOrDisabledEffect()
	{
		base.PlayDestroyedOrDisabledEffect();
		PlayPopBalloonFX();
	}

	protected override void OnItemDestroyedOrDisabled()
	{
		PlayPopBalloonFX();
		if ((bool)balloonDynamics)
		{
			balloonDynamics.ReParent();
		}
		base.transform.parent = DefaultAnchor();
		base.OnItemDestroyedOrDisabled();
	}

	private void PlayPopBalloonFX()
	{
		FXSystem.PlayFXForRig(FXType.BalloonPop, this);
	}

	private void EnableDynamics(bool enable, bool forceKinematicOn = false)
	{
		bool kinematic = false;
		if (forceKinematicOn)
		{
			kinematic = true;
		}
		else if (PhotonNetwork.InRoom && worldShareableInstance != null)
		{
			PhotonView photonView = PhotonView.Get(worldShareableInstance.gameObject);
			if (photonView != null && !photonView.IsMine)
			{
				kinematic = true;
			}
		}
		if ((bool)balloonDynamics)
		{
			balloonDynamics.EnableDynamics(enable, kinematic);
		}
	}

	private void PopBalloon()
	{
		PlayPopBalloonFX();
		EnableDynamics(enable: false);
		mesh.enabled = false;
		lineRenderer.enabled = false;
		if (gripInteractor != null)
		{
			gripInteractor.gameObject.SetActive(value: false);
		}
		if ((object.Equals(originalOwner, PhotonNetwork.LocalPlayer) || !PhotonNetwork.InRoom) && PhotonNetwork.InRoom && worldShareableInstance != null && !worldShareableInstance.guard.isTrulyMine)
		{
			worldShareableInstance.guard.RequestOwnershipImmediatelyWithGuaranteedAuthority();
		}
		if (balloonDynamics != null)
		{
			balloonDynamics.ReParent();
			EnableDynamics(enable: false);
		}
		if (IsMyItem())
		{
			if (InLeftHand())
			{
				EquipmentInteractor.instance.ReleaseLeftHand();
			}
			if (InRightHand())
			{
				EquipmentInteractor.instance.ReleaseRightHand();
			}
		}
	}

	public void PopBalloonRemote()
	{
		if (ShouldSimulate())
		{
			balloonState = BalloonStates.Pop;
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
			EnableDynamics(enable: false, forceKinematicOn: true);
			return;
		}
		if (ShouldSimulate() && (bool)balloonDynamics)
		{
			balloonDynamics.EnableDynamics(enable: true, kinematic: false);
		}
		if ((bool)rb)
		{
			rb.AddForceAtPosition(forceAppliedAsRemote, collisionPtAsRemote, ForceMode.VelocityChange);
			forceAppliedAsRemote = Vector3.zero;
			collisionPtAsRemote = Vector3.zero;
		}
	}

	private void OwnerPopBalloon()
	{
		if (worldShareableInstance != null)
		{
			PhotonView photonView = PhotonView.Get(worldShareableInstance);
			if (photonView != null)
			{
				photonView.RPC("RPCWorldShareable", RpcTarget.Others);
			}
		}
		balloonState = BalloonStates.Pop;
	}

	private void RunLocalPopSM()
	{
		switch (balloonState)
		{
		case BalloonStates.Pop:
			timer = Time.time;
			PopBalloon();
			balloonState = BalloonStates.WaitForOwnershipTransfer;
			lastOwnershipRequest = Time.time;
			break;
		case BalloonStates.WaitForOwnershipTransfer:
			if (!PhotonNetwork.InRoom)
			{
				balloonState = BalloonStates.WaitForReDock;
				ReDock();
			}
			else if (worldShareableInstance != null)
			{
				PhotonView photonView = PhotonView.Get(worldShareableInstance.gameObject);
				if (photonView != null && photonView.Owner == originalOwner)
				{
					balloonState = BalloonStates.WaitForReDock;
					ReDock();
				}
				if (IsLocalObject() && lastOwnershipRequest + 5f < Time.time)
				{
					worldShareableInstance.guard.RequestOwnershipImmediatelyWithGuaranteedAuthority();
					lastOwnershipRequest = Time.time;
				}
			}
			break;
		case BalloonStates.WaitForReDock:
			if (Attached())
			{
				ReDock();
				balloonState = BalloonStates.Waiting;
			}
			break;
		case BalloonStates.Waiting:
			if (Time.time - timer >= poppedTimerLength)
			{
				timer = Time.time;
				mesh.enabled = true;
				localScale = new Vector3(beginScale, beginScale, beginScale);
				base.transform.localScale = localScale;
				balloonInflatSource.Play();
				balloonState = BalloonStates.Refilling;
			}
			break;
		case BalloonStates.Refilling:
		{
			float num = Time.time - timer;
			if (num >= scaleTimerLength)
			{
				balloonState = BalloonStates.Normal;
				if (gripInteractor != null)
				{
					gripInteractor.gameObject.SetActive(value: true);
				}
			}
			num = Mathf.Clamp01(num / scaleTimerLength);
			float num2 = Mathf.Lerp(beginScale, 1f, num);
			localScale = new Vector3(num2, num2, num2);
			base.transform.localScale = localScale;
			break;
		}
		case BalloonStates.Normal:
			break;
		}
	}

	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (previousState != currentState || Time.frameCount == enabledOnFrame)
		{
			if (InHand())
			{
				Grab();
			}
			else if (Dropped())
			{
				Release();
			}
			else if (OnShoulder())
			{
				if ((bool)balloonDynamics && balloonDynamics.enabled)
				{
					EnableDynamics(enable: false);
				}
				lineRenderer.enabled = false;
			}
		}
		if ((object)worldShareableInstance != null && !worldShareableInstance.guard.isMine)
		{
			worldShareableInstance.EnableRemoteSync = ShouldSimulate();
		}
		if (balloonState != 0)
		{
			RunLocalPopSM();
		}
	}

	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
	}

	private void Grab()
	{
		if (!(balloonDynamics == null))
		{
			EnableDynamics(enable: true);
			balloonDynamics.EnableDistanceConstraints(enable: true);
			lineRenderer.enabled = true;
		}
	}

	private void Release()
	{
		if (!(balloonDynamics == null))
		{
			EnableDynamics(enable: true);
			balloonDynamics.EnableDistanceConstraints(enable: false);
			lineRenderer.enabled = false;
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (!ShouldSimulate())
		{
			return;
		}
		bool flag = other.gameObject.layer == LayerMask.NameToLayer("Gorilla Hand");
		Vector3 force = Vector3.zero;
		Vector3 position = Vector3.zero;
		if (flag)
		{
			TransformFollow component = other.gameObject.GetComponent<TransformFollow>();
			if ((bool)component)
			{
				Vector3 vector = (component.transform.position - component.prevPos) / Time.deltaTime;
				if ((bool)rb)
				{
					force = vector * bopSpeed;
					force = Mathf.Min(balloonDynamics.maximumVelocity, force.magnitude) * force.normalized;
					position = other.ClosestPointOnBounds(base.transform.position);
					rb.AddForceAtPosition(force, position, ForceMode.VelocityChange);
					balloonBopSource.Play();
					GorillaTriggerColliderHandIndicator component2 = other.GetComponent<GorillaTriggerColliderHandIndicator>();
					if (component2 != null)
					{
						float amplitude = GorillaTagger.Instance.tapHapticStrength / 4f;
						float fixedDeltaTime = Time.fixedDeltaTime;
						GorillaTagger.Instance.StartVibration(component2.isLeftHand, amplitude, fixedDeltaTime);
					}
				}
			}
		}
		if (!PhotonNetwork.InRoom || worldShareableInstance == null)
		{
			return;
		}
		RequestableOwnershipGuard component3 = PhotonView.Get(worldShareableInstance.gameObject).GetComponent<RequestableOwnershipGuard>();
		if (!component3.isTrulyMine && flag)
		{
			if (force.magnitude > forceAppliedAsRemote.magnitude)
			{
				forceAppliedAsRemote = force;
				collisionPtAsRemote = position;
			}
			component3.RequestOwnershipImmediately(delegate
			{
			});
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (!ShouldSimulate())
		{
			return;
		}
		balloonBopSource.Play();
		if (collision.gameObject.layer == LayerMask.NameToLayer("GorillaThrowable"))
		{
			if (!PhotonNetwork.InRoom)
			{
				OwnerPopBalloon();
			}
			else if (!(worldShareableInstance == null) && PhotonView.Get(worldShareableInstance.gameObject).IsMine)
			{
				OwnerPopBalloon();
			}
		}
	}

	void IFXContext.OnPlayFX()
	{
		GameObject obj = ObjectPools.instance.Instantiate(balloonPopFXPrefab);
		obj.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
		GorillaColorizableBase componentInChildren = obj.GetComponentInChildren<GorillaColorizableBase>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(balloonPopFXColor);
		}
	}
}
