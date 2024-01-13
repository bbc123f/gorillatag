using System;
using System.Collections.Generic;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerCollection : MonoBehaviourPunCallbacks
{
	[NonSerialized]
	[DebugReadout]
	public readonly List<VRRig> containedRigs = new List<VRRig>(10);

	public void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<SphereCollider>())
		{
			VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (!(component == null) && !containedRigs.Contains(component))
			{
				containedRigs.Add(component);
			}
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
		if (component2 == null || !containedRigs.Contains(component2))
		{
			return;
		}
		Collider[] components = GetComponents<Collider>();
		for (int i = 0; i < components.Length; i++)
		{
			if (Physics.ComputePenetration(components[i], base.transform.position, base.transform.rotation, component, component.transform.position, component.transform.rotation, out var _, out var _))
			{
				return;
			}
		}
		containedRigs.Remove(component2);
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		VRRig vRRig = null;
		for (int num = containedRigs.Count - 1; num >= 0; num--)
		{
			vRRig = containedRigs[num];
			if (vRRig?.creator == null || vRRig.creator == otherPlayer)
			{
				containedRigs.RemoveAt(num);
			}
		}
	}
}
