using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

[NetworkBehaviourWeaved(0)]
public abstract class NetworkComponent<T> : NetworkBehaviour, IPunObservable, IPunInstantiateMagicCallback where T : struct, INetworkStruct
{
	protected virtual T data { get; set; }

	protected abstract void DataChangeCallback(PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped));

	protected abstract void NetUpdate(float deltaTime);

	protected virtual void ResimNetUpdate(float deltaTime)
	{
	}

	public virtual void Update()
	{
		this.NetUpdate(Time.deltaTime);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	public virtual void OnSpawned()
	{
		Debug.Log("Spawned callback triggered!", base.gameObject);
	}

	public override void Spawned()
	{
		if (NetworkSystem.Instance.IsOnline && !this.Object.IsSceneObject)
		{
			this.OnSpawned();
		}
	}

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		this.OnSpawned();
	}

	public override void Render()
	{
		this.OnRender();
	}

	protected virtual void OnRender()
	{
	}

	public bool IsLocallyOwned
	{
		get
		{
			return NetworkSystem.Instance.IsObjectLocallyOwned(base.gameObject);
		}
	}

	public bool ShouldWriteObjectData
	{
		get
		{
			return NetworkSystem.Instance.ShouldWriteObjectData(base.gameObject);
		}
	}

	public bool ShouldUpdateobject
	{
		get
		{
			return NetworkSystem.Instance.ShouldUpdateObject(base.gameObject);
		}
	}

	public int OwnerID
	{
		get
		{
			return NetworkSystem.Instance.GetOwningPlayerID(base.gameObject);
		}
	}

	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	public override void CopyStateToBackingFields()
	{
	}

	static Changed<NetworkComponent> $IL2CPP_CHANGED;

	static ChangedDelegate<NetworkComponent> $IL2CPP_CHANGED_DELEGATE;

	static NetworkBehaviourCallbacks<NetworkComponent> $IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;
}
