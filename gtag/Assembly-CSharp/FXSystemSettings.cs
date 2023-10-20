using System;
using UnityEngine;

// Token: 0x020001F6 RID: 502
[CreateAssetMenu(menuName = "ScriptableObjects/FXSystemSettings", order = 2)]
public class FXSystemSettings : ScriptableObject
{
	// Token: 0x06000CD9 RID: 3289 RVA: 0x0004C65C File Offset: 0x0004A85C
	public void Awake()
	{
		int num = (this.callLimits != null) ? this.callLimits.Length : 0;
		int num2 = (this.CallLimitsCooldown != null) ? this.CallLimitsCooldown.Length : 0;
		for (int i = 0; i < num; i++)
		{
			FXType key = this.callLimits[i].Key;
			int num3 = (int)key;
			if (this.callSettings[num3] != null)
			{
				Debug.Log("call setting for " + key.ToString() + " already exists, skipping");
			}
			else
			{
				this.callSettings[num3] = this.callLimits[i];
			}
		}
		for (int i = 0; i < num2; i++)
		{
			FXType key = this.CallLimitsCooldown[i].Key;
			int num3 = (int)key;
			if (this.callSettings[num3] != null)
			{
				Debug.Log("call setting for " + key.ToString() + " already exists, skipping");
			}
			else
			{
				this.callSettings[num3] = this.CallLimitsCooldown[i];
			}
		}
	}

	// Token: 0x0400101F RID: 4127
	[SerializeField]
	private LimiterType[] callLimits;

	// Token: 0x04001020 RID: 4128
	[SerializeField]
	private CooldownType[] CallLimitsCooldown;

	// Token: 0x04001021 RID: 4129
	[NonSerialized]
	public bool forLocalRig;

	// Token: 0x04001022 RID: 4130
	[NonSerialized]
	public CallLimitType<CallLimiter>[] callSettings = new CallLimitType<CallLimiter>[3];
}
