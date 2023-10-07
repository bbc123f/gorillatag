using System;

// Token: 0x020001F4 RID: 500
[Serializable]
public class CallLimitType<T> where T : CallLimiter
{
	// Token: 0x06000CD1 RID: 3281 RVA: 0x0004C3D4 File Offset: 0x0004A5D4
	public static implicit operator CallLimitType<CallLimiter>(CallLimitType<T> clt)
	{
		return new CallLimitType<CallLimiter>
		{
			Key = clt.Key,
			UseNetWorkTime = clt.UseNetWorkTime,
			CallLimitSettings = clt.CallLimitSettings
		};
	}

	// Token: 0x04001017 RID: 4119
	public FXType Key;

	// Token: 0x04001018 RID: 4120
	public bool UseNetWorkTime;

	// Token: 0x04001019 RID: 4121
	public T CallLimitSettings;
}
