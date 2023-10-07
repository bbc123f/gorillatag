using System;
using Photon.Pun;

// Token: 0x0200015D RID: 349
internal interface IGorillaSerializeable
{
	// Token: 0x06000897 RID: 2199
	void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06000898 RID: 2200
	void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);
}
