using System;
using UnityEngine;

public class ZoneDef : MonoBehaviour
{
	public bool hasZone
	{
		get
		{
			return this.zone != GTZone.none;
		}
	}

	public bool hasSubZone
	{
		get
		{
			return this.subZone > GTSubZone.none;
		}
	}

	private void OnEnable()
	{
	}

	public GTZone zone;

	public GTSubZone subZone;

	public BoxCollider[] colliders;

	public BoxCollider activator;
}
