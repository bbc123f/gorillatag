using System;
using GorillaNetworking;

// Token: 0x02000127 RID: 295
public class WardrobeItemButton : GorillaPressableButton
{
	// Token: 0x060007BF RID: 1983 RVA: 0x000312B0 File Offset: 0x0002F4B0
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		CosmeticsController.instance.PressWardrobeItemButton(this.currentCosmeticItem, isLeftHand);
	}

	// Token: 0x04000957 RID: 2391
	public HeadModel controlledModel;

	// Token: 0x04000958 RID: 2392
	public CosmeticsController.CosmeticItem currentCosmeticItem;
}
