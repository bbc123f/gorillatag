using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001F1 RID: 497
public static class FXSystem
{
	// Token: 0x06000CD3 RID: 3283 RVA: 0x0004C550 File Offset: 0x0004A750
	public static void PlayFXForRig(FXType fxType, IFXContext context, PhotonMessageInfo info = default(PhotonMessageInfo))
	{
		FXSystemSettings settings = context.settings;
		if (settings.forLocalRig)
		{
			context.OnPlayFX();
			return;
		}
		CallLimitType<CallLimiter> callLimitType = settings.callSettings[(int)fxType];
		if (callLimitType.UseNetWorkTime ? callLimitType.CallLimitSettings.CheckCallServerTime(info.SentServerTime) : callLimitType.CallLimitSettings.CheckCallTime(Time.time))
		{
			context.OnPlayFX();
		}
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x0004C5B0 File Offset: 0x0004A7B0
	public static void PlayFX<T>(FXType fxType, IFXContextParems<T> context, T args, PhotonMessageInfo info = default(PhotonMessageInfo)) where T : FXSArgs
	{
		FXSystemSettings settings = context.settings;
		if (settings.forLocalRig)
		{
			context.OnPlayFX(args);
			return;
		}
		CallLimitType<CallLimiter> callLimitType = settings.callSettings[(int)fxType];
		if (callLimitType.UseNetWorkTime ? callLimitType.CallLimitSettings.CheckCallServerTime(info.SentServerTime) : callLimitType.CallLimitSettings.CheckCallTime(Time.time))
		{
			context.OnPlayFX(args);
		}
	}
}
