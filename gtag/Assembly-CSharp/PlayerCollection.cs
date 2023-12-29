using System;
using System.Collections.Generic;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerCollection : MonoBehaviourPunCallbacks
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
		SphereCollider component = other.GetComponent<SphereCollider>();
		if (!component)
		{
			return;
		}
		VRRig component2 = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component2 == null)
		{
			return;
		}
		if (this.containedRigs.Contains(component2))
		{
			Collider[] components = base.GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				Vector3 vector;
				float num;
				if (Physics.ComputePenetration(components[i], base.transform.position, base.transform.rotation, component, component.transform.position, component.transform.rotation, out vector, out num))
				{
					return;
				}
			}
			this.containedRigs.Remove(component2);
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

	[DebugReadout]
	[NonSerialized]
	public readonly List<VRRig> containedRigs = new List<VRRig>(10);
}
