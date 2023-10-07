using System;
using GorillaNetworking;
using Photon.Pun;

// Token: 0x0200018E RID: 398
public class GroupJoinButton : GorillaPressableButton
{
	// Token: 0x06000A2C RID: 2604 RVA: 0x0003EE5F File Offset: 0x0003D05F
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (this.inPrivate)
		{
			GorillaComputer.instance.OnGroupJoinButtonPress(this.gameModeIndex, this.friendCollider);
		}
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x0003EE87 File Offset: 0x0003D087
	public void Update()
	{
		this.inPrivate = (PhotonNetwork.InRoom && !PhotonNetwork.CurrentRoom.IsVisible);
		if (!this.inPrivate)
		{
			this.isOn = true;
		}
	}

	// Token: 0x04000C95 RID: 3221
	public int gameModeIndex;

	// Token: 0x04000C96 RID: 3222
	public GorillaFriendCollider friendCollider;

	// Token: 0x04000C97 RID: 3223
	public bool inPrivate;
}
