using System;
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
		if (ZoneEntity.gLocalRig == null)
		{
			ZoneEntity.gLocalRig = VRRigCache.Instance.localRig.Rig;
		}
		return ZoneEntity.gLocalRig == this._entityRig;
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
		if (!Application.isPlaying)
		{
			return;
		}
		BoxCollider boxCollider = c as BoxCollider;
		if (boxCollider == null)
		{
			return;
		}
		ZoneDef zoneDef = ZoneGraph.ColliderToZoneDef(boxCollider);
		if (zoneDef.zoneId != this.lastEnteredNode.zoneId)
		{
			this.sinceZoneEntered = 0;
			this.lastEnteredNode = ZoneGraph.ColliderToNode(boxCollider);
		}
		this.currentZone = zoneDef.zoneId;
		this.currentSubZone = zoneDef.subZoneId;
		if (!this._emitTelemetry)
		{
			return;
		}
		if (!zoneDef.trackEnter)
		{
			return;
		}
		if (!this.IsLocal())
		{
			return;
		}
		GorillaTelemetry.PostZoneEvent(zoneDef.zoneId, zoneDef.subZoneId, GTZoneEventType.zone_enter);
	}

	protected virtual void OnTriggerExit(Collider c)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		BoxCollider boxCollider = c as BoxCollider;
		if (boxCollider == null)
		{
			return;
		}
		ZoneDef zoneDef = ZoneGraph.ColliderToZoneDef(boxCollider);
		this.lastExitedNode = ZoneGraph.ColliderToNode(boxCollider);
		if (!this._emitTelemetry)
		{
			return;
		}
		if (!zoneDef.trackExit)
		{
			return;
		}
		if (!this.IsLocal())
		{
			return;
		}
		GorillaTelemetry.PostZoneEvent(zoneDef.zoneId, zoneDef.subZoneId, GTZoneEventType.zone_exit);
	}

	protected virtual void OnTriggerStay(Collider c)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		BoxCollider boxCollider = c as BoxCollider;
		if (boxCollider == null)
		{
			return;
		}
		ZoneDef zoneDef = ZoneGraph.ColliderToZoneDef(boxCollider);
		if (this.sinceZoneEntered.secondsElapsedInt <= this._zoneStayEventInterval)
		{
			return;
		}
		this.sinceZoneEntered = 0;
		if (!this._emitTelemetry)
		{
			return;
		}
		if (!zoneDef.trackStay)
		{
			return;
		}
		if (!this.IsLocal())
		{
			return;
		}
		GorillaTelemetry.PostZoneEvent(zoneDef.zoneId, zoneDef.subZoneId, GTZoneEventType.zone_stay);
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

	[SerializeField]
	private bool _emitTelemetry = true;

	[SerializeField]
	private int _zoneStayEventInterval = 300;

	[SerializeField]
	private VRRig _entityRig;

	[DebugReadOnly]
	[NonSerialized]
	private int? _entityID;

	[SerializeField]
	private string _entityTag;

	[SerializeField]
	private SphereCollider _collider;

	[SerializeField]
	private Rigidbody _rigidbody;

	[Space]
	[DebugReadOnly]
	[NonSerialized]
	public GTZone currentZone;

	[DebugReadOnly]
	[NonSerialized]
	public GTSubZone currentSubZone;

	[Space]
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
}
