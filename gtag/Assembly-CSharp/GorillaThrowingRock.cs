using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001C1 RID: 449
public class GorillaThrowingRock : GorillaThrowable, IPunInstantiateMagicCallback
{
	// Token: 0x06000B6F RID: 2927 RVA: 0x0004673D File Offset: 0x0004493D
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x00046740 File Offset: 0x00044940
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

	// Token: 0x06000B71 RID: 2929 RVA: 0x000467C0 File Offset: 0x000449C0
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

	// Token: 0x04000EEC RID: 3820
	public float bonkSpeedMin = 1f;

	// Token: 0x04000EED RID: 3821
	public float bonkSpeedMax = 5f;

	// Token: 0x04000EEE RID: 3822
	public VRRig hitRig;
}
