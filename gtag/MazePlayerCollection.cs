using System;
using System.Collections.Generic;
using UnityEngine;

public class MazePlayerCollection : MonoBehaviour
{
	private void Start()
	{
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
	}

	private void OnDestroy()
	{
		NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerLeftRoom;
	}

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

	public void OnPlayerLeftRoom(int otherPlayerId)
	{
		NetPlayer otherPlayer = NetworkSystem.Instance.GetPlayer(otherPlayerId);
		this.containedRigs.RemoveAll((VRRig r) => ((r != null) ? r.creatorWrapped : null) == null || r.creatorWrapped == otherPlayer);
	}

	public List<VRRig> containedRigs = new List<VRRig>();

	public List<MonkeyeAI> monkeyeAis = new List<MonkeyeAI>();
}
