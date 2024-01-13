using Photon.Pun;
using UnityEngine;

public class HoldableObject : MonoBehaviourPunCallbacks
{
	public virtual void OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
	}

	public virtual void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
	}

	public virtual void DropItemCleanup()
	{
	}
}
