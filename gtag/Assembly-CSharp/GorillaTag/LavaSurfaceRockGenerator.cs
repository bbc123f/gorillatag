using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200031B RID: 795
	public class LavaSurfaceRockGenerator : MonoBehaviourPun
	{
		// Token: 0x06001602 RID: 5634 RVA: 0x00079B74 File Offset: 0x00077D74
		private void Update()
		{
			double currentTime = PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time);
			this.RemoveExpiredRocks(currentTime);
			this.SpawnNewRocks(currentTime);
			this.UpdateActiveRocks(currentTime);
		}

		// Token: 0x06001603 RID: 5635 RVA: 0x00079BAC File Offset: 0x00077DAC
		private void RemoveExpiredRocks(double currentTime)
		{
			float lifetimeMultiplier = this.rockLifetimeMultiplierVsLavaProgress.Evaluate(this.lavaManager.LavaProgressLinear);
			for (int i = this.activeRocks.Count - 1; i >= 0; i--)
			{
				if (this.activeRocks[i].GetLifetimeProgress(currentTime, lifetimeMultiplier) >= 1f)
				{
					ObjectPools.instance.Destroy(this.activeRocks[i].rigidbody.gameObject);
					this.activeRocks.RemoveAt(i);
				}
			}
		}

		// Token: 0x06001604 RID: 5636 RVA: 0x00079C34 File Offset: 0x00077E34
		private void SpawnNewRocks(double currentTime)
		{
			if (base.photonView.IsMine && this.lavaManager.GameState == RisingLavaManager.RisingLavaState.Rising)
			{
				int num = (int)this.rockCountVsLavaProgress.Evaluate(this.lavaManager.LavaProgressLinear) - this.activeRocks.Count;
				for (int i = 0; i < num; i++)
				{
					this.SpawnRockAuthority(currentTime, this.lavaManager.LavaProgressLinear);
				}
			}
		}

		// Token: 0x06001605 RID: 5637 RVA: 0x00079CA0 File Offset: 0x00077EA0
		private void UpdateActiveRocks(double currentTime)
		{
			float y = this.lavaSurfacePlane.transform.position.y;
			float lifetimeMultiplier = this.rockLifetimeMultiplierVsLavaProgress.Evaluate(this.lavaManager.LavaProgressLinear);
			for (int i = 0; i < this.activeRocks.Count; i++)
			{
				LavaSurfaceRockGenerator.LavaRock lavaRock = this.activeRocks[i];
				float lifetimeProgress = lavaRock.GetLifetimeProgress(currentTime, lifetimeMultiplier);
				float d = lavaRock.spawnSize * this.rockSizeVsLifetime.Evaluate(lifetimeProgress) * this.scaleFactor;
				lavaRock.position.y = y + this.rockSurfaceOffsetVsLifetime.Evaluate(lifetimeProgress) * this.scaleFactor * lavaRock.spawnSize * 0.5f;
				lavaRock.rigidbody.gameObject.transform.localScale = Vector3.one * d;
				lavaRock.rigidbody.MovePosition(lavaRock.position);
				this.activeRocks[i] = lavaRock;
			}
		}

		// Token: 0x06001606 RID: 5638 RVA: 0x00079D9C File Offset: 0x00077F9C
		private void SpawnRockAuthority(double currentTime, float lavaProgress)
		{
			if (base.photonView.IsMine)
			{
				float num = this.rockMaxSizeMultiplierVsLavaProgress.Evaluate(lavaProgress);
				float num2 = Random.Range(this.rockLifetimeRange.x, this.rockLifetimeRange.y);
				float num3 = Random.Range(this.rockSizeRange.x, this.rockSizeRange.y * num);
				Vector2 vector = Random.insideUnitCircle.normalized * Random.Range(this.surfaceRadiusSpawnRange.x, this.surfaceRadiusSpawnRange.y);
				vector = this.GetSpawnPositionWithClearance(vector, num3 * this.scaleFactor, this.surfaceRadiusSpawnRange.y, this.lavaSurfacePlane.transform.position);
				Quaternion quaternion = Quaternion.Euler(new Vector3((Random.Range(0f, 1f) > 0.5f) ? 180f : 0f, Random.Range(0f, 360f), Random.Range(0f, 15f)));
				base.photonView.RPC("SpawnRockRPC", RpcTarget.Others, new object[]
				{
					vector,
					quaternion,
					num3,
					num2,
					currentTime
				});
				this.SpawnRockLocal(vector, quaternion, num3, num2, currentTime);
			}
		}

		// Token: 0x06001607 RID: 5639 RVA: 0x00079EF8 File Offset: 0x000780F8
		private void SpawnRockLocal(Vector2 surfacePosLocal, Quaternion rotation, float spawnSize, float lifetime, double spawnTime)
		{
			Vector3 position = this.lavaSurfacePlane.transform.position + new Vector3(surfacePosLocal.x, 0f, surfacePosLocal.y);
			LavaSurfaceRockGenerator.LavaRock lavaRock = new LavaSurfaceRockGenerator.LavaRock
			{
				position = position,
				rotation = rotation,
				spawnSize = spawnSize,
				lifetime = lifetime,
				spawnTime = spawnTime
			};
			lavaRock.rigidbody = ObjectPools.instance.Instantiate(this.rockPrefab, lavaRock.position, lavaRock.rotation, 0f).GetComponent<Rigidbody>();
			this.activeRocks.Add(lavaRock);
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x00079F9E File Offset: 0x0007819E
		[PunRPC]
		public void SpawnRockRPC(Vector2 surfacePosLocal, Quaternion rotation, float spawnSize, float lifetime, double spawnTime, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SpawnRockRPC");
			this.SpawnRockLocal(surfacePosLocal, rotation, spawnSize, lifetime, spawnTime);
		}

		// Token: 0x06001609 RID: 5641 RVA: 0x00079FBC File Offset: 0x000781BC
		private Vector2 GetSpawnPositionWithClearance(Vector2 inputPosition, float inputSize, float maxDistance, Vector3 lavaSurfaceOrigin)
		{
			Vector2 vector = inputPosition;
			for (int i = 0; i < this.activeRocks.Count; i++)
			{
				Vector3 vector2 = this.activeRocks[i].position - lavaSurfaceOrigin;
				Vector2 b = new Vector2(vector2.x, vector2.z);
				Vector2 a = vector - b;
				float num = (inputSize + this.activeRocks[i].spawnSize * this.scaleFactor) * 0.5f;
				if (a.sqrMagnitude < num * num)
				{
					float magnitude = a.magnitude;
					if (magnitude > 0.001f)
					{
						Vector2 a2 = a / magnitude;
						vector += a2 * (num - magnitude);
						if (vector.sqrMagnitude > maxDistance * maxDistance)
						{
							vector = vector.normalized * maxDistance;
						}
					}
				}
			}
			return vector;
		}

		// Token: 0x04001811 RID: 6161
		[SerializeField]
		private RisingLavaManager lavaManager;

		// Token: 0x04001812 RID: 6162
		[SerializeField]
		private Transform lavaSurfacePlane;

		// Token: 0x04001813 RID: 6163
		[SerializeField]
		private GameObject rockPrefab;

		// Token: 0x04001814 RID: 6164
		[SerializeField]
		private float scaleFactor = 0.03f;

		// Token: 0x04001815 RID: 6165
		[SerializeField]
		private Vector2 surfaceRadiusSpawnRange = new Vector2(0.1f, 0.7f);

		// Token: 0x04001816 RID: 6166
		[SerializeField]
		private Vector2 rockLifetimeRange = new Vector2(5f, 10f);

		// Token: 0x04001817 RID: 6167
		[SerializeField]
		private Vector2 rockSizeRange = new Vector2(0.5f, 2f);

		// Token: 0x04001818 RID: 6168
		[SerializeField]
		private AnimationCurve rockCountVsLavaProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04001819 RID: 6169
		[SerializeField]
		private AnimationCurve rockLifetimeMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x0400181A RID: 6170
		[SerializeField]
		private AnimationCurve rockMaxSizeMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x0400181B RID: 6171
		[SerializeField]
		private AnimationCurve rockSizeVsLifetime = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400181C RID: 6172
		[SerializeField]
		private AnimationCurve rockSurfaceOffsetVsLifetime = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400181D RID: 6173
		private List<LavaSurfaceRockGenerator.LavaRock> activeRocks = new List<LavaSurfaceRockGenerator.LavaRock>();

		// Token: 0x02000504 RID: 1284
		private struct LavaRock
		{
			// Token: 0x06001F4B RID: 8011 RVA: 0x000A1CA2 File Offset: 0x0009FEA2
			public float GetLifetimeProgress(double currentTime, float lifetimeMultiplier)
			{
				return Mathf.Clamp01((float)(currentTime - this.spawnTime) / (this.lifetime * lifetimeMultiplier));
			}

			// Token: 0x040020E5 RID: 8421
			public Vector3 position;

			// Token: 0x040020E6 RID: 8422
			public Quaternion rotation;

			// Token: 0x040020E7 RID: 8423
			public float spawnSize;

			// Token: 0x040020E8 RID: 8424
			public float lifetime;

			// Token: 0x040020E9 RID: 8425
			public double spawnTime;

			// Token: 0x040020EA RID: 8426
			public Rigidbody rigidbody;
		}
	}
}
