using System;
using System.Collections.Generic;
using CjLib;
using GorillaLocomotion.Climbing;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000292 RID: 658
	[RequireComponent(typeof(Collider))]
	public class WaterVolume : MonoBehaviour
	{
		// Token: 0x14000019 RID: 25
		// (add) Token: 0x0600111E RID: 4382 RVA: 0x0005F8AC File Offset: 0x0005DAAC
		// (remove) Token: 0x0600111F RID: 4383 RVA: 0x0005F8E4 File Offset: 0x0005DAE4
		public event WaterVolume.WaterVolumeEvent ColliderEnteredVolume;

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x06001120 RID: 4384 RVA: 0x0005F91C File Offset: 0x0005DB1C
		// (remove) Token: 0x06001121 RID: 4385 RVA: 0x0005F954 File Offset: 0x0005DB54
		public event WaterVolume.WaterVolumeEvent ColliderExitedVolume;

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x06001122 RID: 4386 RVA: 0x0005F98C File Offset: 0x0005DB8C
		// (remove) Token: 0x06001123 RID: 4387 RVA: 0x0005F9C4 File Offset: 0x0005DBC4
		public event WaterVolume.WaterVolumeEvent ColliderEnteredWater;

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x06001124 RID: 4388 RVA: 0x0005F9FC File Offset: 0x0005DBFC
		// (remove) Token: 0x06001125 RID: 4389 RVA: 0x0005FA34 File Offset: 0x0005DC34
		public event WaterVolume.WaterVolumeEvent ColliderExitedWater;

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06001126 RID: 4390 RVA: 0x0005FA69 File Offset: 0x0005DC69
		public Player.LiquidType LiquidType
		{
			get
			{
				return this.liquidType;
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06001127 RID: 4391 RVA: 0x0005FA71 File Offset: 0x0005DC71
		public WaterCurrent Current
		{
			get
			{
				return this.waterCurrent;
			}
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06001128 RID: 4392 RVA: 0x0005FA79 File Offset: 0x0005DC79
		public WaterParameters Parameters
		{
			get
			{
				return this.waterParams;
			}
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06001129 RID: 4393 RVA: 0x0005FA84 File Offset: 0x0005DC84
		private VRRig PlayerVRRig
		{
			get
			{
				if (this.playerVRRig == null)
				{
					GorillaTagger instance = GorillaTagger.Instance;
					if (instance != null)
					{
						this.playerVRRig = instance.offlineVRRig;
					}
				}
				return this.playerVRRig;
			}
		}

		// Token: 0x0600112A RID: 4394 RVA: 0x0005FAC0 File Offset: 0x0005DCC0
		public bool GetSurfaceQueryForPoint(Vector3 point, out WaterVolume.SurfaceQuery result, bool debugDraw = false)
		{
			result = default(WaterVolume.SurfaceQuery);
			Ray ray = new Ray(new Vector3(point.x, this.volumeMaxHeight, point.z), Vector3.down);
			Ray ray2 = new Ray(new Vector3(point.x, this.volumeMinHeight, point.z), Vector3.up);
			float num = this.volumeMaxHeight - this.volumeMinHeight;
			float num2 = float.MinValue;
			float num3 = float.MaxValue;
			bool flag = false;
			bool flag2 = false;
			float num4 = 0f;
			for (int i = 0; i < this.surfaceColliders.Count; i++)
			{
				bool enabled = this.surfaceColliders[i].enabled;
				this.surfaceColliders[i].enabled = true;
				RaycastHit hit;
				if (this.surfaceColliders[i].Raycast(ray, out hit, num) && hit.point.y > num2 && this.HitOutsideSurfaceOfMesh(ray.direction, this.surfaceColliders[i], hit))
				{
					num2 = hit.point.y;
					flag = true;
					result.surfacePoint = hit.point;
					result.surfaceNormal = hit.normal;
				}
				RaycastHit hit2;
				if (this.surfaceColliders[i].Raycast(ray2, out hit2, num) && hit2.point.y < num3 && this.HitOutsideSurfaceOfMesh(ray2.direction, this.surfaceColliders[i], hit2))
				{
					num3 = hit2.point.y;
					flag2 = true;
					num4 = hit2.point.y;
				}
				this.surfaceColliders[i].enabled = enabled;
			}
			if (!flag && this.surfacePlane != null)
			{
				flag = true;
				result.surfacePoint = point - Vector3.Dot(point - this.surfacePlane.position, this.surfacePlane.up) * this.surfacePlane.up;
				result.surfaceNormal = this.surfacePlane.up;
			}
			if (flag && flag2)
			{
				result.maxDepth = result.surfacePoint.y - num4;
			}
			else if (flag)
			{
				result.maxDepth = result.surfacePoint.y - this.volumeMinHeight;
			}
			else
			{
				result.maxDepth = this.volumeMaxHeight - this.volumeMinHeight;
			}
			if (debugDraw)
			{
				if (flag)
				{
					DebugUtil.DrawLine(ray.origin, ray.origin + ray.direction * num, Color.green, false);
					DebugUtil.DrawSphere(result.surfacePoint, 0.001f, 12, 12, Color.green, false, DebugUtil.Style.SolidColor);
				}
				else
				{
					DebugUtil.DrawLine(ray.origin, ray.origin + ray.direction * num, Color.red, false);
				}
				if (flag2)
				{
					DebugUtil.DrawLine(ray2.origin, ray2.origin + ray2.direction * num, Color.yellow, false);
					DebugUtil.DrawSphere(new Vector3(result.surfacePoint.x, num4, result.surfacePoint.z), 0.001f, 12, 12, Color.yellow, false, DebugUtil.Style.SolidColor);
				}
			}
			return flag;
		}

		// Token: 0x0600112B RID: 4395 RVA: 0x0005FE04 File Offset: 0x0005E004
		private bool HitOutsideSurfaceOfMesh(Vector3 castDir, MeshCollider meshCollider, RaycastHit hit)
		{
			if (!WaterVolume.meshTrianglesDict.TryGetValue(meshCollider.sharedMesh, out this.sharedMeshTris))
			{
				this.sharedMeshTris = (int[])meshCollider.sharedMesh.triangles.Clone();
				WaterVolume.meshTrianglesDict.Add(meshCollider.sharedMesh, this.sharedMeshTris);
			}
			if (!WaterVolume.meshVertsDict.TryGetValue(meshCollider.sharedMesh, out this.sharedMeshVerts))
			{
				this.sharedMeshVerts = (Vector3[])meshCollider.sharedMesh.vertices.Clone();
				WaterVolume.meshVertsDict.Add(meshCollider.sharedMesh, this.sharedMeshVerts);
			}
			Vector3 b = this.sharedMeshVerts[this.sharedMeshTris[hit.triangleIndex * 3]];
			Vector3 a = this.sharedMeshVerts[this.sharedMeshTris[hit.triangleIndex * 3 + 1]];
			Vector3 a2 = this.sharedMeshVerts[this.sharedMeshTris[hit.triangleIndex * 3 + 2]];
			Vector3 vector = meshCollider.transform.TransformDirection(Vector3.Cross(a - b, a2 - b).normalized);
			bool flag = Vector3.Dot(castDir, vector) < 0f;
			if (this.debugDrawSurfaceCast)
			{
				Color color = flag ? Color.blue : Color.red;
				DebugUtil.DrawLine(hit.point, hit.point + vector * 0.3f, color, false);
			}
			return flag;
		}

		// Token: 0x0600112C RID: 4396 RVA: 0x0005FF78 File Offset: 0x0005E178
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
				float d = 0.2f;
				DebugUtil.DrawLine(vector, vector + normalized * d, Color.blue, false);
				DebugUtil.DrawLine(vector2, vector2 + normalized * d, Color.blue, false);
				DebugUtil.DrawLine(vector3, vector3 + normalized * d, Color.blue, false);
				DebugUtil.DrawLine(vector, vector2, Color.blue, false);
				DebugUtil.DrawLine(vector, vector3, Color.blue, false);
				DebugUtil.DrawLine(vector2, vector3, Color.blue, false);
			}
		}

		// Token: 0x0600112D RID: 4397 RVA: 0x000600C4 File Offset: 0x0005E2C4
		public bool RaycastWater(Vector3 origin, Vector3 direction, out RaycastHit hit, float distance, int layerMask)
		{
			if (this.triggerCollider != null)
			{
				return Physics.Raycast(new Ray(origin, direction), out hit, distance, layerMask, QueryTriggerInteraction.Collide);
			}
			hit = default(RaycastHit);
			return false;
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x000600F0 File Offset: 0x0005E2F0
		public bool CheckColliderInVolume(Collider collider, out bool inWater, out bool surfaceDetected)
		{
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				if (this.persistentColliders[i].collider == collider)
				{
					inWater = this.persistentColliders[i].inWater;
					surfaceDetected = this.persistentColliders[i].surfaceDetected;
					return true;
				}
			}
			inWater = false;
			surfaceDetected = false;
			return false;
		}

		// Token: 0x0600112F RID: 4399 RVA: 0x0006015C File Offset: 0x0005E35C
		private void Awake()
		{
			this.triggerCollider = base.GetComponent<Collider>();
			if (this.volumeColliders == null || this.volumeColliders.Count < 1)
			{
				this.volumeColliders = new List<Collider>();
				this.volumeColliders.Add(base.gameObject.GetComponent<Collider>());
			}
			float num = float.MinValue;
			float num2 = float.MaxValue;
			for (int i = 0; i < this.volumeColliders.Count; i++)
			{
				float y = this.volumeColliders[i].bounds.max.y;
				float y2 = this.volumeColliders[i].bounds.min.y;
				if (y > num)
				{
					num = y;
				}
				if (y2 < num2)
				{
					num2 = y2;
				}
			}
			this.volumeMaxHeight = num;
			this.volumeMinHeight = num2;
		}

		// Token: 0x06001130 RID: 4400 RVA: 0x0006022C File Offset: 0x0005E42C
		private void Update()
		{
			if (this.persistentColliders.Count < 1)
			{
				return;
			}
			float time = Time.time;
			this.RemoveCollidersOutsideVolume(time);
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
				bool inWater = waterOverlappingCollider.inWater;
				if (waterOverlappingCollider.inVolume)
				{
					this.CheckColliderAgainstWater(ref waterOverlappingCollider, time);
				}
				else
				{
					waterOverlappingCollider.inWater = false;
				}
				this.TryRegisterOwnershipOfCollider(waterOverlappingCollider.collider, waterOverlappingCollider.inWater, waterOverlappingCollider.surfaceDetected);
				if (waterOverlappingCollider.inWater && !inWater)
				{
					this.OnWaterSurfaceEnter(ref waterOverlappingCollider);
				}
				else if (!waterOverlappingCollider.inWater && inWater)
				{
					this.OnWaterSurfaceExit(ref waterOverlappingCollider, time);
				}
				if (this.HasOwnershipOfCollider(waterOverlappingCollider.collider) && waterOverlappingCollider.surfaceDetected)
				{
					if (!waterOverlappingCollider.inWater)
					{
						this.ColliderOutOfWaterUpdate(ref waterOverlappingCollider, time);
					}
					else
					{
						this.ColliderInWaterUpdate(ref waterOverlappingCollider, time);
					}
				}
				this.persistentColliders[i] = waterOverlappingCollider;
			}
		}

		// Token: 0x06001131 RID: 4401 RVA: 0x00060324 File Offset: 0x0005E524
		private void RemoveCollidersOutsideVolume(float currentTime)
		{
			for (int i = this.persistentColliders.Count - 1; i >= 0; i--)
			{
				WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
				if (waterOverlappingCollider.collider == null || !waterOverlappingCollider.collider.gameObject.activeInHierarchy || (!waterOverlappingCollider.inVolume && (!waterOverlappingCollider.playDripEffect || currentTime - waterOverlappingCollider.lastInWaterTime > this.waterParams.postExitDripDuration)))
				{
					this.UnregisterOwnershipOfCollider(waterOverlappingCollider.collider);
					Player instance = Player.Instance;
					if (waterOverlappingCollider.collider == instance.headCollider || waterOverlappingCollider.collider == instance.bodyCollider)
					{
						instance.OnExitWaterVolume(waterOverlappingCollider.collider, this);
					}
					this.persistentColliders.RemoveAt(i);
				}
			}
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x000603F4 File Offset: 0x0005E5F4
		private void CheckColliderAgainstWater(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			Vector3 position = persistentCollider.collider.transform.position;
			bool flag = true;
			if (persistentCollider.surfaceDetected && persistentCollider.scaleMultiplier > 0.99f && this.isStationary)
			{
				flag = ((position - Vector3.Dot(position - persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal) * persistentCollider.lastSurfaceQuery.surfaceNormal - persistentCollider.lastSurfaceQuery.surfacePoint).sqrMagnitude > this.waterParams.recomputeSurfaceForColliderDist * this.waterParams.recomputeSurfaceForColliderDist);
			}
			if (flag)
			{
				WaterVolume.SurfaceQuery lastSurfaceQuery;
				if (this.GetSurfaceQueryForPoint(position, out lastSurfaceQuery, this.debugDrawSurfaceCast))
				{
					persistentCollider.surfaceDetected = true;
					persistentCollider.lastSurfaceQuery = lastSurfaceQuery;
				}
				else
				{
					persistentCollider.surfaceDetected = false;
					persistentCollider.lastSurfaceQuery = default(WaterVolume.SurfaceQuery);
				}
			}
			if (persistentCollider.surfaceDetected)
			{
				bool flag2 = ((persistentCollider.collider is MeshCollider) ? persistentCollider.collider.ClosestPointOnBounds(position + Vector3.down * 10f) : persistentCollider.collider.ClosestPoint(position + Vector3.down * 10f)).y < persistentCollider.lastSurfaceQuery.surfacePoint.y;
				bool flag3 = ((persistentCollider.collider is MeshCollider) ? persistentCollider.collider.ClosestPointOnBounds(position + Vector3.up * 10f) : persistentCollider.collider.ClosestPoint(position + Vector3.up * 10f)).y > persistentCollider.lastSurfaceQuery.surfacePoint.y - persistentCollider.lastSurfaceQuery.maxDepth;
				persistentCollider.inWater = (flag2 && flag3);
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

		// Token: 0x06001133 RID: 4403 RVA: 0x000605DC File Offset: 0x0005E7DC
		private Vector3 GetColliderVelocity(ref WaterOverlappingCollider persistentCollider)
		{
			Player instance = Player.Instance;
			Vector3 result = Vector3.one * (this.waterParams.splashSpeedRequirement + 0.1f);
			if (persistentCollider.velocityTracker != null)
			{
				result = persistentCollider.velocityTracker.GetAverageVelocity(true, 0.1f, false);
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

		// Token: 0x06001134 RID: 4404 RVA: 0x00060694 File Offset: 0x0005E894
		private void OnWaterSurfaceEnter(ref WaterOverlappingCollider persistentCollider)
		{
			WaterVolume.WaterVolumeEvent colliderEnteredWater = this.ColliderEnteredWater;
			if (colliderEnteredWater != null)
			{
				colliderEnteredWater(persistentCollider.collider);
			}
			Player instance = Player.Instance;
			if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
			{
				instance.OnEnterWaterVolume(persistentCollider.collider, this);
			}
			if (this.HasOwnershipOfCollider(persistentCollider.collider))
			{
				Vector3 colliderVelocity = this.GetColliderVelocity(ref persistentCollider);
				bool flag = Vector3.Dot(colliderVelocity, -persistentCollider.lastSurfaceQuery.surfaceNormal) > this.waterParams.splashSpeedRequirement * persistentCollider.scaleMultiplier;
				bool flag2 = Vector3.Dot(colliderVelocity, -persistentCollider.lastSurfaceQuery.surfaceNormal) > this.waterParams.bigSplashSpeedRequirement * persistentCollider.scaleMultiplier;
				persistentCollider.PlayRippleEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, this.waterParams.rippleEffectScale, Time.time, this);
				if (this.waterParams.playSplashEffect && flag && (flag2 || !persistentCollider.playBigSplash))
				{
					persistentCollider.PlaySplashEffect(this.waterParams.splashEffect, persistentCollider.lastRipplePosition, this.waterParams.splashEffectScale, persistentCollider.playBigSplash && flag2, true, this);
				}
			}
		}

		// Token: 0x06001135 RID: 4405 RVA: 0x000607DC File Offset: 0x0005E9DC
		private void OnWaterSurfaceExit(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			WaterVolume.WaterVolumeEvent colliderExitedWater = this.ColliderExitedWater;
			if (colliderExitedWater != null)
			{
				colliderExitedWater(persistentCollider.collider);
			}
			persistentCollider.nextDripTime = currentTime + this.waterParams.perDripTimeDelay + Random.Range(-this.waterParams.perDripTimeRandRange * 0.5f, this.waterParams.perDripTimeRandRange * 0.5f);
			Player instance = Player.Instance;
			if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
			{
				instance.OnExitWaterVolume(persistentCollider.collider, this);
			}
			if (this.HasOwnershipOfCollider(persistentCollider.collider))
			{
				float num = Vector3.Dot(this.GetColliderVelocity(ref persistentCollider), persistentCollider.lastSurfaceQuery.surfaceNormal);
				bool flag = num > this.waterParams.splashSpeedRequirement * persistentCollider.scaleMultiplier;
				bool flag2 = num > this.waterParams.bigSplashSpeedRequirement * persistentCollider.scaleMultiplier;
				persistentCollider.PlayRippleEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, this.waterParams.rippleEffectScale, Time.time, this);
				if (this.waterParams.playSplashEffect && flag && (flag2 || !persistentCollider.playBigSplash))
				{
					persistentCollider.PlaySplashEffect(this.waterParams.splashEffect, persistentCollider.lastRipplePosition, this.waterParams.splashEffectScale, persistentCollider.playBigSplash && flag2, false, this);
				}
			}
		}

		// Token: 0x06001136 RID: 4406 RVA: 0x00060948 File Offset: 0x0005EB48
		private void ColliderOutOfWaterUpdate(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			if (currentTime < persistentCollider.lastInWaterTime + this.waterParams.postExitDripDuration && currentTime > persistentCollider.nextDripTime && persistentCollider.playDripEffect)
			{
				persistentCollider.nextDripTime = currentTime + this.waterParams.perDripTimeDelay + Random.Range(-this.waterParams.perDripTimeRandRange * 0.5f, this.waterParams.perDripTimeRandRange * 0.5f);
				float dripScale = this.waterParams.rippleEffectScale * 2f * (this.waterParams.perDripDefaultRadius + Random.Range(-this.waterParams.perDripRadiusRandRange * 0.5f, this.waterParams.perDripRadiusRandRange * 0.5f));
				persistentCollider.PlayDripEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, dripScale);
			}
		}

		// Token: 0x06001137 RID: 4407 RVA: 0x00060A30 File Offset: 0x0005EC30
		private void ColliderInWaterUpdate(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			Vector3 vector = Vector3.ProjectOnPlane(persistentCollider.collider.transform.position - persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal) + persistentCollider.lastSurfaceQuery.surfacePoint;
			bool flag;
			if (persistentCollider.overrideBoundingRadius)
			{
				flag = ((persistentCollider.collider.transform.position - vector).sqrMagnitude < persistentCollider.boundingRadiusOverride * persistentCollider.boundingRadiusOverride);
			}
			else
			{
				flag = ((persistentCollider.collider.ClosestPointOnBounds(vector) - vector).sqrMagnitude < 0.001f);
			}
			if (flag)
			{
				float num = Mathf.Max(this.waterParams.minDistanceBetweenRipples, this.waterParams.defaultDistanceBetweenRipples * (persistentCollider.lastRippleScale / this.waterParams.rippleEffectScale));
				bool flag2 = (persistentCollider.lastRipplePosition - vector).sqrMagnitude > num * num;
				bool flag3 = currentTime - persistentCollider.lastRippleTime > this.waterParams.minTimeBetweenRipples;
				if (flag2 || flag3)
				{
					persistentCollider.PlayRippleEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, this.waterParams.rippleEffectScale, currentTime, this);
					return;
				}
			}
			else
			{
				persistentCollider.lastRippleTime = currentTime;
			}
		}

		// Token: 0x06001138 RID: 4408 RVA: 0x00060B80 File Offset: 0x0005ED80
		private void TryRegisterOwnershipOfCollider(Collider collider, bool isInWater, bool isSurfaceDetected)
		{
			WaterVolume waterVolume;
			if (WaterVolume.sharedColliderRegistry.TryGetValue(collider, out waterVolume))
			{
				if (waterVolume != this)
				{
					bool flag;
					bool flag2;
					waterVolume.CheckColliderInVolume(collider, out flag, out flag2);
					if ((isSurfaceDetected && !flag2) || (isInWater && !flag))
					{
						WaterVolume.sharedColliderRegistry.Remove(collider);
						WaterVolume.sharedColliderRegistry.Add(collider, this);
						return;
					}
				}
			}
			else
			{
				WaterVolume.sharedColliderRegistry.Add(collider, this);
			}
		}

		// Token: 0x06001139 RID: 4409 RVA: 0x00060BE2 File Offset: 0x0005EDE2
		private void UnregisterOwnershipOfCollider(Collider collider)
		{
			if (WaterVolume.sharedColliderRegistry.ContainsKey(collider))
			{
				WaterVolume.sharedColliderRegistry.Remove(collider);
			}
		}

		// Token: 0x0600113A RID: 4410 RVA: 0x00060C00 File Offset: 0x0005EE00
		private bool HasOwnershipOfCollider(Collider collider)
		{
			WaterVolume x;
			return WaterVolume.sharedColliderRegistry.TryGetValue(collider, out x) && x == this;
		}

		// Token: 0x0600113B RID: 4411 RVA: 0x00060C28 File Offset: 0x0005EE28
		private void OnTriggerEnter(Collider other)
		{
			WaterVolume.WaterVolumeEvent colliderEnteredVolume = this.ColliderEnteredVolume;
			if (colliderEnteredVolume != null)
			{
				colliderEnteredVolume(other);
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				if (this.persistentColliders[i].collider == other)
				{
					WaterOverlappingCollider value = this.persistentColliders[i];
					value.inVolume = true;
					this.persistentColliders[i] = value;
					return;
				}
			}
			WaterOverlappingCollider waterOverlappingCollider = new WaterOverlappingCollider
			{
				collider = other
			};
			waterOverlappingCollider.inVolume = true;
			waterOverlappingCollider.lastInWaterTime = Time.time - this.waterParams.postExitDripDuration - 10f;
			WaterSplashOverride component = other.GetComponent<WaterSplashOverride>();
			if (component != null)
			{
				if (component.suppressWaterEffects)
				{
					return;
				}
				waterOverlappingCollider.playBigSplash = component.playBigSplash;
				waterOverlappingCollider.playDripEffect = component.playDrippingEffect;
				waterOverlappingCollider.overrideBoundingRadius = component.overrideBoundingRadius;
				waterOverlappingCollider.boundingRadiusOverride = component.boundingRadiusOverride;
				waterOverlappingCollider.scaleMultiplier = (component.scaleByPlayersScale ? Player.Instance.scale : 1f);
			}
			else
			{
				waterOverlappingCollider.playDripEffect = true;
				waterOverlappingCollider.overrideBoundingRadius = false;
				waterOverlappingCollider.scaleMultiplier = 1f;
				waterOverlappingCollider.playBigSplash = false;
			}
			Player instance = Player.Instance;
			GorillaTriggerColliderHandIndicator component2 = other.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (component2 != null)
			{
				waterOverlappingCollider.velocityTracker = (component2.isLeftHand ? instance.leftHandCenterVelocityTracker : instance.rightHandCenterVelocityTracker);
				waterOverlappingCollider.scaleMultiplier = instance.scale;
			}
			else
			{
				waterOverlappingCollider.velocityTracker = other.GetComponent<GorillaVelocityTracker>();
			}
			if (this.PlayerVRRig != null && this.waterParams.sendSplashEffectRPCs && (component2 != null || waterOverlappingCollider.collider == instance.headCollider || waterOverlappingCollider.collider == instance.bodyCollider))
			{
				waterOverlappingCollider.photonViewForRPC = this.PlayerVRRig.photonView;
			}
			this.persistentColliders.Add(waterOverlappingCollider);
		}

		// Token: 0x0600113C RID: 4412 RVA: 0x00060E28 File Offset: 0x0005F028
		private void OnTriggerExit(Collider other)
		{
			WaterVolume.WaterVolumeEvent colliderExitedVolume = this.ColliderExitedVolume;
			if (colliderExitedVolume != null)
			{
				colliderExitedVolume(other);
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				if (this.persistentColliders[i].collider == other)
				{
					WaterOverlappingCollider value = this.persistentColliders[i];
					value.inVolume = false;
					this.persistentColliders[i] = value;
				}
			}
		}

		// Token: 0x040013A5 RID: 5029
		[SerializeField]
		public Transform surfacePlane;

		// Token: 0x040013A6 RID: 5030
		[SerializeField]
		private List<MeshCollider> surfaceColliders = new List<MeshCollider>();

		// Token: 0x040013A7 RID: 5031
		[SerializeField]
		public List<Collider> volumeColliders = new List<Collider>();

		// Token: 0x040013A8 RID: 5032
		[SerializeField]
		private Player.LiquidType liquidType;

		// Token: 0x040013A9 RID: 5033
		[SerializeField]
		private WaterCurrent waterCurrent;

		// Token: 0x040013AA RID: 5034
		[SerializeField]
		private WaterParameters waterParams;

		// Token: 0x040013AB RID: 5035
		[SerializeField]
		public bool isStationary = true;

		// Token: 0x040013AC RID: 5036
		public const string WaterSplashRPC = "PlaySplashEffect";

		// Token: 0x040013AD RID: 5037
		public static float[] splashRPCSendTimes = new float[4];

		// Token: 0x040013AE RID: 5038
		private static Dictionary<Collider, WaterVolume> sharedColliderRegistry = new Dictionary<Collider, WaterVolume>(16);

		// Token: 0x040013AF RID: 5039
		private static Dictionary<Mesh, int[]> meshTrianglesDict = new Dictionary<Mesh, int[]>(16);

		// Token: 0x040013B0 RID: 5040
		private static Dictionary<Mesh, Vector3[]> meshVertsDict = new Dictionary<Mesh, Vector3[]>(16);

		// Token: 0x040013B1 RID: 5041
		private int[] sharedMeshTris;

		// Token: 0x040013B2 RID: 5042
		private Vector3[] sharedMeshVerts;

		// Token: 0x040013B7 RID: 5047
		private VRRig playerVRRig;

		// Token: 0x040013B8 RID: 5048
		private float volumeMaxHeight;

		// Token: 0x040013B9 RID: 5049
		private float volumeMinHeight;

		// Token: 0x040013BA RID: 5050
		private bool debugDrawSurfaceCast;

		// Token: 0x040013BB RID: 5051
		private Collider triggerCollider;

		// Token: 0x040013BC RID: 5052
		private List<WaterOverlappingCollider> persistentColliders = new List<WaterOverlappingCollider>(16);

		// Token: 0x020004AA RID: 1194
		public struct SurfaceQuery
		{
			// Token: 0x17000287 RID: 647
			// (get) Token: 0x06001E40 RID: 7744 RVA: 0x0009E0E8 File Offset: 0x0009C2E8
			public Plane surfacePlane
			{
				get
				{
					return new Plane(this.surfaceNormal, this.surfacePoint);
				}
			}

			// Token: 0x04001F61 RID: 8033
			public Vector3 surfacePoint;

			// Token: 0x04001F62 RID: 8034
			public Vector3 surfaceNormal;

			// Token: 0x04001F63 RID: 8035
			public float maxDepth;
		}

		// Token: 0x020004AB RID: 1195
		// (Invoke) Token: 0x06001E42 RID: 7746
		public delegate void WaterVolumeEvent(Collider collider);
	}
}
