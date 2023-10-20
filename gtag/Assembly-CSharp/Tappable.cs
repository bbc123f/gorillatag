using System;
using System.Diagnostics;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200019D RID: 413
public class Tappable : MonoBehaviour
{
	// Token: 0x06000AAA RID: 2730 RVA: 0x00041F6B File Offset: 0x0004016B
	public void Validate()
	{
		TappableManager.CalculateId(this, true);
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x00041F74 File Offset: 0x00040174
	protected virtual void OnEnable()
	{
		TappableManager.Register(this);
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x00041F7C File Offset: 0x0004017C
	protected virtual void OnDisable()
	{
		TappableManager.Unregister(this);
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x00041F84 File Offset: 0x00040184
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

	// Token: 0x06000AAE RID: 2734 RVA: 0x00041FE2 File Offset: 0x000401E2
	public virtual void OnTapLocal(float tapStrength, float tapTime)
	{
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x00041FE4 File Offset: 0x000401E4
	private void RecalculateId()
	{
		TappableManager.CalculateId(this, true);
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x00041FED File Offset: 0x000401ED
	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		TappableManager.CalculateId(this, false);
	}

	// Token: 0x04000D7F RID: 3455
	public int tappableId;

	// Token: 0x04000D80 RID: 3456
	public string staticId;

	// Token: 0x04000D81 RID: 3457
	public bool useStaticId;

	// Token: 0x04000D82 RID: 3458
	[Space]
	public TappableManager manager;
}
