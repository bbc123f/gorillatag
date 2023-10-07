using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000024 RID: 36
public class MazePlayerCollection : MonoBehaviourPunCallbacks
{
	// Token: 0x060000B8 RID: 184 RVA: 0x000075F4 File Offset: 0x000057F4
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

	// Token: 0x060000B9 RID: 185 RVA: 0x00007644 File Offset: 0x00005844
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

	// Token: 0x060000BA RID: 186 RVA: 0x00007698 File Offset: 0x00005898
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

	// Token: 0x040000DE RID: 222
	public List<VRRig> containedRigs = new List<VRRig>();

	// Token: 0x040000DF RID: 223
	public List<MonkeyeAI> monkeyeAis = new List<MonkeyeAI>();
}
