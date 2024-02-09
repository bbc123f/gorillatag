using System;
using GorillaNetworking;

public class ModeSelectButton : GorillaPressableButton
{
	public override void Start()
	{
		base.Start();
		GorillaComputer.instance.currentGameMode.AddCallback(new Action<string>(this.OnGameModeChanged), true);
	}

	private void OnDestroy()
	{
		if (!ApplicationQuittingState.IsQuitting)
		{
			GorillaComputer.instance.currentGameMode.RemoveCallback(new Action<string>(this.OnGameModeChanged));
		}
	}

	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		GorillaComputer.instance.OnModeSelectButtonPress(this.gameMode, isLeftHand);
	}

	public void OnGameModeChanged(string newGameMode)
	{
		this.buttonRenderer.material = ((newGameMode == this.gameMode) ? this.pressedMaterial : this.unpressedMaterial);
	}

	public string gameMode;
}
