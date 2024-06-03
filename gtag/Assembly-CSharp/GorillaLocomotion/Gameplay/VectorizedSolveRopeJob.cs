using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace GorillaLocomotion.Gameplay
{
	[BurstCompile(FloatPrecision.Low, FloatMode.Fast)]
	public struct VectorizedSolveRopeJob : IJob
	{
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void dot4(ref float4 ax, ref float4 ay, ref float4 az, ref float4 bx, ref float4 by, ref float4 bz, ref float4 output)
		{
			output = ax * bx + ay * by + az * bz;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void length4(ref float4 xVals, ref float4 yVals, ref float4 zVals, ref float4 output)
		{
			float4 x = float4.zero;
			VectorizedSolveRopeJob.dot4(ref xVals, ref yVals, ref zVals, ref xVals, ref yVals, ref zVals, ref x);
			x = math.abs(x);
			output = math.sqrt(x);
		}

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

		[ReadOnly]
		public int applyConstraintIterations;

		[ReadOnly]
		public int finalPassIterations;

		[ReadOnly]
		public float deltaTime;

		[ReadOnly]
		public float lastDeltaTime;

		[ReadOnly]
		public int ropeCount;

		public VectorizedBurstRopeData data;

		[ReadOnly]
		public float gravity;

		[ReadOnly]
		public float nodeDistance;
	}
}
