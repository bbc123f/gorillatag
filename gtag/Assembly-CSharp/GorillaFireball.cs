using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001BC RID: 444
public class GorillaFireball : GorillaThrowable, IPunInstantiateMagicCallback
{
	// Token: 0x06000B50 RID: 2896 RVA: 0x0004582E File Offset: 0x00043A2E
	public override void Start()
	{
		base.Start();
		this.canExplode = false;
		this.explosionStartTime = 0f;
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x00045848 File Offset: 0x00043A48
	private void Update()
	{
		if (this.explosionStartTime != 0f)
		{
			float num = (Time.time - this.explosionStartTime) / this.totalExplosionTime * (this.maxExplosionScale - 0.25f) + 0.25f;
			base.gameObject.transform.localScale = new Vector3(num, num, num);
			if (base.photonView.IsMine && Time.time > this.explosionStartTime + this.totalExplosionTime)
			{
				PhotonNetwork.Destroy(PhotonView.Get(this));
			}
		}
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x000458D0 File Offset: 0x00043AD0
	public override void LateUpdate()
	{
		base.LateUpdate();
		if (this.rigidbody.useGravity)
		{
			this.rigidbody.AddForce(Physics.gravity * -this.gravityStrength * this.rigidbody.mass);
		}
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x0004591C File Offset: 0x00043B1C
	public override void ThrowThisThingo()
	{
		base.ThrowThisThingo();
		this.canExplode = true;
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x0004592B File Offset: 0x00043B2B
	private new void OnCollisionEnter(Collision collision)
	{
		if (base.photonView.IsMine && this.canExplode)
		{
			base.photonView.RPC("Explode", RpcTarget.All, null);
		}
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x00045954 File Offset: 0x00043B54
	public void LocalExplode()
	{
		this.rigidbody.isKinematic = true;
		this.canExplode = false;
		this.explosionStartTime = Time.time;
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x00045974 File Offset: 0x00043B74
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (base.photonView.IsMine)
		{
			if ((bool)base.photonView.InstantiationData[0])
			{
				base.transform.parent = GorillaPlaySpace.Instance.myVRRig.leftHandTransform;
				return;
			}
			base.transform.parent = GorillaPlaySpace.Instance.myVRRig.rightHandTransform;
		}
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x000459D7 File Offset: 0x00043BD7
	[PunRPC]
	public void Explode()
	{
		this.LocalExplode();
	}

	// Token: 0x04000EB0 RID: 3760
	public float maxExplosionScale;

	// Token: 0x04000EB1 RID: 3761
	public float totalExplosionTime;

	// Token: 0x04000EB2 RID: 3762
	public float gravityStrength;

	// Token: 0x04000EB3 RID: 3763
	private bool canExplode;

	// Token: 0x04000EB4 RID: 3764
	private float explosionStartTime;
}
