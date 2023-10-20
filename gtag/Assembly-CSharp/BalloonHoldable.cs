﻿using System;
using GorillaLocomotion.Swimming;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020000D7 RID: 215
public class BalloonHoldable : TransferrableObject, IFXContext
{
	// Token: 0x060004CF RID: 1231 RVA: 0x0001E8A4 File Offset: 0x0001CAA4
	protected override void Awake()
	{
		base.Awake();
		this.balloonDynamics = base.GetComponent<BalloonDynamics>();
		this.mesh = base.GetComponent<MeshRenderer>();
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.itemState = (TransferrableObject.ItemStates)0;
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x0001E8E3 File Offset: 0x0001CAE3
	protected override void Start()
	{
		base.Start();
		this.EnableDynamics(false, false);
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x0001E8F4 File Offset: 0x0001CAF4
	public override void OnEnable()
	{
		base.OnEnable();
		this.EnableDynamics(false, false);
		this.mesh.enabled = true;
		this.lineRenderer.enabled = false;
		if (PhotonNetwork.InRoom)
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

	// Token: 0x060004D2 RID: 1234 RVA: 0x0001E960 File Offset: 0x0001CB60
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (this.worldShareableInstance != null)
		{
			return;
		}
		base.SpawnTransferableObjectViews();
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x0001E97D File Offset: 0x0001CB7D
	private bool ShouldSimulate()
	{
		return (base.Dropped() || base.InHand()) && this.balloonState == BalloonHoldable.BalloonStates.Normal;
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x0001E99A File Offset: 0x0001CB9A
	public override void OnDisable()
	{
		base.OnDisable();
		this.lineRenderer.enabled = false;
		this.EnableDynamics(false, false);
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x0001E9B6 File Offset: 0x0001CBB6
	public override void PreDisable()
	{
		this.originalOwner = null;
		base.PreDisable();
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x0001E9C5 File Offset: 0x0001CBC5
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.balloonState = BalloonHoldable.BalloonStates.Normal;
		base.transform.localScale = Vector3.one;
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x0001E9E4 File Offset: 0x0001CBE4
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

	// Token: 0x060004D8 RID: 1240 RVA: 0x0001EA44 File Offset: 0x0001CC44
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

	// Token: 0x060004D9 RID: 1241 RVA: 0x0001EAC2 File Offset: 0x0001CCC2
	protected override void PlayDestroyedOrDisabledEffect()
	{
		base.PlayDestroyedOrDisabledEffect();
		this.PlayPopBalloonFX();
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x0001EAD0 File Offset: 0x0001CCD0
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

	// Token: 0x060004DB RID: 1243 RVA: 0x0001EB08 File Offset: 0x0001CD08
	private void PlayPopBalloonFX()
	{
		FXSystem.PlayFXForRig(FXType.BalloonPop, this, default(PhotonMessageInfo));
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x0001EB28 File Offset: 0x0001CD28
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

	// Token: 0x060004DD RID: 1245 RVA: 0x0001EB94 File Offset: 0x0001CD94
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
		if ((object.Equals(this.originalOwner, PhotonNetwork.LocalPlayer) || !PhotonNetwork.InRoom) && PhotonNetwork.InRoom && this.worldShareableInstance != null && !this.worldShareableInstance.guard.isTrulyMine)
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

	// Token: 0x060004DE RID: 1246 RVA: 0x0001EC87 File Offset: 0x0001CE87
	public void PopBalloonRemote()
	{
		if (this.ShouldSimulate())
		{
			this.balloonState = BalloonHoldable.BalloonStates.Pop;
		}
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x0001EC98 File Offset: 0x0001CE98
	public void OnOwnerChangeCb(Player newOwner, Player prevOwner)
	{
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x0001EC9C File Offset: 0x0001CE9C
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

	// Token: 0x060004E1 RID: 1249 RVA: 0x0001ED38 File Offset: 0x0001CF38
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

	// Token: 0x060004E2 RID: 1250 RVA: 0x0001ED80 File Offset: 0x0001CF80
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
			if (!PhotonNetwork.InRoom)
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

	// Token: 0x060004E3 RID: 1251 RVA: 0x0001EF74 File Offset: 0x0001D174
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
		if (this.worldShareableInstance != null && !this.worldShareableInstance.guard.isMine)
		{
			this.worldShareableInstance.EnableRemoteSync = this.ShouldSimulate();
		}
		if (this.balloonState != BalloonHoldable.BalloonStates.Normal)
		{
			this.RunLocalPopSM();
		}
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x0001F031 File Offset: 0x0001D231
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x0001F039 File Offset: 0x0001D239
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

	// Token: 0x060004E6 RID: 1254 RVA: 0x0001F06A File Offset: 0x0001D26A
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

	// Token: 0x060004E7 RID: 1255 RVA: 0x0001F09C File Offset: 0x0001D29C
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
					this.balloonBopSource.Play();
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
		if (!PhotonNetwork.InRoom)
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
			if (force.magnitude > this.forceAppliedAsRemote.magnitude)
			{
				this.forceAppliedAsRemote = force;
				this.collisionPtAsRemote = position;
			}
			component3.RequestOwnershipImmediately(delegate
			{
			});
		}
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x0001F248 File Offset: 0x0001D448
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

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x060004E9 RID: 1257 RVA: 0x0001F2BD File Offset: 0x0001D4BD
	FXSystemSettings IFXContext.settings
	{
		get
		{
			return this.ownerRig.fxSettings;
		}
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x0001F2CC File Offset: 0x0001D4CC
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

	// Token: 0x0400059A RID: 1434
	private BalloonDynamics balloonDynamics;

	// Token: 0x0400059B RID: 1435
	private MeshRenderer mesh;

	// Token: 0x0400059C RID: 1436
	private LineRenderer lineRenderer;

	// Token: 0x0400059D RID: 1437
	private Rigidbody rb;

	// Token: 0x0400059E RID: 1438
	private Player originalOwner;

	// Token: 0x0400059F RID: 1439
	public GameObject balloonPopFXPrefab;

	// Token: 0x040005A0 RID: 1440
	public Color balloonPopFXColor;

	// Token: 0x040005A1 RID: 1441
	private float timer;

	// Token: 0x040005A2 RID: 1442
	public float scaleTimerLength = 2f;

	// Token: 0x040005A3 RID: 1443
	public float poppedTimerLength = 2.5f;

	// Token: 0x040005A4 RID: 1444
	public float beginScale = 0.1f;

	// Token: 0x040005A5 RID: 1445
	public float bopSpeed = 1f;

	// Token: 0x040005A6 RID: 1446
	private Vector3 localScale;

	// Token: 0x040005A7 RID: 1447
	public AudioSource balloonBopSource;

	// Token: 0x040005A8 RID: 1448
	public AudioSource balloonInflatSource;

	// Token: 0x040005A9 RID: 1449
	private Vector3 forceAppliedAsRemote;

	// Token: 0x040005AA RID: 1450
	private Vector3 collisionPtAsRemote;

	// Token: 0x040005AB RID: 1451
	private WaterVolume waterVolume;

	// Token: 0x040005AC RID: 1452
	[DebugReadout]
	private BalloonHoldable.BalloonStates balloonState;

	// Token: 0x040005AD RID: 1453
	public float lastOwnershipRequest;

	// Token: 0x020003E5 RID: 997
	private enum BalloonStates
	{
		// Token: 0x04001C62 RID: 7266
		Normal,
		// Token: 0x04001C63 RID: 7267
		Pop,
		// Token: 0x04001C64 RID: 7268
		Waiting,
		// Token: 0x04001C65 RID: 7269
		WaitForOwnershipTransfer,
		// Token: 0x04001C66 RID: 7270
		WaitForReDock,
		// Token: 0x04001C67 RID: 7271
		Refilling
	}
}