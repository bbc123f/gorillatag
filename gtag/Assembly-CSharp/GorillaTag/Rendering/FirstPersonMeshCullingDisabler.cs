using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	public class FirstPersonMeshCullingDisabler : MonoBehaviour
	{
		protected void Awake()
		{
			MeshFilter[] componentsInChildren = base.GetComponentsInChildren<MeshFilter>();
			if (componentsInChildren == null)
			{
				return;
			}
			this.meshes = new Mesh[componentsInChildren.Length];
			this.xforms = new Transform[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.meshes[i] = componentsInChildren[i].mesh;
				this.xforms[i] = componentsInChildren[i].transform;
			}
		}

		protected void OnEnable()
		{
			Camera main = Camera.main;
			if (main == null)
			{
				return;
			}
			Transform transform = main.transform;
			Vector3 position = transform.position;
			Vector3 a = Vector3.Normalize(transform.forward);
			float nearClipPlane = main.nearClipPlane;
			float d = (main.farClipPlane - nearClipPlane) / 2f + nearClipPlane;
			Vector3 position2 = position + a * d;
			for (int i = 0; i < this.meshes.Length; i++)
			{
				Vector3 center = this.xforms[i].InverseTransformPoint(position2);
				this.meshes[i].bounds = new Bounds(center, Vector3.one);
			}
		}

		public FirstPersonMeshCullingDisabler()
		{
		}

		private Mesh[] meshes;

		private Transform[] xforms;
	}
}
