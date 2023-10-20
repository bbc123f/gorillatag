using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000298 RID: 664
	[BurstCompile]
	public struct SolveRopeJob : IJob
	{
		// Token: 0x0600114B RID: 4427 RVA: 0x000614E0 File Offset: 0x0005F6E0
		public void Execute()
		{
			this.Simulate();
			for (int i = 0; i < 20; i++)
			{
				this.ApplyConstraint();
			}
		}

		// Token: 0x0600114C RID: 4428 RVA: 0x00061508 File Offset: 0x0005F708
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

		// Token: 0x0600114D RID: 4429 RVA: 0x00061594 File Offset: 0x0005F794
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

		// Token: 0x040013D5 RID: 5077
		[ReadOnly]
		public float fixedDeltaTime;

		// Token: 0x040013D6 RID: 5078
		[WriteOnly]
		public NativeArray<BurstRopeNode> nodes;

		// Token: 0x040013D7 RID: 5079
		[ReadOnly]
		public Vector3 gravity;

		// Token: 0x040013D8 RID: 5080
		[ReadOnly]
		public Vector3 rootPos;

		// Token: 0x040013D9 RID: 5081
		[ReadOnly]
		public float nodeDistance;
	}
}
