using System.Collections.Generic;
using CjLib;
using GorillaLocomotion.Climbing;
using UnityEngine;

namespace GorillaLocomotion.Swimming;

[RequireComponent(typeof(Collider))]
public class WaterVolume : MonoBehaviour
{
	public struct SurfaceQuery
	{
		public Vector3 surfacePoint;

		public Vector3 surfaceNormal;

		public float maxDepth;

		public Plane surfacePlane => new Plane(surfaceNormal, surfacePoint);
	}

	[SerializeField]
	public Transform surfacePlane;

	[SerializeField]
	private List<MeshCollider> surfaceColliders = new List<MeshCollider>();

	[SerializeField]
	public List<Collider> volumeColliders = new List<Collider>();

	[SerializeField]
	public bool allowBubblesInVolume = true;

	[SerializeField]
	private WaterCurrent waterCurrent;

	[SerializeField]
	private WaterParameters waterParams;

	public const string WaterSplashRPC = "PlaySplashEffect";

	public static float[] splashRPCSendTimes = new float[4];

	private static Dictionary<Collider, WaterVolume> sharedColliderRegistry = new Dictionary<Collider, WaterVolume>(16);

	private static Dictionary<Mesh, int[]> meshTrianglesDict = new Dictionary<Mesh, int[]>(16);

	private static Dictionary<Mesh, Vector3[]> meshVertsDict = new Dictionary<Mesh, Vector3[]>(16);

	private int[] sharedMeshTris;

	private Vector3[] sharedMeshVerts;

	private VRRig playerVRRig;

	private float volumeMaxHeight;

	private float volumeMinHeight;

	private bool debugDrawSurfaceCast;

	private Collider triggerCollider;

	private List<WaterOverlappingCollider> persistentColliders = new List<WaterOverlappingCollider>(16);

	public Player.LiquidType LiquidType => Player.LiquidType.Water;

	public WaterCurrent Current => waterCurrent;

	private VRRig PlayerVRRig
	{
		get
		{
			if (playerVRRig == null)
			{
				GorillaTagger instance = GorillaTagger.Instance;
				if (instance != null)
				{
					playerVRRig = instance.offlineVRRig;
				}
			}
			return playerVRRig;
		}
	}

	public bool GetSurfaceQueryForPoint(Vector3 point, out SurfaceQuery result)
	{
		result = default(SurfaceQuery);
		Ray ray = new Ray(new Vector3(point.x, volumeMaxHeight, point.z), Vector3.down);
		Ray ray2 = new Ray(new Vector3(point.x, volumeMinHeight, point.z), Vector3.up);
		float num = volumeMaxHeight - volumeMinHeight;
		float num2 = float.MinValue;
		float num3 = float.MaxValue;
		bool flag = false;
		bool flag2 = false;
		float num4 = 0f;
		for (int i = 0; i < surfaceColliders.Count; i++)
		{
			bool flag3 = surfaceColliders[i].enabled;
			surfaceColliders[i].enabled = true;
			if (surfaceColliders[i].Raycast(ray, out var hitInfo, num) && hitInfo.point.y > num2 && HitOutsideSurfaceOfMesh(ray.direction, surfaceColliders[i], hitInfo))
			{
				num2 = hitInfo.point.y;
				flag = true;
				result.surfacePoint = hitInfo.point;
				result.surfaceNormal = hitInfo.normal;
			}
			if (surfaceColliders[i].Raycast(ray2, out var hitInfo2, num) && hitInfo2.point.y < num3 && HitOutsideSurfaceOfMesh(ray2.direction, surfaceColliders[i], hitInfo2))
			{
				num3 = hitInfo2.point.y;
				flag2 = true;
				num4 = hitInfo2.point.y;
			}
			surfaceColliders[i].enabled = flag3;
		}
		if (!flag && surfacePlane != null)
		{
			flag = true;
			result.surfacePoint = point - Vector3.Dot(point - surfacePlane.position, surfacePlane.up) * surfacePlane.up;
			result.surfaceNormal = surfacePlane.up;
		}
		if (flag && flag2)
		{
			result.maxDepth = result.surfacePoint.y - num4;
		}
		else if (flag)
		{
			result.maxDepth = result.surfacePoint.y - volumeMinHeight;
		}
		else
		{
			result.maxDepth = volumeMaxHeight - volumeMinHeight;
		}
		if (debugDrawSurfaceCast)
		{
			if (flag)
			{
				DebugUtil.DrawLine(ray.origin, ray.origin + ray.direction * num, Color.green, depthTest: false);
				DebugUtil.DrawSphere(result.surfacePoint, 0.05f, 12, 12, Color.green, depthTest: false, DebugUtil.Style.SolidColor);
			}
			else
			{
				DebugUtil.DrawLine(ray.origin, ray.origin + ray.direction * num, Color.red, depthTest: false);
			}
			if (flag2)
			{
				DebugUtil.DrawLine(ray2.origin, ray2.origin + ray2.direction * num, Color.yellow, depthTest: false);
				DebugUtil.DrawSphere(new Vector3(result.surfacePoint.x, num4, result.surfacePoint.z), 0.05f, 12, 12, Color.yellow, depthTest: false, DebugUtil.Style.SolidColor);
			}
		}
		return flag;
	}

