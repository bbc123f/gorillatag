using System;
using Photon.Pun;

public class TransformViewTeleportSerializer : MonoBehaviourPun, IPunObservable
{
	private void Start()
	{
		this.transformView = base.GetComponent<PhotonTransformView>();
	}

	public void SetWillTeleport()
	{
		this.willTeleport = true;
	}

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

	public TransformViewTeleportSerializer()
	{
	}

	private bool willTeleport;

	private PhotonTransformView transformView;
}
