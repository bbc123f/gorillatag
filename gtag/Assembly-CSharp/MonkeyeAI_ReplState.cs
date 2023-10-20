using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000026 RID: 38
public class MonkeyeAI_ReplState : MonoBehaviour, IPunObservable
{
	// Token: 0x060000DC RID: 220 RVA: 0x00008C18 File Offset: 0x00006E18
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.userId);
			stream.SendNext(this.attackPos);
			stream.SendNext(this.timer);
			stream.SendNext(this.floorEnabled);
			stream.SendNext(this.portalEnabled);
			stream.SendNext(this.freezePlayer);
			stream.SendNext(this.alpha);
			stream.SendNext(this.state);
			return;
		}
		if (info.photonView.Owner == null)
		{
			return;
		}
		if (info.Sender.ActorNumber != info.photonView.Owner.ActorNumber)
		{
			return;
		}
		this.userId = (string)stream.ReceiveNext();
		this.attackPos = (Vector3)stream.ReceiveNext();
		this.timer = (float)stream.ReceiveNext();
		this.floorEnabled = (bool)stream.ReceiveNext();
		this.portalEnabled = (bool)stream.ReceiveNext();
		this.freezePlayer = (bool)stream.ReceiveNext();
		this.alpha = (float)stream.ReceiveNext();
		this.state = (MonkeyeAI_ReplState.EStates)stream.ReceiveNext();
	}

	// Token: 0x04000124 RID: 292
	public MonkeyeAI_ReplState.EStates state;

	// Token: 0x04000125 RID: 293
	public string userId;

	// Token: 0x04000126 RID: 294
	public Vector3 attackPos;

	// Token: 0x04000127 RID: 295
	public float timer;

	// Token: 0x04000128 RID: 296
	public bool floorEnabled;

	// Token: 0x04000129 RID: 297
	public bool portalEnabled;

	// Token: 0x0400012A RID: 298
	public bool freezePlayer;

	// Token: 0x0400012B RID: 299
	public float alpha;

	// Token: 0x0200038B RID: 907
	public enum EStates
	{
		// Token: 0x04001B09 RID: 6921
		Sleeping,
		// Token: 0x04001B0A RID: 6922
		Patrolling,
		// Token: 0x04001B0B RID: 6923
		Chasing,
		// Token: 0x04001B0C RID: 6924
		ReturnToSleepPt,
		// Token: 0x04001B0D RID: 6925
		GoToSleep,
		// Token: 0x04001B0E RID: 6926
		BeginAttack,
		// Token: 0x04001B0F RID: 6927
		OpenFloor,
		// Token: 0x04001B10 RID: 6928
		DropPlayer,
		// Token: 0x04001B11 RID: 6929
		CloseFloor
	}
}
