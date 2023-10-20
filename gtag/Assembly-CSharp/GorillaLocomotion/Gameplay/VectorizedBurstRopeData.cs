using System;
using Unity.Collections;
using Unity.Mathematics;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020002A3 RID: 675
	public struct VectorizedBurstRopeData
	{
		// Token: 0x04001442 RID: 5186
		public NativeArray<float4> posX;

		// Token: 0x04001443 RID: 5187
		public NativeArray<float4> posY;

		// Token: 0x04001444 RID: 5188
		public NativeArray<float4> posZ;

		// Token: 0x04001445 RID: 5189
		public NativeArray<int4> validNodes;

		// Token: 0x04001446 RID: 5190
		public NativeArray<float4> lastPosX;

		// Token: 0x04001447 RID: 5191
		public NativeArray<float4> lastPosY;

		// Token: 0x04001448 RID: 5192
		public NativeArray<float4> lastPosZ;

		// Token: 0x04001449 RID: 5193
		public NativeArray<float3> ropeRoots;

		// Token: 0x0400144A RID: 5194
		public NativeArray<float4> nodeMass;
	}
}
