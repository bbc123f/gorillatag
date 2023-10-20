using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020002A4 RID: 676
	[BurstCompile(FloatPrecision.Low, FloatMode.Fast)]
	public struct VectorizedSolveRopeJob : IJob
	{
		// Token: 0x06001197 RID: 4503 RVA: 0x00063BC8 File Offset: 0x00061DC8
		public void Execute()
		{
			this.Simulate();
			for (int i = 0; i < this.applyConstraintIterations; i++)
			{
				this.ApplyConstraint();
			}
			for (int j = 0; j < this.finalPassIterations; j++)
			{
				this.FinalPass();
			}
		}

		// Token: 0x06001198 RID: 4504 RVA: 0x00063C0C File Offset: 0x00061E0C
		private void Simulate()
		{
			for (int i = 0; i < this.data.posX.Length; i++)
			{
				float4 lhs = (this.data.posX[i] - this.data.lastPosX[i]) / this.lastDeltaTime;
				float4 lhs2 = (this.data.posY[i] - this.data.lastPosY[i]) / this.lastDeltaTime;
				float4 lhs3 = (this.data.posZ[i] - this.data.lastPosZ[i]) / this.lastDeltaTime;
				this.data.lastPosX[i] = this.data.posX[i];
				this.data.lastPosY[i] = this.data.posY[i];
				this.data.lastPosZ[i] = this.data.posZ[i];
				float4 lhs4 = this.data.lastPosX[i] + lhs * this.deltaTime * 0.996f;
				float4 lhs5 = this.data.lastPosY[i] + lhs2 * this.deltaTime;
				float4 lhs6 = this.data.lastPosZ[i] + lhs3 * this.deltaTime * 0.996f;
				lhs5 += this.gravity * this.deltaTime;
				this.data.posX[i] = lhs4 * this.data.validNodes[i];
				this.data.posY[i] = lhs5 * this.data.validNodes[i];
				this.data.posZ[i] = lhs6 * this.data.validNodes[i];
			}
		}

		// Token: 0x06001199 RID: 4505 RVA: 0x00063E5C File Offset: 0x0006205C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void dot4(ref float4 ax, ref float4 ay, ref float4 az, ref float4 bx, ref float4 by, ref float4 bz, ref float4 output)
		{
			output = ax * bx + ay * by + az * bz;
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x00063EB0 File Offset: 0x000620B0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void length4(ref float4 xVals, ref float4 yVals, ref float4 zVals, ref float4 output)
		{
			float4 x = float4.zero;
			VectorizedSolveRopeJob.dot4(ref xVals, ref yVals, ref zVals, ref xVals, ref yVals, ref zVals, ref x);
			x = math.abs(x);
			output = math.sqrt(x);
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x00063EE4 File Offset: 0x000620E4
		private void ConstrainRoots()
		{
			int num = 0;
			for (int i = 0; i < this.data.posX.Length; i += 32)
			{
				for (int j = 0; j < 4; j++)
				{
					float4 value = this.data.posX[i];
					float4 value2 = this.data.posY[i];
					float4 value3 = this.data.posZ[i];
					value[j] = this.data.ropeRoots[num].x;
					value2[j] = this.data.ropeRoots[num].y;
					value3[j] = this.data.ropeRoots[num].z;
					this.data.posX[i] = value;
					this.data.posY[i] = value2;
					this.data.posZ[i] = value3;
					num++;
				}
			}
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x00063FF8 File Offset: 0x000621F8
		private void ApplyConstraint()
		{
			this.ConstrainRoots();
			float4 rhs = math.int4(-1, -1, -1, -1);
			for (int i = 0; i < this.ropeCount; i += 4)
			{
				for (int j = 0; j < 31; j++)
				{
					int num = i / 4 * 32 + j;
					float4 lhs = this.data.validNodes[num];
					float4 @float = this.data.validNodes[num + 1];
					if (math.lengthsq(@float) >= 0.1f)
					{
						float4 float2 = float4.zero;
						float4 lhs2 = this.data.posX[num] - this.data.posX[num + 1];
						float4 lhs3 = this.data.posY[num] - this.data.posY[num + 1];
						float4 lhs4 = this.data.posZ[num] - this.data.posZ[num + 1];
						VectorizedSolveRopeJob.length4(ref lhs2, ref lhs3, ref lhs4, ref float2);
						float4 rhs2 = math.abs(float2 - this.nodeDistance);
						float4 lhs5 = math.sign(float2 - this.nodeDistance);
						float2 += lhs - rhs;
						float2 += 0.01f;
						float4 rhs3 = lhs2 / float2;
						float4 rhs4 = lhs3 / float2;
						float4 rhs5 = lhs4 / float2;
						float4 lhs6 = lhs5 * rhs3 * rhs2;
						float4 lhs7 = lhs5 * rhs4 * rhs2;
						float4 lhs8 = lhs5 * rhs5 * rhs2;
						float4 rhs6 = this.data.nodeMass[num] / (this.data.nodeMass[num] + this.data.nodeMass[num + 1]);
						float4 rhs7 = this.data.nodeMass[num + 1] / (this.data.nodeMass[num] + this.data.nodeMass[num + 1]);
						ref NativeArray<float4> ptr = ref this.data.posX;
						int index = num;
						ptr[index] -= lhs6 * @float * rhs6;
						ptr = ref this.data.posY;
						index = num;
						ptr[index] -= lhs7 * @float * rhs6;
						ptr = ref this.data.posZ;
						index = num;
						ptr[index] -= lhs8 * @float * rhs6;
						ptr = ref this.data.posX;
						index = num + 1;
						ptr[index] += lhs6 * @float * rhs7;
						ptr = ref this.data.posY;
						index = num + 1;
						ptr[index] += lhs7 * @float * rhs7;
						ptr = ref this.data.posZ;
						index = num + 1;
						ptr[index] += lhs8 * @float * rhs7;
					}
				}
			}
		}

		// Token: 0x0600119D RID: 4509 RVA: 0x00064394 File Offset: 0x00062594
		private void FinalPass()
		{
			this.ConstrainRoots();
			float4 rhs = math.int4(-1, -1, -1, -1);
			for (int i = 0; i < this.ropeCount; i += 4)
			{
				for (int j = 0; j < 31; j++)
				{
					int num = i / 4 * 32 + j;
					this.data.validNodes[num];
					float4 rhs2 = this.data.validNodes[num + 1];
					float4 @float = float4.zero;
					float4 lhs = this.data.posX[num] - this.data.posX[num + 1];
					float4 lhs2 = this.data.posY[num] - this.data.posY[num + 1];
					float4 lhs3 = this.data.posZ[num] - this.data.posZ[num + 1];
					VectorizedSolveRopeJob.length4(ref lhs, ref lhs2, ref lhs3, ref @float);
					float4 rhs3 = math.abs(@float - this.nodeDistance);
					float4 lhs4 = math.sign(@float - this.nodeDistance);
					@float += this.data.validNodes[num] - rhs;
					@float += 0.01f;
					float4 rhs4 = lhs / @float;
					float4 rhs5 = lhs2 / @float;
					float4 rhs6 = lhs3 / @float;
					float4 lhs5 = lhs4 * rhs4 * rhs3;
					float4 lhs6 = lhs4 * rhs5 * rhs3;
					float4 lhs7 = lhs4 * rhs6 * rhs3;
					ref NativeArray<float4> ptr = ref this.data.posX;
					int index = num + 1;
					ptr[index] += lhs5 * rhs2;
					ptr = ref this.data.posY;
					index = num + 1;
					ptr[index] += lhs6 * rhs2;
					ptr = ref this.data.posZ;
					index = num + 1;
					ptr[index] += lhs7 * rhs2;
				}
			}
		}

		// Token: 0x0400144B RID: 5195
		[ReadOnly]
		public int applyConstraintIterations;

		// Token: 0x0400144C RID: 5196
		[ReadOnly]
		public int finalPassIterations;

		// Token: 0x0400144D RID: 5197
		[ReadOnly]
		public float deltaTime;

		// Token: 0x0400144E RID: 5198
		[ReadOnly]
		public float lastDeltaTime;

		// Token: 0x0400144F RID: 5199
		[ReadOnly]
		public int ropeCount;

		// Token: 0x04001450 RID: 5200
		public VectorizedBurstRopeData data;

		// Token: 0x04001451 RID: 5201
		[ReadOnly]
		public float gravity;

		// Token: 0x04001452 RID: 5202
		[ReadOnly]
		public float nodeDistance;
	}
}
