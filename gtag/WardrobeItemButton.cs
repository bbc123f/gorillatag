using System;
using GorillaNetworking;

public class WardrobeItemButton : GorillaPressableButton
{
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		CosmeticsController.instance.PressWardrobeItemButton(this.currentCosmeticItem, isLeftHand);
	}

	public HeadModel controlledModel;

	public CosmeticsController.CosmeticItem currentCosmeticItem;
}
