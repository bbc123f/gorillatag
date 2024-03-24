using System;
using BuildSafe;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlinkingLight : SceneBakeTask
{
	public override void OnSceneBake(Scene scene, SceneBakeMode mode)
	{
		MeshRenderer meshRenderer;
		if (base.TryGetComponent<MeshRenderer>(out meshRenderer))
		{
			meshRenderer.material = this.materialArray[BlinkingLight.gColor.NextInt(this.materialArray.Length)];
		}
	}

	public BlinkingLight()
	{
	}

	// Note: this type is marked as 'beforefieldinit'.
	static BlinkingLight()
	{
	}

	[SerializeField]
	private Material[] materialArray;

	private static SRand gColor = new SRand("BlinkingLight");
}
