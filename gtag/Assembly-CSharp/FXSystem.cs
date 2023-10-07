using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001F0 RID: 496
public static class FXSystem
{
	// Token: 0x06000CCD RID: 3277 RVA: 0x0004C304 File Offset: 0x0004A504
	public static void PlayFXForRig(FXType fxType, IFXContext context)
	{
		FXSystemSettings settings = context.settings;
		if (settings.forLocalRig)
		{
			context.OnPlayFX();
			return;
		}
		CallLimitType<CallLimiter> callLimitType = settings.callSettings[(int)fxType];
		if (callLimitType.UseNetWorkTime ? callLimitType.CallLimitSettings.CheckCallServerTime(PhotonNetwork.Time) : callLimitType.CallLimitSettings.CheckCallTime(Time.time))
		{
			context.OnPlayFX();
		}
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x0004C364 File Offset: 0x0004A564
	public static void PlayFX<T>(FXType fxType, IFXContextParems<T> context, T args) where T : FXSArgs
	{
		FXSystemSettings settings = context.settings;
		if (settings.forLocalRig)
		{
			context.OnPlayFX(args);
			return;
		}
		CallLimitType<CallLimiter> callLimitType = settings.callSettings[(int)fxType];
		if (callLimitType.UseNetWorkTime ? callLimitType.CallLimitSettings.CheckCallServerTime(PhotonNetwork.Time) : callLimitType.CallLimitSettings.CheckCallTime(Time.time))
		{
			context.OnPlayFX(args);
		}
	}
}
