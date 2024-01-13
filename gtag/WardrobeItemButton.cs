using GorillaNetworking;

public class WardrobeItemButton : GorillaPressableButton
{
	public HeadModel controlledModel;

	public CosmeticsController.CosmeticItem currentCosmeticItem;

	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		CosmeticsController.instance.PressWardrobeItemButton(currentCosmeticItem, isLeftHand);
	}
}
