using System;
using UnityEngine;

namespace GorillaTag.Rendering.MeshCombiner
{
	// Token: 0x02000330 RID: 816
	public class EdDoNotMeshCombine : MonoBehaviour
	{
		// Token: 0x060016B1 RID: 5809 RVA: 0x0007E6BB File Offset: 0x0007C8BB
		protected void Awake()
		{
			Object.Destroy(this);
		}
	}
}
