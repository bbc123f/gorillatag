using System;

// Token: 0x0200006C RID: 108
public enum NetworkingState
{
	// Token: 0x040002C9 RID: 713
	IsOwner,
	// Token: 0x040002CA RID: 714
	IsBlindClient,
	// Token: 0x040002CB RID: 715
	IsClient,
	// Token: 0x040002CC RID: 716
	ForcefullyTakingOver,
	// Token: 0x040002CD RID: 717
	RequestingOwnership,
	// Token: 0x040002CE RID: 718
	RequestingOwnershipWaitingForSight,
	// Token: 0x040002CF RID: 719
	ForcefullyTakingOverWaitingForSight
}
