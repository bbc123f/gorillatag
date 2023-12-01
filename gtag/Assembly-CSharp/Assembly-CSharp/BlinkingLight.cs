using System;
using UnityEngine;

public class BlinkingLight : MonoBehaviour
{
	private void Awake()
	{
		MeshRenderer meshRenderer;
		if (base.TryGetComponent<MeshRenderer>(out meshRenderer))
		{
			meshRenderer.material = this.materialArray[BlinkingLight.gColor.NextInt(this.materialArray.Length)];
		}
	}

	[SerializeField]
	private Material[] materialArray;

	private static SRand gColor = new SRand("BlinkingLight");
}
