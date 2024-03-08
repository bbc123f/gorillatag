using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class ZoneEntity : MonoBehaviour, IComparer<ZoneEntity>
{
	public virtual int entityID
	{
		get
		{
			return base.GetInstanceID();
		}
	}

	private void OnValidate()
	{
		if (this.collider == null)
		{
			this.collider = base.GetComponent<SphereCollider>();
		}
		if (this._rigidbody == null)
		{
			this._rigidbody = base.GetComponent<Rigidbody>();
			this._rigidbody.isKinematic = true;
			this._rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		}
	}

	protected virtual void OnEnable()
	{
		ZoneGraph.Register<ZoneEntity>(this);
	}

	protected virtual void OnDisable()
	{
		ZoneGraph.Unregister<ZoneEntity>(this);
	}

	private void OnTriggerEnter(Collider other)
	{
		ZoneGraph.NotifyZoneEnter(this, other.GetInstanceID());
	}

	private void OnTriggerExit(Collider other)
	{
		ZoneGraph.NotifyZoneExit(this, other.GetInstanceID());
	}

	public int Compare(ZoneEntity x, ZoneEntity y)
	{
		return ZoneEntity.Compare<ZoneEntity>(x, y);
	}

	public static int Compare<T>(T x, T y) where T : ZoneEntity
	{
		if (x == null && y == null)
		{
			return 0;
		}
		if (x == null)
		{
			return 1;
		}
		if (y == null)
		{
			return -1;
		}
		return x.entityID.CompareTo(y.entityID);
	}

	public SphereCollider collider;

	[SerializeField]
	private Rigidbody _rigidbody;
}
