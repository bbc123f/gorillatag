using System;
using Photon.Pun;

internal interface IGorillaSerializeable
{
	void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);
}
