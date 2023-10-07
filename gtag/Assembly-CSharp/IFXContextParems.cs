using System;

// Token: 0x020001EF RID: 495
public interface IFXContextParems<T> where T : FXSArgs
{
	// Token: 0x17000097 RID: 151
	// (get) Token: 0x06000CCB RID: 3275
	FXSystemSettings settings { get; }

	// Token: 0x06000CCC RID: 3276
	void OnPlayFX(T parems);
}
