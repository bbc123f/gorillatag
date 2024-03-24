﻿using System;
using UnityEngine;

public class GameModeSelectButton : GorillaPressableButton
{
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.selector.SelectEntryOnPage(this.buttonIndex);
	}

	public GameModeSelectButton()
	{
	}

	[SerializeField]
	internal GameModePages selector;

	[SerializeField]
	internal int buttonIndex;
}
