using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000218 RID: 536
[RequireComponent(typeof(MeshRenderer))]
public class MaterialUVOffsetListSetter : MonoBehaviour
{
	// Token: 0x06000D51 RID: 3409 RVA: 0x0004E013 File Offset: 0x0004C213
	private void Awake()
	{
		this.matPropertyBlock = new MaterialPropertyBlock();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshRenderer.GetPropertyBlock(this.matPropertyBlock);
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x0004E040 File Offset: 0x0004C240
	public void SetUVOffset(int listIndex)
	{
		if (listIndex >= this.uvOffsetList.Count || listIndex < 0)
		{
			Debug.LogError("Invalid uv offset list index provided.");
			return;
		}
		Vector2 vector = this.uvOffsetList[listIndex];
		this.matPropertyBlock.SetVector(this.shaderPropertyID, new Vector4(1f, 1f, vector.x, vector.y));
		this.meshRenderer.SetPropertyBlock(this.matPropertyBlock);
	}

	// Token: 0x0400107B RID: 4219
	[SerializeField]
	private List<Vector2> uvOffsetList = new List<Vector2>();

	// Token: 0x0400107C RID: 4220
	private MeshRenderer meshRenderer;

	// Token: 0x0400107D RID: 4221
	private MaterialPropertyBlock matPropertyBlock;

	// Token: 0x0400107E RID: 4222
	private int shaderPropertyID = Shader.PropertyToID("_BaseMap_ST");
}
