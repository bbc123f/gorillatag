using System;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x0200028F RID: 655
	public struct WaterOverlappingCollider
	{
		// Token: 0x06001117 RID: 4375 RVA: 0x0005F474 File Offset: 0x0005D674
		public void PlayRippleEffect(GameObject rippleEffectPrefab, Vector3 surfacePoint, Vector3 surfaceNormal, float defaultRippleScale, float currentTime, WaterVolume volume)
		{
			this.lastRipplePosition = this.GetClosestPositionOnSurface(surfacePoint, surfaceNormal);
			this.lastBoundingRadius = this.GetBoundingRadiusOnSurface(surfaceNormal);
			this.lastRippleScale = defaultRippleScale * this.lastBoundingRadius * 2f * this.scaleMultiplier;
			this.lastRippleTime = currentTime;
			ObjectPools.instance.Instantiate(rippleEffectPrefab, this.lastRipplePosition, Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right), this.lastRippleScale).GetComponent<WaterRippleEffect>().PlayEffect(volume);
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x0005F50C File Offset: 0x0005D70C
		public void PlaySplashEffect(GameObject splashEffectPrefab, Vector3 splashPosition, float splashScale, bool bigSplash, bool enteringWater, WaterVolume volume)
		{
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right);
			ObjectPools.instance.Instantiate(splashEffectPrefab, splashPosition, quaternion, splashScale * this.scaleMultiplier).GetComponent<WaterSplashEffect>().PlayEffect(bigSplash, enteringWater, this.scaleMultiplier, volume);
			if (this.photonViewForRPC != null)
			{
				float time = Time.time;
				int num = -1;
				float num2 = time + 10f;
				for (int i = 0; i < WaterVolume.splashRPCSendTimes.Length; i++)
				{
					if (WaterVolume.splashRPCSendTimes[i] < num2)
					{
						num2 = WaterVolume.splashRPCSendTimes[i];
						num = i;
					}
				}
				if (time - 0.5f > num2)
				{
					WaterVolume.splashRPCSendTimes[num] = time;
					this.photonViewForRPC.RPC("PlaySplashEffect", RpcTarget.Others, new object[]
					{
						splashPosition,
						quaternion,
						splashScale * this.scaleMultiplier,
						this.lastBoundingRadius,
						bigSplash,
						enteringWater
					});
				}
			}
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x0005F62C File Offset: 0x0005D82C
		public void PlayDripEffect(GameObject rippleEffectPrefab, Vector3 surfacePoint, Vector3 surfaceNormal, float dripScale)
		{
			Vector3 closestPositionOnSurface = this.GetClosestPositionOnSurface(surfacePoint, surfaceNormal);
			float d = this.overrideBoundingRadius ? this.boundingRadiusOverride : this.lastBoundingRadius;
			Vector3 b = Vector3.ProjectOnPlane(Random.onUnitSphere * d * 0.5f, surfaceNormal);
			ObjectPools.instance.Instantiate(rippleEffectPrefab, closestPositionOnSurface + b, Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right), dripScale * this.scaleMultiplier);
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x0005F6BA File Offset: 0x0005D8BA
		public Vector3 GetClosestPositionOnSurface(Vector3 surfacePoint, Vector3 surfaceNormal)
		{
			return Vector3.ProjectOnPlane(this.collider.transform.position - surfacePoint, surfaceNormal) + surfacePoint;
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x0005F6E0 File Offset: 0x0005D8E0
		private float GetBoundingRadiusOnSurface(Vector3 surfaceNormal)
		{
			if (this.overrideBoundingRadius)
			{
				this.lastBoundingRadius = this.boundingRadiusOverride;
				return this.boundingRadiusOverride;
			}
			Vector3 extents = this.collider.bounds.extents;
			Vector3 vector = Vector3.ProjectOnPlane(this.collider.transform.right * extents.x, surfaceNormal);
			Vector3 vector2 = Vector3.ProjectOnPlane(this.collider.transform.up * extents.y, surfaceNormal);
			Vector3 vector3 = Vector3.ProjectOnPlane(this.collider.transform.forward * extents.z, surfaceNormal);
			float sqrMagnitude = vector.sqrMagnitude;
			float sqrMagnitude2 = vector2.sqrMagnitude;
			float sqrMagnitude3 = vector3.sqrMagnitude;
			if (sqrMagnitude >= sqrMagnitude2 && sqrMagnitude >= sqrMagnitude3)
			{
				return vector.magnitude;
			}
			if (sqrMagnitude2 >= sqrMagnitude && sqrMagnitude2 >= sqrMagnitude3)
			{
				return vector2.magnitude;
			}
			return vector3.magnitude;
		}

		// Token: 0x04001377 RID: 4983
		public bool playBigSplash;

		// Token: 0x04001378 RID: 4984
		public bool playDripEffect;

		// Token: 0x04001379 RID: 4985
		public bool overrideBoundingRadius;

		// Token: 0x0400137A RID: 4986
		public float boundingRadiusOverride;

		// Token: 0x0400137B RID: 4987
		public float scaleMultiplier;

		// Token: 0x0400137C RID: 4988
		public Collider collider;

		// Token: 0x0400137D RID: 4989
		public GorillaVelocityTracker velocityTracker;

		// Token: 0x0400137E RID: 4990
		public WaterVolume.SurfaceQuery lastSurfaceQuery;

		// Token: 0x0400137F RID: 4991
		public PhotonView photonViewForRPC;

		// Token: 0x04001380 RID: 4992
		public bool surfaceDetected;

		// Token: 0x04001381 RID: 4993
		public bool inWater;

		// Token: 0x04001382 RID: 4994
		public bool inVolume;

		// Token: 0x04001383 RID: 4995
		public float lastBoundingRadius;

		// Token: 0x04001384 RID: 4996
		public Vector3 lastRipplePosition;

		// Token: 0x04001385 RID: 4997
		public float lastRippleScale;

		// Token: 0x04001386 RID: 4998
		public float lastRippleTime;

		// Token: 0x04001387 RID: 4999
		public float lastInWaterTime;

		// Token: 0x04001388 RID: 5000
		public float nextDripTime;
	}
}
