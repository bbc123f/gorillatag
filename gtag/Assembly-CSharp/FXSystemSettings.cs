using System;
using UnityEngine;

// Token: 0x020001F5 RID: 501
[CreateAssetMenu(menuName = "ScriptableObjects/FXSystemSettings", order = 2)]
public class FXSystemSettings : ScriptableObject
{
	// Token: 0x06000CD3 RID: 3283 RVA: 0x0004C40C File Offset: 0x0004A60C
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

	// Token: 0x0400101A RID: 4122
	[SerializeField]
	private LimiterType[] callLimits;

	// Token: 0x0400101B RID: 4123
	[SerializeField]
	private CooldownType[] CallLimitsCooldown;

	// Token: 0x0400101C RID: 4124
	[NonSerialized]
	public bool forLocalRig;

	// Token: 0x0400101D RID: 4125
	[NonSerialized]
	public CallLimitType<CallLimiter>[] callSettings = new CallLimitType<CallLimiter>[2];
}
