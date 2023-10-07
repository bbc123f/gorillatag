using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000103 RID: 259
public class CosmeticStand : GorillaPressableButton
{
	// Token: 0x06000642 RID: 1602 RVA: 0x0002785C File Offset: 0x00025A5C
	public void InitializeCosmetic()
	{
		this.thisCosmeticItem = CosmeticsController.instance.allCosmetics.Find((CosmeticsController.CosmeticItem x) => this.thisCosmeticName == x.displayName);
		this.slotPriceText.text = this.thisCosmeticItem.itemCategory.ToString().ToUpper() + " " + this.thisCosmeticItem.cost.ToString();
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x000278CC File Offset: 0x00025ACC
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCosmeticStandButton(this);
	}

	// Token: 0x040007A4 RID: 1956
	public CosmeticsController.CosmeticItem thisCosmeticItem;

	// Token: 0x040007A5 RID: 1957
	public string thisCosmeticName;

	// Token: 0x040007A6 RID: 1958
	public HeadModel thisHeadModel;

	// Token: 0x040007A7 RID: 1959
	public Text slotPriceText;

	// Token: 0x040007A8 RID: 1960
	public Text addToCartText;

	// Token: 0x040007A9 RID: 1961
	[Tooltip("If this is true then this cosmetic stand should have already been updated when the 'Update Cosmetic Stands' button was pressed in the CosmeticsController inspector.")]
	public bool skipMe;
}
