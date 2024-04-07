using System;
using GorillaNetworking;
using UnityEngine;

public class ZoneEntity : MonoBehaviour
{
	public string entityTag
	{
		get
		{
			return this._entityTag;
		}
	}

	public int entityID
	{
		get
		{
			int num = this._entityID.GetValueOrDefault();
			if (this._entityID == null)
			{
				num = base.GetInstanceID();
				this._entityID = new int?(num);
			}
			return this._entityID.Value;
		}
	}

	public VRRig entityRig
	{
		get
		{
			return this._entityRig;
		}
	}

	public SphereCollider collider
	{
		get
		{
			return this._collider;
		}
	}

	protected bool IsLocal()
	{
		return ZoneEntity.gLocalRig == this._entityRig;
	}

	private void Awake()
	{
		this.ValidateCollider();
		this.ValidateRigidbody();
	}

	private void Validate()
	{
		base.gameObject.layer = LayerMask.NameToLayer("Zone");
		this.ValidateCollider();
		this.ValidateRigidbody();
	}

	private void ValidateCollider()
	{
		this._collider.isTrigger = true;
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Zone");
		this._collider.includeLayers = layerMask;
		this._collider.excludeLayers = ~layerMask;
	}

	private void ValidateRigidbody()
	{
		this._rigidbody.isKinematic = true;
		this._rigidbody.useGravity = false;
		this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		this._rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Zone");
		this._rigidbody.includeLayers = layerMask;
		this._rigidbody.excludeLayers = ~layerMask;
	}

	private void Start()
	{
		if (ZoneEntity.gLocalRig == null)
		{
			ZoneEntity.gLocalRig = GameObject.Find("Local Gorilla Player").GetComponentInChildren<VRRig>();
		}
	}

	protected virtual void OnEnable()
	{
		ZoneGraph.Register(this);
	}

	protected virtual void OnDisable()
	{
		ZoneGraph.Unregister(this);
	}

	protected virtual void OnTriggerEnter(Collider c)
	{
		this.OnZoneTrigger(GTZoneEventType.zone_enter, c);
	}

	protected virtual void OnTriggerExit(Collider c)
	{
		this.OnZoneTrigger(GTZoneEventType.zone_exit, c);
	}

	protected virtual void OnTriggerStay(Collider c)
	{
		if (!this.gLastStayPoll.HasElapsed(1f, true))
		{
			return;
		}
		this.OnZoneTrigger(GTZoneEventType.zone_stay, c);
	}

	protected virtual void OnZoneTrigger(GTZoneEventType zoneEvent, Collider c)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (!this.IsLocal())
		{
			return;
		}
		BoxCollider boxCollider = c as BoxCollider;
		if (boxCollider == null)
		{
			return;
		}
		ZoneDef zoneDef = ZoneGraph.ColliderToZoneDef(boxCollider);
		bool flag = false;
		switch (zoneEvent)
		{
		case GTZoneEventType.zone_enter:
			if (zoneDef.zoneId != this.lastEnteredNode.zoneId)
			{
				this.sinceZoneEntered = 0;
			}
			this.lastEnteredNode = ZoneGraph.ColliderToNode(boxCollider);
			this.currentZone = zoneDef.zoneId;
			this.currentSubZone = zoneDef.subZoneId;
			if (zoneDef.subZoneId == GTSubZone.store_register)
			{
				GorillaTelemetry.PostShopEvent(this._entityRig, GTShopEventType.register_visit, CosmeticsController.instance.currentCart);
			}
			flag = zoneDef.trackEnter;
			break;
		case GTZoneEventType.zone_exit:
			this.lastExitedNode = ZoneGraph.ColliderToNode(boxCollider);
			flag = zoneDef.trackExit;
			break;
		case GTZoneEventType.zone_stay:
		{
			bool flag2 = this.sinceZoneEntered.secondsElapsedInt >= this._zoneStayEventInterval;
			if (flag2)
			{
				this.sinceZoneEntered = 0;
			}
			flag = zoneDef.trackStay && flag2;
			break;
		}
		}
		if (!this._emitTelemetry)
		{
			return;
		}
		if (!flag)
		{
			return;
		}
		GorillaTelemetry.PostZoneEvent(zoneDef.zoneId, zoneDef.subZoneId, zoneEvent);
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

	public ZoneEntity()
	{
	}

	[Space]
	[DebugReadOnly]
	[NonSerialized]
	private int? _entityID;

	[SerializeField]
	private string _entityTag;

	[Space]
	[SerializeField]
	private bool _emitTelemetry = true;

	[SerializeField]
	private int _zoneStayEventInterval = 300;

	[Space]
	[SerializeField]
	private VRRig _entityRig;

	[SerializeField]
	private SphereCollider _collider;

	[SerializeField]
	private Rigidbody _rigidbody;

	[Space]
	[DebugReadOnly]
	[NonSerialized]
	public GTZone currentZone = GTZone.none;

	[DebugReadOnly]
	[NonSerialized]
	public GTSubZone currentSubZone;

	[Space]
	[DebugReadOnly]
	[NonSerialized]
	public ZoneNode currentNode = ZoneNode.Null;

	[DebugReadOnly]
	[NonSerialized]
	public ZoneNode lastEnteredNode = ZoneNode.Null;

	[DebugReadOnly]
	[NonSerialized]
	public ZoneNode lastExitedNode = ZoneNode.Null;

	[Space]
	[DebugReadOnly]
	[NonSerialized]
	private TimeSince sinceZoneEntered = 0;

	[DebugReadOnly]
	private static VRRig gLocalRig;

	private TimeSince gLastStayPoll = 0;
}
