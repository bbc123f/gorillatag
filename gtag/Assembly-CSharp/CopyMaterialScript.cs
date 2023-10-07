using System;
using UnityEngine;

// Token: 0x020001E9 RID: 489
public class CopyMaterialScript : MonoBehaviour
{
	// Token: 0x06000CB5 RID: 3253 RVA: 0x0004BF68 File Offset: 0x0004A168
	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x0004BF7F File Offset: 0x0004A17F
	private void Update()
	{
		if (this.sourceToCopyMaterialFrom.material != this.mySkinnedMeshRenderer.material)
		{
			this.mySkinnedMeshRenderer.material = this.sourceToCopyMaterialFrom.material;
		}
	}

	// Token: 0x04001005 RID: 4101
	public SkinnedMeshRenderer sourceToCopyMaterialFrom;

	// Token: 0x04001006 RID: 4102
	public SkinnedMeshRenderer mySkinnedMeshRenderer;
}
