using Photon.Pun;
using UnityEngine;

public class GorillaThrowingRock : GorillaThrowable, IPunInstantiateMagicCallback
{
	public float bonkSpeedMin = 1f;

	public float bonkSpeedMax = 5f;

	public VRRig hitRig;

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
	}

	public override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		Debug.Log("did a collision enter");
		hitRig = collision.collider.GetComponentInParent<VRRig>();
		if (hitRig != null && hitRig.photonView.Owner != base.photonView.Owner)
		{
			hitRig.photonView.RPC("Bonk", RpcTarget.All, 4);
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		Debug.Log("did a trigger enter");
		hitRig = other.GetComponentInParent<VRRig>();
		if (hitRig != null && !hitRig.isOfflineVRRig && (!PhotonView.Get(hitRig).IsMine || !isHeld))
		{
			Debug.Log("found rig");
			if (!isHeld && rigidbody.velocity.magnitude > bonkSpeedMin)
			{
				PhotonView.Get(hitRig).RPC("Bonk", RpcTarget.All, 4, (Mathf.Clamp(rigidbody.velocity.magnitude, bonkSpeedMin, bonkSpeedMax) - 1f) / 5f + 0.05f);
			}
			else if (isHeld)
			{
				PhotonView.Get(hitRig).RPC("Bonk", RpcTarget.All, 4, (Mathf.Clamp(denormalizedVelocityAverage.magnitude, bonkSpeedMin, bonkSpeedMax) - 1f) / 5f + 0.05f);
			}
		}
	}
}
