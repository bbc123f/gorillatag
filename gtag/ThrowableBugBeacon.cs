using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class ThrowableBugBeacon : MonoBehaviour
{
	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnCall
	{
		[CompilerGenerated]
		add
		{
			ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent = ThrowableBugBeacon.OnCall;
			ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent2;
			do
			{
				throwableBugBeaconEvent2 = throwableBugBeaconEvent;
				ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent3 = (ThrowableBugBeacon.ThrowableBugBeaconEvent)Delegate.Combine(throwableBugBeaconEvent2, value);
				throwableBugBeaconEvent = Interlocked.CompareExchange<ThrowableBugBeacon.ThrowableBugBeaconEvent>(ref ThrowableBugBeacon.OnCall, throwableBugBeaconEvent3, throwableBugBeaconEvent2);
			}
			while (throwableBugBeaconEvent != throwableBugBeaconEvent2);
		}
		[CompilerGenerated]
		remove
		{
			ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent = ThrowableBugBeacon.OnCall;
			ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent2;
			do
			{
				throwableBugBeaconEvent2 = throwableBugBeaconEvent;
				ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent3 = (ThrowableBugBeacon.ThrowableBugBeaconEvent)Delegate.Remove(throwableBugBeaconEvent2, value);
				throwableBugBeaconEvent = Interlocked.CompareExchange<ThrowableBugBeacon.ThrowableBugBeaconEvent>(ref ThrowableBugBeacon.OnCall, throwableBugBeaconEvent3, throwableBugBeaconEvent2);
			}
			while (throwableBugBeaconEvent != throwableBugBeaconEvent2);
		}
	}

	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnDismiss
	{
		[CompilerGenerated]
		add
		{
			ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent = ThrowableBugBeacon.OnDismiss;
			ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent2;
			do
			{
				throwableBugBeaconEvent2 = throwableBugBeaconEvent;
				ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent3 = (ThrowableBugBeacon.ThrowableBugBeaconEvent)Delegate.Combine(throwableBugBeaconEvent2, value);
				throwableBugBeaconEvent = Interlocked.CompareExchange<ThrowableBugBeacon.ThrowableBugBeaconEvent>(ref ThrowableBugBeacon.OnDismiss, throwableBugBeaconEvent3, throwableBugBeaconEvent2);
			}
			while (throwableBugBeaconEvent != throwableBugBeaconEvent2);
		}
		[CompilerGenerated]
		remove
		{
			ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent = ThrowableBugBeacon.OnDismiss;
			ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent2;
			do
			{
				throwableBugBeaconEvent2 = throwableBugBeaconEvent;
				ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent3 = (ThrowableBugBeacon.ThrowableBugBeaconEvent)Delegate.Remove(throwableBugBeaconEvent2, value);
				throwableBugBeaconEvent = Interlocked.CompareExchange<ThrowableBugBeacon.ThrowableBugBeaconEvent>(ref ThrowableBugBeacon.OnDismiss, throwableBugBeaconEvent3, throwableBugBeaconEvent2);
			}
			while (throwableBugBeaconEvent != throwableBugBeaconEvent2);
		}
	}

	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnLock
	{
		[CompilerGenerated]
		add
		{
			ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent = ThrowableBugBeacon.OnLock;
			ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent2;
			do
			{
				throwableBugBeaconEvent2 = throwableBugBeaconEvent;
				ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent3 = (ThrowableBugBeacon.ThrowableBugBeaconEvent)Delegate.Combine(throwableBugBeaconEvent2, value);
				throwableBugBeaconEvent = Interlocked.CompareExchange<ThrowableBugBeacon.ThrowableBugBeaconEvent>(ref ThrowableBugBeacon.OnLock, throwableBugBeaconEvent3, throwableBugBeaconEvent2);
			}
			while (throwableBugBeaconEvent != throwableBugBeaconEvent2);
		}
		[CompilerGenerated]
		remove
		{
			ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent = ThrowableBugBeacon.OnLock;
			ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent2;
			do
			{
				throwableBugBeaconEvent2 = throwableBugBeaconEvent;
				ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent3 = (ThrowableBugBeacon.ThrowableBugBeaconEvent)Delegate.Remove(throwableBugBeaconEvent2, value);
				throwableBugBeaconEvent = Interlocked.CompareExchange<ThrowableBugBeacon.ThrowableBugBeaconEvent>(ref ThrowableBugBeacon.OnLock, throwableBugBeaconEvent3, throwableBugBeaconEvent2);
			}
			while (throwableBugBeaconEvent != throwableBugBeaconEvent2);
		}
	}

	public static event ThrowableBugBeacon.ThrowableBugBeaconEvent OnUnlock
	{
		[CompilerGenerated]
		add
		{
			ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent = ThrowableBugBeacon.OnUnlock;
			ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent2;
			do
			{
				throwableBugBeaconEvent2 = throwableBugBeaconEvent;
				ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent3 = (ThrowableBugBeacon.ThrowableBugBeaconEvent)Delegate.Combine(throwableBugBeaconEvent2, value);
				throwableBugBeaconEvent = Interlocked.CompareExchange<ThrowableBugBeacon.ThrowableBugBeaconEvent>(ref ThrowableBugBeacon.OnUnlock, throwableBugBeaconEvent3, throwableBugBeaconEvent2);
			}
			while (throwableBugBeaconEvent != throwableBugBeaconEvent2);
		}
		[CompilerGenerated]
		remove
		{
			ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent = ThrowableBugBeacon.OnUnlock;
			ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent2;
			do
			{
				throwableBugBeaconEvent2 = throwableBugBeaconEvent;
				ThrowableBugBeacon.ThrowableBugBeaconEvent throwableBugBeaconEvent3 = (ThrowableBugBeacon.ThrowableBugBeaconEvent)Delegate.Remove(throwableBugBeaconEvent2, value);
				throwableBugBeaconEvent = Interlocked.CompareExchange<ThrowableBugBeacon.ThrowableBugBeaconEvent>(ref ThrowableBugBeacon.OnUnlock, throwableBugBeaconEvent3, throwableBugBeaconEvent2);
			}
			while (throwableBugBeaconEvent != throwableBugBeaconEvent2);
		}
	}

	public static event ThrowableBugBeacon.ThrowableBugBeaconFloatEvent OnChangeSpeedMultiplier
	{
		[CompilerGenerated]
		add
		{
			ThrowableBugBeacon.ThrowableBugBeaconFloatEvent throwableBugBeaconFloatEvent = ThrowableBugBeacon.OnChangeSpeedMultiplier;
			ThrowableBugBeacon.ThrowableBugBeaconFloatEvent throwableBugBeaconFloatEvent2;
			do
			{
				throwableBugBeaconFloatEvent2 = throwableBugBeaconFloatEvent;
				ThrowableBugBeacon.ThrowableBugBeaconFloatEvent throwableBugBeaconFloatEvent3 = (ThrowableBugBeacon.ThrowableBugBeaconFloatEvent)Delegate.Combine(throwableBugBeaconFloatEvent2, value);
				throwableBugBeaconFloatEvent = Interlocked.CompareExchange<ThrowableBugBeacon.ThrowableBugBeaconFloatEvent>(ref ThrowableBugBeacon.OnChangeSpeedMultiplier, throwableBugBeaconFloatEvent3, throwableBugBeaconFloatEvent2);
			}
			while (throwableBugBeaconFloatEvent != throwableBugBeaconFloatEvent2);
		}
		[CompilerGenerated]
		remove
		{
			ThrowableBugBeacon.ThrowableBugBeaconFloatEvent throwableBugBeaconFloatEvent = ThrowableBugBeacon.OnChangeSpeedMultiplier;
			ThrowableBugBeacon.ThrowableBugBeaconFloatEvent throwableBugBeaconFloatEvent2;
			do
			{
				throwableBugBeaconFloatEvent2 = throwableBugBeaconFloatEvent;
				ThrowableBugBeacon.ThrowableBugBeaconFloatEvent throwableBugBeaconFloatEvent3 = (ThrowableBugBeacon.ThrowableBugBeaconFloatEvent)Delegate.Remove(throwableBugBeaconFloatEvent2, value);
				throwableBugBeaconFloatEvent = Interlocked.CompareExchange<ThrowableBugBeacon.ThrowableBugBeaconFloatEvent>(ref ThrowableBugBeacon.OnChangeSpeedMultiplier, throwableBugBeaconFloatEvent3, throwableBugBeaconFloatEvent2);
			}
			while (throwableBugBeaconFloatEvent != throwableBugBeaconFloatEvent2);
		}
	}

	public ThrowableBug.BugName BugName
	{
		get
		{
			return this.bugName;
		}
	}

	public float Range
	{
		get
		{
			return this.range;
		}
	}

	public void Call()
	{
		if (ThrowableBugBeacon.OnCall != null)
		{
			ThrowableBugBeacon.OnCall(this);
		}
	}

	public void Dismiss()
	{
		if (ThrowableBugBeacon.OnDismiss != null)
		{
			ThrowableBugBeacon.OnDismiss(this);
		}
	}

	public void Lock()
	{
		if (ThrowableBugBeacon.OnLock != null)
		{
			ThrowableBugBeacon.OnLock(this);
		}
	}

	public void Unlock()
	{
		if (ThrowableBugBeacon.OnUnlock != null)
		{
			ThrowableBugBeacon.OnUnlock(this);
		}
	}

	public void ChangeSpeedMultiplier(float f)
	{
		if (ThrowableBugBeacon.OnChangeSpeedMultiplier != null)
		{
			ThrowableBugBeacon.OnChangeSpeedMultiplier(this, f);
		}
	}

	private void OnDisable()
	{
		if (ThrowableBugBeacon.OnUnlock != null)
		{
			ThrowableBugBeacon.OnUnlock(this);
		}
	}

	public ThrowableBugBeacon()
	{
	}

	[CompilerGenerated]
	private static ThrowableBugBeacon.ThrowableBugBeaconEvent OnCall;

	[CompilerGenerated]
	private static ThrowableBugBeacon.ThrowableBugBeaconEvent OnDismiss;

	[CompilerGenerated]
	private static ThrowableBugBeacon.ThrowableBugBeaconEvent OnLock;

	[CompilerGenerated]
	private static ThrowableBugBeacon.ThrowableBugBeaconEvent OnUnlock;

	[CompilerGenerated]
	private static ThrowableBugBeacon.ThrowableBugBeaconFloatEvent OnChangeSpeedMultiplier;

	[SerializeField]
	private float range;

	[SerializeField]
	private ThrowableBug.BugName bugName;

	public delegate void ThrowableBugBeaconEvent(ThrowableBugBeacon tbb);

	public delegate void ThrowableBugBeaconFloatEvent(ThrowableBugBeacon tbb, float f);
}
