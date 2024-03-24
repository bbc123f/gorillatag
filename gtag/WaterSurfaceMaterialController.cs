using System;
using UnityEngine;

[ExecuteAlways]
public class WaterSurfaceMaterialController : MonoBehaviour
{
	protected void OnEnable()
	{
		this.renderer = base.GetComponent<Renderer>();
		this.matPropBlock = new MaterialPropertyBlock();
		this.ApplyProperties();
	}

	private void ApplyProperties()
	{
		this.matPropBlock.SetVector(WaterSurfaceMaterialController.shaderProp_ScrollSpeedAndScale, new Vector4(this.ScrollX, this.ScrollY, this.Scale, 0f));
		this.renderer.SetPropertyBlock(this.matPropBlock);
	}

	public WaterSurfaceMaterialController()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static WaterSurfaceMaterialController()
	{
	}

	public float ScrollX = 0.6f;

	public float ScrollY = 0.6f;

	public float Scale = 1f;

	private Renderer renderer;

	private MaterialPropertyBlock matPropBlock;

	private static readonly int shaderProp_ScrollSpeedAndScale = Shader.PropertyToID("_ScrollSpeedAndScale");
}
