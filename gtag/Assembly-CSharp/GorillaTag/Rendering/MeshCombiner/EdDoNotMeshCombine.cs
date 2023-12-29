using System;
using UnityEngine;

namespace GorillaTag.Rendering.MeshCombiner
{
	public class EdDoNotMeshCombine : MonoBehaviour
	{
		protected void Awake()
		{
			Object.Destroy(this);
		}
	}
}
