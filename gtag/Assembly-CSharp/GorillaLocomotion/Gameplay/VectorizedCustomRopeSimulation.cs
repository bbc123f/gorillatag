using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020002A2 RID: 674
	public class VectorizedCustomRopeSimulation : MonoBehaviour
	{
		// Token: 0x0600118A RID: 4490 RVA: 0x00063007 File Offset: 0x00061207
		private void Awake()
		{
			VectorizedCustomRopeSimulation.instance = this;
		}

		// Token: 0x0600118B RID: 4491 RVA: 0x0006300F File Offset: 0x0006120F
		public static void Register(GorillaRopeSwing rope)
		{
			VectorizedCustomRopeSimulation.registerQueue.Add(rope);
		}

		// Token: 0x0600118C RID: 4492 RVA: 0x0006301C File Offset: 0x0006121C
		public static void Unregister(GorillaRopeSwing rope)
		{
			VectorizedCustomRopeSimulation.deregisterQueue.Add(rope);
		}

		// Token: 0x0600118D RID: 4493 RVA: 0x0006302C File Offset: 0x0006122C
		private void RegenerateData()
		{
			this.Dispose();
			foreach (GorillaRopeSwing item in VectorizedCustomRopeSimulation.registerQueue)
			{
				this.ropes.Add(item);
			}
			foreach (GorillaRopeSwing item2 in VectorizedCustomRopeSimulation.deregisterQueue)
			{
				this.ropes.Remove(item2);
			}
			VectorizedCustomRopeSimulation.registerQueue.Clear();
			VectorizedCustomRopeSimulation.deregisterQueue.Clear();
			int num = this.ropes.Count;
			while (num % 4 != 0)
			{
				num++;
			}
			int num2 = num * 32 / 4;
			this.burstData = new VectorizedBurstRopeData
			{
				posX = new NativeArray<float4>(num2, Allocator.Persistent, NativeArrayOptions.ClearMemory),
				posY = new NativeArray<float4>(num2, Allocator.Persistent, NativeArrayOptions.ClearMemory),
				posZ = new NativeArray<float4>(num2, Allocator.Persistent, NativeArrayOptions.ClearMemory),
				validNodes = new NativeArray<int4>(num2, Allocator.Persistent, NativeArrayOptions.ClearMemory),
				lastPosX = new NativeArray<float4>(num2, Allocator.Persistent, NativeArrayOptions.ClearMemory),
				lastPosY = new NativeArray<float4>(num2, Allocator.Persistent, NativeArrayOptions.ClearMemory),
				lastPosZ = new NativeArray<float4>(num2, Allocator.Persistent, NativeArrayOptions.ClearMemory),
				ropeRoots = new NativeArray<float3>(num, Allocator.Persistent, NativeArrayOptions.ClearMemory),
				nodeMass = new NativeArray<float4>(num2, Allocator.Persistent, NativeArrayOptions.ClearMemory)
			};
			for (int i = 0; i < this.ropes.Count; i += 4)
			{
				int num3 = 0;
				while (num3 < 4 && this.ropes.Count > i + num3)
				{
					this.ropes[i + num3].ropeDataStartIndex = 32 * i / 4;
					this.ropes[i + num3].ropeDataIndexOffset = num3;
					num3++;
				}
			}
			int num4 = 0;
			for (int j = 0; j < num2; j++)
			{
				float4 value = this.burstData.posX[j];
				float4 value2 = this.burstData.posY[j];
				float4 value3 = this.burstData.posZ[j];
				int4 value4 = this.burstData.validNodes[j];
				for (int k = 0; k < 4; k++)
				{
					int num5 = num4 * 4 + k;
					int num6 = j - num4 * 32;
					if (this.ropes.Count > num5 && this.ropes[num5].nodes.Length > num6)
					{
						Vector3 localPosition = this.ropes[num5].nodes[num6].localPosition;
						value[k] = localPosition.x;
						value2[k] = localPosition.y;
						value3[k] = localPosition.z;
						value4[k] = 1;
					}
					else
					{
						value[k] = 0f;
						value2[k] = 0f;
						value3[k] = 0f;
						value4[k] = 0;
					}
				}
				if (j > 0 && (j + 1) % 32 == 0)
				{
					num4++;
				}
				this.burstData.posX[j] = value;
				this.burstData.posY[j] = value2;
				this.burstData.posZ[j] = value3;
				this.burstData.lastPosX[j] = value;
				this.burstData.lastPosY[j] = value2;
				this.burstData.lastPosZ[j] = value3;
				this.burstData.validNodes[j] = value4;
				this.burstData.nodeMass[j] = math.float4(1f, 1f, 1f, 1f);
			}
			for (int l = 0; l < this.ropes.Count; l++)
			{
				this.burstData.ropeRoots[l] = float3.zero;
			}
		}

		// Token: 0x0600118E RID: 4494 RVA: 0x00063448 File Offset: 0x00061648
		private void Dispose()
		{
			if (!this.burstData.posX.IsCreated)
			{
				return;
			}
			this.burstData.posX.Dispose();
			this.burstData.posY.Dispose();
			this.burstData.posZ.Dispose();
			this.burstData.validNodes.Dispose();
			this.burstData.lastPosX.Dispose();
			this.burstData.lastPosY.Dispose();
			this.burstData.lastPosZ.Dispose();
			this.burstData.ropeRoots.Dispose();
			this.burstData.nodeMass.Dispose();
		}

		// Token: 0x0600118F RID: 4495 RVA: 0x000634F8 File Offset: 0x000616F8
		private void OnDestroy()
		{
			this.Dispose();
		}

		// Token: 0x06001190 RID: 4496 RVA: 0x00063500 File Offset: 0x00061700
		public void SetRopePos(GorillaRopeSwing ropeTarget, Vector3[] positions, bool setCurPos, bool setLastPos, int onlySetIndex = -1)
		{
			if (!this.ropes.Contains(ropeTarget))
			{
				return;
			}
			int ropeDataIndexOffset = ropeTarget.ropeDataIndexOffset;
			for (int i = 0; i < positions.Length; i++)
			{
				if (onlySetIndex < 0 || i == onlySetIndex)
				{
					int index = ropeTarget.ropeDataStartIndex + i;
					if (setCurPos)
					{
						float4 value = this.burstData.posX[index];
						float4 value2 = this.burstData.posY[index];
						float4 value3 = this.burstData.posZ[index];
						value[ropeDataIndexOffset] = positions[i].x;
						value2[ropeDataIndexOffset] = positions[i].y;
						value3[ropeDataIndexOffset] = positions[i].z;
						this.burstData.posX[index] = value;
						this.burstData.posY[index] = value2;
						this.burstData.posZ[index] = value3;
					}
					if (setLastPos)
					{
						float4 value4 = this.burstData.lastPosX[index];
						float4 value5 = this.burstData.lastPosY[index];
						float4 value6 = this.burstData.lastPosZ[index];
						value4[ropeDataIndexOffset] = positions[i].x;
						value5[ropeDataIndexOffset] = positions[i].y;
						value6[ropeDataIndexOffset] = positions[i].z;
						this.burstData.lastPosX[index] = value4;
						this.burstData.lastPosY[index] = value5;
						this.burstData.lastPosZ[index] = value6;
					}
				}
			}
		}

		// Token: 0x06001191 RID: 4497 RVA: 0x000636B4 File Offset: 0x000618B4
		public void SetVelocity(GorillaRopeSwing ropeTarget, Vector3 velocity, bool wholeRope, int boneIndex = 1)
		{
			List<Vector3> list = new List<Vector3>();
			float maxLength = math.min(velocity.magnitude, 15f);
			int ropeDataStartIndex = ropeTarget.ropeDataStartIndex;
			int ropeDataIndexOffset = ropeTarget.ropeDataIndexOffset;
			for (int i = 0; i < ropeTarget.nodes.Length; i++)
			{
				Vector3 vector = new Vector3(this.burstData.lastPosX[ropeDataStartIndex + i][ropeDataIndexOffset], this.burstData.lastPosY[ropeDataStartIndex + i][ropeDataIndexOffset], this.burstData.lastPosZ[ropeDataStartIndex + i][ropeDataIndexOffset]);
				if ((wholeRope || boneIndex == i) && boneIndex > 0)
				{
					Vector3 vector2 = velocity / (float)boneIndex * (float)i;
					vector2 = Vector3.ClampMagnitude(vector2, maxLength);
					list.Add(vector += vector2 * this.lastDelta);
				}
				else
				{
					list.Add(vector);
				}
			}
			int onlySetIndex = -1;
			if (!wholeRope && boneIndex > 0)
			{
				onlySetIndex = boneIndex;
			}
			this.SetRopePos(ropeTarget, list.ToArray(), true, false, onlySetIndex);
		}

		// Token: 0x06001192 RID: 4498 RVA: 0x000637D8 File Offset: 0x000619D8
		public Vector3 GetNodeVelocity(GorillaRopeSwing ropeTarget, int nodeIndex)
		{
			int index = ropeTarget.ropeDataStartIndex + nodeIndex;
			int ropeDataIndexOffset = ropeTarget.ropeDataIndexOffset;
			Vector3 a = new Vector3(this.burstData.posX[index][ropeDataIndexOffset], this.burstData.posY[index][ropeDataIndexOffset], this.burstData.posZ[index][ropeDataIndexOffset]);
			Vector3 b = new Vector3(this.burstData.lastPosX[index][ropeDataIndexOffset], this.burstData.lastPosY[index][ropeDataIndexOffset], this.burstData.lastPosZ[index][ropeDataIndexOffset]);
			return (a - b) / this.lastDelta;
		}

		// Token: 0x06001193 RID: 4499 RVA: 0x000638B0 File Offset: 0x00061AB0
		public void SetMassForPlayers(GorillaRopeSwing ropeTarget, bool hasPlayers, int furthestBoneIndex = 0)
		{
			if (!this.ropes.Contains(ropeTarget))
			{
				return;
			}
			int ropeDataIndexOffset = ropeTarget.ropeDataIndexOffset;
			for (int i = 0; i < 32; i++)
			{
				int index = ropeTarget.ropeDataStartIndex + i;
				float4 value = this.burstData.nodeMass[index];
				if (hasPlayers && i == furthestBoneIndex + 1)
				{
					value[ropeDataIndexOffset] = 0.1f;
				}
				else
				{
					value[ropeDataIndexOffset] = 1f;
				}
				this.burstData.nodeMass[index] = value;
			}
		}

		// Token: 0x06001194 RID: 4500 RVA: 0x00063934 File Offset: 0x00061B34
		private void Update()
		{
			if (VectorizedCustomRopeSimulation.registerQueue.Count > 0 || VectorizedCustomRopeSimulation.deregisterQueue.Count > 0)
			{
				this.RegenerateData();
			}
			if (this.ropes.Count <= 0)
			{
				return;
			}
			float deltaTime = math.min(Time.deltaTime, 0.05f);
			VectorizedSolveRopeJob jobData = new VectorizedSolveRopeJob
			{
				applyConstraintIterations = this.applyConstraintIterations,
				finalPassIterations = this.finalPassIterations,
				lastDeltaTime = this.lastDelta,
				deltaTime = deltaTime,
				gravity = this.gravity,
				data = this.burstData,
				nodeDistance = this.nodeDistance,
				ropeCount = this.ropes.Count
			};
			jobData.Schedule(default(JobHandle)).Complete();
			for (int i = 0; i < this.ropes.Count; i++)
			{
				GorillaRopeSwing gorillaRopeSwing = this.ropes[i];
				if (!gorillaRopeSwing.isIdle || !gorillaRopeSwing.isFullyIdle)
				{
					int ropeDataIndexOffset = gorillaRopeSwing.ropeDataIndexOffset;
					for (int j = 0; j < gorillaRopeSwing.nodes.Length; j++)
					{
						int index = gorillaRopeSwing.ropeDataStartIndex + j;
						gorillaRopeSwing.nodes[j].localPosition = new Vector3(jobData.data.posX[index][ropeDataIndexOffset], jobData.data.posY[index][ropeDataIndexOffset], jobData.data.posZ[index][ropeDataIndexOffset]);
					}
					for (int k = 0; k < gorillaRopeSwing.nodes.Length - 1; k++)
					{
						Transform transform = gorillaRopeSwing.nodes[k];
						int ropeDataStartIndex = gorillaRopeSwing.ropeDataStartIndex;
						transform.up = -(gorillaRopeSwing.nodes[k + 1].localPosition - transform.localPosition);
					}
				}
			}
			this.lastDelta = deltaTime;
		}

		// Token: 0x04001433 RID: 5171
		public static VectorizedCustomRopeSimulation instance;

		// Token: 0x04001434 RID: 5172
		public const int MAX_NODE_COUNT = 32;

		// Token: 0x04001435 RID: 5173
		public const float MAX_ROPE_SPEED = 15f;

		// Token: 0x04001436 RID: 5174
		private List<Transform> nodes = new List<Transform>();

		// Token: 0x04001437 RID: 5175
		[SerializeField]
		private float nodeDistance = 1f;

		// Token: 0x04001438 RID: 5176
		[SerializeField]
		private int applyConstraintIterations = 20;

		// Token: 0x04001439 RID: 5177
		[SerializeField]
		private int finalPassIterations = 1;

		// Token: 0x0400143A RID: 5178
		[SerializeField]
		private float gravity = -0.15f;

		// Token: 0x0400143B RID: 5179
		[SerializeField]
		private float friction = 0.1f;

		// Token: 0x0400143C RID: 5180
		private VectorizedBurstRopeData burstData;

		// Token: 0x0400143D RID: 5181
		private float lastDelta = 0.02f;

		// Token: 0x0400143E RID: 5182
		private List<GorillaRopeSwing> ropes = new List<GorillaRopeSwing>();

		// Token: 0x0400143F RID: 5183
		private static List<GorillaRopeSwing> registerQueue = new List<GorillaRopeSwing>();

		// Token: 0x04001440 RID: 5184
		private static List<GorillaRopeSwing> deregisterQueue = new List<GorillaRopeSwing>();

		// Token: 0x04001441 RID: 5185
		private bool simulateExtraFrame;
	}
}
