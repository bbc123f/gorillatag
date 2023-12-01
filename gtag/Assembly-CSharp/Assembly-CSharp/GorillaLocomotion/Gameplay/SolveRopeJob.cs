using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	[BurstCompile]
	public struct SolveRopeJob : IJob
	{
		public void Execute()
		{
			this.Simulate();
			for (int i = 0; i < 20; i++)
			{
				this.ApplyConstraint();
			}
		}

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

		[ReadOnly]
		public float fixedDeltaTime;

		[WriteOnly]
		public NativeArray<BurstRopeNode> nodes;

		[ReadOnly]
		public Vector3 gravity;

		[ReadOnly]
		public Vector3 rootPos;

		[ReadOnly]
		public float nodeDistance;
	}
}
