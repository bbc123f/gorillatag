using System;
using System.Collections.Generic;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020000D3 RID: 211
public class PlayerCollection : MonoBehaviourPunCallbacks
{
	// Token: 0x060004AA RID: 1194 RVA: 0x0001D734 File Offset: 0x0001B934
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

	// Token: 0x060004AB RID: 1195 RVA: 0x0001D784 File Offset: 0x0001B984
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

	// Token: 0x060004AC RID: 1196 RVA: 0x0001D828 File Offset: 0x0001BA28
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

	// Token: 0x04000558 RID: 1368
	[DebugReadout]
	[NonSerialized]
	public readonly List<VRRig> containedRigs = new List<VRRig>(10);
}
