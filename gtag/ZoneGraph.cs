using System;
using System.Collections.Generic;
using UnityEngine;

public class ZoneGraph : MonoBehaviour
{
	public static ZoneGraph Instance()
	{
		return ZoneGraph.gZoneGraph;
	}

	public static void NotifyZoneEnter(ZoneEntity entity, int colliderID)
	{
		ZoneGraph.gZoneGraph.NotifyZoneEnter_Internal(entity, colliderID);
	}

	public static void NotifyZoneExit(ZoneEntity entity, int colliderID)
	{
		ZoneGraph.gZoneGraph.NotifyZoneExit_Internal(entity, colliderID);
	}

	private void NotifyZoneEnter_Internal(ZoneEntity entity, int colliderID)
	{
		ZoneGraph.Node node;
		if (this.ColliderToNode(colliderID, out node) > -1)
		{
			GorillaTelemetry.PostZoneEvent(node.zone, node.subZone, GTZoneEventType.zone_enter, true);
		}
	}

	private void NotifyZoneExit_Internal(ZoneEntity entity, int colliderID)
	{
		ZoneGraph.Node node;
		if (this.ColliderToNode(colliderID, out node) > -1)
		{
			GorillaTelemetry.PostZoneEvent(node.zone, node.subZone, GTZoneEventType.zone_exit, true);
		}
	}

	private int ColliderToNode(int colliderID, out ZoneGraph.Node node)
	{
		node = ZoneGraph.Node.Null;
		for (int i = 0; i < this._colliders.Length; i++)
		{
			if (this._colliders[i].GetInstanceID() == colliderID)
			{
				node = this._nodes[i];
				return i;
			}
		}
		return -1;
	}

	private void Awake()
	{
		if (ZoneGraph.gZoneGraph != null && ZoneGraph.gZoneGraph != this)
		{
			Object.Destroy(this);
		}
		else
		{
			ZoneGraph.gZoneGraph = this;
		}
		for (int i = 0; i < this._colliders.Length; i++)
		{
			BoxCollider boxCollider = this._colliders[i];
			this._instIdToCollider.Add(boxCollider.GetInstanceID(), boxCollider);
		}
	}

	public static void Register<T>(T entity) where T : ZoneEntity
	{
		T t = entity;
		int num = ((t != null) ? t.entityID : 0);
		if (num == 0)
		{
			return;
		}
		SortedList<int, ZoneEntity> entityList = ZoneGraph.gZoneGraph._entityList;
		if (!entityList.ContainsKey(num))
		{
			entityList.Add(num, entity);
		}
	}

	public static void Unregister<T>(T entity) where T : ZoneEntity
	{
		T t = entity;
		int num = ((t != null) ? t.entityID : 0);
		if (num == 0)
		{
			return;
		}
		ZoneGraph.gZoneGraph._entityList.Remove(num);
	}

	[SerializeField]
	private LayerMask _layerMask;

	private static ZoneGraph gZoneGraph;

	[SerializeField]
	private ZoneGraph.Node[] _nodes = new ZoneGraph.Node[0];

	[SerializeField]
	private BoxCollider[] _colliders = new BoxCollider[0];

	private SortedList<int, ZoneEntity> _entityList = new SortedList<int, ZoneEntity>(16);

	[Space]
	[SerializeField]
	private ZoneDef[] _zoneDefs = new ZoneDef[0];

	[DebugReadOnly]
	[NonSerialized]
	private Dictionary<int, BoxCollider> _instIdToCollider = new Dictionary<int, BoxCollider>(32);

	[Serializable]
	public struct Node
	{
		public GTZone zone;

		public GTSubZone subZone;

		public Vector3 center;

		public Vector3 size;

		public Quaternion rotation;

		public Matrix4x4 TRS;

		public int colliderID;

		public static readonly ZoneGraph.Node Null = new ZoneGraph.Node
		{
			zone = GTZone.none,
			subZone = GTSubZone.none,
			center = Vector3.zero,
			size = Vector3.zero,
			rotation = Quaternion.identity,
			TRS = Matrix4x4.zero,
			colliderID = -1
		};
	}
}
