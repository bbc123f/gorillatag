using System;

// Token: 0x020001EE RID: 494
public interface IFXContext
{
	// Token: 0x17000098 RID: 152
	// (get) Token: 0x06000CCE RID: 3278
	FXSystemSettings settings { get; }

	// Token: 0x06000CCF RID: 3279
	void OnPlayFX();
}
