using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GorillaLocomotion.Gameplay;

public class CustomRopeSimulation : MonoBehaviour
{
	private List<Transform> nodes = new List<Transform>();

	[SerializeField]
	private GameObject ropeNodePrefab;

	[SerializeField]
	private int nodeCount = 10;

	[SerializeField]
	private float nodeDistance = 0.4f;

	[SerializeField]
	private int applyConstraintIterations = 20;

	[SerializeField]
	private Vector3 gravity = new Vector3(0f, -9.81f, 0f);

	[SerializeField]
	private float friction = 0.1f;

	private NativeArray<BurstRopeNode> burstNodes;

	private void Start()
	{
		Vector3 position = base.transform.position;
		for (int i = 0; i < nodeCount; i++)
		{
			GameObject gameObject = Object.Instantiate(ropeNodePrefab);
			gameObject.transform.parent = base.transform;
			gameObject.transform.position = position;
			nodes.Add(gameObject.transform);
			position.y -= nodeDistance;
		}
		nodes[nodes.Count - 1].GetComponentInChildren<Renderer>().enabled = false;
		burstNodes = new NativeArray<BurstRopeNode>(nodes.Count, Allocator.Persistent);
	}

	private void OnDestroy()
	{
		burstNodes.Dispose();
	}

	private void Update()
	{
		SolveRopeJob jobData = default(SolveRopeJob);
		jobData.fixedDeltaTime = Time.deltaTime;
		jobData.gravity = gravity;
		jobData.nodes = burstNodes;
		jobData.nodeDistance = nodeDistance;
		jobData.rootPos = base.transform.position;
		jobData.Run();
		for (int i = 0; i < burstNodes.Length; i++)
		{
			nodes[i].position = burstNodes[i].curPos;
			if (i > 0)
			{
				Vector3 vector = burstNodes[i - 1].curPos - burstNodes[i].curPos;
				nodes[i].up = -vector;
			}
		}
	}
}
