using System;
using Photon.Pun;
using UnityEngine;

public class GorillaThrowingRock : GorillaThrowable, IPunInstantiateMagicCallback
{
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
	}

	public override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		Debug.Log("did a collision enter");
		this.hitRig = collision.collider.GetComponentInParent<VRRig>();
		if (this.hitRig != null && this.hitRig.photonView.Owner != base.photonView.Owner)
		{
			this.hitRig.photonView.RPC("Bonk", RpcTarget.All, new object[] { 4 });
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		Debug.Log("did a trigger enter");
		this.hitRig = other.GetComponentInParent<VRRig>();
		if (this.hitRig != null && !this.hitRig.isOfflineVRRig && (!PhotonView.Get(this.hitRig).IsMine || !this.isHeld))
		{
			Debug.Log("found rig");
			if (!this.isHeld && this.rigidbody.velocity.magnitude > this.bonkSpeedMin)
			{
				PhotonView.Get(this.hitRig).RPC("Bonk", RpcTarget.All, new object[]
				{
					4,
					(Mathf.Clamp(this.rigidbody.velocity.magnitude, this.bonkSpeedMin, this.bonkSpeedMax) - 1f) / 5f + 0.05f
				});
				return;
			}
			if (this.isHeld)
			{
				PhotonView.Get(this.hitRig).RPC("Bonk", RpcTarget.All, new object[]
				{
					4,
					(Mathf.Clamp(this.denormalizedVelocityAverage.magnitude, this.bonkSpeedMin, this.bonkSpeedMax) - 1f) / 5f + 0.05f
				});
			}
		}
	}

	public float bonkSpeedMin = 1f;

	public float bonkSpeedMax = 5f;

	public VRRig hitRig;
}
