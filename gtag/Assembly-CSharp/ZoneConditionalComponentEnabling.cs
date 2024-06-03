using System;
using UnityEngine;

public class ZoneConditionalComponentEnabling : MonoBehaviour
{
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	private void OnZoneChanged()
	{
		if (this.invisibleWhileLoaded)
		{
			if (this.components != null)
			{
				for (int i = 0; i < this.components.Length; i++)
				{
					this.components[i].enabled = !ZoneManagement.IsInZone(this.zone);
				}
				return;
			}
		}
		else if (this.components != null)
		{
			for (int j = 0; j < this.components.Length; j++)
			{
				this.components[j].enabled = ZoneManagement.IsInZone(this.zone);
			}
		}
	}

	public ZoneConditionalComponentEnabling()
	{
	}

	[SerializeField]
	private GTZone zone;

	[SerializeField]
	private bool invisibleWhileLoaded;

	[SerializeField]
	private MonoBehaviour[] components;
}
