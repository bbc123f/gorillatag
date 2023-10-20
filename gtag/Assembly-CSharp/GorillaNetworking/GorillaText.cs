using System;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaNetworking
{
	// Token: 0x020002B3 RID: 691
	[Serializable]
	public class GorillaText
	{
		// Token: 0x0600123E RID: 4670 RVA: 0x00068D7A File Offset: 0x00066F7A
		public void Initialize(MeshRenderer meshRenderer_, Material failureMaterial_)
		{
			this.meshRenderer = meshRenderer_;
			this.failureMaterial = failureMaterial_;
			this.originalMaterials = this.meshRenderer.materials;
			this.originalText = this.text.text;
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x0600123F RID: 4671 RVA: 0x00068DAC File Offset: 0x00066FAC
		// (set) Token: 0x06001240 RID: 4672 RVA: 0x00068DB4 File Offset: 0x00066FB4
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

		// Token: 0x06001241 RID: 4673 RVA: 0x00068DD4 File Offset: 0x00066FD4
		public void EnableFailedState(string failText)
		{
			this.failedState = true;
			this.text.text = failText;
			this.failureText = failText;
			Material[] materials = this.meshRenderer.materials;
			materials[0] = this.failureMaterial;
			this.meshRenderer.materials = materials;
		}

		// Token: 0x06001242 RID: 4674 RVA: 0x00068E1C File Offset: 0x0006701C
		public void DisableFailedState()
		{
			this.failedState = true;
			this.text.text = this.originalText;
			this.failureText = "";
			this.meshRenderer.materials = this.originalMaterials;
		}

		// Token: 0x04001528 RID: 5416
		[SerializeField]
		private Text text;

		// Token: 0x04001529 RID: 5417
		private string failureText;

		// Token: 0x0400152A RID: 5418
		private string originalText;

		// Token: 0x0400152B RID: 5419
		private bool failedState;

		// Token: 0x0400152C RID: 5420
		private Material[] originalMaterials;

		// Token: 0x0400152D RID: 5421
		private Material failureMaterial;

		// Token: 0x0400152E RID: 5422
		private MeshRenderer meshRenderer;
	}
}
