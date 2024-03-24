using System;
using UnityEngine;

public class GorillaTriggerBoxTeleport : GorillaTriggerBox
{
	public override void OnBoxTriggered()
	{
		this.cameraOffest.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
		this.cameraOffest.transform.position = this.teleportLocation;
	}

	public GorillaTriggerBoxTeleport()
	{
	}

	public Vector3 teleportLocation;

	public GameObject cameraOffest;
}
