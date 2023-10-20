using System;
using UnityEngine;

// Token: 0x02000141 RID: 321
public class GorillaQuitBox : GorillaTriggerBox
{
	// Token: 0x0600082E RID: 2094 RVA: 0x000330D1 File Offset: 0x000312D1
	private void Start()
	{
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x000330D3 File Offset: 0x000312D3
	public override void OnBoxTriggered()
	{
		Application.Quit();
	}
}
