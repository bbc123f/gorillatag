using System;
using System.Collections.Generic;
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

	public static void PlayFXForRigValidated(List<int> hashes, FXType fxType, IFXContext context, PhotonMessageInfo info = default(PhotonMessageInfo))
	{
		for (int i = 0; i < hashes.Count; i++)
		{
			if (!ObjectPools.instance.DoesPoolExist(hashes[i]))
			{
				return;
			}
		}
		FXSystem.PlayFXForRig(fxType, context, info);
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
