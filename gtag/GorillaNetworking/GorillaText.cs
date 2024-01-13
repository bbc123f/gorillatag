using System;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaNetworking;

[Serializable]
public class GorillaText
{
	[SerializeField]
	private Text text;

	private string failureText;

	private string originalText;

	private bool failedState;

	private Material[] originalMaterials;

	private Material failureMaterial;

	private MeshRenderer meshRenderer;

	public string Text
	{
		get
		{
			return originalText;
		}
		set
		{
			originalText = value;
			if (!failedState)
			{
				text.text = value;
			}
		}
	}

	public void Initialize(MeshRenderer meshRenderer_, Material failureMaterial_)
	{
		meshRenderer = meshRenderer_;
		failureMaterial = failureMaterial_;
		originalMaterials = meshRenderer.materials;
		originalText = text.text;
	}

	public void EnableFailedState(string failText)
	{
		failedState = true;
		text.text = failText;
		failureText = failText;
		Material[] materials = meshRenderer.materials;
		materials[0] = failureMaterial;
		meshRenderer.materials = materials;
	}

	public void DisableFailedState()
	{
		failedState = true;
		text.text = originalText;
		failureText = "";
		meshRenderer.materials = originalMaterials;
	}
}