	private bool HitOutsideSurfaceOfMesh(Vector3 castDir, MeshCollider meshCollider, RaycastHit hit)
	{
		if (!meshTrianglesDict.TryGetValue(meshCollider.sharedMesh, out sharedMeshTris))
		{
			sharedMeshTris = (int[])meshCollider.sharedMesh.triangles.Clone();
			meshTrianglesDict.Add(meshCollider.sharedMesh, sharedMeshTris);
		}
		if (!meshVertsDict.TryGetValue(meshCollider.sharedMesh, out sharedMeshVerts))
		{
			sharedMeshVerts = (Vector3[])meshCollider.sharedMesh.vertices.Clone();
			meshVertsDict.Add(meshCollider.sharedMesh, sharedMeshVerts);
		}
		Vector3 vector = sharedMeshVerts[sharedMeshTris[hit.triangleIndex * 3]];
		Vector3 vector2 = sharedMeshVerts[sharedMeshTris[hit.triangleIndex * 3 + 1]];
		Vector3 vector3 = sharedMeshVerts[sharedMeshTris[hit.triangleIndex * 3 + 2]];
		Vector3 vector4 = meshCollider.transform.TransformDirection(Vector3.Cross(vector2 - vector, vector3 - vector).normalized);
		bool flag = Vector3.Dot(castDir, vector4) < 0f;
		if (debugDrawSurfaceCast)
		{
			Color color = (flag ? Color.blue : Color.red);
			DebugUtil.DrawLine(hit.point, hit.point + vector4 * 0.3f, color, depthTest: false);
		}
		return flag;
	}

