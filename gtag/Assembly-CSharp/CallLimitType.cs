using System;

// Token: 0x020001F5 RID: 501
[Serializable]
public class CallLimitType<T> where T : CallLimiter
{
	// Token: 0x06000CD7 RID: 3287 RVA: 0x0004C622 File Offset: 0x0004A822
	public static implicit operator CallLimitType<CallLimiter>(CallLimitType<T> clt)
	{
		return new CallLimitType<CallLimiter>
		{
			Key = clt.Key,
			UseNetWorkTime = clt.UseNetWorkTime,
			CallLimitSettings = clt.CallLimitSettings
		};
	}

	// Token: 0x0400101C RID: 4124
	public FXType Key;

	// Token: 0x0400101D RID: 4125
	public bool UseNetWorkTime;

	// Token: 0x0400101E RID: 4126
	public T CallLimitSettings;
}
