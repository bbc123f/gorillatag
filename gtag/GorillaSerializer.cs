﻿using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
internal class GorillaSerializer : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
{
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.successfullInstantiate || this.serializeTarget == null || !this.ValidOnSerialize(stream, info))
		{
			return;
		}
		if (stream.IsReading)
		{
			this.serializeTarget.OnSerializeRead(stream, info);
			return;
		}
		this.serializeTarget.OnSerializeWrite(stream, info);
	}

	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (this.photonView == null)
		{
			return;
		}
		this.successfullInstantiate = this.OnInstantiateSetup(info, out this.targetObject, out this.targetType);
		if (this.successfullInstantiate)
		{
			if (this.targetType != null && this.targetObject.IsNotNull())
			{
				IGorillaSerializeable gorillaSerializeable = this.targetObject.GetComponent(this.targetType) as IGorillaSerializeable;
				if (gorillaSerializeable != null)
				{
					this.serializeTarget = gorillaSerializeable;
				}
			}
			if (this.serializeTarget == null)
			{
				this.successfullInstantiate = false;
			}
		}
		if (this.successfullInstantiate)
		{
			this.OnSuccessfullInstantiate(info);
			return;
		}
		if (PhotonNetwork.InRoom && this.photonView.IsMine)
		{
			PhotonNetwork.Destroy(this.photonView);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		this.photonView.ObservedComponents.Remove(this);
	}

	protected virtual void OnSuccessfullInstantiate(PhotonMessageInfo info)
	{
	}

	protected virtual bool OnInstantiateSetup(PhotonMessageInfo info, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetType = typeof(IGorillaSerializeable);
		outTargetObject = base.gameObject;
		return true;
	}

	protected virtual bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == info.photonView.Owner;
	}

	public virtual T AddRPCComponent<T>() where T : RPCNetworkBase
	{
		T t = base.gameObject.AddComponent<T>();
		this.photonView.RefreshRpcMonoBehaviourCache();
		t.SetClassTarget(this.serializeTarget, this);
		return t;
	}

	protected bool successfullInstantiate;

	protected IGorillaSerializeable serializeTarget;

	private Type targetType;

	protected GameObject targetObject;

	[SerializeField]
	protected PhotonView photonView;
}
