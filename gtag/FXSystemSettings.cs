using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/FXSystemSettings", order = 2)]
public class FXSystemSettings : ScriptableObject
{
	[SerializeField]
	private LimiterType[] callLimits;

	[SerializeField]
	private CooldownType[] CallLimitsCooldown;

	[NonSerialized]
	public bool forLocalRig;

	public CallLimitType<CallLimiter>[] callSettings = new CallLimitType<CallLimiter>[1];

	public void Awake()
	{
		int num = ((callLimits != null) ? callLimits.Length : 0);
		int num2 = ((CallLimitsCooldown != null) ? CallLimitsCooldown.Length : 0);
		int i = 0;
		int num3 = 0;
		FXType fXType = FXType.BalloonPop;
		for (; i < num; i++)
		{
			fXType = callLimits[i].Key;
			num3 = (int)fXType;
			if (callSettings[num3] != null)
			{
				Debug.LogError("call setting for " + fXType.ToString() + " already exists, skipping");
			}
			else
			{
				callSettings[num3] = callLimits[i];
			}
		}
		i = 0;
		num3 = 0;
		fXType = FXType.BalloonPop;
		for (; i < num2; i++)
		{
			fXType = CallLimitsCooldown[i].Key;
			num3 = (int)fXType;
			if (callSettings[num3] != null)
			{
				Debug.LogError("call setting for " + fXType.ToString() + " already exists, skipping");
			}
			else
			{
				callSettings[num3] = CallLimitsCooldown[i];
			}
		}
	}
}
