using System;
using System.Runtime.CompilerServices;
using Fusion;
using Photon.Pun;
using UnityEngine;

[NetworkBehaviourWeaved(0)]
internal abstract class GorillaWrappedSerializer<T> : NetworkBehaviour, IPunObservable, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy, IPhotonViewCallback where T : struct, INetworkStruct
{
	protected virtual T data
	{
		[CompilerGenerated]
		get
		{
			return this.<data>k__BackingField;
		}
		[CompilerGenerated]
		set
		{
			this.<data>k__BackingField = value;
		}
	}

	public bool IsLocallyOwned
	{
		get
		{
			return this.photonView.IsMine;
		}
	}

	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (this.photonView == null)
		{
			return;
		}
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		this.ProcessSpawn(photonMessageInfoWrapped);
	}

	public override void Spawned()
	{
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(this.Object.StateAuthority.PlayerId, this.Runner.Tick.Raw);
		this.ProcessSpawn(photonMessageInfoWrapped);
	}

	private void ProcessSpawn(PhotonMessageInfoWrapped wrappedInfo)
	{
		Debug.Log("Processing spawn");
		this.successfullInstantiate = this.OnSpawnSetupCheck(wrappedInfo, out this.targetObject, out this.targetType);
		Debug.Log("successfullInstantiate" + this.successfullInstantiate.ToString());
		if (this.successfullInstantiate)
		{
			GameObject gameObject = this.targetObject;
			IWrappedSerializable wrappedSerializable = ((gameObject != null) ? gameObject.GetComponent(this.targetType) : null) as IWrappedSerializable;
			if (wrappedSerializable != null)
			{
				this.serializeTarget = wrappedSerializable;
				string text = "target set!";
				IWrappedSerializable wrappedSerializable2 = wrappedSerializable;
				Debug.Log(text + ((wrappedSerializable2 != null) ? wrappedSerializable2.ToString() : null));
			}
			if (this.targetType == null || this.targetObject == null || this.serializeTarget == null)
			{
				Debug.Log("here");
				this.successfullInstantiate = false;
			}
		}
		if (this.successfullInstantiate)
		{
			this.OnSuccesfullySpawned(wrappedInfo);
			return;
		}
		this.FailedToSpawn();
	}

	protected virtual bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetType = typeof(IWrappedSerializable);
		outTargetObject = base.gameObject;
		return true;
	}

	protected abstract void OnSuccesfullySpawned(PhotonMessageInfoWrapped info);

	private void FailedToSpawn()
	{
		Debug.LogError("Failed to network instantiate");
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

	protected abstract void OnFailedSpawn();

	public override void FixedUpdateNetwork()
	{
		if (this.Object.HasStateAuthority)
		{
			this.data = (T)((object)this.serializeTarget.OnSerializeWrite());
			return;
		}
		this.serializeTarget.OnSerializeRead(this.data);
	}

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.successfullInstantiate || info.Sender != info.photonView.Owner || this.serializeTarget == null)
		{
			return;
		}
		if (stream.IsWriting)
		{
			this.serializeTarget.OnSerializeWrite(stream, info);
			return;
		}
		this.serializeTarget.OnSerializeRead(stream, info);
	}

	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		this.OnBeforeDespawn();
	}

	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		this.OnBeforeDespawn();
	}

	protected abstract void OnBeforeDespawn();

	protected GorillaWrappedSerializer()
	{
	}

	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	public override void CopyStateToBackingFields()
	{
	}

	protected bool successfullInstantiate;

	private IWrappedSerializable serializeTarget;

	private Type targetType;

	protected GameObject targetObject;

	[SerializeField]
	protected PhotonView photonView;

	[SerializeField]
	protected NetworkObject networkObject;

	[CompilerGenerated]
	private T <data>k__BackingField;

	static Changed<GorillaWrappedSerializer> $IL2CPP_CHANGED;

	static ChangedDelegate<GorillaWrappedSerializer> $IL2CPP_CHANGED_DELEGATE;

	static NetworkBehaviourCallbacks<GorillaWrappedSerializer> $IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;
}
