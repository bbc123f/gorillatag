using System;
using System.Diagnostics;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200019C RID: 412
public class Tappable : MonoBehaviour
{
	// Token: 0x06000AA5 RID: 2725 RVA: 0x00041E33 File Offset: 0x00040033
	public void Validate()
	{
		TappableManager.CalculateId(this, true);
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x00041E3C File Offset: 0x0004003C
	protected virtual void OnEnable()
	{
		TappableManager.Register(this);
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x00041E44 File Offset: 0x00040044
	protected virtual void OnDisable()
	{
		TappableManager.Unregister(this);
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x00041E4C File Offset: 0x0004004C
	public void OnTap(float tapStrength, float tapTime)
	{
		this.OnTapLocal(tapStrength, tapTime);
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnTapRPC", RpcTarget.Others, new object[]
		{
			this.tappableId,
			tapStrength
		});
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x00041EAA File Offset: 0x000400AA
	public virtual void OnTapLocal(float tapStrength, float tapTime)
	{
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x00041EAC File Offset: 0x000400AC
	private void RecalculateId()
	{
		TappableManager.CalculateId(this, true);
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x00041EB5 File Offset: 0x000400B5
	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		TappableManager.CalculateId(this, false);
	}

	// Token: 0x04000D7B RID: 3451
	public int tappableId;

	// Token: 0x04000D7C RID: 3452
	public string staticId;

	// Token: 0x04000D7D RID: 3453
	public bool useStaticId;

	// Token: 0x04000D7E RID: 3454
	[Space]
	public TappableManager manager;
}
