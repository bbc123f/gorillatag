using System;
using UnityEngine;

// Token: 0x020001EC RID: 492
[Serializable]
public class CallLimiterWithCooldown : CallLimiter
{
	// Token: 0x06000CC5 RID: 3269 RVA: 0x0004C2A9 File Offset: 0x0004A4A9
	public CallLimiterWithCooldown(float coolDownSpam, int historyLength, float coolDown) : base(historyLength, coolDown, 0.5f)
	{
		this.spamCoolDown = coolDownSpam;
	}

	// Token: 0x06000CC6 RID: 3270 RVA: 0x0004C2BF File Offset: 0x0004A4BF
	public CallLimiterWithCooldown(float coolDownSpam, int historyLength, float coolDown, float latencyMax) : base(historyLength, coolDown, latencyMax)
	{
		this.spamCoolDown = coolDownSpam;
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x0004C2D2 File Offset: 0x0004A4D2
	public override bool CheckCallTime(float time)
	{
		if (this.blockCall && time < this.blockStartTime + this.spamCoolDown)
		{
			this.blockStartTime = time;
			return false;
		}
		return base.CheckCallTime(time);
	}

	// Token: 0x04001012 RID: 4114
	[SerializeField]
	private float spamCoolDown;
}
