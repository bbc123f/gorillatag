﻿using System;
using UnityEngine;
using UnityEngine.Events;

public class GorillaActionButton : GorillaPressableButton
{
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.onPress.Invoke();
	}

	public GorillaActionButton()
	{
	}

	[SerializeField]
	public UnityEvent onPress;
}
