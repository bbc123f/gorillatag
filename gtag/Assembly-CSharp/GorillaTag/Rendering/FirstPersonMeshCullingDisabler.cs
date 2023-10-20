using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x0200032A RID: 810
	public class FirstPersonMeshCullingDisabler : MonoBehaviour
	{
		// Token: 0x06001683 RID: 5763 RVA: 0x0007D738 File Offset: 0x0007B938
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

		// Token: 0x06001684 RID: 5764 RVA: 0x0007D79C File Offset: 0x0007B99C
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

		// Token: 0x04001898 RID: 6296
		private Mesh[] meshes;

		// Token: 0x04001899 RID: 6297
		private Transform[] xforms;
	}
}
