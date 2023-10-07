using System;
using GorillaNetworking;

// Token: 0x020001C2 RID: 450
public class UnlockCompButton : GorillaPressableButton
{
	// Token: 0x06000B6F RID: 2927 RVA: 0x00046705 File Offset: 0x00044905
	public override void Start()
	{
		this.initialized = false;
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x00046710 File Offset: 0x00044910
	public void Update()
	{
		if (this.testPress)
		{
			this.testPress = false;
			this.ButtonActivation();
		}
		if (!this.initialized && GorillaComputer.instance != null)
		{
			this.isOn = GorillaComputer.instance.allowedInCompetitive;
			this.UpdateColor();
			this.initialized = true;
		}
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x00046768 File Offset: 0x00044968
	public override void ButtonActivation()
	{
		if (!this.isOn)
		{
			base.ButtonActivation();
			GorillaComputer.instance.CompQueueUnlockButtonPress();
			this.isOn = true;
			this.UpdateColor();
		}
	}

	// Token: 0x04000EEC RID: 3820
	public string gameMode;

	// Token: 0x04000EED RID: 3821
	private bool initialized;
}
