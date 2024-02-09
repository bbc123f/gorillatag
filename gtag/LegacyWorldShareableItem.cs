using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LegacyWorldShareableItem : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IOnPhotonViewOwnerChange, IPhotonViewCallback, IPunObservable
{
	public LegacyWorldTargetItem Target
	{
		get
		{
			return this.target;
		}
	}

	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	public override void OnEnable()
	{
		base.OnEnable();
		this.target.Invalidate();
	}

	public override void OnDisable()
	{
		base.OnDisable();
		this.target.Invalidate();
	}

	public override void OnLeftRoom()
	{
		base.OnLeftRoom();
		this.rpcCallBack = null;
		this.onOwnerChangeCb = null;
		this.target.Invalidate();
	}

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

	void IOnPhotonViewOwnerChange.OnOwnerChange(Player newOwner, Player previousOwner)
	{
		if (this.onOwnerChangeCb != null)
		{
			this.onOwnerChangeCb(newOwner, previousOwner);
		}
	}

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

	private bool ReadTargetFromStream(PhotonStream stream)
	{
		this.prevTarget.owner = this.target.owner;
		this.prevTarget.itemIdx = this.target.itemIdx;
		this.target.owner = (Player)stream.ReceiveNext();
		this.target.itemIdx = (int)stream.ReceiveNext();
		return this.target.owner == this.prevTarget.owner && this.target.itemIdx == this.prevTarget.itemIdx;
	}

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
		this.validShareable = legacyTransferrableObject.canDrop || legacyTransferrableObject.shareable;
		if (!this.validShareable)
		{
			base.gameObject.SetActive(false);
			return;
		}
		legacyTransferrableObject.SetWorldShareableItem(base.gameObject);
	}

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	[PunRPC]
	private void RPCWorldShareable(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RPCWorldShareable");
		this.rpcCallBack();
	}

	private bool validShareable = true;

	private PhotonView view;

	private Transform targetXf;

	private LegacyWorldTargetItem prevTarget = new LegacyWorldTargetItem();

	private LegacyWorldTargetItem target = new LegacyWorldTargetItem();

	public LegacyWorldShareableItem.Delegate rpcCallBack;

	public LegacyWorldShareableItem.OnOwnerChangeDelegate onOwnerChangeCb;

	private bool enableRemoteSync = true;

	public delegate void Delegate();

	public delegate void OnOwnerChangeDelegate(Player newOwner, Player prevOwner);
}
