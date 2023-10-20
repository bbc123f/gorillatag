﻿using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020000E1 RID: 225
public class LegacyWorldShareableItem : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IOnPhotonViewOwnerChange, IPhotonViewCallback, IPunObservable
{
	// Token: 0x1700003D RID: 61
	// (get) Token: 0x0600052E RID: 1326 RVA: 0x00021113 File Offset: 0x0001F313
	public LegacyWorldTargetItem Target
	{
		get
		{
			return this.target;
		}
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x0002111B File Offset: 0x0001F31B
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000530 RID: 1328 RVA: 0x00021129 File Offset: 0x0001F329
	public override void OnEnable()
	{
		base.OnEnable();
		this.target.Invalidate();
	}

	// Token: 0x06000531 RID: 1329 RVA: 0x0002113C File Offset: 0x0001F33C
	public override void OnDisable()
	{
		base.OnDisable();
		this.target.Invalidate();
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x0002114F File Offset: 0x0001F34F
	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		this.rpcCallBack = null;
		this.onOwnerChangeCb = null;
		this.target.Invalidate();
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x00021170 File Offset: 0x0001F370
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		object[] instantiationData = info.photonView.InstantiationData;
		if (instantiationData != null && instantiationData.Length >= 2)
		{
			this.target.itemIdx = (int)instantiationData[0];
			this.target.owner = (Player)instantiationData[1];
		}
		this.SyncToTarget();
		if (!this.validShareable && info.Sender != null)
		{
			GorillaNot.instance.SendReport("invalid world shareable", info.Sender.UserId, info.Sender.NickName);
		}
		PhotonView.Get(this).AddCallbackTarget(this);
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x00021201 File Offset: 0x0001F401
	void IOnPhotonViewOwnerChange.OnOwnerChange(Player newOwner, Player previousOwner)
	{
		if (this.onOwnerChangeCb != null)
		{
			this.onOwnerChangeCb(newOwner, previousOwner);
		}
	}

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x06000535 RID: 1333 RVA: 0x00021218 File Offset: 0x0001F418
	// (set) Token: 0x06000536 RID: 1334 RVA: 0x00021220 File Offset: 0x0001F420
	public bool EnableRemoteSync
	{
		get
		{
			return this.enableRemoteSync;
		}
		set
		{
			this.enableRemoteSync = value;
		}
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x0002122C File Offset: 0x0001F42C
	private void Update()
	{
		if (!this.target.IsValid())
		{
			return;
		}
		if (this.view.IsMine)
		{
			base.transform.position = this.targetXf.transform.position;
			base.transform.rotation = this.targetXf.transform.rotation;
			return;
		}
		if (this.targetXf != null && this.EnableRemoteSync)
		{
			this.targetXf.position = base.transform.position;
			this.targetXf.rotation = base.transform.rotation;
		}
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x000212D0 File Offset: 0x0001F4D0
	private bool ReadTargetFromStream(PhotonStream stream)
	{
		this.prevTarget.owner = this.target.owner;
		this.prevTarget.itemIdx = this.target.itemIdx;
		this.target.owner = (Player)stream.ReceiveNext();
		this.target.itemIdx = (int)stream.ReceiveNext();
		return this.target.owner == this.prevTarget.owner && this.target.itemIdx == this.prevTarget.itemIdx;
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x00021368 File Offset: 0x0001F568
	private void SyncToTarget()
	{
		VRRig vrrig;
		if (this.target.owner == PhotonNetwork.LocalPlayer)
		{
			vrrig = GorillaTagger.Instance.offlineVRRig;
		}
		else
		{
			PhotonView photonView = GorillaGameManager.instance.FindVRRigForPlayer(this.target.owner);
			if (photonView == null)
			{
				this.target.Invalidate();
				return;
			}
			vrrig = photonView.gameObject.GetComponent<VRRig>();
		}
		if (vrrig == null)
		{
			this.target.Invalidate();
			return;
		}
		BodyDockPositions myBodyDockPositions = vrrig.myBodyDockPositions;
		LegacyTransferrableObject legacyTransferrableObject = null;
		this.targetXf = legacyTransferrableObject.gameObject.GetComponent<Transform>();
		this.validShareable = (legacyTransferrableObject.canDrop || legacyTransferrableObject.shareable);
		if (!this.validShareable)
		{
			base.gameObject.SetActive(false);
			return;
		}
		legacyTransferrableObject.SetWorldShareableItem(base.gameObject);
	}

	// Token: 0x0600053A RID: 1338 RVA: 0x00021434 File Offset: 0x0001F634
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x00021436 File Offset: 0x0001F636
	[PunRPC]
	private void RPCWorldShareable(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RPCWorldShareable");
		this.rpcCallBack();
	}

	// Token: 0x04000621 RID: 1569
	private bool validShareable = true;

	// Token: 0x04000622 RID: 1570
	private PhotonView view;

	// Token: 0x04000623 RID: 1571
	private Transform targetXf;

	// Token: 0x04000624 RID: 1572
	private LegacyWorldTargetItem prevTarget = new LegacyWorldTargetItem();

	// Token: 0x04000625 RID: 1573
	private LegacyWorldTargetItem target = new LegacyWorldTargetItem();

	// Token: 0x04000626 RID: 1574
	public LegacyWorldShareableItem.Delegate rpcCallBack;

	// Token: 0x04000627 RID: 1575
	public LegacyWorldShareableItem.OnOwnerChangeDelegate onOwnerChangeCb;

	// Token: 0x04000628 RID: 1576
	private bool enableRemoteSync = true;

	// Token: 0x020003E9 RID: 1001
	// (Invoke) Token: 0x06001BC8 RID: 7112
	public delegate void Delegate();

	// Token: 0x020003EA RID: 1002
	// (Invoke) Token: 0x06001BCC RID: 7116
	public delegate void OnOwnerChangeDelegate(Player newOwner, Player prevOwner);
}