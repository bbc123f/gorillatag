using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag
{
	public class LavaSurfaceRockGenerator : MonoBehaviourPun
	{
		private void Update()
		{
			double currentTime = PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time);
			this.RemoveExpiredRocks(currentTime);
			this.SpawnNewRocks(currentTime);
			this.UpdateActiveRocks(currentTime);
		}

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

		[PunRPC]
		public void SpawnRockRPC(Vector2 surfacePosLocal, Quaternion rotation, float spawnSize, float lifetime, double spawnTime, PhotonMessageInfo info)
		{
			GorillaNot.IncrementRPCCall(info, "SpawnRockRPC");
			this.SpawnRockLocal(surfacePosLocal, rotation, spawnSize, lifetime, spawnTime);
		}

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

		[SerializeField]
		private RisingLavaManager lavaManager;

		[SerializeField]
		private Transform lavaSurfacePlane;

		[SerializeField]
		private GameObject rockPrefab;

		[SerializeField]
		private float scaleFactor = 0.03f;

		[SerializeField]
		private Vector2 surfaceRadiusSpawnRange = new Vector2(0.1f, 0.7f);

		[SerializeField]
		private Vector2 rockLifetimeRange = new Vector2(5f, 10f);

		[SerializeField]
		private Vector2 rockSizeRange = new Vector2(0.5f, 2f);

		[SerializeField]
		private AnimationCurve rockCountVsLavaProgress = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[SerializeField]
		private AnimationCurve rockLifetimeMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		[SerializeField]
		private AnimationCurve rockMaxSizeMultiplierVsLavaProgress = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		[SerializeField]
		private AnimationCurve rockSizeVsLifetime = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[SerializeField]
		private AnimationCurve rockSurfaceOffsetVsLifetime = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		private List<LavaSurfaceRockGenerator.LavaRock> activeRocks = new List<LavaSurfaceRockGenerator.LavaRock>();

		private struct LavaRock
		{
			public float GetLifetimeProgress(double currentTime, float lifetimeMultiplier)
			{
				return Mathf.Clamp01((float)(currentTime - this.spawnTime) / (this.lifetime * lifetimeMultiplier));
			}

			public Vector3 position;

			public Quaternion rotation;

			public float spawnSize;

			public float lifetime;

			public double spawnTime;

			public Rigidbody rigidbody;
		}
	}
}
