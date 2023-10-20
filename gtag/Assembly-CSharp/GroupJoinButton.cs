using System;
using GorillaNetworking;
using Photon.Pun;

// Token: 0x0200018F RID: 399
public class GroupJoinButton : GorillaPressableButton
{
	// Token: 0x06000A31 RID: 2609 RVA: 0x0003EF8F File Offset: 0x0003D18F
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (this.inPrivate)
		{
			GorillaComputer.instance.OnGroupJoinButtonPress(this.gameModeIndex, this.friendCollider);
		}
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x0003EFB7 File Offset: 0x0003D1B7
	public void Update()
	{
		this.inPrivate = (PhotonNetwork.InRoom && !PhotonNetwork.CurrentRoom.IsVisible);
		if (!this.inPrivate)
		{
			this.isOn = true;
		}
	}

	// Token: 0x04000C99 RID: 3225
	public int gameModeIndex;

	// Token: 0x04000C9A RID: 3226
	public GorillaFriendCollider friendCollider;

	// Token: 0x04000C9B RID: 3227
	public bool inPrivate;
}
