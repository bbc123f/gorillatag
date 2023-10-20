using System;
using UnityEngine;

// Token: 0x020001B0 RID: 432
public class GorillaDevButton : GorillaPressableButton
{
	// Token: 0x17000084 RID: 132
	// (get) Token: 0x06000B09 RID: 2825 RVA: 0x00044439 File Offset: 0x00042639
	// (set) Token: 0x06000B0A RID: 2826 RVA: 0x00044441 File Offset: 0x00042641
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

	// Token: 0x06000B0B RID: 2827 RVA: 0x00044459 File Offset: 0x00042659
	public void OnEnable()
	{
		this.UpdateColor();
	}

	// Token: 0x04000E42 RID: 3650
	public DevButtonType Type;

	// Token: 0x04000E43 RID: 3651
	public LogType levelType;

	// Token: 0x04000E44 RID: 3652
	public DevConsoleInstance targetConsole;

	// Token: 0x04000E45 RID: 3653
	public int lineNumber;

	// Token: 0x04000E46 RID: 3654
	public bool repeatIfHeld;

	// Token: 0x04000E47 RID: 3655
	public float holdForSeconds;

	// Token: 0x04000E48 RID: 3656
	private Coroutine pressCoroutine;
}
