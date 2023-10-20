using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000105 RID: 261
public class FittingRoomButton : GorillaPressableButton
{
	// Token: 0x0600064D RID: 1613 RVA: 0x0002780C File Offset: 0x00025A0C
	public override void Start()
	{
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x00027820 File Offset: 0x00025A20
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

	// Token: 0x0600064F RID: 1615 RVA: 0x000278B4 File Offset: 0x00025AB4
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		CosmeticsController.instance.PressFittingRoomButton(this, isLeftHand);
	}

	// Token: 0x040007AA RID: 1962
	public CosmeticsController.CosmeticItem currentCosmeticItem;

	// Token: 0x040007AB RID: 1963
	public Image currentImage;

	// Token: 0x040007AC RID: 1964
	public MeshRenderer button;

	// Token: 0x040007AD RID: 1965
	public Material blank;

	// Token: 0x040007AE RID: 1966
	public string noCosmeticText;

	// Token: 0x040007AF RID: 1967
	public Text buttonText;
}
