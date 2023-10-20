using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000296 RID: 662
	public class CustomRopeSimulation : MonoBehaviour
	{
		// Token: 0x06001147 RID: 4423 RVA: 0x000612E0 File Offset: 0x0005F4E0
		private void Start()
		{
			Vector3 position = base.transform.position;
			for (int i = 0; i < this.nodeCount; i++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.ropeNodePrefab);
				gameObject.transform.parent = base.transform;
				gameObject.transform.position = position;
				this.nodes.Add(gameObject.transform);
				position.y -= this.nodeDistance;
			}
			this.nodes[this.nodes.Count - 1].GetComponentInChildren<Renderer>().enabled = false;
			this.burstNodes = new NativeArray<BurstRopeNode>(this.nodes.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x00061390 File Offset: 0x0005F590
		private void OnDestroy()
		{
			this.burstNodes.Dispose();
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x000613A0 File Offset: 0x0005F5A0
		private void Update()
		{
			new SolveRopeJob
			{
				fixedDeltaTime = Time.deltaTime,
				gravity = this.gravity,
				nodes = this.burstNodes,
				nodeDistance = this.nodeDistance,
				rootPos = base.transform.position
			}.Run<SolveRopeJob>();
			for (int i = 0; i < this.burstNodes.Length; i++)
			{
				this.nodes[i].position = this.burstNodes[i].curPos;
				if (i > 0)
				{
					Vector3 a = this.burstNodes[i - 1].curPos - this.burstNodes[i].curPos;
					this.nodes[i].up = -a;
				}
			}
		}

		// Token: 0x040013CB RID: 5067
		private List<Transform> nodes = new List<Transform>();

		// Token: 0x040013CC RID: 5068
		[SerializeField]
		private GameObject ropeNodePrefab;

		// Token: 0x040013CD RID: 5069
		[SerializeField]
		private int nodeCount = 10;

		// Token: 0x040013CE RID: 5070
		[SerializeField]
		private float nodeDistance = 0.4f;

		// Token: 0x040013CF RID: 5071
		[SerializeField]
		private int applyConstraintIterations = 20;

		// Token: 0x040013D0 RID: 5072
		[SerializeField]
		private Vector3 gravity = new Vector3(0f, -9.81f, 0f);

		// Token: 0x040013D1 RID: 5073
		[SerializeField]
		private float friction = 0.1f;

		// Token: 0x040013D2 RID: 5074
		private NativeArray<BurstRopeNode> burstNodes;
	}
}
