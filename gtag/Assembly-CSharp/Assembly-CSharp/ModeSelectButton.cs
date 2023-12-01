using System;
using GorillaNetworking;

public class ModeSelectButton : GorillaPressableButton
{
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		GorillaComputer.instance.OnModeSelectButtonPress(this.gameMode, isLeftHand);
	}

	public string gameMode;
}
