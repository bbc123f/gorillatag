using System;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaNetworking
{
	[Serializable]
	public class GorillaText
	{
		public void Initialize(MeshRenderer meshRenderer_, Material failureMaterial_)
		{
			this.meshRenderer = meshRenderer_;
			this.failureMaterial = failureMaterial_;
			this.originalMaterials = this.meshRenderer.materials;
			this.originalText = this.text.text;
		}

		public string Text
		{
			get
			{
				return this.originalText;
			}
			set
			{
				this.originalText = value;
				if (!this.failedState)
				{
					this.text.text = value;
				}
			}
		}

		public void EnableFailedState(string failText)
		{
			this.failedState = true;
			this.text.text = failText;
			this.failureText = failText;
			Material[] materials = this.meshRenderer.materials;
			materials[0] = this.failureMaterial;
			this.meshRenderer.materials = materials;
		}

		public void DisableFailedState()
		{
			this.failedState = true;
			this.text.text = this.originalText;
			this.failureText = "";
			this.meshRenderer.materials = this.originalMaterials;
		}

		[SerializeField]
		private Text text;

		private string failureText;

		private string originalText;

		private bool failedState;

		private Material[] originalMaterials;

		private Material failureMaterial;

		private MeshRenderer meshRenderer;
	}
}
