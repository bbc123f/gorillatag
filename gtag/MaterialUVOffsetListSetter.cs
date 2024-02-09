using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MaterialUVOffsetListSetter : MonoBehaviour
{
	private void Awake()
	{
		this.matPropertyBlock = new MaterialPropertyBlock();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshRenderer.GetPropertyBlock(this.matPropertyBlock);
	}

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

	[SerializeField]
	private List<Vector2> uvOffsetList = new List<Vector2>();

	private MeshRenderer meshRenderer;

	private MaterialPropertyBlock matPropertyBlock;

	private int shaderPropertyID = Shader.PropertyToID("_BaseMap_ST");
}
