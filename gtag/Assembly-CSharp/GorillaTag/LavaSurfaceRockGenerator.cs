using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200031D RID: 797
	public class LavaSurfaceRockGenerator : MonoBehaviourPun
	{
		// Token: 0x0600160B RID: 5643 RVA: 0x0007A05C File Offset: 0x0007825C
		private void Update()
		{
			double currentTime = PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time);
			this.RemoveExpiredRocks(currentTime);
			this.SpawnNewRocks(currentTime);
			this.UpdateActiveRocks(currentTime);
		}

		// Token: 0x0600160C RID: 5644 RVA: 0x0007A094 File Offset: 0x00078294
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

		// Token: 0x0600160D RID: 5645 RVA: 0x0007A11C File Offset: 0x0007831C
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

		// Token: 0x0600160E RID: 5646 RVA: 0x0007A188 File Offset: 0x00078388
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

		// Token: 0x0600160F RID: 5647 RVA: 0x0007A284 File Offset: 0x00078484
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

		// Token: 0x06001610 RID: 5648 RVA: 0x0007A3E0 File Offset: 0x000785E0
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

		// Token: 0x06001611 RID: 5649 RVA: 0x0007A486 File Offset: 0x00078686
		[PunRPC]
		public void SpawnRockRPC(Vector2 surfacePosLocal, Quaternion rotation, float spawnSize, float lifetime, double spawnTime, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SpawnRockRPC");
			this.SpawnRockLocal(surfacePosLocal, rotation, spawnSize, lifetime, spawnTime);
		}

		// Token: 0x06001612 RID: 5650 RVA: 0x0007A4A4 File Offset: 0x000786A4
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

		// Token: 0x0400181E RID: 6174
		[SerializeField]
		private RisingLavaManager lavaManager;

		// Token: 0x0400181F RID: 6175
		[SerializeField]
		private Transform lavaSurfacePlane;

		// Token: 0x04001820 RID: 6176
		[SerializeField]
		private GameObject rockPrefab;

		// Token: 0x04001821 RID: 6177
		[SerializeField]
		private float scaleFactor = 0.03f;

		// Token: 0x04001822 RID: 6178
		[SerializeField]
		private Vector2 surfaceRadiusSpawnRange = new Vector2(0.1f, 0.7f);

		// Token: 0x04001823 RID: 6179
		[SerializeField]
		private Vector2 rockLifetimeRange = new Vector2(5f, 10f);

		// Token: 0x04001824 RID: 6180
		[SerializeField]
		private Vector2 rockSizeRange = new Vector2(0.5f, 2f);

		// Token: 0x04001825 RID: 6181
		[SerializeField]
		private AnimationCurve rockCountVsLavaProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04001826 RID: 6182
		[SerializeField]
		private AnimationCurve rockLifetimeMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x04001827 RID: 6183
		[SerializeField]
		private AnimationCurve rockMaxSizeMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x04001828 RID: 6184
		[SerializeField]
		private AnimationCurve rockSizeVsLifetime = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04001829 RID: 6185
		[SerializeField]
		private AnimationCurve rockSurfaceOffsetVsLifetime = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400182A RID: 6186
		private List<LavaSurfaceRockGenerator.LavaRock> activeRocks = new List<LavaSurfaceRockGenerator.LavaRock>();

		// Token: 0x02000506 RID: 1286
		private struct LavaRock
		{
			// Token: 0x06001F54 RID: 8020 RVA: 0x000A1FAE File Offset: 0x000A01AE
			public float GetLifetimeProgress(double currentTime, float lifetimeMultiplier)
			{
				return Mathf.Clamp01((float)(currentTime - this.spawnTime) / (this.lifetime * lifetimeMultiplier));
			}

			// Token: 0x040020F2 RID: 8434
			public Vector3 position;

			// Token: 0x040020F3 RID: 8435
			public Quaternion rotation;

			// Token: 0x040020F4 RID: 8436
			public float spawnSize;

			// Token: 0x040020F5 RID: 8437
			public float lifetime;

			// Token: 0x040020F6 RID: 8438
			public double spawnTime;

			// Token: 0x040020F7 RID: 8439
			public Rigidbody rigidbody;
		}
	}
}