	private void DebugDrawMeshColliderHitTriangle(RaycastHit hit)
	{
		MeshCollider meshCollider = hit.collider as MeshCollider;
		if (meshCollider != null)
		{
			Mesh sharedMesh = meshCollider.sharedMesh;
			int[] triangles = sharedMesh.triangles;
			Vector3[] vertices = sharedMesh.vertices;
			Vector3 vector = meshCollider.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3]]);
			Vector3 vector2 = meshCollider.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 1]]);
			Vector3 vector3 = meshCollider.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 2]]);
			Vector3 normalized = Vector3.Cross(vector2 - vector, vector3 - vector).normalized;
			float num = 0.2f;
			DebugUtil.DrawLine(vector, vector + normalized * num, Color.blue, depthTest: false);
			DebugUtil.DrawLine(vector2, vector2 + normalized * num, Color.blue, depthTest: false);
			DebugUtil.DrawLine(vector3, vector3 + normalized * num, Color.blue, depthTest: false);
			DebugUtil.DrawLine(vector, vector2, Color.blue, depthTest: false);
			DebugUtil.DrawLine(vector, vector3, Color.blue, depthTest: false);
			DebugUtil.DrawLine(vector2, vector3, Color.blue, depthTest: false);
		}
	}

	public bool RaycastWater(Vector3 origin, Vector3 direction, out RaycastHit hit, float distance, int layerMask)
	{
		if (triggerCollider != null)
		{
			return Physics.Raycast(new Ray(origin, direction), out hit, distance, layerMask, QueryTriggerInteraction.Collide);
		}
		hit = default(RaycastHit);
		return false;
	}

	public bool CheckColliderInVolume(Collider collider, out bool inWater, out bool surfaceDetected)
	{
		for (int i = 0; i < persistentColliders.Count; i++)
		{
			if (persistentColliders[i].collider == collider)
			{
				inWater = persistentColliders[i].inWater;
				surfaceDetected = persistentColliders[i].surfaceDetected;
				return true;
			}
		}
		inWater = false;
		surfaceDetected = false;
		return false;
	}

	private void Awake()
	{
		triggerCollider = GetComponent<Collider>();
		if (volumeColliders == null || volumeColliders.Count < 1)
		{
			volumeColliders = new List<Collider>();
			volumeColliders.Add(base.gameObject.GetComponent<Collider>());
		}
		float num = float.MinValue;
		float num2 = float.MaxValue;
		for (int i = 0; i < volumeColliders.Count; i++)
		{
			float y = volumeColliders[i].bounds.max.y;
			float y2 = volumeColliders[i].bounds.min.y;
			if (y > num)
			{
				num = y;
			}
			if (y2 < num2)
			{
				num2 = y2;
			}
		}
		volumeMaxHeight = num;
		volumeMinHeight = num2;
	}

	private void Update()
	{
		if (persistentColliders.Count < 1)
		{
			return;
		}
		float time = Time.time;
		RemoveCollidersOutsideVolume(time);
		for (int i = 0; i < persistentColliders.Count; i++)
		{
			WaterOverlappingCollider persistentCollider = persistentColliders[i];
			bool inWater = persistentCollider.inWater;
			if (persistentCollider.inVolume)
			{
				CheckColliderAgainstWater(ref persistentCollider, time);
			}
			else
			{
				persistentCollider.inWater = false;
			}
			TryRegisterOwnershipOfCollider(persistentCollider.collider, persistentCollider.inWater, persistentCollider.surfaceDetected);
			if (persistentCollider.inWater && !inWater)
			{
				OnWaterSurfaceEnter(ref persistentCollider);
			}
			else if (!persistentCollider.inWater && inWater)
			{
				OnWaterSurfaceExit(ref persistentCollider, time);
			}
			if (HasOwnershipOfCollider(persistentCollider.collider) && persistentCollider.surfaceDetected)
			{
				if (!persistentCollider.inWater)
				{
					ColliderOutOfWaterUpdate(ref persistentCollider, time);
				}
				else
				{
					ColliderInWaterUpdate(ref persistentCollider, time);
				}
			}
			persistentColliders[i] = persistentCollider;
		}
	}

	private void RemoveCollidersOutsideVolume(float currentTime)
	{
		for (int num = persistentColliders.Count - 1; num >= 0; num--)
		{
			WaterOverlappingCollider waterOverlappingCollider = persistentColliders[num];
			if (waterOverlappingCollider.collider == null || !waterOverlappingCollider.collider.gameObject.activeInHierarchy || (!waterOverlappingCollider.inVolume && (!waterOverlappingCollider.playDripEffect || currentTime - waterOverlappingCollider.lastInWaterTime > waterParams.postExitDripDuration)))
			{
				UnregisterOwnershipOfCollider(waterOverlappingCollider.collider);
				Player instance = Player.Instance;
				if (waterOverlappingCollider.collider == instance.headCollider || waterOverlappingCollider.collider == instance.bodyCollider)
				{
					instance.OnExitWaterVolume(waterOverlappingCollider.collider, this);
				}
				persistentColliders.RemoveAt(num);
			}
		}
	}

	private void CheckColliderAgainstWater(ref WaterOverlappingCollider persistentCollider, float currentTime)
	{
		Vector3 position = persistentCollider.collider.transform.position;
		bool flag = true;
		if (persistentCollider.surfaceDetected)
		{
			flag = (position - Vector3.Dot(position - persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal) * persistentCollider.lastSurfaceQuery.surfaceNormal - persistentCollider.lastSurfaceQuery.surfacePoint).sqrMagnitude > waterParams.recomputeSurfaceForColliderDist * waterParams.recomputeSurfaceForColliderDist;
		}
		if (flag)
		{
			if (GetSurfaceQueryForPoint(position, out var result))
			{
				persistentCollider.surfaceDetected = true;
				persistentCollider.lastSurfaceQuery = result;
			}
			else
			{
				persistentCollider.surfaceDetected = false;
				persistentCollider.lastSurfaceQuery = default(SurfaceQuery);
			}
		}
		if (persistentCollider.surfaceDetected)
		{
			bool flag2 = persistentCollider.collider.ClosestPoint(position + Vector3.down * 10f).y < persistentCollider.lastSurfaceQuery.surfacePoint.y;
			bool flag3 = persistentCollider.collider.ClosestPoint(position + Vector3.up * 10f).y > persistentCollider.lastSurfaceQuery.surfacePoint.y - persistentCollider.lastSurfaceQuery.maxDepth;
			persistentCollider.inWater = flag2 && flag3;
		}
		else
		{
			persistentCollider.inWater = false;
		}
		if (persistentCollider.inWater)
		{
			persistentCollider.lastInWaterTime = currentTime;
		}
	}

	private Vector3 GetColliderVelocity(ref WaterOverlappingCollider persistentCollider)
	{
		Player instance = Player.Instance;
		Vector3 result = Vector3.one * (waterParams.splashSpeedRequirement + 0.1f);
		if (persistentCollider.velocityTracker != null)
		{
			result = persistentCollider.velocityTracker.GetAverageVelocity(worldSpace: true, 0.1f);
		}
		else if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
		{
			result = instance.currentVelocity;
		}
		else if (persistentCollider.collider.attachedRigidbody != null && !persistentCollider.collider.attachedRigidbody.isKinematic)
		{
			result = persistentCollider.collider.attachedRigidbody.velocity;
		}
		return result;
	}

	private void OnWaterSurfaceEnter(ref WaterOverlappingCollider persistentCollider)
	{
		Player instance = Player.Instance;
		if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
		{
			instance.OnEnterWaterVolume(persistentCollider.collider, this);
		}
		if (HasOwnershipOfCollider(persistentCollider.collider))
		{
			Vector3 colliderVelocity = GetColliderVelocity(ref persistentCollider);
			bool flag = Vector3.Dot(colliderVelocity, -persistentCollider.lastSurfaceQuery.surfaceNormal) > waterParams.splashSpeedRequirement;
			bool flag2 = Vector3.Dot(colliderVelocity, -persistentCollider.lastSurfaceQuery.surfaceNormal) > waterParams.bigSplashSpeedRequirement;
			persistentCollider.PlayRippleEffect(waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, waterParams.rippleEffectScale, Time.time);
			if (waterParams.playSplashEffect && flag && (flag2 || !persistentCollider.playBigSplash))
			{
				persistentCollider.PlaySplashEffect(waterParams.splashEffect, persistentCollider.lastRipplePosition, waterParams.splashEffectScale, persistentCollider.playBigSplash && flag2, enteringWater: true);
			}
		}
	}

	private void OnWaterSurfaceExit(ref WaterOverlappingCollider persistentCollider, float currentTime)
	{
		persistentCollider.nextDripTime = currentTime + waterParams.perDripTimeDelay + Random.Range((0f - waterParams.perDripTimeRandRange) * 0.5f, waterParams.perDripTimeRandRange * 0.5f);
		Player instance = Player.Instance;
		if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
		{
			instance.OnExitWaterVolume(persistentCollider.collider, this);
		}
		if (HasOwnershipOfCollider(persistentCollider.collider))
		{
			float num = Vector3.Dot(GetColliderVelocity(ref persistentCollider), persistentCollider.lastSurfaceQuery.surfaceNormal);
			bool flag = num > waterParams.splashSpeedRequirement;
			bool flag2 = num > waterParams.bigSplashSpeedRequirement;
			persistentCollider.PlayRippleEffect(waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, waterParams.rippleEffectScale, Time.time);
			if (waterParams.playSplashEffect && flag && (flag2 || !persistentCollider.playBigSplash))
			{
				persistentCollider.PlaySplashEffect(waterParams.splashEffect, persistentCollider.lastRipplePosition, waterParams.splashEffectScale, persistentCollider.playBigSplash && flag2, enteringWater: false);
			}
		}
	}

	private void ColliderOutOfWaterUpdate(ref WaterOverlappingCollider persistentCollider, float currentTime)
	{
		if (currentTime < persistentCollider.lastInWaterTime + waterParams.postExitDripDuration && currentTime > persistentCollider.nextDripTime && persistentCollider.playDripEffect)
		{
			persistentCollider.nextDripTime = currentTime + waterParams.perDripTimeDelay + Random.Range((0f - waterParams.perDripTimeRandRange) * 0.5f, waterParams.perDripTimeRandRange * 0.5f);
			float dripScale = waterParams.rippleEffectScale * 2f * (waterParams.perDripDefaultRadius + Random.Range((0f - waterParams.perDripRadiusRandRange) * 0.5f, waterParams.perDripRadiusRandRange * 0.5f));
			persistentCollider.PlayDripEffect(waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, dripScale);
		}
	}

	private void ColliderInWaterUpdate(ref WaterOverlappingCollider persistentCollider, float currentTime)
	{
		Vector3 vector = Vector3.ProjectOnPlane(persistentCollider.collider.transform.position - persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal) + persistentCollider.lastSurfaceQuery.surfacePoint;
		bool flag = false;
		if ((!persistentCollider.overrideBoundingRadius) ? ((persistentCollider.collider.ClosestPointOnBounds(vector) - vector).sqrMagnitude < 0.001f) : ((persistentCollider.collider.transform.position - vector).sqrMagnitude < persistentCollider.boundingRadiusOverride * persistentCollider.boundingRadiusOverride))
		{
			float num = Mathf.Max(waterParams.minDistanceBetweenRipples, waterParams.defaultDistanceBetweenRipples * (persistentCollider.lastRippleScale / waterParams.rippleEffectScale));
			bool num2 = (persistentCollider.lastRipplePosition - vector).sqrMagnitude > num * num;
			bool flag2 = currentTime - persistentCollider.lastRippleTime > waterParams.minTimeBetweenRipples;
			if (num2 || flag2)
			{
				persistentCollider.PlayRippleEffect(waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, waterParams.rippleEffectScale, currentTime);
			}
		}
		else
		{
			persistentCollider.lastRippleTime = currentTime;
		}
	}

	private void TryRegisterOwnershipOfCollider(Collider collider, bool isInWater, bool isSurfaceDetected)
	{
		if (sharedColliderRegistry.TryGetValue(collider, out var value))
		{
			if (value != this)
			{
				value.CheckColliderInVolume(collider, out var inWater, out var surfaceDetected);
				if ((isSurfaceDetected && !surfaceDetected) || (isInWater && !inWater))
				{
					sharedColliderRegistry.Remove(collider);
					sharedColliderRegistry.Add(collider, this);
				}
			}
		}
		else
		{
			sharedColliderRegistry.Add(collider, this);
		}
	}

	private void UnregisterOwnershipOfCollider(Collider collider)
	{
		if (sharedColliderRegistry.ContainsKey(collider))
		{
			sharedColliderRegistry.Remove(collider);
		}
	}

	private bool HasOwnershipOfCollider(Collider collider)
	{
		if (sharedColliderRegistry.TryGetValue(collider, out var value))
		{
			return value == this;
		}
		return false;
	}

	private void OnTriggerEnter(Collider other)
	{
		for (int i = 0; i < persistentColliders.Count; i++)
		{
			if (persistentColliders[i].collider == other)
			{
				WaterOverlappingCollider value = persistentColliders[i];
				value.inVolume = true;
				persistentColliders[i] = value;
				return;
			}
		}
		WaterOverlappingCollider waterOverlappingCollider = default(WaterOverlappingCollider);
		waterOverlappingCollider.collider = other;
		WaterOverlappingCollider item = waterOverlappingCollider;
		item.inVolume = true;
		item.lastInWaterTime = Time.time - waterParams.postExitDripDuration - 10f;
		GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component != null)
		{
			item.velocityTracker = (component.isLeftHand ? Player.Instance.leftHandCenterVelocityTracker : Player.Instance.rightHandCenterVelocityTracker);
		}
		else
		{
			item.velocityTracker = other.GetComponent<GorillaVelocityTracker>();
		}
		WaterSplashOverride component2 = other.GetComponent<WaterSplashOverride>();
		if (component2 != null)
		{
			item.playSplashEffects = !component2.suppressWaterEffects;
			item.playBigSplash = component2.playBigSplash;
			item.playDripEffect = component2.playDrippingEffect;
			item.overrideBoundingRadius = component2.overrideBoundingRadius;
			item.boundingRadiusOverride = component2.boundingRadiusOverride;
		}
		else
		{
			item.playSplashEffects = true;
			item.playDripEffect = true;
			item.overrideBoundingRadius = false;
			item.playBigSplash = false;
		}
		Player instance = Player.Instance;
		if (PlayerVRRig != null && waterParams.sendSplashEffectRPCs && (component != null || item.collider == instance.headCollider || item.collider == instance.bodyCollider))
		{
			item.photonViewForRPC = PlayerVRRig.photonView;
		}
		persistentColliders.Add(item);
	}

	private void OnTriggerExit(Collider other)
	{
		for (int i = 0; i < persistentColliders.Count; i++)
		{
			if (persistentColliders[i].collider == other)
			{
				WaterOverlappingCollider value = persistentColliders[i];
				value.inVolume = false;
				persistentColliders[i] = value;
			}
		}
	}
}
