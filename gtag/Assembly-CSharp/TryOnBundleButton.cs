using System;
using GorillaNetworking;

public class TryOnBundleButton : GorillaPressableButton
{
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		CosmeticsController.instance.PressTryOnBundleButton(this, isLeftHand);
	}

	public TryOnBundleButton()
	{
	}

	public int bundleIndex;
}
