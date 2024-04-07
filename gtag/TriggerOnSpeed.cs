using System;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

public class TriggerOnSpeed : MonoBehaviour, ITickSystemTick
{
	private void OnEnable()
	{
		TickSystem<object>.AddCallbackTarget(this);
	}

	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
	}

	public void Tick()
	{
		bool flag = this.velocityEstimator.linearVelocity.IsLongerThan(this.speedThreshold);
		if (flag != this.wasFaster)
		{
			if (flag)
			{
				this.onFaster.Invoke();
			}
			else
			{
				this.onSlower.Invoke();
			}
			this.wasFaster = flag;
		}
	}

	public bool TickRunning
	{
		[CompilerGenerated]
		get
		{
			return this.<TickRunning>k__BackingField;
		}
		[CompilerGenerated]
		set
		{
			this.<TickRunning>k__BackingField = value;
		}
	}

	public TriggerOnSpeed()
	{
	}

	[SerializeField]
	private float speedThreshold;

	[SerializeField]
	private UnityEvent onFaster;

	[SerializeField]
	private UnityEvent onSlower;

	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	private bool wasFaster;

	[CompilerGenerated]
	private bool <TickRunning>k__BackingField;
}
