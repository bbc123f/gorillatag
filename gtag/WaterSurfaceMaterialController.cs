using UnityEngine;

[ExecuteAlways]
public class WaterSurfaceMaterialController : MonoBehaviour
{
	public float ScrollX = 0.6f;

	public float ScrollY = 0.6f;

	public float Scale = 1f;

	private Renderer renderer;

	private MaterialPropertyBlock matPropBlock;

	private static readonly int shaderProp_ScrollSpeedAndScale = Shader.PropertyToID("_ScrollSpeedAndScale");

	protected void OnEnable()
	{
		renderer = GetComponent<Renderer>();
		matPropBlock = new MaterialPropertyBlock();
		ApplyProperties();
	}

	private void ApplyProperties()
	{
		matPropBlock.SetVector(shaderProp_ScrollSpeedAndScale, new Vector4(ScrollX, ScrollY, Scale, 0f));
		renderer.SetPropertyBlock(matPropBlock);
	}
}
