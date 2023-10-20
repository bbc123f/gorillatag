using System;
using UnityEngine;

// Token: 0x020001ED RID: 493
[Serializable]
public class CallLimiterWithCooldown : CallLimiter
{
	// Token: 0x06000CCB RID: 3275 RVA: 0x0004C4F5 File Offset: 0x0004A6F5
	public CallLimiterWithCooldown(float coolDownSpam, int historyLength, float coolDown) : base(historyLength, coolDown, 0.5f)
	{
		this.spamCoolDown = coolDownSpam;
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x0004C50B File Offset: 0x0004A70B
	public CallLimiterWithCooldown(float coolDownSpam, int historyLength, float coolDown, float latencyMax) : base(historyLength, coolDown, latencyMax)
	{
		this.spamCoolDown = coolDownSpam;
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x0004C51E File Offset: 0x0004A71E
	public override bool CheckCallTime(float time)
	{
		if (this.blockCall && time < this.blockStartTime + this.spamCoolDown)
		{
			this.blockStartTime = time;
			return false;
		}
		return base.CheckCallTime(time);
	}

	// Token: 0x04001016 RID: 4118
	[SerializeField]
	private float spamCoolDown;
}
