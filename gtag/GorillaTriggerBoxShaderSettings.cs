using System;
using GorillaTag.Rendering;
using UnityEngine;

public class GorillaTriggerBoxShaderSettings : GorillaTriggerBox
{
	private void Awake()
	{
		this.settingsRef.TryResolve<ZoneShaderSettings>(out this.settings);
	}

	public override void OnBoxTriggered()
	{
		if (this.settings != null)
		{
			this.settings.BecomeActiveInstance();
			return;
		}
		ZoneShaderSettings.ActivateDefaultSettings();
	}

	public GorillaTriggerBoxShaderSettings()
	{
	}

	[SerializeField]
	private XSceneRef settingsRef;

	private ZoneShaderSettings settings;
}
