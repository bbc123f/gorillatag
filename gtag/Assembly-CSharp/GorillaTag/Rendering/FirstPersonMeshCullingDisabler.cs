using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02000328 RID: 808
	public class FirstPersonMeshCullingDisabler : MonoBehaviour
	{
		// Token: 0x0600167A RID: 5754 RVA: 0x0007D250 File Offset: 0x0007B450
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

		// Token: 0x0600167B RID: 5755 RVA: 0x0007D2B4 File Offset: 0x0007B4B4
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

		// Token: 0x0400188B RID: 6283
		private Mesh[] meshes;

		// Token: 0x0400188C RID: 6284
		private Transform[] xforms;
	}
}
