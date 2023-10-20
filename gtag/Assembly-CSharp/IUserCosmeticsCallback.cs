using System;

// Token: 0x020001F7 RID: 503
internal interface IUserCosmeticsCallback
{
	// Token: 0x06000CDB RID: 3291
	bool OnGetUserCosmetics(string cosmetics);

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x06000CDC RID: 3292
	// (set) Token: 0x06000CDD RID: 3293
	bool PendingUpdate { get; set; }
}
