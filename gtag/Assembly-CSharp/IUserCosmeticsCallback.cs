using System;

// Token: 0x020001F6 RID: 502
internal interface IUserCosmeticsCallback
{
	// Token: 0x06000CD5 RID: 3285
	bool OnGetUserCosmetics(string cosmetics);

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x06000CD6 RID: 3286
	// (set) Token: 0x06000CD7 RID: 3287
	bool PendingUpdate { get; set; }
}
