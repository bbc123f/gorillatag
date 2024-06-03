using System;
using UnityEngine;

public class GameModePageButton : GorillaPressableButton
{
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.selector.ChangePage(this.left);
	}

	public GameModePageButton()
	{
	}

	[SerializeField]
	private GameModePages selector;

	[SerializeField]
	private bool left;
}
