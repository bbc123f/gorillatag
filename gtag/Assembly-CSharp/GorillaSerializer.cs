using System;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
internal class GorillaSerializer : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
{
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.successfullInstantiate || info.Sender != info.photonView.Owner || this.serializeTarget == null)
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
			GameObject gameObject = this.targetObject;
			IGorillaSerializeable gorillaSerializeable = ((gameObject != null) ? gameObject.GetComponent(this.targetType) : null) as IGorillaSerializeable;
			if (gorillaSerializeable != null)
			{
				this.serializeTarget = gorillaSerializeable;
			}
			if (this.targetType == null || this.targetObject == null || this.serializeTarget == null)
			{
				this.successfullInstantiate = false;
			}
		}
		if (this.successfullInstantiate)
		{
			this.OnSuccessfullInstantiate(info);
			return;
		}
		if (this.photonView.IsMine)
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

	protected bool successfullInstantiate;

	private IGorillaSerializeable serializeTarget;

	private Type targetType;

	protected GameObject targetObject;

	[SerializeField]
	protected PhotonView photonView;
}
