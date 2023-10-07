using System;
using Photon.Pun;

// Token: 0x020001E5 RID: 485
public class TransformViewTeleportSerializer : MonoBehaviourPun, IPunObservable
{
	// Token: 0x06000C84 RID: 3204 RVA: 0x0004B89B File Offset: 0x00049A9B
	private void Start()
	{
		this.transformView = base.GetComponent<PhotonTransformView>();
	}

	// Token: 0x06000C85 RID: 3205 RVA: 0x0004B8A9 File Offset: 0x00049AA9
	public void SetWillTeleport()
	{
		this.willTeleport = true;
	}

	// Token: 0x06000C86 RID: 3206 RVA: 0x0004B8B2 File Offset: 0x00049AB2
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

	// Token: 0x04000FF3 RID: 4083
	private bool willTeleport;

	// Token: 0x04000FF4 RID: 4084
	private PhotonTransformView transformView;
}
