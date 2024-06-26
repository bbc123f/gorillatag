﻿using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class FlockingManager : MonoBehaviourPunCallbacks, IPunObservable
{
	private void Awake()
	{
		foreach (GameObject gameObject in this.fishAreaContainer)
		{
			Flocking[] componentsInChildren = gameObject.GetComponentsInChildren<Flocking>(false);
			FlockingManager.FishArea fishArea = new FlockingManager.FishArea();
			fishArea.id = gameObject.name;
			fishArea.colliders = gameObject.GetComponentsInChildren<BoxCollider>();
			fishArea.colliderCenter = fishArea.colliders[0].bounds.center;
			fishArea.fishList.AddRange(componentsInChildren);
			fishArea.zoneBasedObject = gameObject.GetComponent<ZoneBasedObject>();
			this.areaToWaypointDict[fishArea.id] = Vector3.zero;
			Flocking[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].FishArea = fishArea;
			}
			this.fishAreaList.Add(fishArea);
			this.allFish.AddRange(fishArea.fishList);
			SlingshotProjectileHitNotifier component = gameObject.GetComponent<SlingshotProjectileHitNotifier>();
			if (component != null)
			{
				component.OnProjectileTriggerEnter += this.ProjectileHitReceiver;
				component.OnProjectileTriggerExit += this.ProjectileHitExit;
			}
			else
			{
				Debug.LogError("Needs SlingshotProjectileHitNotifier added to each fish area");
			}
		}
	}

	private void OnDestroy()
	{
		this.fishAreaList.Clear();
		this.areaToWaypointDict.Clear();
		this.allFish.Clear();
		foreach (GameObject gameObject in this.fishAreaContainer)
		{
			SlingshotProjectileHitNotifier component = gameObject.GetComponent<SlingshotProjectileHitNotifier>();
			if (component != null)
			{
				component.OnProjectileTriggerExit -= this.ProjectileHitExit;
				component.OnProjectileTriggerEnter -= this.ProjectileHitReceiver;
			}
		}
	}

	private void Update()
	{
		if (Random.Range(0, 10000) < 50)
		{
			foreach (FlockingManager.FishArea fishArea in this.fishAreaList)
			{
				if (fishArea.zoneBasedObject != null)
				{
					fishArea.zoneBasedObject.gameObject.SetActive(fishArea.zoneBasedObject.IsLocalPlayerInZone());
				}
				fishArea.nextWaypoint = this.GetRandomPointInsideCollider(fishArea);
				this.areaToWaypointDict[fishArea.id] = fishArea.nextWaypoint;
				Debug.DrawLine(fishArea.nextWaypoint, Vector3.forward * 5f, Color.magenta);
			}
		}
	}

	public Vector3 GetRandomPointInsideCollider(FlockingManager.FishArea fishArea)
	{
		int num = Random.Range(0, fishArea.colliders.Length);
		BoxCollider boxCollider = fishArea.colliders[num];
		Vector3 vector = boxCollider.size / 2f;
		Vector3 position = new Vector3(Random.Range(-vector.x, vector.x), Random.Range(-vector.y, vector.y), Random.Range(-vector.z, vector.z));
		return boxCollider.transform.TransformPoint(position);
	}

	public bool IsInside(Vector3 point, FlockingManager.FishArea fish)
	{
		foreach (BoxCollider boxCollider in fish.colliders)
		{
			Vector3 center = boxCollider.center;
			Vector3 vector = boxCollider.transform.InverseTransformPoint(point);
			vector -= center;
			Vector3 size = boxCollider.size;
			if (Mathf.Abs(vector.x) < size.x / 2f && Mathf.Abs(vector.y) < size.y / 2f && Mathf.Abs(vector.z) < size.z / 2f)
			{
				return true;
			}
		}
		return false;
	}

	public Vector3 RestrictPointToArea(Vector3 point, FlockingManager.FishArea fish)
	{
		Vector3 result = default(Vector3);
		float num = float.MaxValue;
		foreach (BoxCollider boxCollider in fish.colliders)
		{
			Vector3 center = boxCollider.center;
			Vector3 vector = boxCollider.transform.InverseTransformPoint(point);
			Vector3 vector2 = vector - center;
			Vector3 size = boxCollider.size;
			float num2 = size.x / 2f;
			float num3 = size.y / 2f;
			float num4 = size.z / 2f;
			if (Mathf.Abs(vector2.x) < num2 && Mathf.Abs(vector2.y) < num3 && Mathf.Abs(vector2.z) < num4)
			{
				return point;
			}
			Vector3 vector3 = new Vector3(center.x - num2, center.y - num3, center.z - num4);
			Vector3 vector4 = new Vector3(center.x + num2, center.y + num3, center.z + num4);
			Vector3 vector5 = new Vector3(Mathf.Clamp(vector.x, vector3.x, vector4.x), Mathf.Clamp(vector.y, vector3.y, vector4.y), Mathf.Clamp(vector.z, vector3.z, vector4.z));
			float num5 = Vector3.Distance(vector, vector5);
			if (num5 < num)
			{
				num = num5;
				if (num5 > 1f)
				{
					Vector3 a = Vector3.Normalize(vector - vector5);
					result = boxCollider.transform.TransformPoint(vector5 + a * 1f);
				}
				else
				{
					result = point;
				}
			}
		}
		return result;
	}

	private void ProjectileHitReceiver(SlingshotProjectile projectile, Collider collider)
	{
		bool arg = projectile.CompareTag(this.foodProjectileTag);
		UnityAction<SlingshotProjectile, bool> unityAction = this.onFoodDetected;
		if (unityAction == null)
		{
			return;
		}
		unityAction(projectile, arg);
	}

	private void ProjectileHitExit(SlingshotProjectile projectile, Collider collider1)
	{
		UnityAction unityAction = this.onFoodDestroyed;
		if (unityAction == null)
		{
			return;
		}
		unityAction();
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	public static void RegisterAvoidPoint(GameObject obj)
	{
		FlockingManager.avoidPoints.Add(obj);
	}

	public static void UnregisterAvoidPoint(GameObject obj)
	{
		FlockingManager.avoidPoints.Remove(obj);
	}

	public FlockingManager()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static FlockingManager()
	{
	}

	public List<GameObject> fishAreaContainer;

	public string foodProjectileTag = "WaterBalloonProjectile";

	private Dictionary<string, Vector3> areaToWaypointDict = new Dictionary<string, Vector3>();

	private List<FlockingManager.FishArea> fishAreaList = new List<FlockingManager.FishArea>();

	private List<Flocking> allFish = new List<Flocking>();

	public UnityAction<SlingshotProjectile, bool> onFoodDetected;

	public UnityAction onFoodDestroyed;

	private bool hasBeenSerialized;

	public static readonly List<GameObject> avoidPoints = new List<GameObject>();

	public class FishArea
	{
		public FishArea()
		{
		}

		public string id;

		public List<Flocking> fishList = new List<Flocking>();

		public Vector3 colliderCenter;

		public BoxCollider[] colliders;

		public Vector3 nextWaypoint = Vector3.zero;

		public ZoneBasedObject zoneBasedObject;
	}
}
