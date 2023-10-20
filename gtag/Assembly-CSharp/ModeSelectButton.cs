using System;
using GorillaNetworking;

// Token: 0x020001A1 RID: 417
public class ModeSelectButton : GorillaPressableButton
{
	// Token: 0x06000AC2 RID: 2754 RVA: 0x00042420 File Offset: 0x00040620
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		GorillaComputer.instance.OnModeSelectButtonPress(this.gameMode, isLeftHand);
	}

	// Token: 0x04000D8A RID: 3466
	public string gameMode;
}
