using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/FXSystemSettings", order = 2)]
public class FXSystemSettings : ScriptableObject
{
	public void Awake()
	{
		int num = ((this.callLimits != null) ? this.callLimits.Length : 0);
		int num2 = ((this.CallLimitsCooldown != null) ? this.CallLimitsCooldown.Length : 0);
		for (int i = 0; i < num; i++)
		{
			FXType fxtype = this.callLimits[i].Key;
			int num3 = (int)fxtype;
			if (this.callSettings[num3] != null)
			{
				Debug.Log("call setting for " + fxtype.ToString() + " already exists, skipping");
			}
			else
			{
				this.callSettings[num3] = this.callLimits[i];
			}
		}
		for (int i = 0; i < num2; i++)
		{
			FXType fxtype = this.CallLimitsCooldown[i].Key;
			int num3 = (int)fxtype;
			if (this.callSettings[num3] != null)
			{
				Debug.Log("call setting for " + fxtype.ToString() + " already exists, skipping");
			}
			else
			{
				this.callSettings[num3] = this.CallLimitsCooldown[i];
			}
		}
	}

	[SerializeField]
	private LimiterType[] callLimits;

	[SerializeField]
	private CooldownType[] CallLimitsCooldown;

	[NonSerialized]
	public bool forLocalRig;

	[NonSerialized]
	public CallLimitType<CallLimiter>[] callSettings = new CallLimitType<CallLimiter>[5];
}
