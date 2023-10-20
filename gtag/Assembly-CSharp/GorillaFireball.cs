using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001BD RID: 445
public class GorillaFireball : GorillaThrowable, IPunInstantiateMagicCallback
{
	// Token: 0x06000B56 RID: 2902 RVA: 0x00045A96 File Offset: 0x00043C96
	public override void Start()
	{
		base.Start();
		this.canExplode = false;
		this.explosionStartTime = 0f;
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x00045AB0 File Offset: 0x00043CB0
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

	// Token: 0x06000B58 RID: 2904 RVA: 0x00045B38 File Offset: 0x00043D38
	public override void LateUpdate()
	{
		base.LateUpdate();
		if (this.rigidbody.useGravity)
		{
			this.rigidbody.AddForce(Physics.gravity * -this.gravityStrength * this.rigidbody.mass);
		}
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x00045B84 File Offset: 0x00043D84
	public override void ThrowThisThingo()
	{
		base.ThrowThisThingo();
		this.canExplode = true;
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x00045B93 File Offset: 0x00043D93
	private new void OnCollisionEnter(Collision collision)
	{
		if (base.photonView.IsMine && this.canExplode)
		{
			base.photonView.RPC("Explode", RpcTarget.All, null);
		}
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x00045BBC File Offset: 0x00043DBC
	public void LocalExplode()
	{
		this.rigidbody.isKinematic = true;
		this.canExplode = false;
		this.explosionStartTime = Time.time;
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x00045BDC File Offset: 0x00043DDC
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

	// Token: 0x06000B5D RID: 2909 RVA: 0x00045C3F File Offset: 0x00043E3F
	[PunRPC]
	public void Explode()
	{
		this.LocalExplode();
	}

	// Token: 0x04000EB4 RID: 3764
	public float maxExplosionScale;

	// Token: 0x04000EB5 RID: 3765
	public float totalExplosionTime;

	// Token: 0x04000EB6 RID: 3766
	public float gravityStrength;

	// Token: 0x04000EB7 RID: 3767
	private bool canExplode;

	// Token: 0x04000EB8 RID: 3768
	private float explosionStartTime;
}
