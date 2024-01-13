using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LegacyBalloonHoldable : LegacyTransferrableObject
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

	private BalloonStates balloonState;

	protected override void Awake()
	{
		base.Awake();
		balloonDynamics = GetComponent<BalloonDynamics>();
		mesh = GetComponent<MeshRenderer>();
		lineRenderer = GetComponent<LineRenderer>();
		itemState = (TransferrableObject.ItemStates)0;
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
		if (InHand())
		{
			Grab();
		}
		else if (Dropped())
		{
			Release();
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

	protected override void OnWorldShareableItemSpawn()
	{
		LegacyWorldShareableItem component = worldShareableInstance.GetComponent<LegacyWorldShareableItem>();
		if (component != null)
		{
			component.rpcCallBack = PopBalloonRemote;
			component.onOwnerChangeCb = OnOwnerChangeCb;
			component.EnableRemoteSync = ShouldSimulate();
		}
		originalOwner = component.Target.owner;
	}

	protected override void OnWorldShareableItemDeallocated(Player player)
	{
		if (player == originalOwner || player == PhotonNetwork.LocalPlayer)
		{
			if (originalOwner != PhotonNetwork.LocalPlayer)
			{
				PlayPopBalloonFX();
				if ((bool)balloonDynamics)
				{
					balloonDynamics.ReParent();
				}
				base.transform.parent = DefaultAnchor();
				Object.Destroy(base.gameObject);
			}
			else
			{
				OwnerPopBalloon();
			}
		}
		if (player != PhotonNetwork.LocalPlayer && PhotonNetwork.InRoom && originalOwner == PhotonNetwork.LocalPlayer && worldShareableInstance != null)
		{
			PhotonView.Get(worldShareableInstance.gameObject).TransferOwnership(PhotonNetwork.LocalPlayer);
		}
	}

	private void PlayPopBalloonFX()
	{
		GameObject obj = ObjectPools.instance.Instantiate(balloonPopFXPrefab);
		obj.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
		GorillaColorizableBase componentInChildren = obj.GetComponentInChildren<GorillaColorizableBase>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(balloonPopFXColor);
		}
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
		if ((originalOwner == PhotonNetwork.LocalPlayer || !PhotonNetwork.InRoom) && PhotonNetwork.InRoom && worldShareableInstance != null)
		{
			PhotonView photonView = PhotonView.Get(worldShareableInstance.gameObject);
			if (photonView != null && !photonView.IsMine)
			{
				photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
			}
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
		balloonState = BalloonStates.Pop;
	}

	public void OnOwnerChangeCb(Player newOwner, Player prevOwner)
	{
		if (newOwner != PhotonNetwork.LocalPlayer)
		{
			EnableDynamics(enable: false, forceKinematicOn: true);
			return;
		}
		if (ShouldSimulate() && (bool)balloonDynamics)
		{
			balloonDynamics.EnableDynamics(enable: true, kinematic: false);
		}
		rb.AddForceAtPosition(forceAppliedAsRemote, collisionPtAsRemote, ForceMode.VelocityChange);
		forceAppliedAsRemote = Vector3.zero;
		collisionPtAsRemote = Vector3.zero;
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
			else if (OnShoulder() && (bool)balloonDynamics && balloonDynamics.enabled)
			{
				EnableDynamics(enable: false);
			}
		}
		if (worldShareableInstance != null)
		{
			LegacyWorldShareableItem component = worldShareableInstance.GetComponent<LegacyWorldShareableItem>();
			if (component != null)
			{
				PhotonView photonView = PhotonView.Get(component);
				if (photonView != null && !photonView.IsMine)
				{
					component.EnableRemoteSync = ShouldSimulate();
				}
			}
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
		balloonBopSource.Play();
		if (!PhotonNetwork.InRoom || worldShareableInstance == null)
		{
			return;
		}
		PhotonView photonView = PhotonView.Get(worldShareableInstance.gameObject);
		if (!photonView.IsMine && flag)
		{
			if (force.magnitude > forceAppliedAsRemote.magnitude)
			{
				forceAppliedAsRemote = force;
				collisionPtAsRemote = position;
			}
			photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
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
}
