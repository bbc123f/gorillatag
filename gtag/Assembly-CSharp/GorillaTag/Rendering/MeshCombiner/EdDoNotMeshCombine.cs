using System;
using UnityEngine;

namespace GorillaTag.Rendering.MeshCombiner
{
	// Token: 0x0200032E RID: 814
	public class EdDoNotMeshCombine : MonoBehaviour
	{
		// Token: 0x060016A8 RID: 5800 RVA: 0x0007E1D3 File Offset: 0x0007C3D3
		protected void Awake()
		{
			Object.Destroy(this);
		}
	}
}
