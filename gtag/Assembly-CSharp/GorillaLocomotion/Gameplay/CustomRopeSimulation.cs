using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000294 RID: 660
	public class CustomRopeSimulation : MonoBehaviour
	{
		// Token: 0x06001140 RID: 4416 RVA: 0x00060F04 File Offset: 0x0005F104
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

		// Token: 0x06001141 RID: 4417 RVA: 0x00060FB4 File Offset: 0x0005F1B4
		private void OnDestroy()
		{
			this.burstNodes.Dispose();
		}

		// Token: 0x06001142 RID: 4418 RVA: 0x00060FC4 File Offset: 0x0005F1C4
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

		// Token: 0x040013BE RID: 5054
		private List<Transform> nodes = new List<Transform>();

		// Token: 0x040013BF RID: 5055
		[SerializeField]
		private GameObject ropeNodePrefab;

		// Token: 0x040013C0 RID: 5056
		[SerializeField]
		private int nodeCount = 10;

		// Token: 0x040013C1 RID: 5057
		[SerializeField]
		private float nodeDistance = 0.4f;

		// Token: 0x040013C2 RID: 5058
		[SerializeField]
		private int applyConstraintIterations = 20;

		// Token: 0x040013C3 RID: 5059
		[SerializeField]
		private Vector3 gravity = new Vector3(0f, -9.81f, 0f);

		// Token: 0x040013C4 RID: 5060
		[SerializeField]
		private float friction = 0.1f;

		// Token: 0x040013C5 RID: 5061
		private NativeArray<BurstRopeNode> burstNodes;
	}
}
