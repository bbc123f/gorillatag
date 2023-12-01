using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MazePlayerCollection : MonoBehaviourPunCallbacks
{
	public void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (!this.containedRigs.Contains(component))
		{
			this.containedRigs.Add(component);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (this.containedRigs.Contains(component))
		{
			this.containedRigs.Remove(component);
		}
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		for (int i = this.containedRigs.Count - 1; i >= 0; i--)
		{
			VRRig vrrig = this.containedRigs[i];
			if (((vrrig != null) ? vrrig.creator : null) == null || vrrig.creator == otherPlayer)
			{
				this.containedRigs.RemoveAt(i);
			}
		}
	}

	public List<VRRig> containedRigs = new List<VRRig>();

	public List<MonkeyeAI> monkeyeAis = new List<MonkeyeAI>();
}
