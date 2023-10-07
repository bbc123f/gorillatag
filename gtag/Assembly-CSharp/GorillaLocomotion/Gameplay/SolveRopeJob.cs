using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000296 RID: 662
	[BurstCompile]
	public struct SolveRopeJob : IJob
	{
		// Token: 0x06001144 RID: 4420 RVA: 0x00061104 File Offset: 0x0005F304
		public void Execute()
		{
			this.Simulate();
			for (int i = 0; i < 20; i++)
			{
				this.ApplyConstraint();
			}
		}

		// Token: 0x06001145 RID: 4421 RVA: 0x0006112C File Offset: 0x0005F32C
		private void Simulate()
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				BurstRopeNode burstRopeNode = this.nodes[i];
				Vector3 b = burstRopeNode.curPos - burstRopeNode.lastPos;
				burstRopeNode.lastPos = burstRopeNode.curPos;
				Vector3 vector = burstRopeNode.curPos + b;
				vector += this.gravity * this.fixedDeltaTime;
				burstRopeNode.curPos = vector;
				this.nodes[i] = burstRopeNode;
			}
		}

		// Token: 0x06001146 RID: 4422 RVA: 0x000611B8 File Offset: 0x0005F3B8
		private void ApplyConstraint()
		{
			BurstRopeNode value = this.nodes[0];
			value.curPos = this.rootPos;
			this.nodes[0] = value;
			for (int i = 0; i < this.nodes.Length - 1; i++)
			{
				BurstRopeNode burstRopeNode = this.nodes[i];
				BurstRopeNode burstRopeNode2 = this.nodes[i + 1];
				float magnitude = (burstRopeNode.curPos - burstRopeNode2.curPos).magnitude;
				float d = Mathf.Abs(magnitude - this.nodeDistance);
				Vector3 a = Vector3.zero;
				if (magnitude > this.nodeDistance)
				{
					a = (burstRopeNode.curPos - burstRopeNode2.curPos).normalized;
				}
				else if (magnitude < this.nodeDistance)
				{
					a = (burstRopeNode2.curPos - burstRopeNode.curPos).normalized;
				}
				Vector3 a2 = a * d;
				burstRopeNode.curPos -= a2 * 0.5f;
				burstRopeNode2.curPos += a2 * 0.5f;
				this.nodes[i] = burstRopeNode;
				this.nodes[i + 1] = burstRopeNode2;
			}
		}

		// Token: 0x040013C8 RID: 5064
		[ReadOnly]
		public float fixedDeltaTime;

		// Token: 0x040013C9 RID: 5065
		[WriteOnly]
		public NativeArray<BurstRopeNode> nodes;

		// Token: 0x040013CA RID: 5066
		[ReadOnly]
		public Vector3 gravity;

		// Token: 0x040013CB RID: 5067
		[ReadOnly]
		public Vector3 rootPos;

		// Token: 0x040013CC RID: 5068
		[ReadOnly]
		public float nodeDistance;
	}
}
