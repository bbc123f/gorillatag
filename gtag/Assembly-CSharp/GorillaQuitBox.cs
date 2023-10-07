using System;
using UnityEngine;

// Token: 0x02000141 RID: 321
public class GorillaQuitBox : GorillaTriggerBox
{
	// Token: 0x0600082D RID: 2093 RVA: 0x00033291 File Offset: 0x00031491
	private void Start()
	{
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x00033293 File Offset: 0x00031493
	public override void OnBoxTriggered()
	{
		Application.Quit();
	}
}
