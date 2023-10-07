using System;
using Unity.Collections;
using Unity.Mathematics;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020002A1 RID: 673
	public struct VectorizedBurstRopeData
	{
		// Token: 0x04001435 RID: 5173
		public NativeArray<float4> posX;

		// Token: 0x04001436 RID: 5174
		public NativeArray<float4> posY;

		// Token: 0x04001437 RID: 5175
		public NativeArray<float4> posZ;

		// Token: 0x04001438 RID: 5176
		public NativeArray<int4> validNodes;

		// Token: 0x04001439 RID: 5177
		public NativeArray<float4> lastPosX;

		// Token: 0x0400143A RID: 5178
		public NativeArray<float4> lastPosY;

		// Token: 0x0400143B RID: 5179
		public NativeArray<float4> lastPosZ;

		// Token: 0x0400143C RID: 5180
		public NativeArray<float3> ropeRoots;

		// Token: 0x0400143D RID: 5181
		public NativeArray<float4> nodeMass;
	}
}
