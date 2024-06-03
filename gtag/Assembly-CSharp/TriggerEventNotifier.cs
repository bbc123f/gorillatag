using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class TriggerEventNotifier : MonoBehaviour
{
	public event TriggerEventNotifier.TriggerEvent TriggerEnterEvent
	{
		[CompilerGenerated]
		add
		{
			TriggerEventNotifier.TriggerEvent triggerEvent = this.TriggerEnterEvent;
			TriggerEventNotifier.TriggerEvent triggerEvent2;
			do
			{
				triggerEvent2 = triggerEvent;
				TriggerEventNotifier.TriggerEvent value2 = (TriggerEventNotifier.TriggerEvent)Delegate.Combine(triggerEvent2, value);
				triggerEvent = Interlocked.CompareExchange<TriggerEventNotifier.TriggerEvent>(ref this.TriggerEnterEvent, value2, triggerEvent2);
			}
			while (triggerEvent != triggerEvent2);
		}
		[CompilerGenerated]
		remove
		{
			TriggerEventNotifier.TriggerEvent triggerEvent = this.TriggerEnterEvent;
			TriggerEventNotifier.TriggerEvent triggerEvent2;
			do
			{
				triggerEvent2 = triggerEvent;
				TriggerEventNotifier.TriggerEvent value2 = (TriggerEventNotifier.TriggerEvent)Delegate.Remove(triggerEvent2, value);
				triggerEvent = Interlocked.CompareExchange<TriggerEventNotifier.TriggerEvent>(ref this.TriggerEnterEvent, value2, triggerEvent2);
			}
			while (triggerEvent != triggerEvent2);
		}
	}

	public event TriggerEventNotifier.TriggerEvent TriggerExitEvent
	{
		[CompilerGenerated]
		add
		{
			TriggerEventNotifier.TriggerEvent triggerEvent = this.TriggerExitEvent;
			TriggerEventNotifier.TriggerEvent triggerEvent2;
			do
			{
				triggerEvent2 = triggerEvent;
				TriggerEventNotifier.TriggerEvent value2 = (TriggerEventNotifier.TriggerEvent)Delegate.Combine(triggerEvent2, value);
				triggerEvent = Interlocked.CompareExchange<TriggerEventNotifier.TriggerEvent>(ref this.TriggerExitEvent, value2, triggerEvent2);
			}
			while (triggerEvent != triggerEvent2);
		}
		[CompilerGenerated]
		remove
		{
			TriggerEventNotifier.TriggerEvent triggerEvent = this.TriggerExitEvent;
			TriggerEventNotifier.TriggerEvent triggerEvent2;
			do
			{
				triggerEvent2 = triggerEvent;
				TriggerEventNotifier.TriggerEvent value2 = (TriggerEventNotifier.TriggerEvent)Delegate.Remove(triggerEvent2, value);
				triggerEvent = Interlocked.CompareExchange<TriggerEventNotifier.TriggerEvent>(ref this.TriggerExitEvent, value2, triggerEvent2);
			}
			while (triggerEvent != triggerEvent2);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		TriggerEventNotifier.TriggerEvent triggerEnterEvent = this.TriggerEnterEvent;
		if (triggerEnterEvent == null)
		{
			return;
		}
		triggerEnterEvent(this, other);
	}

	private void OnTriggerExit(Collider other)
	{
		TriggerEventNotifier.TriggerEvent triggerExitEvent = this.TriggerExitEvent;
		if (triggerExitEvent == null)
		{
			return;
		}
		triggerExitEvent(this, other);
	}

	public TriggerEventNotifier()
	{
	}

	[CompilerGenerated]
	private TriggerEventNotifier.TriggerEvent TriggerEnterEvent;

	[CompilerGenerated]
	private TriggerEventNotifier.TriggerEvent TriggerExitEvent;

	public int maskIndex;

	public delegate void TriggerEvent(TriggerEventNotifier notifier, Collider collider);
}
