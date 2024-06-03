using System;
using System.Collections.Generic;
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
			int value = this._entityID.GetValueOrDefault();
			if (this._entityID == null)
			{
				value = base.GetInstanceID();
				this._entityID = new int?(value);
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

	public GroupJoinZone GroupZone
	{
		get
		{
			return (this.currentGroupZone & ~this.currentExcludeGroupZone) | this.previousGroupZone;
		}
	}

	protected virtual void OnEnable()
	{
		this.insideBoxes.Clear();
		ZoneGraph.Register(this);
	}

	protected virtual void OnDisable()
	{
		this.insideBoxes.Clear();
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
		if (Time.time >= this.groupZoneClearAtTimestamp)
		{
			this.previousGroupZone = (this.currentGroupZone & ~this.currentExcludeGroupZone);
			this.currentGroupZone = zoneDef.groupZone;
			this.currentExcludeGroupZone = zoneDef.excludeGroupZone;
			this.groupZoneClearAtTimestamp = Time.time + this.groupZoneClearInterval;
		}
		else
		{
			this.currentGroupZone |= zoneDef.groupZone;
			this.currentExcludeGroupZone |= zoneDef.excludeGroupZone;
		}
		if (!this.gLastStayPoll.HasElapsed(1f, true))
		{
			return;
		}
		this.OnZoneTrigger(GTZoneEventType.zone_stay, boxCollider);
	}

	protected virtual void OnZoneTrigger(GTZoneEventType zoneEvent, Collider c)
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
		ZoneDef zone = ZoneGraph.ColliderToZoneDef(boxCollider);
		this.OnZoneTrigger(zoneEvent, zone, boxCollider);
	}

	private void OnZoneTrigger(GTZoneEventType zoneEvent, ZoneDef zone, BoxCollider box)
	{
		bool flag = false;
		switch (zoneEvent)
		{
		case GTZoneEventType.zone_enter:
		{
			if (zone.zoneId != this.lastEnteredNode.zoneId)
			{
				this.sinceZoneEntered = 0;
			}
			this.lastEnteredNode = ZoneGraph.ColliderToNode(box);
			ZoneDef zoneDef = ZoneGraph.ColliderToZoneDef(box);
			this.insideBoxes.Add(box);
			if (zoneDef.priority > this.currentZonePriority)
			{
				this.currentZone = zone.zoneId;
				this.currentSubZone = zone.subZoneId;
				this.currentZonePriority = zoneDef.priority;
			}
			if (zone.subZoneId == GTSubZone.store_register)
			{
				GorillaTelemetry.PostShopEvent(this._entityRig, GTShopEventType.register_visit, CosmeticsController.instance.currentCart);
			}
			flag = zone.trackEnter;
			break;
		}
		case GTZoneEventType.zone_exit:
			this.lastExitedNode = ZoneGraph.ColliderToNode(box);
			this.insideBoxes.Remove(box);
			if (this.currentZone == this.lastExitedNode.zoneId)
			{
				int num = 0;
				ZoneDef zoneDef2 = null;
				foreach (BoxCollider collider in this.insideBoxes)
				{
					ZoneDef zoneDef3 = ZoneGraph.ColliderToZoneDef(collider);
					if (zoneDef3.priority > num)
					{
						zoneDef2 = zoneDef3;
						num = zoneDef3.priority;
					}
				}
				if (zoneDef2 != null)
				{
					this.currentZone = zoneDef2.zoneId;
					this.currentSubZone = zoneDef2.subZoneId;
					this.currentZonePriority = zoneDef2.priority;
				}
				else
				{
					this.currentZone = GTZone.none;
					this.currentSubZone = GTSubZone.none;
					this.currentZonePriority = 0;
				}
			}
			flag = zone.trackExit;
			break;
		case GTZoneEventType.zone_stay:
		{
			bool flag2 = this.sinceZoneEntered.secondsElapsedInt >= this._zoneStayEventInterval;
			if (flag2)
			{
				this.sinceZoneEntered = 0;
			}
			flag = (zone.trackStay && flag2);
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
		if (!this._entityRig.isOfflineVRRig)
		{
			return;
		}
		GorillaTelemetry.PostZoneEvent(zone.zoneId, zone.subZoneId, zoneEvent);
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

	[DebugReadOnly]
	[NonSerialized]
	private GroupJoinZone currentGroupZone;

	[DebugReadOnly]
	[NonSerialized]
	private GroupJoinZone previousGroupZone;

	[DebugReadOnly]
	[NonSerialized]
	private GroupJoinZone currentExcludeGroupZone;

	private HashSet<BoxCollider> insideBoxes = new HashSet<BoxCollider>();

	private int currentZonePriority;

	private float groupZoneClearAtTimestamp;

	private float groupZoneClearInterval = 0.1f;

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

	private TimeSince gLastStayPoll = 0;
}
