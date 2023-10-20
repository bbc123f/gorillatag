using System;
using UnityEngine;

// Token: 0x020001EA RID: 490
public class CopyMaterialScript : MonoBehaviour
{
	// Token: 0x06000CBB RID: 3259 RVA: 0x0004C1D0 File Offset: 0x0004A3D0
	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x0004C1E7 File Offset: 0x0004A3E7
	private void Update()
	{
		if (this.sourceToCopyMaterialFrom.material != this.mySkinnedMeshRenderer.material)
		{
			this.mySkinnedMeshRenderer.material = this.sourceToCopyMaterialFrom.material;
		}
	}

	// Token: 0x04001009 RID: 4105
	public SkinnedMeshRenderer sourceToCopyMaterialFrom;

	// Token: 0x0400100A RID: 4106
	public SkinnedMeshRenderer mySkinnedMeshRenderer;
}
