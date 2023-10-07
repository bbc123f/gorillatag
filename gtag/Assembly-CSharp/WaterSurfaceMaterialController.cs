using System;
using UnityEngine;

// Token: 0x02000023 RID: 35
[ExecuteAlways]
public class WaterSurfaceMaterialController : MonoBehaviour
{
	// Token: 0x060000B4 RID: 180 RVA: 0x00007559 File Offset: 0x00005759
	protected void OnEnable()
	{
		this.renderer = base.GetComponent<Renderer>();
		this.matPropBlock = new MaterialPropertyBlock();
		this.ApplyProperties();
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x00007578 File Offset: 0x00005778
	private void ApplyProperties()
	{
		this.matPropBlock.SetVector(WaterSurfaceMaterialController.shaderProp_ScrollSpeedAndScale, new Vector4(this.ScrollX, this.ScrollY, this.Scale, 0f));
		this.renderer.SetPropertyBlock(this.matPropBlock);
	}

	// Token: 0x040000D8 RID: 216
	public float ScrollX = 0.6f;

	// Token: 0x040000D9 RID: 217
	public float ScrollY = 0.6f;

	// Token: 0x040000DA RID: 218
	public float Scale = 1f;

	// Token: 0x040000DB RID: 219
	private Renderer renderer;

	// Token: 0x040000DC RID: 220
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x040000DD RID: 221
	private static readonly int shaderProp_ScrollSpeedAndScale = Shader.PropertyToID("_ScrollSpeedAndScale");
}
