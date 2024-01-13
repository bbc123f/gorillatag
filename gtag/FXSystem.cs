using Photon.Pun;
using UnityEngine;

public static class FXSystem
{
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
}
