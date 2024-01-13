using System;
using System.Collections.Generic;
using UnityEngine;

public class ApplyMaterialBlock : MonoBehaviour
{
	public Material materialToApplyBlock;

	public List<CustomMaterialData> customData;

	private void Start()
	{
		Apply();
	}

	public void Apply()
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		foreach (CustomMaterialData customDatum in customData)
		{
			switch (customDatum.type)
			{
			case SuportedTypes.Color:
				materialPropertyBlock.SetColor(customDatum.name, customDatum.color);
				break;
			case SuportedTypes.Float:
				materialPropertyBlock.SetFloat(customDatum.name, customDatum.@float);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		GetComponent<Renderer>().SetPropertyBlock(materialPropertyBlock);
	}
}
