using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000217 RID: 535
[RequireComponent(typeof(MeshRenderer))]
public class MaterialUVOffsetListSetter : MonoBehaviour
{
	// Token: 0x06000D4B RID: 3403 RVA: 0x0004DDB3 File Offset: 0x0004BFB3
	private void Awake()
	{
		this.matPropertyBlock = new MaterialPropertyBlock();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshRenderer.GetPropertyBlock(this.matPropertyBlock);
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x0004DDE0 File Offset: 0x0004BFE0
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

	// Token: 0x04001076 RID: 4214
	[SerializeField]
	private List<Vector2> uvOffsetList = new List<Vector2>();

	// Token: 0x04001077 RID: 4215
	private MeshRenderer meshRenderer;

	// Token: 0x04001078 RID: 4216
	private MaterialPropertyBlock matPropertyBlock;

	// Token: 0x04001079 RID: 4217
	private int shaderPropertyID = Shader.PropertyToID("_BaseMap_ST");
}
