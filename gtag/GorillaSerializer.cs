using System;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
internal class GorillaSerializer : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
{
	protected bool successfullInstantiate;

	private IGorillaSerializeable serializeTarget;

	private Type targetType;

	protected GameObject targetObject;

	[SerializeField]
	protected PhotonView photonView;

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (successfullInstantiate && info.Sender == info.photonView.Owner && serializeTarget != null)
		{
			if (stream.IsReading)
			{
				serializeTarget.OnSerializeRead(stream, info);
			}
			else
			{
				serializeTarget.OnSerializeWrite(stream, info);
			}
		}
	}

	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (photonView == null)
		{
			return;
		}
		successfullInstantiate = OnInstantiateSetup(info, out targetObject, out targetType);
		if (successfullInstantiate)
		{
			if (targetObject?.GetComponent(targetType) is IGorillaSerializeable gorillaSerializeable)
			{
				serializeTarget = gorillaSerializeable;
			}
			if (targetType == null || targetObject == null || serializeTarget == null)
			{
				successfullInstantiate = false;
			}
		}
		if (successfullInstantiate)
		{
			OnSuccessfullInstantiate(info);
			return;
		}
		Debug.Log("failed to network instantiate");
		if (photonView.IsMine)
		{
			PhotonNetwork.Destroy(photonView);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
		photonView.ObservedComponents.Remove(this);
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
}
