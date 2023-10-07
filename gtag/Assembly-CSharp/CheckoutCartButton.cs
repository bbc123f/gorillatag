using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000FC RID: 252
public class CheckoutCartButton : GorillaPressableButton
{
	// Token: 0x06000630 RID: 1584 RVA: 0x000271FF File Offset: 0x000253FF
	public override void Start()
	{
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x00027214 File Offset: 0x00025414
	public override void UpdateColor()
	{
		if (this.currentCosmeticItem.itemName == "null")
		{
			this.button.material = this.unpressedMaterial;
			this.buttonText.text = this.noCosmeticText;
			return;
		}
		if (this.isOn)
		{
			this.button.material = this.pressedMaterial;
			this.buttonText.text = this.onText;
			return;
		}
		this.button.material = this.unpressedMaterial;
		this.buttonText.text = this.offText;
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x000272A8 File Offset: 0x000254A8
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCheckoutCartButton(this, isLeftHand);
	}

	// Token: 0x04000788 RID: 1928
	public CosmeticsController.CosmeticItem currentCosmeticItem;

	// Token: 0x04000789 RID: 1929
	public Image currentImage;

	// Token: 0x0400078A RID: 1930
	public MeshRenderer button;

	// Token: 0x0400078B RID: 1931
	public Material blank;

	// Token: 0x0400078C RID: 1932
	public string noCosmeticText;

	// Token: 0x0400078D RID: 1933
	public Text buttonText;
}
