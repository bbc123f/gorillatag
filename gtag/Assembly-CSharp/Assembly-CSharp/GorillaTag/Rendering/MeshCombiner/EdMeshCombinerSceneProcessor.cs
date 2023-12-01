using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Rendering.MeshCombiner
{
	public class EdMeshCombinerSceneProcessor : MonoBehaviour
	{
		protected void Awake()
		{
			if (Application.isPlaying)
			{
				Object.Destroy(this);
			}
		}

		protected void OnEnable()
		{
		}

		public const bool kDebugAllowMeshCombining = true;

		[NonSerialized]
		private static string _dummyProp = "";

		private static Dictionary<Hash128, Mesh> meshCache;

		private static int aliveInstanceCount;
	}
}
