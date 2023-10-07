using System;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaNetworking
{
	// Token: 0x020002B1 RID: 689
	[Serializable]
	public class GorillaText
	{
		// Token: 0x06001237 RID: 4663 RVA: 0x000688AE File Offset: 0x00066AAE
		public void Initialize(MeshRenderer meshRenderer_, Material failureMaterial_)
		{
			this.meshRenderer = meshRenderer_;
			this.failureMaterial = failureMaterial_;
			this.originalMaterials = this.meshRenderer.materials;
			this.originalText = this.text.text;
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06001238 RID: 4664 RVA: 0x000688E0 File Offset: 0x00066AE0
		// (set) Token: 0x06001239 RID: 4665 RVA: 0x000688E8 File Offset: 0x00066AE8
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

		// Token: 0x0600123A RID: 4666 RVA: 0x00068908 File Offset: 0x00066B08
		public void EnableFailedState(string failText)
		{
			this.failedState = true;
			this.text.text = failText;
			this.failureText = failText;
			Material[] materials = this.meshRenderer.materials;
			materials[0] = this.failureMaterial;
			this.meshRenderer.materials = materials;
		}

		// Token: 0x0600123B RID: 4667 RVA: 0x00068950 File Offset: 0x00066B50
		public void DisableFailedState()
		{
			this.failedState = true;
			this.text.text = this.originalText;
			this.failureText = "";
			this.meshRenderer.materials = this.originalMaterials;
		}

		// Token: 0x0400151B RID: 5403
		[SerializeField]
		private Text text;

		// Token: 0x0400151C RID: 5404
		private string failureText;

		// Token: 0x0400151D RID: 5405
		private string originalText;

		// Token: 0x0400151E RID: 5406
		private bool failedState;

		// Token: 0x0400151F RID: 5407
		private Material[] originalMaterials;

		// Token: 0x04001520 RID: 5408
		private Material failureMaterial;

		// Token: 0x04001521 RID: 5409
		private MeshRenderer meshRenderer;
	}
}
