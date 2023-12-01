using System;
using UnityEngine;

public class GorillaQuitBox : GorillaTriggerBox
{
	private void Start()
	{
	}

	public override void OnBoxTriggered()
	{
		Application.Quit();
	}
}
