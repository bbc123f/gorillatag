using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Rendering.MeshCombiner
{
	[DefaultExecutionOrder(-2147483648)]
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

		public EdMeshCombinerSceneProcessor()
		{
		}

		// Note: this type is marked as 'beforefieldinit'.
		static EdMeshCombinerSceneProcessor()
		{
		}

		[NonSerialized]
		private static string _dummyProp = "";

		public const int kMaxVertCount = 65535;

		public const bool kDebugAllowMeshCombining = true;

		public const bool debugVisualsOnly = true;

		private static Dictionary<Hash128, Mesh> meshCache;

		private static int aliveInstanceCount;
	}
}
