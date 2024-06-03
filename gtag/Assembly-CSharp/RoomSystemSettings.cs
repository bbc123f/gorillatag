using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/RoomSystemSettings", order = 2)]
internal class RoomSystemSettings : ScriptableObject
{
	public CallLimiterWithCooldown StatusEffectLimiter
	{
		get
		{
			return this.statusEffectLimiter;
		}
	}

	public CallLimiterWithCooldown SoundEffectLimiter
	{
		get
		{
			return this.soundEffectLimiter;
		}
	}

	public CallLimiterWithCooldown SoundEffectOtherLimiter
	{
		get
		{
			return this.soundEffectOtherLimiter;
		}
	}

	public GameObject PlayerImpactEffect
	{
		get
		{
			return this.playerImpactEffect;
		}
	}

	public RoomSystemSettings()
	{
	}

	[SerializeField]
	private CallLimiterWithCooldown statusEffectLimiter;

	[SerializeField]
	private CallLimiterWithCooldown soundEffectLimiter;

	[SerializeField]
	private CallLimiterWithCooldown soundEffectOtherLimiter;

	[SerializeField]
	private GameObject playerImpactEffect;
}
