using System;
using Photon.Pun;
using UnityEngine;

public static class FXSystem
{
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
