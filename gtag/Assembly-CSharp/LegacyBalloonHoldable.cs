using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020000DF RID: 223
public class LegacyBalloonHoldable : LegacyTransferrableObject
{
	// Token: 0x06000516 RID: 1302 RVA: 0x0002074B File Offset: 0x0001E94B
	protected override void Awake()
	{
		base.Awake();
		this.balloonDynamics = base.GetComponent<BalloonDynamics>();
		this.mesh = base.GetComponent<MeshRenderer>();
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.itemState = (TransferrableObject.ItemStates)0;
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x0002078A File Offset: 0x0001E98A
	protected override void Start()
	{
		base.Start();
		this.EnableDynamics(false, false);
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x0002079C File Offset: 0x0001E99C
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

	// Token: 0x06000519 RID: 1305 RVA: 0x000207EC File Offset: 0x0001E9EC
	private bool ShouldSimulate()
	{
		return (base.Dropped() || base.InHand()) && this.balloonState == LegacyBalloonHoldable.BalloonStates.Normal;
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x00020809 File Offset: 0x0001EA09
	public override void OnDisable()
	{
		base.OnDisable();
		this.lineRenderer.enabled = false;
		this.EnableDynamics(false, false);
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x00020828 File Offset: 0x0001EA28
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

	// Token: 0x0600051C RID: 1308 RVA: 0x0002088C File Offset: 0x0001EA8C
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

	// Token: 0x0600051D RID: 1309 RVA: 0x00020940 File Offset: 0x0001EB40
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

	// Token: 0x0600051E RID: 1310 RVA: 0x0002099C File Offset: 0x0001EB9C
	private void EnableDynamics(bool enable, bool forceKinematicOn = false)
	{
		bool kinematic = false;
		if (forceKinematicOn)
		{
			kinematic = true;
		}
		else if (PhotonNetwork.InRoom && this.worldShareableInstance != null)
		{
			PhotonView photonView = PhotonView.Get(this.worldShareableInstance.gameObject);
			if (photonView != null && !photonView.IsMine)
			{
				kinematic = true;
			}
		}
		if (this.balloonDynamics)
		{
			this.balloonDynamics.EnableDynamics(enable, kinematic);
		}
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x00020A08 File Offset: 0x0001EC08
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

	// Token: 0x06000520 RID: 1312 RVA: 0x00020B01 File Offset: 0x0001ED01
	public void PopBalloonRemote()
	{
		this.balloonState = LegacyBalloonHoldable.BalloonStates.Pop;
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x00020B0C File Offset: 0x0001ED0C
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

	// Token: 0x06000522 RID: 1314 RVA: 0x00020B7C File Offset: 0x0001ED7C
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

	// Token: 0x06000523 RID: 1315 RVA: 0x00020BC4 File Offset: 0x0001EDC4
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

	// Token: 0x06000524 RID: 1316 RVA: 0x00020D78 File Offset: 0x0001EF78
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

	// Token: 0x06000525 RID: 1317 RVA: 0x00020E45 File Offset: 0x0001F045
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x00020E4D File Offset: 0x0001F04D
	private void Grab()
	{
		if (this.balloonDynamics == null)
		{
			return;
		}
		this.EnableDynamics(true, false);
		this.balloonDynamics.EnableDistanceConstraints(true);
		this.lineRenderer.enabled = true;
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x00020E7E File Offset: 0x0001F07E
	private void Release()
	{
		if (this.balloonDynamics == null)
		{
			return;
		}
		this.EnableDynamics(true, false);
		this.balloonDynamics.EnableDistanceConstraints(false);
		this.lineRenderer.enabled = false;
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x00020EB0 File Offset: 0x0001F0B0
	public void OnTriggerEnter(Collider other)
	{
		if (!this.ShouldSimulate())
		{
			return;
		}
		bool flag = other.gameObject.layer == LayerMask.NameToLayer("Gorilla Hand");
		Vector3 force = Vector3.zero;
		Vector3 position = Vector3.zero;
		if (flag)
		{
			TransformFollow component = other.gameObject.GetComponent<TransformFollow>();
			if (component)
			{
				Vector3 a = (component.transform.position - component.prevPos) / Time.deltaTime;
				if (this.rb)
				{
					force = a * this.bopSpeed;
					force = Mathf.Min(this.balloonDynamics.maximumVelocity, force.magnitude) * force.normalized;
					position = other.ClosestPointOnBounds(base.transform.position);
					this.rb.AddForceAtPosition(force, position, ForceMode.VelocityChange);
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
				if (force.magnitude > this.forceAppliedAsRemote.magnitude)
				{
					this.forceAppliedAsRemote = force;
					this.collisionPtAsRemote = position;
				}
				photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
			}
			return;
		}
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x0002103C File Offset: 0x0001F23C
	public void OnCollisionEnter(Collision collision)
	{
		if (!this.ShouldSimulate())
		{
			return;
		}
		this.balloonBopSource.Play();
		if (collision.gameObject.layer != LayerMask.NameToLayer("GorillaThrowable"))
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

	// Token: 0x0400060D RID: 1549
	private BalloonDynamics balloonDynamics;

	// Token: 0x0400060E RID: 1550
	private MeshRenderer mesh;

	// Token: 0x0400060F RID: 1551
	private LineRenderer lineRenderer;

	// Token: 0x04000610 RID: 1552
	private Rigidbody rb;

	// Token: 0x04000611 RID: 1553
	private Player originalOwner;

	// Token: 0x04000612 RID: 1554
	public GameObject balloonPopFXPrefab;

	// Token: 0x04000613 RID: 1555
	public Color balloonPopFXColor;

	// Token: 0x04000614 RID: 1556
	private float timer;

	// Token: 0x04000615 RID: 1557
	public float scaleTimerLength = 2f;

	// Token: 0x04000616 RID: 1558
	public float poppedTimerLength = 2.5f;

	// Token: 0x04000617 RID: 1559
	public float beginScale = 0.1f;

	// Token: 0x04000618 RID: 1560
	public float bopSpeed = 1f;

	// Token: 0x04000619 RID: 1561
	private Vector3 localScale;

	// Token: 0x0400061A RID: 1562
	public AudioSource balloonBopSource;

	// Token: 0x0400061B RID: 1563
	public AudioSource balloonInflatSource;

	// Token: 0x0400061C RID: 1564
	private Vector3 forceAppliedAsRemote;

	// Token: 0x0400061D RID: 1565
	private Vector3 collisionPtAsRemote;

	// Token: 0x0400061E RID: 1566
	private LegacyBalloonHoldable.BalloonStates balloonState;

	// Token: 0x020003E8 RID: 1000
	private enum BalloonStates
	{
		// Token: 0x04001C6E RID: 7278
		Normal,
		// Token: 0x04001C6F RID: 7279
		Pop,
		// Token: 0x04001C70 RID: 7280
		Waiting,
		// Token: 0x04001C71 RID: 7281
		WaitForOwnershipTransfer,
		// Token: 0x04001C72 RID: 7282
		WaitForReDock,
		// Token: 0x04001C73 RID: 7283
		Refilling
	}
}
