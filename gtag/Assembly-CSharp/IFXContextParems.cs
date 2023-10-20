using System;

// Token: 0x020001F0 RID: 496
public interface IFXContextParems<T> where T : FXSArgs
{
	// Token: 0x17000099 RID: 153
	// (get) Token: 0x06000CD1 RID: 3281
	FXSystemSettings settings { get; }

	// Token: 0x06000CD2 RID: 3282
	void OnPlayFX(T parems);
}
