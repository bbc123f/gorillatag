using System;
using UnityEngine;

// Token: 0x020001AF RID: 431
public class GorillaDevButton : GorillaPressableButton
{
	// Token: 0x17000082 RID: 130
	// (get) Token: 0x06000B04 RID: 2820 RVA: 0x00044301 File Offset: 0x00042501
	// (set) Token: 0x06000B05 RID: 2821 RVA: 0x00044309 File Offset: 0x00042509
	public bool on
	{
		get
		{
			return this.isOn;
		}
		set
		{
			if (this.isOn != value)
			{
				this.isOn = value;
				this.UpdateColor();
			}
		}
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x00044321 File Offset: 0x00042521
	public void OnEnable()
	{
		this.UpdateColor();
	}

	// Token: 0x04000E3E RID: 3646
	public DevButtonType Type;

	// Token: 0x04000E3F RID: 3647
	public LogType levelType;

	// Token: 0x04000E40 RID: 3648
	public DevConsoleInstance targetConsole;

	// Token: 0x04000E41 RID: 3649
	public int lineNumber;

	// Token: 0x04000E42 RID: 3650
	public bool repeatIfHeld;

	// Token: 0x04000E43 RID: 3651
	public float holdForSeconds;

	// Token: 0x04000E44 RID: 3652
	private Coroutine pressCoroutine;
}
