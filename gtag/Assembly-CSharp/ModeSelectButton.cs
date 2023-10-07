using System;
using GorillaNetworking;

// Token: 0x020001A0 RID: 416
public class ModeSelectButton : GorillaPressableButton
{
	// Token: 0x06000ABD RID: 2749 RVA: 0x000422E8 File Offset: 0x000404E8
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		GorillaComputer.instance.OnModeSelectButtonPress(this.gameMode, isLeftHand);
	}

	// Token: 0x04000D86 RID: 3462
	public string gameMode;
}
