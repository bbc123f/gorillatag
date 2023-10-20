using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200015C RID: 348
[RequireComponent(typeof(PhotonView))]
internal class GorillaSerializer : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
{
	// Token: 0x06000893 RID: 2195 RVA: 0x00034DA8 File Offset: 0x00032FA8
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

	// Token: 0x06000894 RID: 2196 RVA: 0x00034DFC File Offset: 0x00032FFC
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
		Debug.Log("failed to network instantiate");
		if (this.photonView.IsMine)
		{
			PhotonNetwork.Destroy(this.photonView);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
		this.photonView.ObservedComponents.Remove(this);
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x00034EDE File Offset: 0x000330DE
	protected virtual void OnSuccessfullInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x00034EE0 File Offset: 0x000330E0
	protected virtual bool OnInstantiateSetup(PhotonMessageInfo info, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetType = typeof(IGorillaSerializeable);
		outTargetObject = base.gameObject;
		return true;
	}

	// Token: 0x04000AC8 RID: 2760
	protected bool successfullInstantiate;

	// Token: 0x04000AC9 RID: 2761
	private IGorillaSerializeable serializeTarget;

	// Token: 0x04000ACA RID: 2762
	private Type targetType;

	// Token: 0x04000ACB RID: 2763
	protected GameObject targetObject;

	// Token: 0x04000ACC RID: 2764
	[SerializeField]
	protected PhotonView photonView;
}
