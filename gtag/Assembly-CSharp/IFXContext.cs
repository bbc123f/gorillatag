using System;

// Token: 0x020001ED RID: 493
public interface IFXContext
{
	// Token: 0x17000096 RID: 150
	// (get) Token: 0x06000CC8 RID: 3272
	FXSystemSettings settings { get; }

	// Token: 0x06000CC9 RID: 3273
	void OnPlayFX();
}
