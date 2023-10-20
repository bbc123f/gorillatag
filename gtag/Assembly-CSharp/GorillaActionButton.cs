﻿using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000057 RID: 87
public class GorillaActionButton : GorillaPressableButton
{
	// Token: 0x06000194 RID: 404 RVA: 0x0000C21E File Offset: 0x0000A41E
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.onPress.Invoke();
	}

	// Token: 0x04000279 RID: 633
	[SerializeField]
	public UnityEvent onPress;
}