using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MaterialUVOffsetListSetter : MonoBehaviour
{
	[SerializeField]
	private List<Vector2> uvOffsetList = new List<Vector2>();

	private MeshRenderer meshRenderer;

	private MaterialPropertyBlock matPropertyBlock;

	private int shaderPropertyID = Shader.PropertyToID("_MainTex_ST");

	private void Awake()
	{
		matPropertyBlock = new MaterialPropertyBlock();
		meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.GetPropertyBlock(matPropertyBlock);
	}

	public void SetUVOffset(int listIndex)
	{
		if (listIndex >= uvOffsetList.Count || listIndex < 0)
		{
			Debug.LogError("Invalid uv offset list index provided.");
			return;
		}
		Vector2 vector = uvOffsetList[listIndex];
		matPropertyBlock.SetVector(shaderPropertyID, new Vector4(1f, 1f, vector.x, vector.y));
		meshRenderer.SetPropertyBlock(matPropertyBlock);
	}
}
