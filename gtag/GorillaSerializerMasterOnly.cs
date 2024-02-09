using System;
using Photon.Pun;

internal class GorillaSerializerMasterOnly : GorillaSerializer
{
	protected override bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == PhotonNetwork.MasterClient;
	}
}
