using System;
using GorillaNetworking;

// Token: 0x020001C3 RID: 451
public class UnlockCompButton : GorillaPressableButton
{
	// Token: 0x06000B75 RID: 2933 RVA: 0x0004696D File Offset: 0x00044B6D
	public override void Start()
	{
		this.initialized = false;
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x00046978 File Offset: 0x00044B78
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

	// Token: 0x06000B77 RID: 2935 RVA: 0x000469D0 File Offset: 0x00044BD0
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

	// Token: 0x04000EF0 RID: 3824
	public string gameMode;

	// Token: 0x04000EF1 RID: 3825
	private bool initialized;
}
