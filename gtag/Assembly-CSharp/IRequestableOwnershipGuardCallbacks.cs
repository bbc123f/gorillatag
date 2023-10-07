using System;
using Photon.Realtime;

// Token: 0x0200006E RID: 110
public interface IRequestableOwnershipGuardCallbacks
{
	// Token: 0x06000231 RID: 561
	void OnOwnershipTransferred(Player toPlayer, Player fromPlayer);

	// Token: 0x06000232 RID: 562
	bool OnOwnershipRequest(Player fromPlayer);

	// Token: 0x06000233 RID: 563
	void OnMyOwnerLeft();

	// Token: 0x06000234 RID: 564
	bool OnMasterClientAssistedTakeoverRequest(Player fromPlayer, Player toPlayer);

	// Token: 0x06000235 RID: 565
	void OnMyCreatorLeft();
}
