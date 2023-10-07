using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001C0 RID: 448
public class GorillaThrowingRock : GorillaThrowable, IPunInstantiateMagicCallback
{
	// Token: 0x06000B69 RID: 2921 RVA: 0x000464D5 File Offset: 0x000446D5
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x000464D8 File Offset: 0x000446D8
	public override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		Debug.Log("did a collision enter");
		this.hitRig = collision.collider.GetComponentInParent<VRRig>();
		if (this.hitRig != null && this.hitRig.photonView.Owner != base.photonView.Owner)
		{
			this.hitRig.photonView.RPC("Bonk", RpcTarget.All, new object[]
			{
				4
			});
		}
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x00046558 File Offset: 0x00044758
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

	// Token: 0x04000EE8 RID: 3816
	public float bonkSpeedMin = 1f;

	// Token: 0x04000EE9 RID: 3817
	public float bonkSpeedMax = 5f;

	// Token: 0x04000EEA RID: 3818
	public VRRig hitRig;
}
