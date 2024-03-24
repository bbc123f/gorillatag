using System;

[Serializable]
public class CooldownType : CallLimitType<CallLimiterWithCooldown>
{
	public CooldownType()
	{
	}
}
