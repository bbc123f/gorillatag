using GorillaNetworking;

public class ModeSelectButton : GorillaPressableButton
{
	public string gameMode;

	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		GorillaComputer.instance.OnModeSelectButtonPress(gameMode, isLeftHand);
	}
}
