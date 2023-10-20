using System;
using Photon.Pun;

// Token: 0x020001E6 RID: 486
public class TransformViewTeleportSerializer : MonoBehaviourPun, IPunObservable
{
	// Token: 0x06000C8A RID: 3210 RVA: 0x0004BB03 File Offset: 0x00049D03
	private void Start()
	{
		this.transformView = base.GetComponent<PhotonTransformView>();
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x0004BB11 File Offset: 0x00049D11
	public void SetWillTeleport()
	{
		this.willTeleport = true;
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x0004BB1A File Offset: 0x00049D1A
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.willTeleport);
			this.willTeleport = false;
			return;
		}
		if ((bool)stream.ReceiveNext())
		{
			this.transformView.GTAddition_DoTeleport();
		}
	}

	// Token: 0x04000FF7 RID: 4087
	private bool willTeleport;

	// Token: 0x04000FF8 RID: 4088
	private PhotonTransformView transformView;
}
